namespace AspireCleanArch.Shared.DTOs.User
{

    public record UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }


    public record RegisterUserRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public int Role { get; set; } // 0 = Customer, 1 = Vendor
    }

    public record LoginUserRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // AspireCleanArch.Shared/DTOs/User/UpdateUserDto.cs
    public record UpdateUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }


    public record AuthResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public UserDto User { get; set; }
    }


    public record ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }


    public record ResetPasswordRequest
    {
        public string Email { get; set; }
    }



    public record SetNewPasswordRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }



    public record UserAnalyticsDto
    {
        public Guid UserId { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public int TotalProductsPurchased { get; set; }
        public DateTime LastOrderDate { get; set; }
    }


    public record RefreshTokenRequest
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }




    public record UpdateProfileImageRequest
    {
        public string ProfileImageUrl { get; set; }
    }


    public record VerifyEmailRequest
    {
        public string Token { get; set; }
    }


    public record ForgotPasswordRequest
    {
        public string Email { get; set; }
    }



    public record CurrenUserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }
}
