namespace CasualAdmin.Infrastructure.Services;

using System;
using System.Threading.Tasks;
using CasualAdmin.Domain.Infrastructure.Services;
using Microsoft.Extensions.Configuration;

/// <summary>
/// 配置服务实现
/// </summary>
public class ConfigurationService : IConfigurationService
{
    private readonly IConfigurationRoot? _configurationRoot;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="configuration">配置对象</param>
    public ConfigurationService(IConfiguration configuration)
    {
        // 获取配置根对象，用于重新加载
        _configurationRoot = configuration as IConfigurationRoot;
    }

    /// <summary>
    /// 重新读取配置文件
    /// </summary>
    /// <returns>是否成功</returns>
    public Task<bool> ReloadConfigurationAsync()
    {
        try
        {
            // 重新加载配置
            _configurationRoot?.Reload();
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Reload configuration failed: {ex.Message}");
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// 获取配置值
    /// </summary>
    /// <typeparam name="T">配置类型</typeparam>
    /// <param name="key">配置键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>配置值</returns>
    public T GetValue<T>(string key, T defaultValue = default!)
    {
        if (_configurationRoot == null)
        {
            return defaultValue;
        }

        return _configurationRoot.GetValue<T>(key) ?? defaultValue;
    }
}
