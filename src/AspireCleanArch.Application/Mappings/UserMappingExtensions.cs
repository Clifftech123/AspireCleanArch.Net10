using AspireCleanArch.Domain.Entities;
using AspireCleanArch.Domain.Entities.ValueObjects;
using AspireCleanArch.Shared.DTOs.User;

namespace AspireCleanArch.Application.Mappings
{
    /// <summary>
    /// Extension methods for mapping between User domain entities and DTOs
    /// </summary>
    public static class UserMappingExtensions
    {
        // ============================================
        // Domain Entity ? DTO (for queries/responses)
        // ============================================

        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.ToString(),
                Status = user.Status.ToString(),
                ProfileImageUrl = user.ProfileImageUrl,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }

        public static CurrenUserResponse ToCurrentUserResponse(this User user)
        {
            return new CurrenUserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString()
            };
        }
    }
}
