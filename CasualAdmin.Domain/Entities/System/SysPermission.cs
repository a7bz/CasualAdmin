namespace CasualAdmin.Domain.Entities.System;
using CasualAdmin.Domain.Common;
using SqlSugar;

/// <summary>
/// 权限实体
/// </summary>
[SugarTable("sys_permissions")]
public class SysPermission : BaseEntity
{
    /// <summary>
    /// 权限ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
    public Guid PermissionId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 权限名称
    /// </summary>
    [SugarColumn(Length = 50)]
    public string PermissionName { get; set; } = string.Empty;

    /// <summary>
    /// 权限编码
    /// </summary>
    [SugarColumn(Length = 100)]
    public string PermissionCode { get; set; } = string.Empty;

    /// <summary>
    /// 权限类型
    /// </summary>
    public PermissionType PermissionType { get; set; } = PermissionType.Menu;

    /// <summary>
    /// 所属模块
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// 菜单ID
    /// </summary>
    public Guid? MenuId { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public Status Status { get; set; } = Status.Enabled;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 200)]
    public string Remark { get; set; } = string.Empty;

    /// <summary>
    /// 所属菜单
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public SysMenu? Menu { get; set; }

    /// <summary>
    /// 角色权限关联列表
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysRolePermission> RolePermissions { get; set; } = [];

    /// <summary>
    /// 角色列表
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysRole> Roles { get; set; } = [];
}