using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Mapster;
using CAS.API.helpers.extensions;
using CAS.API.infrastructure.exceptions;
using CAS.API.models.dto;
using CAS.DB.models;
using CAS.DB.models.lookupcodes;

namespace CAS.API.services
{
    public class ManageTypesService
    {
        /// <summary>
        /// Handles ManageTypes or LookupCodes. 
        /// </summary>
        private CourtAdminDbContext Db { get; }

        public ManageTypesService(CourtAdminDbContext dbContext )
        {
            Db = dbContext;
        }

        public async Task<LookupCode> Add(AddLookupCodeDto addLookupCode)
        {
            var lookupCd = addLookupCode.Adapt<LookupCode>();
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            lookupCd.Location = await Db.Location.FindAsync(addLookupCode.LocationId);
            lookupCd.SortOrder = null;

            if (Db.LookupCode.AsNoTracking()
                .Any(lc => lc.Code == addLookupCode.Code && lc.LocationId == addLookupCode.LocationId))
                throw new BusinessLayerException("Attempted to create a duplicate entry.");

            await Db.LookupCode.AddAsync(lookupCd);
            await Db.SaveChangesAsync();
            if (addLookupCode.SortOrderForLocation != null)
            {
                var lookupSortOrder = new LookupSortOrder
                {
                    LookupCodeId = lookupCd.Id,
                    LocationId = addLookupCode.SortOrderForLocation.LocationId,
                    SortOrder = addLookupCode.SortOrderForLocation.SortOrder
                };
                await Db.LookupSortOrder.AddAsync(lookupSortOrder);
                await Db.SaveChangesAsync();
                lookupCd.SortOrderForLocation = lookupSortOrder;
            }
            scope.Complete();
            return lookupCd;
        }

        public async Task<LookupCode> Update(LookupCode lookupCode)
        {
            lookupCode.SortOrderForLocation.ThrowBusinessExceptionIfNull("SortOrderForLocation cannot be null");
            var savedLookup = await Db.LookupCode.Include(lc => lc.SortOrder.Where(so =>
                    so.LookupCodeId == lookupCode.Id &&
                    so.LocationId == lookupCode.SortOrderForLocation.LocationId))
                .FirstOrDefaultAsync(lc => lc.Id == lookupCode.Id);
            savedLookup.ThrowBusinessExceptionIfNull($"Couldn't find lookup code with id: {lookupCode.Id}");

            if ((savedLookup.Code != lookupCode.Code ||
                 savedLookup.LocationId != lookupCode.LocationId)
                &&
                Db.LookupCode.AsNoTracking()
                    .Any(lc => lc.Code == lookupCode.Code && lc.LocationId == lookupCode.LocationId))
                throw new BusinessLayerException($"Attempted to create a duplicate entry.");

            savedLookup.Location = await Db.Location.FindAsync(lookupCode.LocationId);

            Db.Entry(savedLookup).CurrentValues.SetValues(lookupCode);
            Db.Entry(savedLookup).Property(x => x.ExpiryDate).IsModified = false;

            var sortOrder = savedLookup.SortOrder.FirstOrDefault();
            if (sortOrder != null)
            {
                sortOrder.SortOrder = lookupCode.SortOrderForLocation.SortOrder;
                await Db.SaveChangesAsync();
            }
            else
            {
                var lookupSortOrder = new LookupSortOrder
                {
                    LookupCodeId = savedLookup.Id,
                    LocationId = lookupCode.SortOrderForLocation.LocationId,
                    SortOrder = lookupCode.SortOrderForLocation.SortOrder
                };
                await Db.LookupSortOrder.AddAsync(lookupSortOrder);
                await Db.SaveChangesAsync();
                lookupCode.SortOrderForLocation = lookupSortOrder;
            }
            return lookupCode;
        }

        public async Task UpdateSortOrders(SortOrdersDto sortOrders)
        {
            var locationId = sortOrders.SortOrderLocationId;
            foreach (var sortOrder in sortOrders.SortOrders)
            {
                var existingSortOrder = await Db.LookupSortOrder.FirstOrDefaultAsync(lso =>
                    lso.LocationId == locationId && lso.LookupCodeId == sortOrder.LookupCodeId);
                if (existingSortOrder == null)
                {
                    await Db.LookupSortOrder.AddAsync(new LookupSortOrder
                    {
                        LocationId = locationId,
                        LookupCodeId = sortOrder.LookupCodeId,
                        SortOrder = sortOrder.SortOrder
                    });
                }
                else
                {
                    existingSortOrder.SortOrder = sortOrder.SortOrder;
                }
            }
            await Db.SaveChangesAsync();
        }

        public async Task<List<LookupCode>> GetAll(LookupTypes? codeType, int? locationId, bool showExpired = false)
        {
            var lookupCodes = await Db.LookupCode.AsNoTracking()
                .Include(lc => lc.SortOrder.Where(so => so.LocationId == locationId))
                .Where(lc =>
                    (codeType == null || lc.Type == codeType) &&
                    (locationId == null || lc.LocationId == null || lc.LocationId == locationId) &&
                    (showExpired || lc.ExpiryDate == null))
                .OrderBy(a => (int)a.Type)
                .ThenBy(a => a.SortOrder.FirstOrDefault().SortOrder)
                .ThenBy(a => a.Id)
                .ToListAsync();

            lookupCodes.ForEach(lc => lc.SortOrderForLocation = lc.SortOrder.FirstOrDefault());

            return lookupCodes;
        }

        public async Task<LookupCode> Find(int id) => await Db.LookupCode.AsNoTracking().FirstOrDefaultAsync(lc => lc.Id == id);
        
        public async Task<LookupCode> Expire(int id)
        {
            var entity = await Db.LookupCode.Include(lc => lc.SortOrder)
                .FirstOrDefaultAsync(lc => lc.Id == id);
            entity.ThrowBusinessExceptionIfNull($"Couldn't find lookup code with id: {id}");
            entity.ExpiryDate = DateTime.UtcNow;
            Db.LookupSortOrder.RemoveRange(Db.LookupSortOrder.Where(lso => lso.LookupCodeId == id));
            await Db.SaveChangesAsync();
            return entity;
        }

        public async Task<LookupCode> Unexpire(int id)
        {
            var entity = await Db.LookupCode.FindAsync(id);
            entity.ThrowBusinessExceptionIfNull($"Couldn't find lookup code with id: {id}");
            entity.ExpiryDate = null;
            await Db.SaveChangesAsync();
            return entity;
        }
    }
}
