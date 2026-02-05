namespace CasualAdmin.Application.Models.DTOs.Responses.System;
using CasualAdmin.Application.Models.DTOs;
using CasualAdmin.Domain.Common;

/// <summary>
/// 菜单DTO
/// </summary>
public class SysMenuDto : BaseDto
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public Guid? MenuId { get; set; }

    /// <summary>
    /// 父菜单ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    public string? MenuName { get; set; }

    /// <summary>
    /// 菜单类型：0-目录，1-菜单，2-按钮
    /// </summary>
    public int? MenuType { get; set; }

    /// <summary>
    /// 路由路径
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    public string? Component { get; set; }

    /// <summary>
    /// 权限标识
    /// </summary>
    public string? Permission { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int? Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public Status? Status { get; set; }

    /// <summary>
    /// 是否显示：0-隐藏，1-显示
    /// </summary>
    public int? IsVisible { get; set; }

    /// <summary>
    /// 是否缓存：0-不缓存，1-缓存
    /// </summary>
    public int? IsCache { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}
