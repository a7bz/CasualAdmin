namespace CasualAdmin.Application.Models.DTOs.Requests.System;
using CasualAdmin.Domain.Common;

/// <summary>
/// 创建角色DTO
/// </summary>
public class SysRoleCreateDto
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色代码
    /// </summary>
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// 角色类型：0-系统角色，1-租户角色
    /// </summary>
    public int RoleType { get; set; } = 1;

    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public Status Status { get; set; } = Status.Enabled;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; } = 0;
}
