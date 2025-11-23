namespace AspireCleanArch.Domain.Events
{
    public sealed record UserRegisteredEvent(
        Guid UserId,
        string Email,
        string FullName
    ) : DomainEvent;

    public sealed record UserRoleChangedEvent(
        Guid UserId,
        string OldRole,
        string NewRole
    ) : DomainEvent;

    public sealed record UserDeactivatedEvent(
        Guid UserId,
        DateTime DeactivatedAt
    ) : DomainEvent;

    public sealed record UserReactivatedEvent(
        Guid UserId,
        DateTime ReactivatedAt
    ) : DomainEvent;

    public sealed record UserEmailVerifiedEvent(
        Guid UserId,
        DateTime VerifiedAt
    ) : DomainEvent;
}
