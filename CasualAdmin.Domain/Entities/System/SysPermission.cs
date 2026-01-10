namespace CasualAdmin.Domain.Entities.System;
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
    /// 权限类型：0-菜单权限，1-功能权限
    /// </summary>
    public int PermissionType { get; set; } = 0;

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