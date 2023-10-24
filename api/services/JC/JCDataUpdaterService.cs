using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JCCommon.Clients.LocationServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CAS.API.helpers;
using CAS.API.helpers.extensions;
using CAS.API.infrastructure.exceptions;
using CAS.API.Models.DB;
using CAS.DB.models;
using CAS.DB.models.auth;
using CAS.DB.models.jc;
using CAS.DB.models.lookupcodes;
using Region = db.models.Region;

namespace CAS.API.services.jc
{
    public class JCDataUpdaterService
    {
        private ILogger Logger { get; }
        private LocationServicesClient LocationClient { get; }
        private CourtAdminDbContext Db { get; }
        private IConfiguration Configuration { get; }
        private bool ExpireRegions { get; }
        private bool ExpireLocations { get; }
        private bool ExpireRooms { get; }
        private bool SkipLocationUpdates { get; }
        private bool AssociateUsersWithNoLocationToVictoria { get; }
        private TimeSpan UpdateEvery { get; }

        public JCDataUpdaterService(CourtAdminDbContext dbContext, LocationServicesClient locationClient, IConfiguration configuration, ILogger<JCDataUpdaterService> logger)
        {
            LocationClient = locationClient;
            Db = dbContext;
            Configuration = configuration;
            SkipLocationUpdates = Configuration.GetBoolValue("SkipLocationUpdates").Equals("true");
            ExpireRegions = Configuration.GetNonEmptyValue("JCSynchronization:ExpireRegions").Equals("true");
            ExpireLocations = Configuration.GetNonEmptyValue("JCSynchronization:ExpireLocations").Equals("true");
            ExpireRooms = Configuration.GetNonEmptyValue("JCSynchronization:ExpireCourtRooms").Equals("true");
            AssociateUsersWithNoLocationToVictoria = Configuration.GetNonEmptyValue("JCSynchronization:AssociateUsersWithNoLocationToVictoria").Equals("true");
            UpdateEvery = TimeSpan.Parse(configuration.GetNonEmptyValue("JCSynchronization:UpdateEvery"));
            Logger = logger;
        }

        public async Task<bool> ShouldSynchronize()
        {
            //Only a single row here. 
            var jcSynchronization = await Db.JcSynchronization.FirstOrDefaultAsync(jc => jc.Id == 1);
            if (jcSynchronization == null)
            {
                await Db.JcSynchronization.AddAsync(new JcSynchronization { Id = 1, LastSynchronization = DateTimeOffset.UtcNow });
                await Db.SaveChangesAsync();
                return true;
            }

            if(SkipLocationUpdates) return false;
            if (jcSynchronization.LastSynchronization.Add(UpdateEvery) > DateTimeOffset.UtcNow) return false;
            jcSynchronization.LastSynchronization = DateTimeOffset.UtcNow;
            await Db.SaveChangesAsync();
            return true;
        }

        public async Task SyncRegions()
        {
            var regionsDb = (await LocationClient.RegionsAsync()).SelectToList(r => new Region { JustinId = r.RegionId, Name = r.RegionName, CreatedById = User.SystemUser });
            await Db.Region.UpsertRange(regionsDb)
                            .On(v => v.JustinId)
                            .WhenMatched((r, rnew) => new Region
                            {
                                Name = rnew.Name,
                                JustinId = rnew.JustinId,
                                UpdatedOn = DateTime.UtcNow
                            })
                            .RunAsync();
            
            //Any regions that aren't on this list expire/disable for now. This is for regions that may have been deleted. 
            if (ExpireRegions)
            {
                var disableRegions = Db.Region.WhereToList(r => r.ExpiryDate == null && regionsDb.All(rdb => rdb.JustinId != r.JustinId));
                foreach (var disableRegion in disableRegions)
                {
                    Logger.LogDebug($"Expiring Region {disableRegion.Id}: {disableRegion.Name}");
                    disableRegion.ExpiryDate = DateTime.UtcNow;
                    disableRegion.UpdatedOn = DateTime.UtcNow;
                    disableRegion.UpdatedById = User.SystemUser;
                }
                await Db.SaveChangesAsync();
            }
        }

