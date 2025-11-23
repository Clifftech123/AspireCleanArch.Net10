using AspireCleanArch.Domain.Enums;
using AspireCleanArch.Domain.Entities.ValueObjects;
using AspireCleanArch.Domain.Events;
using AspireCleanArch.Domain.Exceptions;

namespace AspireCleanArch.Domain.Entities
{
    /// <summary>
    /// Vendor aggregate root - represents a marketplace seller
    /// </summary>
    public class Vendor : BaseEntity
    {
        public Guid UserId { get; private set; }  // Link to User entity
        public string BusinessName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;
        public string ContactPersonName { get; private set; } = string.Empty;
        public Address BusinessAddress { get; private set; } = null!;
        public VendorStatus Status { get; private set; }
        public decimal CommissionRate { get; private set; }
        public Money TotalRevenue { get; private set; } = Money.Zero;
        public string? TaxIdentificationNumber { get; private set; }
        public string? BankAccountNumber { get; private set; }
        public string? LogoUrl { get; private set; }

        private readonly List<Product> _products = new();
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        public DateTime? ApprovedAt { get; private set; }
        public DateTime? RejectedAt { get; private set; }
        public string? RejectionReason { get; private set; }
        public DateTime? SuspendedAt { get; private set; }
        public string? SuspensionReason { get; private set; }

        // EF Core constructor
        private Vendor() : base(Guid.NewGuid()) { }

        // Factory method for vendor registration
        public static Vendor Register(
            Guid userId,
            string businessName,
            string email,
            string phoneNumber,
            string contactPersonName,
            Address businessAddress,
            string? taxIdentificationNumber = null,
            string? bankAccountNumber = null,
            decimal commissionRate = 0.15m)
        {
            if (userId == Guid.Empty)
                throw new VendorValidationException("User ID is required");

            if (string.IsNullOrWhiteSpace(businessName))
                throw new VendorValidationException("Business name is required");

            if (string.IsNullOrWhiteSpace(email))
                throw new VendorValidationException("Email is required");

            if (businessAddress == null)
                throw new VendorValidationException("Business address is required");

            if (commissionRate < 0 || commissionRate > 1)
                throw new VendorValidationException("Commission rate must be between 0 and 1");

            var vendor = new Vendor
            {
                UserId = userId,
                BusinessName = businessName,
                Email = email.ToLowerInvariant(),
                PhoneNumber = phoneNumber,
                ContactPersonName = contactPersonName,
                BusinessAddress = businessAddress,
                Status = VendorStatus.Pending,
                CommissionRate = commissionRate,
                TaxIdentificationNumber = taxIdentificationNumber,
                BankAccountNumber = bankAccountNumber,
                TotalRevenue = Money.Zero
            };

            vendor.SetCreatedAt(DateTime.UtcNow);

            vendor.RaiseDomainEvent(new VendorRegisteredEvent(
                vendor.Id,
                userId,
                businessName,
                email
            ));

            return vendor;
        }

        // Business methods

        public void Approve()
        {
            if (Status != VendorStatus.Pending)
                throw new InvalidVendorStateException($"Cannot approve vendor in {Status} state");

            Status = VendorStatus.Active;
            ApprovedAt = DateTime.UtcNow;
            RejectedAt = null;
            RejectionReason = null;
            SetUpdatedAt(DateTime.UtcNow);

            RaiseDomainEvent(new VendorApprovedEvent(
                Id,
                ApprovedAt.Value
            ));
        }

        public void Reject(string reason)
        {
            if (Status != VendorStatus.Pending)
                throw new InvalidVendorStateException($"Cannot reject vendor in {Status} state");

            if (string.IsNullOrWhiteSpace(reason))
                throw new VendorValidationException("Rejection reason is required");

            Status = VendorStatus.Deactivated;
            RejectedAt = DateTime.UtcNow;
            RejectionReason = reason;
            SetUpdatedAt(DateTime.UtcNow);

            RaiseDomainEvent(new VendorRejectedEvent(
                Id,
                reason,
                RejectedAt.Value
            ));
        }

        public void Suspend(string reason)
        {
            if (Status != VendorStatus.Active)
                throw new InvalidVendorStateException($"Cannot suspend vendor in {Status} state");

            if (string.IsNullOrWhiteSpace(reason))
                throw new VendorValidationException("Suspension reason is required");

            Status = VendorStatus.Suspended;
            SuspendedAt = DateTime.UtcNow;
            SuspensionReason = reason;
            SetUpdatedAt(DateTime.UtcNow);

            RaiseDomainEvent(new VendorSuspendedEvent(
                Id,
                reason,
                SuspendedAt.Value
            ));
        }

        public void Reactivate()
        {
            if (Status != VendorStatus.Suspended)
                throw new InvalidVendorStateException($"Cannot reactivate vendor in {Status} state");

            Status = VendorStatus.Active;
            SuspendedAt = null;
            SuspensionReason = null;
            SetUpdatedAt(DateTime.UtcNow);

            RaiseDomainEvent(new VendorReactivatedEvent(
                Id,
                DateTime.UtcNow
            ));
        }

        public void UpdateBusinessInfo(
            string businessName,
            string email,
            string phoneNumber,
            string contactPersonName,
            Address businessAddress)
        {
            if (Status == VendorStatus.Deactivated)
                throw new InvalidVendorStateException("Cannot update deactivated vendor");

            if (string.IsNullOrWhiteSpace(businessName))
                throw new VendorValidationException("Business name is required");

            BusinessName = businessName;
            Email = email.ToLowerInvariant();
            PhoneNumber = phoneNumber;
            ContactPersonName = contactPersonName;
            BusinessAddress = businessAddress;
            SetUpdatedAt(DateTime.UtcNow);
        }

        public void UpdateBankingInfo(string? taxId, string? bankAccount)
        {
            TaxIdentificationNumber = taxId;
            BankAccountNumber = bankAccount;
            SetUpdatedAt(DateTime.UtcNow);
        }

        public void UpdateLogo(string logoUrl)
        {
            if (string.IsNullOrWhiteSpace(logoUrl))
                throw new VendorValidationException("Logo URL cannot be empty");

            LogoUrl = logoUrl;
            SetUpdatedAt(DateTime.UtcNow);
        }

        public void AddRevenue(Money amount)
        {
            if (amount.Amount <= 0)
                throw new VendorValidationException("Revenue amount must be positive");

            TotalRevenue = TotalRevenue.Add(amount);
            SetUpdatedAt(DateTime.UtcNow);
        }

        public Money CalculateCommission(Money saleAmount)
        {
            return saleAmount.Multiply(CommissionRate);
        }

        // Query methods
        public bool IsApproved() => Status == VendorStatus.Active;
        public bool IsPending() => Status == VendorStatus.Pending;
        public bool IsSuspended() => Status == VendorStatus.Suspended;
        public bool CanSellProducts() => Status == VendorStatus.Active;
        public int GetProductCount() => _products.Count;
    }
}
