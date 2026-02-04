namespace CasualAdmin.API.Configurations;

using System.ComponentModel.DataAnnotations;
using CasualAdmin.Infrastructure.FileStorage;

/// <summary>
/// 应用程序配置验证
/// </summary>
public class AppSettingsValidator
{
    /// <summary>
    /// 连接字符串
    /// </summary>
    [Required(ErrorMessage = "ConnectionStrings.DefaultConnection 是必需的")]
    public string? DefaultConnection { get; set; }

    /// <summary>
    /// 数据库配置
    /// </summary>
    [Required(ErrorMessage = "Database 配置是必需的")]
    public DatabaseConfig? Database { get; set; }

    /// <summary>
    /// JWT 配置
    /// </summary>
    [Required(ErrorMessage = "Jwt 配置是必需的")]
    public JwtConfig? Jwt { get; set; }

    /// <summary>
    /// 文件存储配置
    /// </summary>
    [Required(ErrorMessage = "FileStorage 配置是必需的")]
    public FileStorageConfig? FileStorage { get; set; }
}

/// <summary>
/// 数据库配置
/// </summary>
public class DatabaseConfig
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    [Required(ErrorMessage = "Database.Type 是必需的")]
    public string? Type { get; set; }
}

/// <summary>
/// JWT 配置
/// </summary>
public class JwtConfig
{
    /// <summary>
    /// 签发者
    /// </summary>
    [Required(ErrorMessage = "Jwt.Issuer 是必需的")]
    public string? Issuer { get; set; }

    /// <summary>
    /// 受众
    /// </summary>
    [Required(ErrorMessage = "Jwt.Audience 是必需的")]
    public string? Audience { get; set; }

    /// <summary>
    /// 密钥
    /// </summary>
    [Required(ErrorMessage = "Jwt.Key 是必需的")]
    [MinLength(32, ErrorMessage = "Jwt.Key 长度至少为 32 个字符")]
    public string? Key { get; set; }

    /// <summary>
    /// 过期时间（小时）
    /// </summary>
    [Required(ErrorMessage = "Jwt.ExpireHours 是必需的")]
    [Range(1, 168, ErrorMessage = "Jwt.ExpireHours 必须在 1 到 168 之间")]
    public string? ExpireHours { get; set; }
}

/// <summary>
/// 速率限制配置
/// </summary>
public class RateLimitingConfig
{
    /// <summary>
    /// 时间窗口（秒）
    /// </summary>
    [Required(ErrorMessage = "RateLimiting.WindowSeconds 是必需的")]
    [Range(1, 86400, ErrorMessage = "RateLimiting.WindowSeconds 必须在 1 到 86400 之间")]
    public int WindowSeconds { get; set; }

    /// <summary>
    /// 最大请求数
    /// </summary>
    [Required(ErrorMessage = "RateLimiting.MaxRequests 是必需的")]
    [Range(1, 10000, ErrorMessage = "RateLimiting.MaxRequests 必须在 1 到 10000 之间")]
    public int MaxRequests { get; set; }
}

/// <summary>
/// 配置验证扩展
/// </summary>
public static class ConfigurationValidationExtensions
{
    /// <summary>
    /// 验证配置
    /// </summary>
    /// <param name="configuration">配置对象</param>
    /// <returns>验证结果</returns>
    public static ConfigValidationResult ValidateConfiguration(this IConfiguration configuration)
    {
        var errors = new List<string>();

        // 验证数据库连接字符串
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            errors.Add("数据库连接字符串不能为空");
        }

        // 验证基本配置（不包括 ConnectionStrings）
        var appSettings = new AppSettingsValidator
        {
            DefaultConnection = connectionString
        };
        configuration.Bind(appSettings);

        var validationContext = new ValidationContext(appSettings);
        var validationResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(appSettings, validationContext, validationResults, true))
        {
            errors.AddRange(validationResults.Select(r => r.ErrorMessage ?? "配置验证失败"));
        }

        // 验证速率限制配置
        var rateLimitingConfig = new RateLimitingConfig();
        configuration.GetSection("RateLimiting").Bind(rateLimitingConfig);

        var rateLimitingContext = new ValidationContext(rateLimitingConfig);
        var rateLimitingResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(rateLimitingConfig, rateLimitingContext, rateLimitingResults, true))
        {
            errors.AddRange(rateLimitingResults.Select(r => r.ErrorMessage ?? "速率限制配置验证失败"));
        }

        // 验证端口
        var port = configuration.GetValue("Port", 0);
        if (port <= 0 || port > 65535)
        {
            errors.Add("Port 必须在 1 到 65535 之间");
        }

        return new ConfigValidationResult(errors.Count == 0, errors);
    }
}

/// <summary>
/// 配置验证结果
/// </summary>
public class ConfigValidationResult
{
    /// <summary>
    /// 是否验证成功
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// 错误信息列表
    /// </summary>
    public List<string> Errors { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="isValid">是否验证成功</param>
    /// <param name="errors">错误信息列表</param>
    public ConfigValidationResult(bool isValid, List<string> errors)
    {
        IsValid = isValid;
        Errors = errors;
    }

    /// <summary>
    /// 获取错误消息
    /// </summary>
    /// <returns>错误消息字符串</returns>
    public string GetErrorMessage()
    {
        return string.Join(Environment.NewLine, Errors);
    }
}