        public async Task SyncLocations()
        {
            var locationsDb = await GenerateLocationsAndLinkToRegions();

            await Db.Location.UpsertRange(locationsDb)
                               .On(v => v.AgencyId)
                               .WhenMatched((l, lnew) => new Location
                               {
                                   Name = lnew.Name,
                                   RegionId = lnew.RegionId,
                                   UpdatedOn = DateTime.UtcNow,
                                   Timezone = lnew.Timezone
                               })
                               .RunAsync();

            //Set to false for now, because some Locations are brought in via Migration and not the JC-Interface.
            //Any Locations that aren't on this list expire/disable for now.  This is for locations that may have been deleted. 
            if (ExpireLocations)
            {
                var disableLocations = Db.Location.WhereToList(l => l.ExpiryDate == null && locationsDb.All(rdb => rdb.AgencyId != l.AgencyId ));
                foreach (var disableLocation in disableLocations)
                {
                    Logger.LogDebug($"Expiring Location {disableLocation.Id}: {disableLocation.Name}");
                    disableLocation.ExpiryDate = DateTime.UtcNow;
                    disableLocation.UpdatedOn = DateTime.UtcNow;
                    disableLocation.UpdatedById = User.SystemUser;
                }
                await Db.SaveChangesAsync();
            }

            if (AssociateUsersWithNoLocationToVictoria)
            {
                var sheriffsWithNoHomeLocation = Db.CourtAdmin.Where(s => !s.HomeLocationId.HasValue);
                var victoriaLocation = Db.Location.AsNoTracking().FirstOrDefault(l => l.Name == "Victoria Law Courts");
                if (victoriaLocation == null)
                    return;
                foreach (var sheriff in sheriffsWithNoHomeLocation)
                {
                    Logger.LogDebug($"Setting ${sheriff.LastName}, ${sheriff.FirstName} - HomeLocation to ${victoriaLocation.Id}");
                    sheriff.HomeLocationId = victoriaLocation.Id;
                }
                await Db.SaveChangesAsync();
            }

            //Associate Migrated Location to regions. 
            await AssociateAdhocLocationToRegion();
        }

        public async Task SyncCourtRooms()
        {
            var courtRoomsLookups = await LocationClient.LocationsRoomsAsync();
            //To list so we don't need to re-query on each select.
            var locations = Db.Location.ToList();
            var courtRooms = courtRoomsLookups.SelectToList(cr =>
                new CAS.DB.models.LookupCode
                {
                    Type = LookupTypes.CourtRoom,
                    Code = cr.Code,
                    LocationId = locations.FirstOrDefault(l => l.JustinCode == cr.Flex)
                        ?.Id,
                    CreatedById = User.SystemUser
                }).WhereToList(cr => cr.LocationId != null);

            //Some court rooms, don't have a location. This should probably be fixed in the JC-Interface
            var courtRoomNoLocation = courtRoomsLookups.WhereToList(crl => locations.All(l => l.JustinCode != crl.Flex));
            Logger.LogDebug("Court rooms without a location: ");
            Logger.LogDebug(JsonConvert.SerializeObject(courtRoomNoLocation));

            await Db.LookupCode.UpsertRange(courtRooms)
                 .On(v => new { v.Type, v.Code, v.LocationId })
                 .WhenMatched((cr, crNew) => new CAS.DB.models.LookupCode
                 {
                     Code = crNew.Code,
                     LocationId = crNew.LocationId,
                     UpdatedOn = DateTime.UtcNow
                 })
                 .RunAsync();

            //Any court rooms that aren't from this list, expire/disable for now. This is for CourtRooms that may have been deleted. 
            if (ExpireRooms)
            {
                var disableCourtRooms = Db.LookupCode.WhereToList(lc =>
                    lc.ExpiryDate == null &&
                    lc.Type == LookupTypes.CourtRoom &&
                    !courtRooms.Any(cr => cr.Type == lc.Type && cr.Code == lc.Code && cr.LocationId == lc.LocationId));

                foreach (var disableCourtRoom in disableCourtRooms)
                {
                    Logger.LogDebug($"Expiring CourtRoom {disableCourtRoom.Id}: {disableCourtRoom.Code}");
                    disableCourtRoom.ExpiryDate = DateTime.UtcNow;
                    disableCourtRoom.UpdatedOn = DateTime.UtcNow;
                    disableCourtRoom.UpdatedById = User.SystemUser;
                }
                await Db.SaveChangesAsync();
            }
        }


