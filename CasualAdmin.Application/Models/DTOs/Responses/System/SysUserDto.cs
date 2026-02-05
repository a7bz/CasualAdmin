namespace CasualAdmin.Application.Models.DTOs.Responses.System;
using CasualAdmin.Application.Models.DTOs;
using CasualAdmin.Domain.Common;

/// <summary>
/// 用户基本信息DTO
/// </summary>
public class SysUserDto : BaseDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string? RealName { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

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
    public Status? Status { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}