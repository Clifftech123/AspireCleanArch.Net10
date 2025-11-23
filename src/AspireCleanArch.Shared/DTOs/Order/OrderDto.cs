namespace AspireCleanArch.Shared.DTOs.Order
{
    public record OrderDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
        public decimal SubtotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public ShippingAddressDto ShippingAddress { get; set; }
        public Guid? PaymentId { get; set; }
        public string? TrackingNumber { get; set; }
        public string? CourierService { get; set; }
        public List<OrderItemDto> Items { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
    }


    public record CreateOrderReques
    {
        public Guid UserId { get; set; }
        public List<CreateOrderItemRequest> Items { get; set; }
        public ShippingAddressDto ShippingAddress { get; set; }
        public decimal ShippingAmount { get; set; }
        public string? CustomerNotes { get; set; }
    }


    public record OrderItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSKU { get; set; }
        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; }
    }


    public record CreateOrderItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }


    public record CancelOrderRequest
    {
        public string Reason { get; set; }
    }


    public record UpdateOrderStatusRequest
    {
        public string Status { get; set; }
        public string? TrackingNumber { get; set; }
        public string? CourierService { get; set; }
    }
}
