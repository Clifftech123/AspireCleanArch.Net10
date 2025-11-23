namespace AspireCleanArch.Shared.DTOs.Payment
{
    public record PaymentDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string? TransactionId { get; set; }
        public string Provider { get; set; }
        public string? CardLast4Digits { get; set; }
        public string? CardBrand { get; set; }
        public DateTime InitiatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? FailureReason { get; set; }
    }


    public record ProcessPaymentDto
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public int PaymentMethod { get; set; }
        public PaymentDetailsDto PaymentDetails { get; set; }
    }


    public record PaymentDetailsDto
    {
        public string? CardNumber { get; set; }
        public string? CardHolderName { get; set; }
        public string? ExpiryMonth { get; set; }
        public string? ExpiryYear { get; set; }
        public string? CVV { get; set; }
        public string? PayPalEmail { get; set; }
    }


    public record RefundPaymentDto
    {
        public decimal Amount { get; set; }
        public string Reason { get; set; }
    }
}
