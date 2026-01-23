using CasualAdmin.API.Configurations;
using CasualAdmin.Infrastructure.Data.Context;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var port = builder.Configuration.GetValue("Port", 5000);
builder.WebHost.UseUrls($"http://*:{port}");

builder.ConfigureSerilog();

builder.Services.ConfigureDatabase();

// 添加内存缓存服务
builder.Services.AddMemoryCache();

builder.Services.RegisterServices(builder.Configuration);

builder.Services.ConfigureAutoMapper();

builder.Services.ConfigureJwt(builder.Configuration);

builder.Services.ConfigureSwagger();

// 注册授权排除路径配置
builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var excludePaths = new AuthorizationExcludePaths();
    configuration.GetSection("AuthorizationExcludePaths").Bind(excludePaths);
    return excludePaths;
});

// 添加全局授权策略
builder.Services.AddAuthorization(options =>
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

builder.Services.AddControllers();

// 添加FluentValidation自动验证
builder.Services.AddFluentValidationAutoValidation()
    .AddValidatorsFromAssembly(typeof(CasualAdmin.Application.Validators.System.SysDeptCreateValidator).Assembly);

var app = builder.Build();

// 配置基础中间件
app.ConfigureMiddleware();

// 启用 Swagger 中间件
app.UseSwaggerMiddleware();

// 映射控制器
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var sqlSugarContext = scope.ServiceProvider.GetRequiredService<SqlSugarContext>();
    sqlSugarContext.ExecuteCodeFirst();
}

app.Run();