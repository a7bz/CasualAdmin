namespace CasualAdmin.Application.Models.DTOs.Responses.System;
using CasualAdmin.Application.Models.DTOs;
using CasualAdmin.Domain.Common;

/// <summary>
/// 权限DTO
/// </summary>
public class SysPermissionDto : BaseDto
{
    /// <summary>
    /// 权限ID
    /// </summary>
    public Guid? PermissionId { get; set; }

    /// <summary>
    /// 权限名称
    /// </summary>
    public string? PermissionName { get; set; }

    /// <summary>
    /// 权限编码
    /// </summary>
    public string? PermissionCode { get; set; }

    /// <summary>
    /// 权限类型
    /// </summary>
    public PermissionType? PermissionType { get; set; }

    /// <summary>
    /// 所属模块
    /// </summary>
    public string? Module { get; set; }

    /// <summary>
    /// 菜单ID
    /// </summary>
    public Guid? MenuId { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public Status? Status { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int? Sort { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}
