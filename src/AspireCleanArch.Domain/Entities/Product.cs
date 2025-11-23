using AspireCleanArch.Domain.Enums;
using AspireCleanArch.Domain.Entities.ValueObjects;
using AspireCleanArch.Domain.Events;
using AspireCleanArch.Domain.Exceptions;

namespace AspireCleanArch.Domain.Entities
{
    /// <summary>
    /// Product aggregate root - represents a product in the marketplace
    /// </summary>
    public class Product : BaseEntity
    {
        public Guid VendorId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string SKU { get; private set; } = string.Empty;
        public Money Price { get; private set; } = Money.Zero;
        public int StockQuantity { get; private set; }
        public int ReservedQuantity { get; private set; }
        public int AvailableQuantity => StockQuantity - ReservedQuantity;
        public ProductCategory Category { get; private set; }
        public ProductStatus Status { get; private set; }
        public decimal Weight { get; private set; }
        public string? Brand { get; private set; }
        public string? Manufacturer { get; private set; }

        private readonly List<ProductImage> _images = new();
        public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

        private readonly List<ProductSpecification> _specifications = new();
        public IReadOnlyCollection<ProductSpecification> Specifications => _specifications.AsReadOnly();

        // EF Core constructor
        private Product() : base(Guid.NewGuid()) { }

        // Factory method for creating product
        public static Product Create(
            Guid vendorId,
            string name,
            string description,
            string sku,
            Money price,
            ProductCategory category,
            int initialStock = 0,
            decimal weight = 0,
            string? brand = null,
            string? manufacturer = null)
        {
            if (vendorId == Guid.Empty)
                throw new ProductValidationException("Vendor ID is required");

            if (string.IsNullOrWhiteSpace(name))
                throw new ProductValidationException("Product name is required");

            if (string.IsNullOrWhiteSpace(sku))
                throw new ProductValidationException("SKU is required");

            if (price.Amount < 0)
                throw new ProductValidationException("Price cannot be negative");

            if (initialStock < 0)
                throw new ProductValidationException("Stock quantity cannot be negative");

            var product = new Product
            {
                VendorId = vendorId,
                Name = name,
                Description = description,
                SKU = sku.ToUpperInvariant(),
                Price = price,
                Category = category,
                StockQuantity = initialStock,
                ReservedQuantity = 0,
                Status = ProductStatus.Draft,
                Weight = weight,
                Brand = brand,
                Manufacturer = manufacturer
            };

            product.SetCreatedAt(DateTime.UtcNow);

            product.RaiseDomainEvent(new ProductCreatedEvent(
                product.Id,
                vendorId,
                name,
                price.Amount,
                price.Currency
            ));

            return product;
        }

        // Business methods

        public void UpdateDetails(string name, string description, ProductCategory category, decimal weight, string? brand, string? manufacturer)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ProductValidationException("Product name is required");

            Name = name;
            Description = description;
            Category = category;
            Weight = weight;
            Brand = brand;
            Manufacturer = manufacturer;
            SetUpdatedAt(DateTime.UtcNow);
        }

        public void UpdatePrice(Money newPrice)
        {
            if (newPrice.Amount < 0)
                throw new ProductValidationException("Price cannot be negative");

            var oldPrice = Price;
            Price = newPrice;
            SetUpdatedAt(DateTime.UtcNow);

            RaiseDomainEvent(new ProductPriceChangedEvent(
                Id,
                oldPrice.Amount,
                newPrice.Amount,
                newPrice.Currency
            ));
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0)
                throw new ProductValidationException("Quantity must be positive");

            var oldStock = StockQuantity;
            StockQuantity += quantity;
            SetUpdatedAt(DateTime.UtcNow);

            // Update status if product was out of stock
            if (Status == ProductStatus.OutOfStock && AvailableQuantity > 0)
            {
                Status = ProductStatus.Active;
            }

