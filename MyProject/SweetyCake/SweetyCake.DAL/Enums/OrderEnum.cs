

namespace OutbornE_commerce.DAL.Enums
{
    public enum OrderStatus
    {
        Pending = 0,
        Failed = 1,
        Confirmed = 2,
        Processing = 3,
        CANCELLED = 4,
        Returned = 5,
        Refunded = 6
    }

    public enum ShippedStatus
    {
        Shipped = 0,
        Delivered = 1,
        Processing = 2,
    }

    public enum PaymentMethod
    {
        Strip = 0,
        CashOnDelivery = 1,
        Wallet = 2
    }

    public enum PaymentStatus
    {
        Paid = 0,
        UnPaid = 1,
        // SiteWallet = 2
    }
}