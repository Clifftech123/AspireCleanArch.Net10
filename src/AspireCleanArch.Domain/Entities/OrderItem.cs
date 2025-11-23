using AspireCleanArch.Domain.Entities.ValueObjects;

namespace AspireCleanArch.Domain.Entities
{
    /// <summary>
    /// Represents an item within an order (Entity owned by Order aggregate)
    /// </summary>
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public Guid VendorId { get; private set; }
        public string ProductName { get; private set; }
        public string? ProductSku { get; private set; }
        public int Quantity { get; private set; }
        public Money UnitPrice { get; private set; }
        public Money TotalPrice { get; private set; }
        public decimal TaxRate { get; private set; }
        public Money TaxAmount { get; private set; }

        // EF Core navigation property
        public Order Order { get; private set; } = null!;

        // EF Core requires parameterless constructor
        private OrderItem() : base(Guid.NewGuid()) { }

        // Factory method for creating OrderItem
        public static OrderItem Create(
            Guid productId,
            Guid vendorId,
            string productName,
            string? productSku,
            int quantity,
            Money unitPrice,
            decimal taxRate)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
            
            if (unitPrice.Amount < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

            if (taxRate < 0 || taxRate > 1)
                throw new ArgumentException("Tax rate must be between 0 and 1", nameof(taxRate));

            var subtotal = new Money(unitPrice.Amount * quantity, unitPrice.Currency);
            var taxAmount = new Money(subtotal.Amount * taxRate, unitPrice.Currency);
            var totalPrice = new Money(subtotal.Amount + taxAmount.Amount, unitPrice.Currency);

            var orderItem = new OrderItem
            {
                ProductId = productId,
                VendorId = vendorId,
                ProductName = productName,
                ProductSku = productSku,
                Quantity = quantity,
                UnitPrice = unitPrice,
                TaxRate = taxRate,
                TaxAmount = taxAmount,
                TotalPrice = totalPrice
            };

            orderItem.SetCreatedAt(DateTime.UtcNow);
            return orderItem;
        }

        // Business methods
        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));

            Quantity = newQuantity;
            RecalculateAmounts();
        }

        public void UpdatePrice(Money newUnitPrice)
        {
            if (newUnitPrice.Amount < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(newUnitPrice));

            UnitPrice = newUnitPrice;
            RecalculateAmounts();
        }

        private void RecalculateAmounts()
        {
            var subtotal = new Money(UnitPrice.Amount * Quantity, UnitPrice.Currency);
            TaxAmount = new Money(subtotal.Amount * TaxRate, UnitPrice.Currency);
            TotalPrice = new Money(subtotal.Amount + TaxAmount.Amount, UnitPrice.Currency);
            SetUpdatedAt(DateTime.UtcNow);
        }

        internal void SetOrderId(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
