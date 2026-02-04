namespace CasualAdmin.API.Configurations;

using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;

public static class AuthorizationConfiguration
{
    public static void ConfigureAuthorization(this IServiceCollection services)
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
        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAssertion(context =>
                {
                    if (context.Resource is not HttpContext httpContext) return true;

                    var excludePaths = httpContext.RequestServices
                        .GetRequiredService<AuthorizationExcludePaths>();

                    return excludePaths.ShouldSkipAuthorization(httpContext.Request.Path) ||
                           context.User.Identity?.IsAuthenticated == true;
                })
                .Build());
    }

    public static void ConfigureFluentValidation(this IServiceCollection services)
    {
        // 添加 FluentValidation 自动验证
        services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(typeof(Application.Validators.System.SysDeptCreateValidator).Assembly);
    }
}