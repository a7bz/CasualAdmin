namespace CasualAdmin.Shared.Localization;

using Microsoft.Extensions.Localization;

/// <summary>
/// 本地化服务实现
/// </summary>
public class LocalizationService : ILocalizationService
{
    private readonly IStringLocalizer _localizer;
    private string? _currentLanguage;

    public LocalizationService(IStringLocalizerFactory factory)
    {
        _localizer = factory.Create("Resources", typeof(LocalizationService).Assembly.GetName().Name!);
    }

    /// <summary>
    /// 获取本地化字符串
    /// </summary>
    /// <param name="key">资源键</param>
    /// <returns>本地化字符串</returns>
    public string GetString(string key)
    {
        var localizedString = _localizer[key];
        return localizedString.ResourceNotFound ? key : localizedString.Value;
    }

    /// <summary>
    /// 获取本地化字符串（带参数）
    /// </summary>
    /// <param name="key">资源键</param>
    /// <param name="args">参数</param>
    /// <returns>本地化字符串</returns>
    public string GetString(string key, params object[] args)
    {
        var localizedString = _localizer[key, args];
        return localizedString.ResourceNotFound ? key : localizedString.Value;
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

        // 默认返回中文
        return "zh";
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