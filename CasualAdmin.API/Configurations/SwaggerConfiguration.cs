namespace CasualAdmin.API.Configurations;

using System.Reflection;
using IGeekFan.AspNetCore.Knife4jUI;

public static class SwaggerConfiguration
{
    public static void ConfigureSwagger(this IServiceCollection services)
    {
        // 简化Swagger配置
        services.AddSwaggerGen(options =>
        {
            // 配置Swagger文档信息
            options.SwaggerDoc("v1", new()
            {
                Title = "Casual Admin API",
                Version = "v1",
                Description = "Casual Admin API文档"
            });

            // 启用XML注释
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });
    }

    public static void UseSwaggerMiddleware(this WebApplication app)
    {
        // 启用Swagger中间件
        app.UseSwagger();

        // 启用Knife4j UI中间件
        app.UseKnife4UI(options =>
        {
            options.SwaggerEndpoint("/v1/swagger.json", "Casual Admin API v1");
        });

        // 获取应用程序正在使用的地址
        var address = app.Urls.FirstOrDefault() ?? $"http://localhost:{app.Configuration.GetValue("Port", 5000)}";
        var basePath = app.Configuration.GetValue("BasePath", string.Empty);
        Console.WriteLine($"启动成功，swagger：{address}{basePath?.TrimEnd('/')}/swagger");
    }
}