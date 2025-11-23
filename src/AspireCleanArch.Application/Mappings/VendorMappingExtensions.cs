using AspireCleanArch.Domain.Entities;
using AspireCleanArch.Domain.Entities.ValueObjects;
using AspireCleanArch.Shared.Common;
using AspireCleanArch.Shared.DTOs.Product;

namespace AspireCleanArch.Application.Mappings
{
    /// <summary>
    /// Extension methods for mapping between Vendor domain entities and DTOs
    /// </summary>
    public static class VendorMappingExtensions
    {
        // ============================================
        // Domain Entity ? DTO (for queries/responses)
        // ============================================

        public static VendorDto ToDto(this Vendor vendor)
        {
            return new VendorDto
            {
                Id = vendor.Id,
                BusinessName = vendor.BusinessName,
                Email = vendor.Email,
                PhoneNumber = vendor.PhoneNumber,
                ContactPersonName = vendor.ContactPersonName,
                BusinessAddress = vendor.BusinessAddress.ToDto(),
                Status = vendor.Status.ToString(),
                CommissionRate = vendor.CommissionRate,
                TotalRevenue = vendor.TotalRevenue.Amount,
                CreatedAt = vendor.CreatedAt,
                ApprovedAt = vendor.ApprovedAt
            };
        }

        // ============================================
        // Value Object Mappings
        // ============================================

        public static AddressDto ToDto(this Address address)
        {
            return new AddressDto
            {
                Street = address.Street,
                City = address.City,
                State = address.State,
                ZipCode = address.ZipCode,
                Country = address.Country
            };
        }

        public static Address ToDomainAddress(this AddressDto dto)
        {
            return new Address
            {
                Street = dto.Street,
                City = dto.City,
                State = dto.State,
                ZipCode = dto.ZipCode,
                Country = dto.Country
            };
        }
    }
}
