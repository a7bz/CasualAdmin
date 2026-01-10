namespace CasualAdmin.Domain.Entities.System;
using SqlSugar;

/// <summary>
/// 用户角色关联实体
/// </summary>
[SugarTable("sys_user_roles")]
public class SysUserRole : BaseEntity
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid UserId { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid RoleId { get; set; }
}
