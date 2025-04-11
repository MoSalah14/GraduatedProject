using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.SMTP_Server
{
    public class SMTPForCreationDto
    {
        public string Host { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
    }
}
