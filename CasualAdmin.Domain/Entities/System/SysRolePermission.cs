namespace CasualAdmin.Domain.Entities.System;
using SqlSugar;

/// <summary>
/// 角色权限关联实体
/// </summary>
[SugarTable("sys_role_permissions")]
public class SysRolePermission : BaseEntity
{
    /// <summary>
    /// 角色ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid RoleId { get; set; }

    /// <summary>
    /// 权限ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid PermissionId { get; set; }

    /// <summary>
    /// 所属角色
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public SysRole? Role { get; set; }

    /// <summary>
    /// 关联权限
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public SysPermission? Permission { get; set; }
}