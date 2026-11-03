using Application.Common;
using Application.DTOs.Admin;
using Application.Interfaces.Admin;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminUserService _adminUserService;
    private readonly IClock _clock;

    public AdminController(IAdminUserService adminUserService, IClock clock)
    {
        _adminUserService = adminUserService;
        _clock = clock;
    }

    /// <summary>
    /// Kiểm tra quyền Admin (Ping API để test Authorize)
    /// </summary>
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(new
        {
            message = "Xác thực quyền Quản trị viên (Admin) thành công!",
            timestamp = _clock.UtcNow
        });
    }

    /// <summary>
    /// Lấy danh sách người dùng trong hệ thống (có phân trang và lọc)
    /// </summary>
    [HttpGet("users")]
    [ProducesResponseType(typeof(AdminUserListResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] UserRole? role = null,
        [FromQuery] UserStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _adminUserService.GetUsersAsync(page, pageSize, search, role, status, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Cập nhật trạng thái người dùng (Khóa / Mở khóa tài khoản)
    /// </summary>
    [HttpPatch("users/{id:guid}/status")]
    [ProducesResponseType(typeof(UpdateUserStatusResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUserStatus(
        Guid id,
        [FromBody] UpdateUserStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _adminUserService.UpdateUserStatusAsync(id, request.Status, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Phân quyền người dùng (Admin / Member)
    /// </summary>
    [HttpPatch("users/{id:guid}/role")]
    [ProducesResponseType(typeof(UpdateUserRoleResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUserRole(
        Guid id,
        [FromBody] UpdateUserRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _adminUserService.UpdateUserRoleAsync(id, request.Role, cancellationToken);
        return Ok(result);
    }
}
