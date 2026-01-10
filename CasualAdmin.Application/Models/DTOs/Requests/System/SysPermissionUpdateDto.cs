namespace CasualAdmin.Application.Models.DTOs.Requests.System;

/// <summary>
/// 更新权限DTO
/// </summary>
public class SysPermissionUpdateDto
{
    /// <summary>
    /// 权限名称
    /// </summary>
    public string PermissionName { get; set; } = string.Empty;

    /// <summary>
    /// 权限编码
    /// </summary>
    public string PermissionCode { get; set; } = string.Empty;

    /// <summary>
    /// 权限类型：0-菜单权限，1-功能权限
    /// </summary>
    public int PermissionType { get; set; } = 0;

    /// <summary>
    /// 所属模块
    /// </summary>
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// 菜单ID
    /// </summary>
    public Guid? MenuId { get; set; }

    /// <summary>
    /// 状态：0-禁用，1-启用
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; } = string.Empty;
}
