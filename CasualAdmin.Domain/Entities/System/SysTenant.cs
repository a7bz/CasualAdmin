namespace CasualAdmin.Domain.Entities.System;
using SqlSugar;

/// <summary>
/// 租户实体
/// </summary>
[SugarTable("sys_tenants")]
public class SysTenant : BaseEntity
{
    /// <summary>
    /// 租户ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = false, ColumnName = "tenant_id")]
    public new Guid TenantId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 租户名称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string TenantName { get; set; } = string.Empty;

    /// <summary>
    /// 租户编码
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string TenantCode { get; set; } = string.Empty;

    /// <summary>
    /// 联系人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Contact { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Email { get; set; }

    /// <summary>
    /// 租户状态：0-禁用，1-启用
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 过期时间
    /// </summary>
    public DateTime? ExpireTime { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; set; }

    /// <summary>
    /// 租户配置
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public SysTenantConfig? TenantConfig { get; set; }
}
