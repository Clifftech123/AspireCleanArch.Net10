using AspireCleanArch.Shared.Common;

namespace AspireCleanArch.Shared.DTOs.Product
{
    public record VendorDto
    {
        public Guid Id { get; set; }
        public string BusinessName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactPersonName { get; set; }
        public AddressDto BusinessAddress { get; set; }
        public string Status { get; set; }
        public decimal CommissionRate { get; set; }
        public decimal TotalRevenue { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }


    public record CreateVendorRequest
    {
        public string BusinessName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactPersonName { get; set; }
        public AddressDto BusinessAddress { get; set; }
        public string? TaxIdentificationNumber { get; set; }
        public string? BankAccountNumber { get; set; }
    }


    public class UpdateVendorRequest
    {
        public string BusinessName { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactPersonName { get; set; }
        public AddressDto BusinessAddress { get; set; }
    }


    public class VendorAnalyticsDto
    {
        public Guid VendorId { get; set; }
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int ProductsSold { get; set; }
    }
}
