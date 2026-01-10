namespace CasualAdmin.Domain.Entities.System;
using SqlSugar;

/// <summary>
/// 菜单实体
/// </summary>
[SugarTable("sys_menus")]
public class SysMenu : BaseEntity
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
    public Guid MenuId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 父菜单ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    [SugarColumn(Length = 50)]
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 菜单类型：0-目录，1-菜单，2-按钮
    /// </summary>
    public int MenuType { get; set; } = 0;

    /// <summary>
    /// 路由路径
    /// </summary>
    [SugarColumn(Length = 200)]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// 组件路径
    /// </summary>
    [SugarColumn(Length = 200)]
    public string Component { get; set; } = string.Empty;

    /// <summary>
    /// 权限标识
    /// </summary>
    [SugarColumn(Length = 100)]
    public string Permission { get; set; } = string.Empty;

    /// <summary>
    /// 图标
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态：0-禁用，1-启用
    /// </summary>
    public int Status { get; set; } = 1;

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
    [SugarColumn(Length = 200)]
    public string Remark { get; set; } = string.Empty;

    /// <summary>
    /// 子菜单列表
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysMenu> Children { get; set; } = [];

    /// <summary>
    /// 父菜单
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public SysMenu? Parent { get; set; }
}