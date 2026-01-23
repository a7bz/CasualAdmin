namespace CasualAdmin.Application.Interfaces.Services;

/// <summary>
/// 配置服务接口
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// 重新读取配置文件
    /// </summary>
    /// <returns>是否成功</returns>
    Task<bool> ReloadConfigurationAsync();

    /// <summary>
    /// 获取配置值
    /// </summary>
    /// <typeparam name="T">配置类型</typeparam>
    /// <param name="key">配置键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>配置值</returns>
    T GetValue<T>(string key, T defaultValue = default!);
}
