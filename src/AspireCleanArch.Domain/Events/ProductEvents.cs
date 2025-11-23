namespace AspireCleanArch.Domain.Events
{
    public sealed record ProductCreatedEvent(
        Guid ProductId,
        Guid VendorId,
        string Name,
        decimal Price,
        string Currency
    ) : DomainEvent;

    public sealed record ProductPriceChangedEvent(
        Guid ProductId,
        decimal OldPrice,
        decimal NewPrice,
        string Currency
    ) : DomainEvent;

    public sealed record ProductStockUpdatedEvent(
        Guid ProductId,
        int OldStock,
        int NewStock
    ) : DomainEvent;

    public sealed record ProductPublishedEvent(
        Guid ProductId,
        DateTime PublishedAt
    ) : DomainEvent;

    public sealed record ProductDiscontinuedEvent(
        Guid ProductId,
        DateTime DiscontinuedAt
    ) : DomainEvent;
}
