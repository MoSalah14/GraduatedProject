using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.External_Logins
{
    public class FacebookUserInfo
    {
        public string Id { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("picture")]

        public FacebookPictureData Picture { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
