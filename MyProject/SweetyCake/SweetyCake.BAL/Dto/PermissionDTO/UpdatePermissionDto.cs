using OutbornE_commerce.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.PermissionDTO
{
    public class UpdatePermissionDto
    {
        public Guid Id { get; set; }
        public Permission Permission { get; set; }
        public TypePermission TypePermission { get; set; }
    }
}
