using AspireCleanArch.Domain.Exceptions;

namespace AspireCleanArch.Domain.Entities
{
    /// <summary>
    /// ProductSpecification entity - owned by Product aggregate
    /// </summary>
    public class ProductSpecification : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Value { get; private set; } = string.Empty;

        // EF Core navigation
        public Product Product { get; private set; } = null!;

        // EF Core constructor
        private ProductSpecification() : base(Guid.NewGuid()) { }

        // Factory method
        internal static ProductSpecification Create(
            Guid productId,
            string name,
            string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ProductValidationException("Specification name is required");

            if (string.IsNullOrWhiteSpace(value))
                throw new ProductValidationException("Specification value is required");

            var specification = new ProductSpecification
            {
                ProductId = productId,
                Name = name,
                Value = value
            };

            specification.SetCreatedAt(DateTime.UtcNow);
            return specification;
        }

        // Business methods
        internal void UpdateValue(string newValue)
        {
            if (string.IsNullOrWhiteSpace(newValue))
                throw new ProductValidationException("Specification value is required");

            Value = newValue;
            SetUpdatedAt(DateTime.UtcNow);
        }
    }
}
