using AspireCleanArch.Domain.Events;

namespace AspireCleanArch.Domain.Entities
{
    public abstract class BaseEntity
    {
        private readonly List<DomainEvent> _domainEvents = new();

        public Guid Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public bool IsDeleted { get; private set; } = false;

        /// <summary>
        /// Domain events raised by this entity
        /// </summary>
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected BaseEntity(Guid id)
        {
            Id = id;
        }

        // EF Core requires parameterless constructor
        protected BaseEntity() : this(Guid.NewGuid()) { }

        public void SetId(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException($"{nameof(id)} may not be empty");
            }

            Id = id;
        }

        public void SetCreatedAt(DateTime createdAt)
        {
            CreatedAt = createdAt;
        }

        public void SetUpdatedAt(DateTime updatedAt)
        {
            UpdatedAt = updatedAt;
        }

        public void SetDeletedAt(DateTime deletedAt)
        {
            DeletedAt = deletedAt;
            IsDeleted = true;
        }

        /// <summary>
        /// Adds a domain event to be dispatched
        /// </summary>
        protected void RaiseDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// Clears all domain events (called after dispatching)
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
