using Newtonsoft.Json;
using OutbornE_commerce.BAL.Dto.Delivery_Order_Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Delivery
{
    public class DeliveryWebhookResponse
    {
        [JsonProperty("eventTrigger")]
        public string EventTrigger { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("order")]
        public DeliveryOrderResponse Order { get; set; }
    }
}