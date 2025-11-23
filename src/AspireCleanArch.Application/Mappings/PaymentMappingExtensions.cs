using AspireCleanArch.Domain.Entities;
using AspireCleanArch.Domain.Enums;
using AspireCleanArch.Shared.DTOs.Payment;

namespace AspireCleanArch.Application.Mappings
{
    /// <summary>
    /// Extension methods for mapping between Payment domain entities and DTOs
    /// </summary>
    public static class PaymentMappingExtensions
    {
        // ============================================
        // Domain Entity ? DTO (for queries/responses)
        // ============================================

        public static PaymentDto ToDto(this Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                UserId = payment.UserId,
                Amount = payment.Amount.Amount,
                Currency = payment.Amount.Currency,
                PaymentMethod = payment.PaymentMethod.ToString(),
                Status = payment.Status.ToString(),
                TransactionId = payment.TransactionId,
                Provider = payment.Provider.ToString(),
                CardLast4Digits = payment.CardLast4Digits,
                CardBrand = payment.CardBrand,
                InitiatedAt = payment.InitiatedAt,
                CompletedAt = payment.CompletedAt,
                FailureReason = payment.FailureReason
            };
        }

        // ============================================
        // Helper methods for parsing enums
        // ============================================

        public static PaymentMethod ToPaymentMethod(this int methodInt)
        {
            return Enum.IsDefined(typeof(PaymentMethod), methodInt)
                ? (PaymentMethod)methodInt
                : throw new ArgumentException($"Invalid payment method: {methodInt}");
        }

        public static PaymentProvider ToPaymentProvider(this string providerString)
        {
            return Enum.TryParse<PaymentProvider>(providerString, out var provider)
                ? provider
                : throw new ArgumentException($"Invalid payment provider: {providerString}");
        }
    }
}
