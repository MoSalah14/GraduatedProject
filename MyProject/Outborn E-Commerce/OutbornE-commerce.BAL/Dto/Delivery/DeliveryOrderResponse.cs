using Newtonsoft.Json;

namespace OutbornE_commerce.BAL.Dto.Delivery_Order_Response
{
    public class DeliveryOrderResponse
    {
        [JsonProperty("reference")]
        public string? Reference { get; set; }

        [JsonProperty("swftboxTracking")]
        public string? TrackingNumber { get; set; }

        [JsonProperty("trackingUrl")]
        public string? TrackingUrl { get; set; }

        [JsonProperty("shippingLabelUrl")]
        public string? ShippingLabelUrl { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("isReverse")]
        public bool? IsReverse { get; set; }

        // Use It for Map Json to Object
    }

    public class DeliveryOrderDataResponse
    {
        [JsonProperty("data")]
        public List<DeliveryOrderResponse>? Data { get; set; }
    }
}