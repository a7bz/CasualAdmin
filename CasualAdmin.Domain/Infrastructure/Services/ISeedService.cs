namespace CasualAdmin.Domain.Infrastructure.Services;

/// <summary>
/// 数据初始化服务接口
/// </summary>
public interface ISeedService
{
    /// <summary>
    /// 执行数据初始化
    /// </summary>
    /// <returns>初始化结果</returns>
    Task SeedAsync();

    /// <summary>
    /// 检查是否需要初始化
    /// </summary>
    /// <returns>是否需要初始化</returns>
    Task<bool> NeedsSeedAsync();
}
