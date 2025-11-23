using AspireCleanArch.Domain.Enums;
using AspireCleanArch.Domain.Entities.ValueObjects;
using AspireCleanArch.Domain.Events;
using AspireCleanArch.Domain.Exceptions;

namespace AspireCleanArch.Domain.Entities
{
    /// <summary>
    /// Payment aggregate root - represents a payment transaction
    /// </summary>
    public class Payment : BaseEntity
    {
        public Guid OrderId { get; private set; }
        public Guid UserId { get; private set; }
        public Money Amount { get; private set; } = Money.Zero;
        public PaymentMethod PaymentMethod { get; private set; }
        public PaymentStatus Status { get; private set; }
        public string? TransactionId { get; private set; }
        public string? PaymentGatewayResponse { get; private set; }
        public PaymentProvider Provider { get; private set; }
        public string? CardLast4Digits { get; private set; }
        public string? CardBrand { get; private set; }

        public DateTime InitiatedAt { get; private set; }
        public DateTime? ProcessingAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public DateTime? FailedAt { get; private set; }
        public string? FailureReason { get; private set; }
        public string? RefundTransactionId { get; private set; }
        public DateTime? RefundedAt { get; private set; }
        public Money? RefundAmount { get; private set; }

        // EF Core constructor
        private Payment() : base(Guid.NewGuid()) { }

        // Factory method for initiating payment
        public static Payment Initiate(
            Guid orderId,
            Guid userId,
            Money amount,
            PaymentMethod paymentMethod,
            PaymentProvider provider,
            string? cardLast4Digits = null,
            string? cardBrand = null)
        {
            if (orderId == Guid.Empty)
                throw new PaymentValidationException("Order ID is required");

            if (userId == Guid.Empty)
                throw new PaymentValidationException("User ID is required");

            if (amount.Amount <= 0)
                throw new PaymentValidationException("Payment amount must be positive");

            var payment = new Payment
            {
                OrderId = orderId,
                UserId = userId,
                Amount = amount,
                PaymentMethod = paymentMethod,
                Provider = provider,
                Status = PaymentStatus.Pending,
                InitiatedAt = DateTime.UtcNow,
                CardLast4Digits = cardLast4Digits,
                CardBrand = cardBrand
            };

            payment.SetCreatedAt(DateTime.UtcNow);

            payment.RaiseDomainEvent(new PaymentInitiatedEvent(
                payment.Id,
                orderId,
                userId,
                amount.Amount,
                amount.Currency,
                paymentMethod.ToString()
            ));

            return payment;
        }

        // Business methods

        public void MarkAsProcessing()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidPaymentStateException($"Cannot mark payment as processing in {Status} state");

            Status = PaymentStatus.Processing;
            ProcessingAt = DateTime.UtcNow;
            SetUpdatedAt(DateTime.UtcNow);
        }

        public void Complete(string transactionId, string? gatewayResponse = null)
        {
            if (Status != PaymentStatus.Processing && Status != PaymentStatus.Pending)
                throw new InvalidPaymentStateException($"Cannot complete payment in {Status} state");

            if (string.IsNullOrWhiteSpace(transactionId))
                throw new PaymentValidationException("Transaction ID is required");

            Status = PaymentStatus.Completed;
            TransactionId = transactionId;
            PaymentGatewayResponse = gatewayResponse;
            CompletedAt = DateTime.UtcNow;
            SetUpdatedAt(DateTime.UtcNow);

            RaiseDomainEvent(new PaymentCompletedEvent(
                Id,
                OrderId,
                transactionId,
                CompletedAt.Value
            ));
        }

        public void Fail(string reason, string? gatewayResponse = null)
        {
            if (Status == PaymentStatus.Completed)
                throw new InvalidPaymentStateException("Cannot fail a completed payment");

            if (Status == PaymentStatus.Refunded)
                throw new InvalidPaymentStateException("Cannot fail a refunded payment");

            if (string.IsNullOrWhiteSpace(reason))
                throw new PaymentValidationException("Failure reason is required");

            Status = PaymentStatus.Failed;
            FailureReason = reason;
            PaymentGatewayResponse = gatewayResponse;
            FailedAt = DateTime.UtcNow;
            SetUpdatedAt(DateTime.UtcNow);

            RaiseDomainEvent(new PaymentFailedEvent(
                Id,
                OrderId,
                reason,
                FailedAt.Value
            ));
        }

        public void Refund(Money refundAmount, string refundTransactionId)
        {
            if (Status != PaymentStatus.Completed)
                throw new InvalidPaymentStateException("Can only refund completed payments");

            if (refundAmount.Amount <= 0)
                throw new PaymentValidationException("Refund amount must be positive");

            if (refundAmount.Amount > Amount.Amount)
                throw new PaymentValidationException($"Refund amount cannot exceed payment amount of {Amount}");

            if (string.IsNullOrWhiteSpace(refundTransactionId))
                throw new PaymentValidationException("Refund transaction ID is required");

            Status = PaymentStatus.Refunded;
            RefundAmount = refundAmount;
            RefundTransactionId = refundTransactionId;
            RefundedAt = DateTime.UtcNow;
            SetUpdatedAt(DateTime.UtcNow);

            RaiseDomainEvent(new PaymentRefundedEvent(
                Id,
                OrderId,
                refundAmount.Amount,
                refundAmount.Currency,
                RefundedAt.Value
            ));
        }

        public void Cancel()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidPaymentStateException($"Cannot cancel payment in {Status} state");

            Status = PaymentStatus.Cancelled;
            SetUpdatedAt(DateTime.UtcNow);
        }

        // Query methods
        public bool IsCompleted() => Status == PaymentStatus.Completed;
        public bool IsPending() => Status == PaymentStatus.Pending;
        public bool HasFailed() => Status == PaymentStatus.Failed;
        public bool IsRefunded() => Status == PaymentStatus.Refunded;
        public bool CanBeRefunded() => Status == PaymentStatus.Completed && !RefundedAt.HasValue;
        public Money GetRefundableAmount() => RefundAmount != null ? Amount.Subtract(RefundAmount) : Amount;
    }
}
