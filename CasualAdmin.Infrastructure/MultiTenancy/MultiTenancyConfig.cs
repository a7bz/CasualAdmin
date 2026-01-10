namespace CasualAdmin.Infrastructure.MultiTenancy;

/// <summary>
/// 多租户配置
/// </summary>
public class MultiTenancyConfig
{
    /// <summary>
    /// 是否启用多租户
    /// </summary>
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// 默认租户ID
    /// </summary>
    public Guid DefaultTenantId { get; set; } = Guid.Empty;
}
