namespace AspireCleanArch.Domain.Exceptions
{
    public sealed class VendorValidationException : DomainException
    {
        public VendorValidationException(string message) : base(message) { }
    }

    public sealed class InvalidVendorStateException : DomainException
    {
        public InvalidVendorStateException(string message) : base(message) { }
    }

    public sealed class ProductValidationException : DomainException
    {
        public ProductValidationException(string message) : base(message) { }
    }

    public sealed class InvalidProductStateException : DomainException
    {
        public InvalidProductStateException(string message) : base(message) { }
    }

    public sealed class InsufficientStockException : DomainException
    {
        public InsufficientStockException(string message) : base(message) { }
    }

    public sealed class PaymentValidationException : DomainException
    {
        public PaymentValidationException(string message) : base(message) { }
    }

    public sealed class InvalidPaymentStateException : DomainException
    {
        public InvalidPaymentStateException(string message) : base(message) { }
    }

    public sealed class UserValidationException : DomainException
    {
        public UserValidationException(string message) : base(message) { }
    }

    public sealed class InvalidUserStateException : DomainException
    {
        public InvalidUserStateException(string message) : base(message) { }
    }
}
