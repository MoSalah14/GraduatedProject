using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.UserPermissionsDTO
{
    public class AssignPermissionDTO
    {
        public string Email { get; set; }
        public List<Guid> PermissionId { get; set; } = new List<Guid>();
    }
}
