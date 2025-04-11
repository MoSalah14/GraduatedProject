using OutbornE_commerce.BAL.Dto.PermissionDTO;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.PermissionRepo
{
    public interface IpermissionRepository : IBaseRepository<PermissionEntity>
    {
      
        Task AddPermission(AddPermissionRequest permission);
        Task<bool> UserHasPermission(string userId, Permission permission);

        Task<List<PermissionEntity>> GetPermissionsUser(string UserId);
    }
}
