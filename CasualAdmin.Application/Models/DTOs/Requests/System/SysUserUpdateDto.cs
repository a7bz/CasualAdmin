namespace CasualAdmin.Application.Models.DTOs.Requests.System;
using CasualAdmin.Domain.Common;

/// <summary>
/// 更新用户DTO
/// </summary>
public class SysUserUpdateDto
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码（可选，留空则不修改密码）
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string RealName { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 手机号
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 部门ID
    /// </summary>
    public Guid? DeptId { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid? RoleId { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public Status Status { get; set; } = Status.Enabled;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}