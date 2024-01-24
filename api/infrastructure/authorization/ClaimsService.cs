using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CAS.API.helpers.extensions;
using CAS.COMMON.authorization;
using CAS.DB.models;
using CAS.DB.models.auth;

namespace CAS.API.infrastructure.authorization
{
    /// <summary>
    /// This should load up our User context with claims from the database. 
    /// </summary>
    public class ClaimsService
    {
        private CourtAdminDbContext Db { get; }

        public ClaimsService(CourtAdminDbContext dbContext)
        {
            Db = dbContext;
        }

        public async Task<List<Claim>> GenerateClaims(List<Claim> currentClaims)
        {
            var claims = new List<Claim>();
            var idirName = currentClaims.GetIdirUserName();
            var idirId = currentClaims.GetIdirId();

            //Match by IdirID (already logged into SSO before) or Idir with no IdirID (created, but hasn't logged in yet).
            //Hopefully IdirID doesn't change when the base IdirName does (getting married / divorced etc).
            var user = await Db.User.AsSingleQuery().AsNoTracking()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .Where(u => u.IdirId == idirId || !u.IdirId.HasValue && u.IdirName == idirName)
                .Select(u => new User
                {
                    Id = u.Id,
                    UserRoles = u.UserRoles,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    HomeLocationId = u.HomeLocationId,
                    IsEnabled = u.IsEnabled
                }).FirstOrDefaultAsync();

            if (user?.IsEnabled != true) 
                return claims;

            claims.AddRange(user.ActiveRoles.SelectToList(r => new Claim(ClaimTypes.Role, r.Role.Name)));
            claims.AddRange(user.Permissions.SelectDistinctToList(p => new Claim(CustomClaimTypes.Permission, p.Name)));
            claims.Add(new Claim(CustomClaimTypes.FirstName, user.FirstName));
            claims.Add(new Claim(CustomClaimTypes.LastName, user.LastName));
            claims.Add(new Claim(CustomClaimTypes.UserId, user.Id.ToString()));
            if (user.HomeLocationId.HasValue)
                claims.Add(new Claim(CustomClaimTypes.HomeLocationId, user.HomeLocationId.ToString()));
            return claims;
        }
    }
}
