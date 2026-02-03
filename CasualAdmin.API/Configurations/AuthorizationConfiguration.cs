namespace CasualAdmin.API.Configurations;

using FluentValidation;
using FluentValidation.AspNetCore;

public static class AuthorizationConfiguration
{
    public static void ConfigureAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        // 注册授权排除路径配置
        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var excludePaths = new AuthorizationExcludePaths();
            config.GetSection("AuthorizationExcludePaths").Bind(excludePaths);
            return excludePaths;
        });

        // 添加全局授权策略
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .RequireAssertion(context =>
                {
                    // 检查请求路径是否为授权排除路径
                    var httpContext = context.Resource as HttpContext;
                    if (httpContext != null)
                    {
                        var excludePaths = httpContext.RequestServices.GetRequiredService<AuthorizationExcludePaths>();
                        return excludePaths.ShouldSkipAuthorization(httpContext.Request.Path) ||
                               context.User.Identity?.IsAuthenticated == true;
                    }
                    return true;
                })
                .Build();
        });
    }

    public static void ConfigureFluentValidation(this IServiceCollection services)
    {
        // 添加 FluentValidation 自动验证
        services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(typeof(Application.Validators.System.SysDeptCreateValidator).Assembly);
    }
}