namespace AspireCleanArch.Domain.Events
{
    public sealed record PaymentInitiatedEvent(
        Guid PaymentId,
        Guid OrderId,
        Guid UserId,
        decimal Amount,
        string Currency,
        string PaymentMethod
    ) : DomainEvent;

    public sealed record PaymentCompletedEvent(
        Guid PaymentId,
        Guid OrderId,
        string TransactionId,
        DateTime CompletedAt
    ) : DomainEvent;

    public sealed record PaymentFailedEvent(
        Guid PaymentId,
        Guid OrderId,
        string Reason,
        DateTime FailedAt
    ) : DomainEvent;

    public sealed record PaymentRefundedEvent(
        Guid PaymentId,
        Guid OrderId,
        decimal RefundAmount,
        string Currency,
        DateTime RefundedAt
    ) : DomainEvent;
}
