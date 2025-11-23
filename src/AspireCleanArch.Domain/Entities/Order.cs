using AspireCleanArch.Domain.Enums;
using AspireCleanArch.Domain.Events;
using AspireCleanArch.Domain.Exceptions;
using AspireCleanArch.Domain.Entities.ValueObjects;

namespace AspireCleanArch.Domain.Entities
{
    /// <summary>
    /// Order aggregate root - encapsulates order business logic
    /// </summary>
    public class Order : BaseEntity
    {
        // Strongly-typed identifiers
        public Guid UserId { get; private set; }
        public string OrderNumber { get; private set; } = string.Empty;
        public OrderStatus Status { get; private set; }
        
        // Monetary values
        public Money SubtotalAmount { get; private set; } = Money.Zero;
        public Money TaxAmount { get; private set; } = Money.Zero;
        public Money ShippingAmount { get; private set; } = Money.Zero;
        public Money DiscountAmount { get; private set; } = Money.Zero;
        public Money TotalAmount { get; private set; } = Money.Zero;
        
        // Address and shipping info
        public ShippingAddress ShippingAddress { get; private set; } = null!;
        public string? TrackingNumber { get; private set; }
        public string? CourierService { get; private set; }
        
        // Payment info
        public Guid? PaymentId { get; private set; }
        
        // Notes
        public string? CustomerNotes { get; private set; }
        public string? InternalNotes { get; private set; }

        // Order items collection - private backing field for aggregate control
        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        // Important dates
        public DateTime OrderDate { get; private set; }
        public DateTime? PaymentConfirmedDate { get; private set; }
        public DateTime? ShippedDate { get; private set; }
        public DateTime? DeliveredDate { get; private set; }
        public DateTime? CompletedDate { get; private set; }
        public DateTime? CancelledDate { get; private set; }
        public string? CancellationReason { get; private set; }

        // EF Core requires parameterless constructor
        private Order() : base(Guid.NewGuid()) { }

        // Factory method for creating new orders
        public static Order Create(
            Guid userId,
            ShippingAddress shippingAddress,
            IEnumerable<OrderItem> items,
            Money shippingAmount,
            Money discountAmount = null!,
            string? customerNotes = null)
        {
            if (userId == Guid.Empty)
                throw new OrderValidationException("User ID cannot be empty");

            if (shippingAddress == null)
                throw new OrderValidationException("Shipping address is required");

            var itemsList = items.ToList();
            if (!itemsList.Any())
                throw new OrderValidationException("Order must contain at least one item");

            var order = new Order
            {
                UserId = userId,
                OrderNumber = GenerateOrderNumber(),
                Status = OrderStatus.Pending,
                ShippingAddress = shippingAddress,
                ShippingAmount = shippingAmount ?? Money.Zero,
                DiscountAmount = discountAmount ?? Money.Zero,
                CustomerNotes = customerNotes,
                OrderDate = DateTime.UtcNow
            };

            order.SetCreatedAt(DateTime.UtcNow);

            // Add items and set their OrderId
            foreach (var item in itemsList)
            {
                order.AddItem(item);
            }

            // Calculate totals
            order.RecalculateTotals();

            // Raise domain event
            order.RaiseDomainEvent(new OrderPlacedEvent(
                order.Id,
                order.UserId,
                order.OrderNumber,
                order.TotalAmount.Amount,
                order.TotalAmount.Currency,
                order._items.Select(i => new OrderItemDto(
                    i.ProductId,
                    i.VendorId,
                    i.ProductName,
                    i.Quantity,
                    i.UnitPrice.Amount,
                    i.TotalPrice.Amount
                )).ToList()
            ));

            return order;
        }

        // Business methods

        /// <summary>
        /// Adds an item to the order (only valid for Pending orders)
        /// </summary>
        public void AddItem(OrderItem item)
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOrderStateException($"Cannot add items to order in {Status} state");

            if (item == null)
                throw new OrderItemValidationException("Order item cannot be null");

