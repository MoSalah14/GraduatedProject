using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto.PermissionDTO;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.BAL.Repositories.ProductCateories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.PermissionRepo
{
    public class PermissionRepository : BaseRepository<PermissionEntity>, IpermissionRepository
    {
        private readonly UserManager<User> _userManager;

        public PermissionRepository(ApplicationDbContext context, UserManager<User> userManager) : base(context)
        {
            _userManager = userManager;
        }
      

        public async Task AddPermission(AddPermissionRequest permission)
        {


            if (await _context.Permissions.AnyAsync(p => p.Permission == permission.Permission))
            {
                throw new InvalidOperationException("This permission already exists in the database");
            }


            var newPermission = new PermissionEntity()
            {
                Permission = permission.Permission,
                TypePermission = permission.TypePermission,
                CreatedOn = DateTime.Now,
                CreatedBy = "Admin"
            };
            
            _context.Permissions.Add(newPermission);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserHasPermission(string userId, Permission permission)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            return user.UserPermissions.Any(up => up.Permission.Permission == permission);
        }

        public async Task<List<PermissionEntity>> GetPermissionsUser(string UserId)
     {
            var permissions = await _context.UserPermissions.Where(x => x.UserId == UserId).Select(x => x.Permission).ToListAsync();
            return permissions;
        }


    }


}

