using Newtonsoft.Json;

namespace OutbornE_commerce.BAL.External_Logins
{
    public class FacebookPictureData
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}