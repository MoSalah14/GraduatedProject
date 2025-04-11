using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.UserPermissionsDTO
{
    public class UpdateUserPermissionDto
    {

        public string Email { get; set; }
        public List<Guid> PermissionIds { get; set; } = new List<Guid>();

    }
}
