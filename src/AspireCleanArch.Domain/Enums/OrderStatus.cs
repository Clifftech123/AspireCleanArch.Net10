namespace AspireCleanArch.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 0,
        PaymentConfirmed = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4,
        Completed = 5,
        Cancelled = 6,
        Refunded = 7
    }
}
