namespace CasualAdmin.Application.Models.DTOs.Responses.System;
using CasualAdmin.Application.Models.DTOs;
using CasualAdmin.Domain.Common;

/// <summary>
/// 角色DTO
/// </summary>
public class SysRoleDto : BaseDto
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid? RoleId { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 角色代码
    /// </summary>
    public string? RoleCode { get; set; }

    /// <summary>
    /// 角色类型：0-系统角色，1-租户角色
    /// </summary>
    public int? RoleType { get; set; }

    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public Status? Status { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int? Sort { get; set; }
}
