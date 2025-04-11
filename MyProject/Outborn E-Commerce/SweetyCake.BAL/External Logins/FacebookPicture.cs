using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.External_Logins
{
    public class FacebookPicture
    {
        [JsonProperty("data")]
        public FacebookPictureData Data { get; set; }
    }
}
