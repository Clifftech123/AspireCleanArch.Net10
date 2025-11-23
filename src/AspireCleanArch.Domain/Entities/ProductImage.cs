using AspireCleanArch.Domain.Exceptions;

namespace AspireCleanArch.Domain.Entities
{
    /// <summary>
    /// ProductImage entity - owned by Product aggregate
    /// </summary>
    public class ProductImage : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public string Url { get; private set; } = string.Empty;
        public string AltText { get; private set; } = string.Empty;
        public int DisplayOrder { get; private set; }
        public bool IsPrimary { get; private set; }

        // EF Core navigation
        public Product Product { get; private set; } = null!;

        // EF Core constructor
        private ProductImage() : base(Guid.NewGuid()) { }

        // Factory method
        internal static ProductImage Create(
            Guid productId,
            string url,
            string altText,
            int displayOrder,
            bool isPrimary = false)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ProductValidationException("Image URL is required");

            var image = new ProductImage
            {
                ProductId = productId,
                Url = url,
                AltText = altText ?? string.Empty,
                DisplayOrder = displayOrder,
                IsPrimary = isPrimary
            };

            image.SetCreatedAt(DateTime.UtcNow);
            return image;
        }

        // Business methods
        internal void SetAsPrimary()
        {
            IsPrimary = true;
            SetUpdatedAt(DateTime.UtcNow);
        }

        internal void SetNonPrimary()
        {
            IsPrimary = false;
            SetUpdatedAt(DateTime.UtcNow);
        }

        internal void UpdateOrder(int newOrder)
        {
            DisplayOrder = newOrder;
            SetUpdatedAt(DateTime.UtcNow);
        }
    }
}
