namespace AspireCleanArch.Shared.DTOs
{

    public class ProductDto
    {
        public Guid Id { get; set; }
        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public int StockQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public decimal Weight { get; set; }
        public string? Brand { get; set; }
        public List<ProductImageDto> Images { get; set; }
        public List<ProductSpecificationDto> Specifications { get; set; }
        public DateTime CreatedAt { get; set; }
    }


    public class CreateProductRequest
    {
        public Guid VendorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public int StockQuantity { get; set; }
        public int Category { get; set; }
        public decimal Weight { get; set; }
        public string? Brand { get; set; }
        public string? Manufacturer { get; set; }
        public List<CreateProductImageRequest>? Images { get; set; }
        public List<CreateProductSpecificationRequest>? Specifications { get; set; }
    }


    public class UpdateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? Brand { get; set; }
    }


    public class UpdateStockRequest
    {
        public int Quantity { get; set; }
        public string Operation { get; set; } // "add" or "set"
    }


    public class ProductImageDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string AltText { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPrimary { get; set; }
    }

    public class CreateProductImageRequest
    {
        public string Url { get; set; }
        public string AltText { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPrimary { get; set; }
    }


    public class ProductSpecificationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }


    public class CreateProductSpecificationRequest
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
