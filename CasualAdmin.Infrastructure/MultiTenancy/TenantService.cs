namespace CasualAdmin.Infrastructure.MultiTenancy;
using Microsoft.Extensions.Options;

/// <summary>
/// 租户服务接口
/// </summary>
public interface ITenantService
{
    /// <summary>
    /// 获取当前租户ID
    /// </summary>
    /// <returns>租户ID</returns>
    Task<Guid?> GetCurrentTenantIdAsync();

    /// <summary>
    /// 设置当前租户ID
    /// </summary>
    /// <param name="tenantId">租户ID</param>
    void SetCurrentTenantId(Guid? tenantId);
}

/// <summary>
/// 租户服务实现
/// </summary>
public class TenantService : ITenantService
{
    private readonly MultiTenancyConfig _config;
    private Guid? _currentTenantId;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="config">多租户配置</param>
    public TenantService(IOptions<MultiTenancyConfig> config)
    {
        _config = config.Value;
    }

    /// <summary>
    /// 获取当前租户ID
    /// </summary>
    /// <returns>租户ID</returns>
    public Task<Guid?> GetCurrentTenantIdAsync()
    {
        // 如果已设置了当前租户ID，则直接返回
        if (_currentTenantId.HasValue)
        {
            return Task.FromResult(_currentTenantId);
        }

        // 如果启用了多租户，则返回默认租户ID，否则返回null
        return Task.FromResult(_config.IsEnabled ? _config.DefaultTenantId : (Guid?)null);
    }

    /// <summary>
    /// 设置当前租户ID
    /// </summary>
    /// <param name="tenantId">租户ID</param>
    public void SetCurrentTenantId(Guid? tenantId)
    {
        _currentTenantId = tenantId;
    }
}
