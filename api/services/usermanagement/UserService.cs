using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CAS.API.helpers.extensions;
using CAS.DB.models;
using CAS.DB.models.auth;

namespace CAS.API.services.usermanagement
{
    public class UserService
    {
        private CourtAdminDbContext Db { get; }
        public UserService(CourtAdminDbContext courtAdminDbContext)
        {
            Db = courtAdminDbContext;
        }
        
        public async Task<User> DisableUser(Guid id)
        {
            var user = await Db.User.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.Id == id);
            user.ThrowBusinessExceptionIfNull($"User with the id: {id} could not be found. ");

            user.IsEnabled = false;
            foreach (var userRole in user.UserRoles)
                userRole.ExpiryDate = DateTimeOffset.UtcNow;

            await Db.SaveChangesAsync();
            return user;
        }

        public async Task<User> EnableUser(Guid id)
        {
            var user = await Db.User.FindAsync(id);
            user.ThrowBusinessExceptionIfNull($"User with the id: {id} could not be found. ");

            user.IsEnabled = true;
            await Db.SaveChangesAsync();
            return user;
        }

        public async Task AssignRolesToUser(List<UserRole> assignRoles)
        {
            foreach (var assignRole in assignRoles)
            {
                var user = await Db.User.FindAsync(assignRole.UserId);
                user.ThrowBusinessExceptionIfNull($"User with id {assignRole.UserId} does not exist.");

                var role = await Db.Role.AsSingleQuery().Include(r => r.UserRoles).FirstOrDefaultAsync(r => r.Id == assignRole.RoleId);
                role.ThrowBusinessExceptionIfNull($"Role with id {assignRole.RoleId} does not exist.");

                var savedUserRole = user.UserRoles.FirstOrDefault(ur =>
                    ur.UserId == assignRole.UserId &&
                    ur.RoleId == assignRole.RoleId);

                if (savedUserRole != null)
                {
                    savedUserRole.Role = role;
                    savedUserRole.User = user;
                    savedUserRole.ExpiryDate = assignRole.ExpiryDate;
                    savedUserRole.EffectiveDate = assignRole.EffectiveDate;
                    savedUserRole.ExpiryReason = null;
                }
                else
                {
                    user.UserRoles.Add(new UserRole
                    {
                        Role = role,
                        User = user,
                        ExpiryDate = assignRole.ExpiryDate,
                        EffectiveDate = assignRole.EffectiveDate
                    });
                }
            }
            await Db.SaveChangesAsync();
        }

        public async Task UnassignRoleFromUser(List<UserRole> unassignRoles)
        {
            foreach (var unassignRole in unassignRoles)
            {
                var user = await Db.User.AsSingleQuery().Include(r => r.UserRoles).FirstOrDefaultAsync(u => u.Id == unassignRole.UserId);
                user.ThrowBusinessExceptionIfNull($"User with id {unassignRole.UserId} does not exist.");

                var userRole = user.UserRoles.FirstOrDefault(r => r.UserId == unassignRole.UserId && r.RoleId == unassignRole.RoleId);
                if (userRole == null) 
                    continue;
                userRole.ExpiryDate = DateTime.UtcNow;
                userRole.ExpiryReason = unassignRole.ExpiryReason;
            }
            await Db.SaveChangesAsync();
        }
    }
}
