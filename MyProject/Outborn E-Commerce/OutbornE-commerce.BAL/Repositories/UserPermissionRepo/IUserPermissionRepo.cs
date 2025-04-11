using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.UserPermissionRepo
{
    public interface IUserPermissionRepo:IBaseRepository<UserPermission>
    {
       
        Task AssignPermissionsToUser(string email, List<Guid> permissionIds);
        Task UpdatePermissionsForUser(string email, List<Guid> permissionIds);
    }
}
