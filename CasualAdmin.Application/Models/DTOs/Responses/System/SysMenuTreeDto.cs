namespace CasualAdmin.Application.Models.DTOs.Responses.System;
using CasualAdmin.Application.Models.DTOs;
using CasualAdmin.Domain.Common;

/// <summary>
/// 菜单树DTO
/// </summary>
public class SysMenuTreeDto : BaseTreeDto<SysMenuTreeDto>
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public Guid MenuId { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 菜单类型：0-目录，1-菜单，2-按钮
    /// </summary>
    public int MenuType { get; set; } = 0;

    /// <summary>
    /// 路由路径
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// 组件路径
    /// </summary>
    public string Component { get; set; } = string.Empty;

    /// <summary>
    /// 权限标识
    /// </summary>
    public string Permission { get; set; } = string.Empty;

    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态
    /// </summary>
    public Status Status { get; set; } = Status.Enabled;

    /// <summary>
    /// 是否显示：0-隐藏，1-显示
    /// </summary>
    public int IsVisible { get; set; } = 1;

    /// <summary>
    /// 是否缓存：0-不缓存，1-缓存
    /// </summary>
    public int IsCache { get; set; } = 0;

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; } = string.Empty;
}