            // Check if item with same product already exists
            var existingItem = _items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + item.Quantity);
            }
            else
            {
                item.SetOrderId(Id);
                _items.Add(item);
            }

            RecalculateTotals();
            SetUpdatedAt(DateTime.UtcNow);
        }

        /// <summary>
        /// Removes an item from the order (only valid for Pending orders)
        /// </summary>
        public void RemoveItem(Guid orderItemId)
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOrderStateException($"Cannot remove items from order in {Status} state");

            var item = _items.FirstOrDefault(i => i.Id == orderItemId);
            if (item == null)
                throw new OrderItemValidationException($"Order item with ID {orderItemId} not found");

            _items.Remove(item);

            if (!_items.Any())
                throw new OrderValidationException("Order must contain at least one item");

            RecalculateTotals();
            SetUpdatedAt(DateTime.UtcNow);
        }

        /// <summary>
        /// Confirms payment for the order
        /// </summary>
        public void ConfirmPayment(Guid paymentId)
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOrderStateException($"Cannot confirm payment for order in {Status} state");

            if (paymentId == Guid.Empty)
                throw new OrderValidationException("Payment ID cannot be empty");

            PaymentId = paymentId;
            Status = OrderStatus.PaymentConfirmed;
            PaymentConfirmedDate = DateTime.UtcNow;
            SetUpdatedAt(DateTime.UtcNow);

            RaiseDomainEvent(new OrderPaymentConfirmedEvent(
                Id,
                paymentId,
                PaymentConfirmedDate.Value
            ));
        }

        /// <summary>
        /// Marks order as processing
        /// </summary>
        public void StartProcessing()
        {
            if (Status != OrderStatus.PaymentConfirmed)
                throw new InvalidOrderStateException($"Cannot start processing order in {Status} state");

            Status = OrderStatus.Processing;
            SetUpdatedAt(DateTime.UtcNow);
        }

        /// <summary>
        /// Ships the order with tracking information
        /// </summary>
        public void Ship(string trackingNumber, string courierService)
        {
            if (Status != OrderStatus.Processing)
                throw new InvalidOrderStateException($"Cannot ship order in {Status} state");

            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new OrderValidationException("Tracking number is required");

            if (string.IsNullOrWhiteSpace(courierService))
                throw new OrderValidationException("Courier service is required");

            TrackingNumber = trackingNumber;
            CourierService = courierService;
            Status = OrderStatus.Shipped;
            ShippedDate = DateTime.UtcNow;
            SetUpdatedAt(DateTime.UtcNow);

            RaiseDomainEvent(new OrderShippedEvent(
                Id,
                trackingNumber,
                courierService,
                ShippedDate.Value
            ));
        }

        /// <summary>
        /// Marks order as delivered
        /// </summary>
        public void MarkAsDelivered()
        {
            if (Status != OrderStatus.Shipped)
                throw new InvalidOrderStateException($"Cannot mark order as delivered in {Status} state");

            Status = OrderStatus.Delivered;
            DeliveredDate = DateTime.UtcNow;
            SetUpdatedAt(DateTime.UtcNow);

            RaiseDomainEvent(new OrderDeliveredEvent(
                Id,
                DeliveredDate.Value
            ));
        }

        /// <summary>
        /// Completes the order (final state)
        /// </summary>
        public void Complete()
        {
            if (Status != OrderStatus.Delivered)
                throw new InvalidOrderStateException($"Cannot complete order in {Status} state");

            Status = OrderStatus.Completed;
            CompletedDate = DateTime.UtcNow;
            SetUpdatedAt(DateTime.UtcNow);

            RaiseDomainEvent(new OrderCompletedEvent(
                Id,
                CompletedDate.Value
            ));
        }

        /// <summary>
        /// Cancels the order with a reason
        /// </summary>
        public void Cancel(string reason)
        {
            if (Status == OrderStatus.Completed || Status == OrderStatus.Cancelled)
                throw new InvalidOrderStateException($"Cannot cancel order in {Status} state");

            if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
                throw new InvalidOrderStateException("Cannot cancel order that has been shipped or delivered");

            if (string.IsNullOrWhiteSpace(reason))
                throw new OrderValidationException("Cancellation reason is required");

            Status = OrderStatus.Cancelled;
            CancellationReason = reason;
            CancelledDate = DateTime.UtcNow;
            SetUpdatedAt(DateTime.UtcNow);

            RaiseDomainEvent(new OrderCancelledEvent(
                Id,
                reason,
                CancelledDate.Value
            ));
        }

        /// <summary>
        /// Updates shipping address (only for pending orders)
        /// </summary>
        public void UpdateShippingAddress(ShippingAddress newAddress)
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOrderStateException($"Cannot update shipping address for order in {Status} state");

            if (newAddress == null)
                throw new OrderValidationException("Shipping address cannot be null");

            ShippingAddress = newAddress;
            SetUpdatedAt(DateTime.UtcNow);
        }

        /// <summary>
        /// Applies a discount to the order
        /// </summary>
        public void ApplyDiscount(Money discountAmount)
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOrderStateException($"Cannot apply discount to order in {Status} state");

            if (discountAmount.Amount < 0)
                throw new OrderValidationException("Discount amount cannot be negative");

            DiscountAmount = discountAmount;
            RecalculateTotals();
            SetUpdatedAt(DateTime.UtcNow);
        }

        /// <summary>
        /// Updates internal notes (admin only)
        /// </summary>
        public void UpdateInternalNotes(string notes)
        {
            InternalNotes = notes;
            SetUpdatedAt(DateTime.UtcNow);
        }

        // Helper methods

        private void RecalculateTotals()
        {
            if (!_items.Any())
            {
                SubtotalAmount = Money.Zero;
                TaxAmount = Money.Zero;
                TotalAmount = Money.Zero;
                return;
            }

            var currency = _items.First().UnitPrice.Currency;

            SubtotalAmount = new Money(
                _items.Sum(i => i.TotalPrice.Amount - i.TaxAmount.Amount),
                currency
            );

            TaxAmount = new Money(
                _items.Sum(i => i.TaxAmount.Amount),
                currency
            );

            var subtotalWithTax = SubtotalAmount.Amount + TaxAmount.Amount;
            var totalBeforeDiscount = subtotalWithTax + ShippingAmount.Amount;
            var finalTotal = totalBeforeDiscount - DiscountAmount.Amount;

            TotalAmount = new Money(Math.Max(0, finalTotal), currency);
        }

        private static string GenerateOrderNumber()
        {
            // Format: ORD-YYYYMMDD-XXXXX
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = Random.Shared.Next(10000, 99999);
            return $"ORD-{datePart}-{randomPart}";
        }

        // Query methods
        public bool CanBeCancelled() => Status is OrderStatus.Pending or OrderStatus.PaymentConfirmed or OrderStatus.Processing;
        
        public bool CanBeModified() => Status == OrderStatus.Pending;
        
        public bool IsInFinalState() => Status is OrderStatus.Completed or OrderStatus.Cancelled or OrderStatus.Refunded;
        
        public int GetTotalItemCount() => _items.Sum(i => i.Quantity);
        
        public IEnumerable<Guid> GetVendorIds() => _items.Select(i => i.VendorId).Distinct();
    }
}
