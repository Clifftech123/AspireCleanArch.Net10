using AspireCleanArch.Application.Mappings;
using AspireCleanArch.Domain.Entities;
using AspireCleanArch.Domain.Entities.ValueObjects;
using AspireCleanArch.Shared.DTOs.Order;
using MediatR;

namespace AspireCleanArch.Application.Orders.Commands.CreateOrder
{
    /// <summary>
    /// Command to create a new order
    /// </summary>
    public record CreateOrderCommand : IRequest<OrderDto>
    {
        public Guid UserId { get; init; }
        public ShippingAddressDto ShippingAddress { get; init; } = null!;
        public List<CreateOrderItemRequest> Items { get; init; } = new();
        public decimal ShippingAmount { get; init; }
        public string Currency { get; init; } = "USD";
        public decimal? DiscountAmount { get; init; }
        public string? CustomerNotes { get; init; }
    }

    /// <summary>
    /// Handler for CreateOrderCommand - demonstrates manual mapping usage
    /// </summary>
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        // Repository interfaces would be injected here
        // private readonly IOrderRepository _orderRepository;
        // private readonly IProductRepository _productRepository;

        public async Task<OrderDto> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            // 1. Fetch product information from repository
            var productIds = command.Items.Select(i => i.ProductId).ToList();
            // var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

            // 2. Build product info dictionary for mapping
            var productInfo = new Dictionary<Guid, (string Name, string? Sku, Guid VendorId, Money Price, decimal TaxRate)>();
            
            // Example: populate from fetched products
            // foreach (var product in products)
            // {
            //     productInfo[product.Id] = (
            //         product.Name,
            //         product.SKU,
            //         product.VendorId,
            //         product.Price,
            //         0.10m // Tax rate - could come from config or product
            //     );
            // }

            // 3. Map DTO to Value Object using extension method
            var shippingAddress = command.ShippingAddress.ToDomainShippingAddress();

            // 4. Map DTOs to OrderItems using extension method
            var orderItems = command.Items.ToOrderItems(productInfo);

            // 5. Create Order using Domain factory method (NOT new Order()!)
            var order = Order.Create(
                command.UserId,
                shippingAddress,
                orderItems,
                new Money(command.ShippingAmount, command.Currency),
                command.DiscountAmount.HasValue 
                    ? new Money(command.DiscountAmount.Value, command.Currency) 
                    : null,
                command.CustomerNotes
            );

            // 6. Save to repository
            // await _orderRepository.AddAsync(order, cancellationToken);
            // await _orderRepository.SaveChangesAsync(cancellationToken);

            // 7. Map Domain Entity back to DTO using extension method
            return order.ToDto();
        }
    }
}
