namespace AspireCleanArch.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending = 0,
        Processing = 1,
        Completed = 2,
        Failed = 3,
        Refunded = 4,
        PartiallyRefunded = 5,
        Cancelled = 6
    }
}
