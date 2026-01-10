namespace CasualAdmin.Application.Models.DTOs.Requests.System;

/// <summary>
/// 创建用户DTO
/// </summary>
public class SysUserCreateDto
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;

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
    /// 状态：0-禁用，1-启用
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; } = string.Empty;
}