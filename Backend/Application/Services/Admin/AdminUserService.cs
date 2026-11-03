using Application.Common;
using Application.DTOs.Admin;
using Application.Interfaces.Admin;
using Application.Interfaces.Repositories;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Application.Services.Admin;

public sealed class AdminUserService : IAdminUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IClock _clock;
    private readonly ILogger<AdminUserService> _logger;

    public AdminUserService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IClock clock,
        ILogger<AdminUserService> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _clock = clock;
        _logger = logger;
    }

    public async Task<AdminUserListResponseDto> GetUsersAsync(
        int page = 1,
        int pageSize = 20,
        string? search = null,
        UserRole? role = null,
        UserStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (users, total) = await _unitOfWork.Users.GetPagedUsersAsync(
            page,
            pageSize,
            search,
            role,
            status,
            cancellationToken);

        var items = users.Select(u => new AdminUserItemDto
        {
            Id = u.Id,
            Email = u.Email,
            DisplayName = u.DisplayName,
            AvatarUrl = u.AvatarUrl,
            EmailConfirmed = u.EmailConfirmed,
            Role = u.Role.ToString(),
            Status = u.Status.ToString(),
            CreatedAtUtc = u.CreatedAtUtc
        }).ToList();

        return new AdminUserListResponseDto
        {
            Total = total,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public async Task<UpdateUserStatusResponseDto> UpdateUserStatusAsync(
        Guid targetUserId,
        UserStatus status,
        CancellationToken cancellationToken = default)
    {
        var currentAdminId = _currentUserService.UserId;
        if (currentAdminId.HasValue && currentAdminId.Value == targetUserId && status == UserStatus.Suspended)
        {
            throw new ValidationAppException("Status", "Quản trị viên không thể tự khóa tài khoản của chính mình.");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(targetUserId, cancellationToken)
            ?? throw new NotFoundException("User", targetUserId);

        var nowUtc = _clock.UtcNow;
        user.Status = status;
        user.UpdatedAtUtc = nowUtc;
        _unitOfWork.Users.Update(user);

        // Nếu khóa tài khoản, thu hồi toàn bộ RefreshToken đang hoạt động của người dùng đó
        if (status == UserStatus.Suspended)
        {
            var activeTokens = await _unitOfWork.RefreshTokens.GetActiveTokensByUserIdAsync(targetUserId, cancellationToken);
            foreach (var token in activeTokens)
            {
                token.RevokedAtUtc = nowUtc;
                _unitOfWork.RefreshTokens.Update(token);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Admin đã đổi trạng thái tài khoản {UserId} sang {Status}", targetUserId, status);

        return new UpdateUserStatusResponseDto
        {
            Message = $"Đã cập nhật trạng thái tài khoản sang {status}",
            UserId = user.Id,
            Status = user.Status.ToString()
        };
    }

    public async Task<UpdateUserRoleResponseDto> UpdateUserRoleAsync(
        Guid targetUserId,
        UserRole role,
        CancellationToken cancellationToken = default)
    {
        var currentAdminId = _currentUserService.UserId;
        if (currentAdminId.HasValue && currentAdminId.Value == targetUserId && role != UserRole.Admin)
        {
            throw new ValidationAppException("Role", "Quản trị viên không thể tự hạ quyền của chính mình.");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(targetUserId, cancellationToken)
            ?? throw new NotFoundException("User", targetUserId);

        user.Role = role;
        user.UpdatedAtUtc = _clock.UtcNow;
        _unitOfWork.Users.Update(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Admin đã đổi vai trò tài khoản {UserId} sang {Role}", targetUserId, role);

        return new UpdateUserRoleResponseDto
        {
            Message = $"Đã cập nhật vai trò tài khoản sang {role}",
            UserId = user.Id,
            Role = user.Role.ToString()
        };
    }
}
