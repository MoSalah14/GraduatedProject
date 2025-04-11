using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.UserPermissionsDTO
{
    public class GetUserPermissionDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<PermissionDetailDto> Permissions { get; set; } = new List<PermissionDetailDto>();

    }
    public class PermissionDetailDto
    {
        public Guid PermissionId { get; set; }
        public string PermissionName { get; set; }
       
    }
}
