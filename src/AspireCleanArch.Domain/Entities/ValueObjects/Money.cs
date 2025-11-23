namespace AspireCleanArch.Domain.Entities.ValueObjects
{
    /// <summary>
    /// Money value object representing an amount in a specific currency
    /// Immutable record type for DDD value object pattern
    /// </summary>
    public record Money
    {
        public decimal Amount { get; init; }
        public string Currency { get; init; } = string.Empty;

        public static Money Zero => new(0, "USD");

        // Primary constructor with validation
        public Money(decimal amount, string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be null or empty", nameof(currency));

            Amount = amount;
            Currency = currency.ToUpperInvariant();
        }

        // Parameterless constructor for EF Core
        private Money() { }

        // Arithmetic operations
        public Money Add(Money other)
        {
            ValidateSameCurrency(other);
            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            ValidateSameCurrency(other);
            return new Money(Amount - other.Amount, Currency);
        }

        public Money Multiply(decimal multiplier)
        {
            return new Money(Amount * multiplier, Currency);
        }

        public Money Divide(decimal divisor)
        {
            if (divisor == 0)
                throw new DivideByZeroException("Cannot divide money by zero");
                
            return new Money(Amount / divisor, Currency);
        }

        // Comparison
        public bool IsGreaterThan(Money other)
        {
            ValidateSameCurrency(other);
            return Amount > other.Amount;
        }

        public bool IsLessThan(Money other)
        {
            ValidateSameCurrency(other);
            return Amount < other.Amount;
        }

        // Helper methods
        private void ValidateSameCurrency(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException($"Cannot operate on different currencies: {Currency} and {other.Currency}");
        }

        public override string ToString() => $"{Amount:N2} {Currency}";
    }
}
