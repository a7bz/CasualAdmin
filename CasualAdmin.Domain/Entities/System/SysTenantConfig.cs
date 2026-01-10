namespace CasualAdmin.Domain.Entities.System;
using SqlSugar;

/// <summary>
/// 租户配置实体
/// </summary>
[SugarTable("sys_tenant_configs")]
public class SysTenantConfig : BaseEntity
{
    /// <summary>
    /// 租户配置ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = false, ColumnName = "tenant_config_id")]
    public Guid TenantConfigId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 配置键
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string ConfigKey { get; set; } = string.Empty;

    /// <summary>
    /// 配置值
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? ConfigValue { get; set; }

    /// <summary>
    /// 配置描述
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Description { get; set; }

    /// <summary>
    /// 关联的租户
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public SysTenant? Tenant { get; set; }
}
