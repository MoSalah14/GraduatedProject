using OutbornE_commerce.BAL.Dto.Address;
using OutbornE_commerce.DAL.Enums;
using System.Text.Json.Serialization;

namespace OutbornE_commerce.BAL.Dto.OrderDto
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public string UserId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatus Status { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentStatus PaymentStatus { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ShippedStatus ShippedStatus { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentMethod PaymentMethod { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid? AddressId { get; set; }
        public bool IsReverse { get; set; } = false;
        public AddressForCreationDto? Address { get; set; }
        public string? TrackingUrl { get; set; }
        public int? MinReturnDay { get; set; }

        public List<OrderItemDto.OrderItemDto>? OrderItems { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}