            RaiseDomainEvent(new ProductStockUpdatedEvent(
                Id,
                oldStock,
                StockQuantity
            ));
        }

        public void RemoveStock(int quantity)
        {
            if (quantity <= 0)
                throw new ProductValidationException("Quantity must be positive");

            if (quantity > AvailableQuantity)
                throw new InsufficientStockException($"Cannot remove {quantity} items. Only {AvailableQuantity} available");

            var oldStock = StockQuantity;
            StockQuantity -= quantity;
            SetUpdatedAt(DateTime.UtcNow);

            // Update status if product is now out of stock
            if (AvailableQuantity == 0 && Status == ProductStatus.Active)
            {
                Status = ProductStatus.OutOfStock;
            }

            RaiseDomainEvent(new ProductStockUpdatedEvent(
                Id,
                oldStock,
                StockQuantity
            ));
        }

        public void ReserveStock(int quantity)
        {
            if (quantity <= 0)
                throw new ProductValidationException("Quantity must be positive");

            if (quantity > AvailableQuantity)
                throw new InsufficientStockException($"Cannot reserve {quantity} items. Only {AvailableQuantity} available");

            ReservedQuantity += quantity;
            SetUpdatedAt(DateTime.UtcNow);

            // Update status if all stock is now reserved
            if (AvailableQuantity == 0 && Status == ProductStatus.Active)
            {
                Status = ProductStatus.OutOfStock;
            }
        }

        public void ReleaseReservedStock(int quantity)
        {
            if (quantity <= 0)
                throw new ProductValidationException("Quantity must be positive");

            if (quantity > ReservedQuantity)
                throw new ProductValidationException($"Cannot release {quantity} items. Only {ReservedQuantity} reserved");

            ReservedQuantity -= quantity;
            SetUpdatedAt(DateTime.UtcNow);

            // Update status if product is now available
            if (Status == ProductStatus.OutOfStock && AvailableQuantity > 0)
            {
                Status = ProductStatus.Active;
            }
        }

        public void ConfirmReservation(int quantity)
        {
            if (quantity <= 0)
                throw new ProductValidationException("Quantity must be positive");

            if (quantity > ReservedQuantity)
                throw new ProductValidationException($"Cannot confirm {quantity} items. Only {ReservedQuantity} reserved");

            ReservedQuantity -= quantity;
            StockQuantity -= quantity;
            SetUpdatedAt(DateTime.UtcNow);
        }

        public void Publish()
        {
            if (Status != ProductStatus.Draft)
                throw new InvalidProductStateException($"Cannot publish product in {Status} state");

            if (!_images.Any(i => i.IsPrimary))
                throw new ProductValidationException("Product must have a primary image before publishing");

            if (Price.Amount <= 0)
                throw new ProductValidationException("Product must have a valid price before publishing");

            Status = AvailableQuantity > 0 ? ProductStatus.Active : ProductStatus.OutOfStock;
            SetUpdatedAt(DateTime.UtcNow);

            RaiseDomainEvent(new ProductPublishedEvent(
                Id,
                DateTime.UtcNow
            ));
        }

        public void Discontinue()
        {
            if (Status == ProductStatus.Discontinued)
                return;

            Status = ProductStatus.Discontinued;
            SetUpdatedAt(DateTime.UtcNow);

            RaiseDomainEvent(new ProductDiscontinuedEvent(
                Id,
                DateTime.UtcNow
            ));
        }

        public void AddImage(string url, string altText, int displayOrder, bool isPrimary = false)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ProductValidationException("Image URL is required");

            // If setting as primary, unset other primary images
            if (isPrimary)
            {
                foreach (var image in _images.Where(i => i.IsPrimary))
                {
                    image.SetNonPrimary();
                }
            }

            var productImage = ProductImage.Create(Id, url, altText, displayOrder, isPrimary);
            _images.Add(productImage);
            SetUpdatedAt(DateTime.UtcNow);
        }

        public void RemoveImage(Guid imageId)
        {
            var image = _images.FirstOrDefault(i => i.Id == imageId);
            if (image == null)
                throw new ProductValidationException($"Image with ID {imageId} not found");

            _images.Remove(image);
            SetUpdatedAt(DateTime.UtcNow);
        }

        public void AddSpecification(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ProductValidationException("Specification name is required");

            var specification = ProductSpecification.Create(Id, name, value);
            _specifications.Add(specification);
            SetUpdatedAt(DateTime.UtcNow);
        }

        public void RemoveSpecification(Guid specificationId)
        {
            var spec = _specifications.FirstOrDefault(s => s.Id == specificationId);
            if (spec == null)
                throw new ProductValidationException($"Specification with ID {specificationId} not found");

            _specifications.Remove(spec);
            SetUpdatedAt(DateTime.UtcNow);
        }

        // Query methods
        public bool IsPublished() => Status == ProductStatus.Active || Status == ProductStatus.OutOfStock;
        public bool IsAvailable() => Status == ProductStatus.Active && AvailableQuantity > 0;
        public bool IsOutOfStock() => AvailableQuantity == 0;
        public bool HasSufficientStock(int quantity) => AvailableQuantity >= quantity;
        public ProductImage? GetPrimaryImage() => _images.FirstOrDefault(i => i.IsPrimary);
    }
}
