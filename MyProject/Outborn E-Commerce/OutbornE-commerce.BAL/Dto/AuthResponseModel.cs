using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto
{
    public class AuthResponseModel
    {
        public string? Id { get; set; }
        public string? MessageEn { get; set; }
        public string? MessageAr { get; set; }
        public bool IsError { get; set; }
        public int StatusCode { get; set; }
        public string? Token { get; set; }
        public List<string>? Roles { get; set; }
        public List<string>? Permissions { get; set;}
        public string? RefreshToken { get; set; }
        public string? Email { get; set;}
        public int AccountType { get; set; }
       
    }
}
