using OutbornE_commerce.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.PermissionDTO
{
    public class BaseAllPermissionsDto
    {
        public string TypePermission { get; set; }
        public List<GetAllPermissionDto> getAllPermissionDtos  { get; set; }

    }
}
