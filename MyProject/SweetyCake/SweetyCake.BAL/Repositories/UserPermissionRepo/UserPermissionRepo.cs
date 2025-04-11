using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OutbornE_commerce.BAL.Repositories.UserPermissionRepo
{
    public class UserPermissionRepo : BaseRepository<UserPermission>, IUserPermissionRepo
    {
        public UserPermissionRepo(ApplicationDbContext context) : base(context) { }

        public async Task AssignPermissionsToUser(string email, List<Guid> permissionIds)
        {
            // Retrieve user with permissions included
            var user = await _context.Users
                                .Include(u => u.UserPermissions)
                                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                throw new ArgumentException("User not found");

            // Retrieve all permissions in a single call
            var permissionEntities = await _context.Permissions
                                                   .Where(p => permissionIds.Contains(p.Id))
                                                   .ToListAsync();

            if (permissionEntities.Count != permissionIds.Count)
                throw new ArgumentException("One or more permissions not found");

            // Filter permissions that the user doesn't already have
            var newPermissions = permissionEntities
                                 .Where(pe => !user.UserPermissions.Any(up => up.PermissionId == pe.Id))
                                 .Select(pe => new UserPermission
                                 {
                                     UserId = user.Id,
                                     PermissionId = pe.Id,
                                     CreatedBy = "Admin"
                                 })
                                 .ToList();

            // Check if there are any permissions to add
            if (newPermissions.Count == 0)
                throw new InvalidOperationException("User already has all requested permissions");

            // Add new permissions and save changes
            foreach (var permission in newPermissions)
            {
                user.UserPermissions.Add(permission);
            }
            await _context.SaveChangesAsync();
        }


        public async Task UpdatePermissionsForUser(string email, List<Guid> permissionIds)
        {
            var user = await _context.Users
                                .Include(u => u.UserPermissions)
                                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                throw new ArgumentException("User not found");

            var permissionEntities = await _context.Permissions
                                                   .Where(p => permissionIds.Contains(p.Id))
                                                   .ToListAsync();

            if (permissionEntities.Count != permissionIds.Count)
                throw new ArgumentException("One or more permissions not found");

            var permissionsToAdd = permissionEntities
                                   .Where(pe => !user.UserPermissions.Any(up => up.PermissionId == pe.Id))
                                   .Select(pe => new UserPermission
                                   {
                                       UserId = user.Id,
                                       PermissionId = pe.Id,
                                       CreatedBy = "Admin"
                                   })
                                   .ToList();

            var permissionsToRemove = user.UserPermissions
                                           .Where(up => !permissionIds.Contains(up.PermissionId))
                                           .ToList();

            foreach (var permission in permissionsToAdd)
            {
                user.UserPermissions.Add(permission);
            }

           
            foreach (var permission in permissionsToRemove)
            {
                user.UserPermissions.Remove(permission);
            }

            await _context.SaveChangesAsync();
        }
    }
}
