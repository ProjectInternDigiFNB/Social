using Domain.Enums;

namespace Application.DTOs.Admin;

public class UpdateUserStatusRequest
{
    public UserStatus Status { get; set; }
}

public class UpdateUserRoleRequest
{
    public UserRole Role { get; set; }
}

public class AdminUserItemDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool EmailConfirmed { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}

public class AdminUserListResponseDto
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public IReadOnlyList<AdminUserItemDto> Items { get; set; } = Array.Empty<AdminUserItemDto>();
}

public class UpdateUserStatusResponseDto
{
    public string Message { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class UpdateUserRoleResponseDto
{
    public string Message { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
}
