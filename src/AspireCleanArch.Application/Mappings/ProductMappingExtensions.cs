using AspireCleanArch.Domain.Entities;
using AspireCleanArch.Domain.Entities.ValueObjects;
using AspireCleanArch.Domain.Enums;
using AspireCleanArch.Shared.DTOs;

namespace AspireCleanArch.Application.Mappings
{
    /// <summary>
    /// Extension methods for mapping between Product domain entities and DTOs
    /// </summary>
    public static class ProductMappingExtensions
    {
        // ============================================
        // Domain Entity ? DTO (for queries/responses)
        // ============================================

        public static ProductDto ToDto(this Product product, string? vendorName = null)
        {
            return new ProductDto
            {
                Id = product.Id,
                VendorId = product.VendorId,
                VendorName = vendorName ?? string.Empty, // Populated from join or separate query
                Name = product.Name,
                Description = product.Description,
                SKU = product.SKU,
                Price = product.Price.Amount,
                Currency = product.Price.Currency,
                StockQuantity = product.StockQuantity,
                AvailableQuantity = product.AvailableQuantity,
                Category = product.Category.ToString(),
                Status = product.Status.ToString(),
                Weight = product.Weight,
                Brand = product.Brand,
                Images = product.Images.Select(i => i.ToDto()).ToList(),
                Specifications = product.Specifications.Select(s => s.ToDto()).ToList(),
                CreatedAt = product.CreatedAt
            };
        }

        public static ProductImageDto ToDto(this ProductImage image)
        {
            return new ProductImageDto
            {
                Id = image.Id,
                Url = image.Url,
                AltText = image.AltText,
                DisplayOrder = image.DisplayOrder,
                IsPrimary = image.IsPrimary
            };
        }

        public static ProductSpecificationDto ToDto(this ProductSpecification spec)
        {
            return new ProductSpecificationDto
            {
                Id = spec.Id,
                Name = spec.Name,
                Value = spec.Value
            };
        }

        // ============================================
        // DTO ? Domain Entity (for commands/creation)
        // ============================================

        /// <summary>
        /// Helper to parse category from DTO
        /// </summary>
        public static ProductCategory ToProductCategory(this string categoryString)
        {
            return Enum.TryParse<ProductCategory>(categoryString, out var category)
                ? category
                : throw new ArgumentException($"Invalid product category: {categoryString}");
        }

        /// <summary>
        /// Helper to parse category from int
        /// </summary>
        public static ProductCategory ToProductCategory(this int categoryInt)
        {
            return Enum.IsDefined(typeof(ProductCategory), categoryInt)
                ? (ProductCategory)categoryInt
                : throw new ArgumentException($"Invalid product category: {categoryInt}");
        }
    }
}
