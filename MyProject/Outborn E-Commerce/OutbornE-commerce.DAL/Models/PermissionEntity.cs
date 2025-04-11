using OutbornE_commerce.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class PermissionEntity : BaseEntity
    {
      
        public Permission Permission { get; set; }
        public TypePermission TypePermission { get; set; }
    }
}
