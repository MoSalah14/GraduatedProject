using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto
{
    public class ResetPasswordModelDto
    {
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
    }
}
