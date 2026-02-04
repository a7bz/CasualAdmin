namespace CasualAdmin.API.Controllers.System;

using CasualAdmin.Domain.Infrastructure.Services;
using CasualAdmin.Shared.Common;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// 配置控制器
/// </summary>
[ApiController]
[Route("[controller]")]
public class ConfigController : ControllerBase
{
    private readonly IConfigurationService _configurationService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="configurationService">配置服务</param>
    public ConfigController(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    /// <summary>
    /// 重新加载配置文件
    /// </summary>
    /// <returns>操作结果</returns>
    [HttpPost("reload")]
    public async Task<ApiResponse<bool>> ReloadConfiguration()
    {
        var result = await _configurationService.ReloadConfigurationAsync();
        if (result)
        {
            return ApiResponse<bool>.Success(result, "配置重新加载成功");
        }
        return ApiResponse<bool>.Failed("配置重新加载失败");
    }

    /// <summary>
    /// 获取配置值
    /// </summary>
    /// <param name="key">配置键</param>
    /// <returns>配置值</returns>
    [HttpGet("value/{key}")]
    public ApiResponse<string> GetConfigurationValue(string key)
    {
        var value = _configurationService.GetValue<string>(key);
        return ApiResponse<string>.Success(value, "获取配置成功");
    }
}
