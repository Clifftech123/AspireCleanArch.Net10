namespace AspireCleanArch.Domain.Events
{
    public sealed record OrderPlacedEvent(
        Guid OrderId,
        Guid UserId,
        string OrderNumber,
        decimal TotalAmount,
        string Currency,
        IReadOnlyList<OrderItemDto> Items
    ) : DomainEvent;

    public sealed record OrderPaymentConfirmedEvent(
        Guid OrderId,
        Guid PaymentId,
        DateTime PaymentConfirmedDate
    ) : DomainEvent;

    public sealed record OrderShippedEvent(
        Guid OrderId,
        string TrackingNumber,
        string CourierService,
        DateTime ShippedDate
    ) : DomainEvent;

    public sealed record OrderDeliveredEvent(
        Guid OrderId,
        DateTime DeliveredDate
    ) : DomainEvent;

    public sealed record OrderCancelledEvent(
        Guid OrderId,
        string Reason,
        DateTime CancelledDate
    ) : DomainEvent;

    public sealed record OrderCompletedEvent(
        Guid OrderId,
        DateTime CompletedDate
    ) : DomainEvent;

    // DTO for OrderItem data in events
    public sealed record OrderItemDto(
        Guid ProductId,
        Guid VendorId,
        string ProductName,
        int Quantity,
        decimal UnitPrice,
        decimal TotalPrice
    );
}
