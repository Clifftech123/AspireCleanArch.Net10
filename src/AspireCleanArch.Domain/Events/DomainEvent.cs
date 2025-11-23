namespace AspireCleanArch.Domain.Events
{
    /// <summary>
    /// Base class for all domain events
    /// </summary>
    public abstract record DomainEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }
}
