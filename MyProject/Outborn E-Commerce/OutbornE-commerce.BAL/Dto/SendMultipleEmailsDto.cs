using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto
{
    public class SendMultipleEmailsDto
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> Emails { get; set; }
    }
}
