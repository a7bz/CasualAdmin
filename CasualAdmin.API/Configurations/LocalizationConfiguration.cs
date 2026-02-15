namespace CasualAdmin.API.Configurations;

using CasualAdmin.API.Services;
using CasualAdmin.Shared.Localization;

/// <summary>
/// 本地化配置
/// </summary>
public static class LocalizationConfiguration
{
    /// <summary>
    /// 配置本地化服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    public static void ConfigureLocalization(this IServiceCollection services, IConfiguration configuration)
    {
        // 注册本地化服务
        services.AddLocalization(options => options.ResourcesPath = "Resources");

        // 注册 HttpContextAccessor
        services.AddHttpContextAccessor();

        // 注册 HTTP 本地化服务（实现 ILocalizationService）
        services.AddScoped<ILocalizationService, HttpLocalizationService>();

        // 配置请求本地化选项
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                new System.Globalization.CultureInfo("zh-CN"),
                new System.Globalization.CultureInfo("en-US")
            };

            options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("zh-CN");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;

            // 设置语言提供器优先级
            options.RequestCultureProviders = new List<Microsoft.AspNetCore.Localization.IRequestCultureProvider>
            {
                // 1. 从 Query 参数获取（如 ?lang=en）
                new Microsoft.AspNetCore.Localization.QueryStringRequestCultureProvider(),
                // 2. 从 Cookie 获取
                new Microsoft.AspNetCore.Localization.CookieRequestCultureProvider(),
                // 3. 从 Header 获取（如 Accept-Language: en-US）
                new Microsoft.AspNetCore.Localization.AcceptLanguageHeaderRequestCultureProvider()
            };
        });
    }
}