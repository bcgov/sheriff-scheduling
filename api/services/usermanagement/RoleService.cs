using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using CAS.API.helpers.extensions;
using CAS.API.infrastructure.exceptions;
using CAS.DB.models;
using CAS.DB.models.auth;

namespace CAS.API.services.usermanagement
{
    public class RoleService
    {
        private CourtAdminDbContext Db { get; }

        public RoleService(CourtAdminDbContext dbContext)
        {
            Db = dbContext;
        }

        public async Task<List<Role>> Roles() => await Db.Role.AsNoTracking().ToListAsync();

        public async Task<Role> Role(int id) =>
            await Db.Role.AsNoTracking()
                .AsSingleQuery()
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission).SingleOrDefaultAsync(r => r.Id == id);

        public async Task<Role> AddRole(Role role, List<int> permissionIds)
        {
            var roleAlreadyExistsWithName = await Db.Role.AnyAsync(r => r.Name.ToLower() == role.Name.ToLower());
            if (roleAlreadyExistsWithName)
                throw new BusinessLayerException($"{nameof(Role)} with name {role.Name} already exists.");

            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            role.RolePermissions = null;
            role.UserRoles = null;
            await Db.Role.AddAsync(role);
            await Db.SaveChangesAsync();
            await AssignPermissionsToRole(role.Id, permissionIds);
            await Db.SaveChangesAsync();
            scope.Complete();
            return role;
        }

        public async Task RemoveRole(int roleId)
        {
            var role = await Db.Role.FindAsync(roleId);
            Db.Role.Remove(role);
            await Db.SaveChangesAsync();
        }

        public async Task<Role> UpdateRole(Role role, List<int> permissionIds)
        {
            var savedRole = await Db.Role.AsSingleQuery().Include(r => r.RolePermissions)
                                          .FirstOrDefaultAsync(r => r.Id == role.Id);
            if (savedRole.Name != role.Name)
            {
                var roleAlreadyExistsWithName = await Db.Role.AnyAsync(r => r.Name.ToLower() == role.Name.ToLower());
                if (roleAlreadyExistsWithName)
                    throw new BusinessLayerException($"{nameof(Role)} with name {role.Name} already exists.");
            }

            var permissionIdsToRemove =
                savedRole.RolePermissions.Select(rp => rp.PermissionId).Except(permissionIds).ToList();

            Db.Entry(savedRole).CurrentValues.SetValues(role);

            await AssignPermissionsToRole(role.Id, permissionIds);
            await UnassignPermissionsFromRole(role.Id, permissionIdsToRemove);
            await Db.SaveChangesAsync();

            return savedRole;
        }

        private async Task AssignPermissionsToRole(int roleId, IEnumerable<int> permissionIds)
        {
            var role = await Db.Role.AsSingleQuery().Include(r => r.RolePermissions)
                                    .ThenInclude(p => p.Permission)
                                     .FirstOrDefaultAsync( r=> r.Id == roleId);
            role.ThrowBusinessExceptionIfNull($"{nameof(Role)} with id {roleId} does not exist.");

            foreach (var permissionId in permissionIds)
            {
                var permission = await Db.Permission.FindAsync(permissionId);
                if (permission == null || role.RolePermissions.Any(rp => rp.Role.Id == roleId && rp.Permission?.Id == permissionId))
                    continue;

                role.RolePermissions.Add(new RolePermission
                {
                    Role = role,
                    Permission = permission
                });
            }
        }

        private async Task UnassignPermissionsFromRole(int roleId, ICollection<int> permissionIds)
        {
            var role = await Db.Role.AsSingleQuery().Include(r => r.RolePermissions)
                                     .FirstOrDefaultAsync(r => r.Id == roleId);
            role.ThrowBusinessExceptionIfNull($"{nameof(Role)} with id {roleId} does not exist.");
            
            Db.RemoveRange(role.RolePermissions.Where(rp => permissionIds.Contains(rp.PermissionId) && rp.Role.Id == roleId));
        }
    }
}
