namespace AspireCleanArch.Domain.Events
{
    public sealed record VendorRegisteredEvent(
        Guid VendorId,
        Guid UserId,
        string BusinessName,
        string Email
    ) : DomainEvent;

    public sealed record VendorApprovedEvent(
        Guid VendorId,
        DateTime ApprovedAt
    ) : DomainEvent;

    public sealed record VendorRejectedEvent(
        Guid VendorId,
        string Reason,
        DateTime RejectedAt
    ) : DomainEvent;

    public sealed record VendorSuspendedEvent(
        Guid VendorId,
        string Reason,
        DateTime SuspendedAt
    ) : DomainEvent;

    public sealed record VendorReactivatedEvent(
        Guid VendorId,
        DateTime ReactivatedAt
    ) : DomainEvent;
}
