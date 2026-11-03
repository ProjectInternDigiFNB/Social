using Application.DTOs.Admin;
using Domain.Enums;

namespace Application.Interfaces.Admin;

public interface IAdminUserService
{
    Task<AdminUserListResponseDto> GetUsersAsync(
        int page = 1,
        int pageSize = 20,
        string? search = null,
        UserRole? role = null,
        UserStatus? status = null,
        CancellationToken cancellationToken = default);

    Task<UpdateUserStatusResponseDto> UpdateUserStatusAsync(
        Guid targetUserId,
        UserStatus status,
        CancellationToken cancellationToken = default);

    Task<UpdateUserRoleResponseDto> UpdateUserRoleAsync(
        Guid targetUserId,
        UserRole role,
        CancellationToken cancellationToken = default);
}
