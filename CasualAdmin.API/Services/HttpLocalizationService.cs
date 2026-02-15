namespace CasualAdmin.API.Services;

using CasualAdmin.Shared.Localization.Resources;
using CasualAdmin.Shared.Localization;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Resources;

/// <summary>
/// HTTP 本地化服务
/// </summary>
public class HttpLocalizationService : ILocalizationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ResourceManager _resourceManager;
    private string? _currentLanguage;

    public HttpLocalizationService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _resourceManager = new ResourceManager("CasualAdmin.Shared.Localization.Resources.SharedResources", typeof(SharedResources).Assembly);
    }

    /// <summary>
    /// 获取本地化字符串
    /// </summary>
    /// <param name="key">资源键</param>
    /// <returns>本地化字符串</returns>
    public string GetString(string key)
    {
        var culture = GetCurrentCulture();
        var value = _resourceManager.GetString(key, culture);
        return value ?? key;
    }

    /// <summary>
    /// 获取本地化字符串（带参数）
    /// </summary>
    /// <param name="key">资源键</param>
    /// <param name="args">参数</param>
    /// <returns>本地化字符串</returns>
    public string GetString(string key, params object[] args)
    {
        var culture = GetCurrentCulture();
        var value = _resourceManager.GetString(key, culture);
        if (value == null)
        {
            return key;
        }
        return string.Format(culture, value, args);
    }

    /// <summary>
    /// 获取当前 Culture
    /// </summary>
    /// <returns>当前 Culture</returns>
    private CultureInfo? GetCurrentCulture()
    {
        var languageCode = GetCurrentLanguage();
        if (string.IsNullOrEmpty(languageCode))
        {
            return null;
        }
        try
        {
            return new CultureInfo(languageCode);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 获取当前语言
    /// </summary>
    /// <returns>当前语言代码</returns>
    public string GetCurrentLanguage()
    {
        if (_currentLanguage != null)
        {
            return _currentLanguage;
        }

        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            // 优先从 Header 获取语言
            var acceptLanguage = context.Request.Headers["Accept-Language"].FirstOrDefault();
            if (!string.IsNullOrEmpty(acceptLanguage))
            {
                // 提取完整的语言代码（如 "zh-CN", "en-US"）
                var languageCode = acceptLanguage.Split(',')[0].Trim();
                // 映射常见的语言代码
                var mappedLanguage = MapLanguageCode(languageCode);
                if (!string.IsNullOrEmpty(mappedLanguage))
                {
                    return mappedLanguage;
                }
            }

            // 其次从 Query 参数获取
            var languageFromQuery = context.Request.Query["lang"].FirstOrDefault();
            if (!string.IsNullOrEmpty(languageFromQuery))
            {
                var mappedLanguage = MapLanguageCode(languageFromQuery);
                if (!string.IsNullOrEmpty(mappedLanguage))
                {
                    return mappedLanguage;
                }
            }
        }

        // 默认返回中文
        return "zh-CN";
    }

    /// <summary>
    /// 映射语言代码到支持的格式
    /// </summary>
    /// <param name="languageCode">输入的语言代码</param>
    /// <returns>映射后的语言代码</returns>
    private string? MapLanguageCode(string languageCode)
    {
        if (string.IsNullOrEmpty(languageCode))
        {
            return null;
        }

        // 标准化语言代码
        languageCode = languageCode.ToLowerInvariant();

        // 映射到支持的语言代码
        if (languageCode.StartsWith("zh") || languageCode.Contains("zh-cn") || languageCode.Contains("zh-hans"))
        {
            return "zh-CN";
        }
        if (languageCode.StartsWith("en") || languageCode.Contains("en-us"))
        {
            return "en-US";
        }

        return null;
    }

    /// <summary>
    /// 设置当前语言
    /// </summary>
    /// <param name="language">语言代码</param>
    public void SetCurrentLanguage(string language)
    {
        _currentLanguage = language;
    }
}