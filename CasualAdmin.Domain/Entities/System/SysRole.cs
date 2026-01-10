namespace CasualAdmin.Domain.Entities.System;
using SqlSugar;

/// <summary>
/// 角色实体
/// </summary>
[SugarTable("sys_roles")]
public class SysRole : BaseEntity
{
    /// <summary>
    /// 角色ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
    public Guid RoleId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 角色名称
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色代码
    /// </summary>
    [SugarColumn(Length = 50)]
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// 角色类型：0-系统角色，1-租户角色
    /// </summary>
    public int RoleType { get; set; } = 1;

    /// <summary>
    /// 角色描述
    /// </summary>
    [SugarColumn(Length = 200)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 状态：0-禁用，1-启用
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 用户角色关联列表
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysUserRole> UserRoles { get; set; } = [];

    /// <summary>
    /// 角色权限关联列表
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysRolePermission> RolePermissions { get; set; } = [];

    /// <summary>
    /// 权限列表
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysPermission> Permissions { get; set; } = [];

    /// <summary>
    /// 用户列表
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysUser> Users { get; set; } = [];
}
