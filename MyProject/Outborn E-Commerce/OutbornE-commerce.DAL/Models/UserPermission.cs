using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class UserPermission 
    {
        public string UserId { get; set; }
        public User User { get; set; }

        public Guid PermissionId { get; set; }
        public PermissionEntity Permission { get; set; }

        public string CreatedBy { get; set; } = "Admin";
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
