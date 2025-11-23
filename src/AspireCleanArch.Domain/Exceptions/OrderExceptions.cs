namespace AspireCleanArch.Domain.Exceptions
{
    /// <summary>
    /// Base exception for all domain-related errors
    /// </summary>
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message) { }
        
        protected DomainException(string message, Exception innerException) 
            : base(message, innerException) { }
    }

    public sealed class InvalidOrderStateException : DomainException
    {
        public InvalidOrderStateException(string message) : base(message) { }
    }

    public sealed class OrderValidationException : DomainException
    {
        public OrderValidationException(string message) : base(message) { }
    }

    public sealed class OrderItemValidationException : DomainException
    {
        public OrderItemValidationException(string message) : base(message) { }
    }
}