        private async Task<List<Location>> GenerateLocationsAndLinkToRegions()
        {
            var regionDictionary = new Dictionary<int, ICollection<int>>();
            //RegionsRegionIdLocationsCodesAsync returns a LIST of locationIds.
            foreach (var region in Db.Region)
                regionDictionary[region.Id] = await LocationClient.RegionsRegionIdLocationsCodesAsync(region.JustinId.ToString());

            //Reverse the dictionary and flatten.
            var locationToRegion = new Dictionary<string, int>();
            foreach (var (region, locationId) in regionDictionary.SelectMany(region => region.Value.Select(locationId => (region, locationId))))
                locationToRegion[locationId.ToString()] = region.Key;

            var locationWithoutRegion = new List<Location>();
            var locations = await LocationClient.LocationsAsync(null, true, false);
            var locationsDb = locations.SelectToList(loc =>
            {
                var regionId = locationToRegion.ContainsKey(loc.ShortDesc) ? locationToRegion[loc.ShortDesc] : null as int?;
                var location = new Location { JustinCode = loc.ShortDesc, Name = loc.LongDesc, AgencyId = loc.Code, RegionId = regionId, CreatedById = User.SystemUser };
                //Some locations don't have a region, this should be fixed in the JC-Interface. 
                if (regionId == null)
                    locationWithoutRegion.Add(location);

                return location;
            });

            locationsDb = AssociateLocationToTimezone(locationsDb);

            Logger.LogDebug("Locations without a region: ");
            Logger.LogDebug(JsonConvert.SerializeObject(locationWithoutRegion));
            return locationsDb;
        }

        private async Task AssociateAdhocLocationToRegion()
        {
            var locationsToRegions =
                Configuration.GetSection("JCSynchronization:NonJcInterfaceLocationRegions").Get<Dictionary<string, string>>() ??
                throw new ConfigurationException("Couldn't not build dictionary based on NonJcInterfaceLocationRegions");

            foreach (var locationToRegion in locationsToRegions)
            {
                var location = await Db.Location.FirstOrDefaultAsync(l => l.Name == locationToRegion.Key);
                if (location != null && location.RegionId == null)
                    location.RegionId = (await Db.Region.AsNoTracking().FirstOrDefaultAsync(r => r.Name == locationToRegion.Value))?.Id;
            }
            await Db.SaveChangesAsync();
        }

        private List<Location> AssociateLocationToTimezone(List<Location> locations)
        {
            foreach (var location in locations)
            {
                var configurationSections =
                    Configuration.GetSection("JCSynchronization:LocationTimeZones").Get<Dictionary<string, string>>() ??
                    throw new ConfigurationException("Couldn't not build dictionary based on LocationTimeZones");

                var otherTimezone =
                    configurationSections.FirstOrDefault(cs => cs.Value.Split(",").Select(s => s.Trim())
                            .Any(partialName => location.Name.Contains(partialName)))
                        .Key;

                location.Timezone = otherTimezone ?? "America/Vancouver";
            }

            return locations;
        }

    }
}
