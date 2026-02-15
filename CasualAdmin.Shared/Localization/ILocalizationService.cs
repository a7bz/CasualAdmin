namespace CasualAdmin.Shared.Localization;

/// <summary>
/// 本地化服务接口
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// 获取本地化字符串
    /// </summary>
    /// <param name="key">资源键</param>
    /// <returns>本地化字符串</returns>
    string GetString(string key);

    /// <summary>
    /// 获取本地化字符串（带参数）
    /// </summary>
    /// <param name="key">资源键</param>
    /// <param name="args">参数</param>
    /// <returns>本地化字符串</returns>
    string GetString(string key, params object[] args);

    /// <summary>
    /// 获取当前语言
    /// </summary>
    /// <returns>当前语言代码</returns>
    string GetCurrentLanguage();

    /// <summary>
    /// 设置当前语言
    /// </summary>
    /// <param name="language">语言代码</param>
    void SetCurrentLanguage(string language);
}