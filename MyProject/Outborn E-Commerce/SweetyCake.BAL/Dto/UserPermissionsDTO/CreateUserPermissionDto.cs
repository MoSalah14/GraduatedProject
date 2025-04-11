using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.UserPermissionsDTO
{
    public class CreateUserPermissionDto
    {
        public Guid PermissionId { get; set; }
        public string UserId { get; set; }


    }
}
