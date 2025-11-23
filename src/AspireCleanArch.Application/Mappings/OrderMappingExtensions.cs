using AspireCleanArch.Domain.Entities;
using AspireCleanArch.Domain.Entities.ValueObjects;
using AspireCleanArch.Shared.Common;
using AspireCleanArch.Shared.DTOs.Order;

namespace AspireCleanArch.Application.Mappings
{
    /// <summary>
    /// Extension methods for mapping between Order domain entities and DTOs
    /// </summary>
    public static class OrderMappingExtensions
    {
        // ============================================
        // Domain Entity → DTO (for queries/responses)
        // ============================================

        public static OrderDto ToDto(this Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                UserId = order.UserId,
                UserName = string.Empty, // Will be populated by join or separate query
                Status = order.Status.ToString(),
                SubtotalAmount = order.SubtotalAmount.Amount,
                TaxAmount = order.TaxAmount.Amount,
                ShippingAmount = order.ShippingAmount.Amount,
                DiscountAmount = order.DiscountAmount.Amount,
                TotalAmount = order.TotalAmount.Amount,
                Currency = order.TotalAmount.Currency,
                ShippingAddress = order.ShippingAddress.ToDto(),
                PaymentId = order.PaymentId,
                TrackingNumber = order.TrackingNumber,
                CourierService = order.CourierService,
                Items = order.Items.Select(i => i.ToDto()).ToList(),
                OrderDate = order.OrderDate,
                ShippedDate = order.ShippedDate,
                DeliveredDate = order.DeliveredDate
            };
        }

        public static OrderItemDto ToDto(this OrderItem orderItem)
        {
            return new OrderItemDto
            {
                Id = orderItem.Id,
                ProductId = orderItem.ProductId,
                ProductName = orderItem.ProductName,
                ProductSKU = orderItem.ProductSku ?? string.Empty,
                VendorId = orderItem.VendorId,
                VendorName = string.Empty, // Will be populated by join or separate query
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice.Amount,
                TotalPrice = orderItem.TotalPrice.Amount,
                Currency = orderItem.UnitPrice.Currency
            };
        }

        // ============================================
        // DTO → Domain Entity (for commands/creation)
        // ============================================

        /// <summary>
        /// Converts CreateOrderItemRequest DTOs to OrderItem domain entities
        /// Note: This requires product information which should be fetched from repository
        /// </summary>
        public static IEnumerable<OrderItem> ToOrderItems(
            this IEnumerable<CreateOrderItemRequest> requests,
            Dictionary<Guid, (string Name, string? Sku, Guid VendorId, Money Price, decimal TaxRate)> productInfo)
        {
            return requests.Select(request =>
            {
                if (!productInfo.TryGetValue(request.ProductId, out var info))
                    throw new ArgumentException($"Product info not found for ProductId: {request.ProductId}");

                return OrderItem.Create(
                    request.ProductId,
                    info.VendorId,
                    info.Name,
                    info.Sku,
                    request.Quantity,
                    info.Price,
                    info.TaxRate
                );
            });
        }

        // ============================================
        // Value Object Mappings
        // ============================================

        public static ShippingAddressDto ToDto(this ShippingAddress address)
        {
            return new ShippingAddressDto
            {
                Street = address.Street,
                City = address.City,
                State = address.State,
                ZipCode = address.ZipCode,
                Country = address.Country,
                RecipientName = address.RecipientName,
                PhoneNumber = address.PhoneNumber,
                DeliveryInstructions = address.DeliveryInstructions
            };
        }

        public static ShippingAddress ToDomainShippingAddress(this ShippingAddressDto dto)
        {
            return new ShippingAddress(
                dto.Street,
                dto.City,
                dto.State,
                dto.ZipCode,
                dto.Country,
                dto.RecipientName,
                dto.PhoneNumber,
                dto.DeliveryInstructions
            );
        }
    }
}
