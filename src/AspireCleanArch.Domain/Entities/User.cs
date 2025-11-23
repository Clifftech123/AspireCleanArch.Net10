using AspireCleanArch.Domain.Entities.ValueObjects;
using AspireCleanArch.Domain.Enums;
using AspireCleanArch.Domain.Events;
using AspireCleanArch.Domain.Exceptions;

namespace AspireCleanArch.Domain.Entities
{
    /// <summary>
    /// User aggregate root - represents a system user (Customer, Vendor, or Admin)
    /// </summary>
    public class User : BaseEntity
    {
        public string IdentityId { get; private set; } = string.Empty; // Link to ASP.NET Identity
        public string Email { get; private set; } = string.Empty;
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string PhoneNumber { get; private set; } = string.Empty;
        public UserRole Role { get; private set; }
        public UserStatus Status { get; private set; }
        public string? ProfileImageUrl { get; private set; }
        public DateTime? DateOfBirth { get; private set; }

        // Vendor relationship - if user is a vendor
        public Guid? VendorId { get; private set; }

        private readonly List<Address> _addresses = new();
        public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

        public DateTime? LastLoginAt { get; private set; }
        public DateTime? EmailVerifiedAt { get; private set; }

        // EF Core constructor
        private User() : base(Guid.NewGuid()) { }

        // Factory method for creating new user
        public static User Create(
            string identityId,
            string email,
            string firstName,
            string lastName,
            string phoneNumber,
            UserRole role = UserRole.Customer)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new UserValidationException("Email is required");

            if (string.IsNullOrWhiteSpace(firstName))
                throw new UserValidationException("First name is required");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new UserValidationException("Last name is required");

            var user = new User
            {
                IdentityId = identityId,
                Email = email.ToLowerInvariant(),
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                Role = role,
                Status = UserStatus.Active
            };

            user.SetCreatedAt(DateTime.UtcNow);

            user.RaiseDomainEvent(new UserRegisteredEvent(
                user.Id,
                user.Email,
                user.FullName
            ));

            return user;
        }

        // Business methods

        public void UpdateProfile(string firstName, string lastName, string phoneNumber, DateTime? dateOfBirth = null)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new UserValidationException("First name is required");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new UserValidationException("Last name is required");

            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            DateOfBirth = dateOfBirth;
            SetUpdatedAt(DateTime.UtcNow);
        }

        public void UpdateProfileImage(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new UserValidationException("Image URL cannot be empty");

            ProfileImageUrl = imageUrl;
            SetUpdatedAt(DateTime.UtcNow);
        }

        public void ChangeRole(UserRole newRole)
        {
            if (Status != UserStatus.Active)
                throw new InvalidUserStateException("Cannot change role for inactive user");

            var oldRole = Role;
            Role = newRole;
            SetUpdatedAt(DateTime.UtcNow);

            RaiseDomainEvent(new UserRoleChangedEvent(
                Id,
                oldRole.ToString(),
                newRole.ToString()
            ));
        }

        public void BecomeVendor(Guid vendorId)
        {
            if (Role != UserRole.Customer)
                throw new InvalidUserStateException("Only customers can become vendors");

            if (VendorId.HasValue)
                throw new InvalidUserStateException("User is already a vendor");

            Role = UserRole.Vendor;
            VendorId = vendorId;
            SetUpdatedAt(DateTime.UtcNow);

            RaiseDomainEvent(new UserRoleChangedEvent(
                Id,
                UserRole.Customer.ToString(),
                UserRole.Vendor.ToString()
            ));
        }

        public void Deactivate()
        {
            if (Status == UserStatus.Deactivated)
                return;

            Status = UserStatus.Deactivated;
            SetUpdatedAt(DateTime.UtcNow);

            RaiseDomainEvent(new UserDeactivatedEvent(
                Id,
                DateTime.UtcNow
            ));
        }

        public void Reactivate()
        {
            if (Status == UserStatus.Active)
                return;

            Status = UserStatus.Active;
            SetUpdatedAt(DateTime.UtcNow);

            RaiseDomainEvent(new UserReactivatedEvent(
                Id,
                DateTime.UtcNow
            ));
        }

        public void RecordLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            SetUpdatedAt(DateTime.UtcNow);
        }

        public void VerifyEmail()
        {
            if (EmailVerifiedAt.HasValue)
                return;

            EmailVerifiedAt = DateTime.UtcNow;
            SetUpdatedAt(DateTime.UtcNow);

            RaiseDomainEvent(new UserEmailVerifiedEvent(
                Id,
                EmailVerifiedAt.Value
            ));
        }

        public void AddAddress(Address address)
        {
            if (address == null)
                throw new UserValidationException("Address cannot be null");

            _addresses.Add(address);
            SetUpdatedAt(DateTime.UtcNow);
        }

        // Query methods
        public bool IsVendor() => Role == UserRole.Vendor && VendorId.HasValue;
        public bool IsAdmin() => Role == UserRole.Admin;
        public bool IsActive() => Status == UserStatus.Active;
        public bool IsEmailVerified() => EmailVerifiedAt.HasValue;
    }
}
