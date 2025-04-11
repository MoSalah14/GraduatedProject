using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.External_Logins
{
    public class ExternalLoginResult
    {
        public string? Token { get; set; }

        public bool IsSuccessLogin { get; set; }

        public string ReturnedMessage { get; set; }
    }
}
