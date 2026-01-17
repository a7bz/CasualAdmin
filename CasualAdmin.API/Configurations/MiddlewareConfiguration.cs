namespace CasualAdmin.API.Configurations;
using CasualAdmin.API.Middleware;
using CasualAdmin.Infrastructure.FileStorage;
using Microsoft.Extensions.FileProviders;

public static class MiddlewareConfiguration
{
    public static void ConfigureMiddleware(this WebApplication app)
    {
        // 从配置文件中读取basePath配置
        var basePath = app.Configuration.GetValue<string>("BasePath") ?? string.Empty;
        // 确保basePath以/开头但不以/结尾
        basePath = basePath.Trim('/');
        if (!string.IsNullOrEmpty(basePath))
        {
            basePath = $"/{basePath}";
            app.UsePathBase(basePath);
        }

        // 配置静态文件访问
        app.UseStaticFiles();

        // 配置文件服务，将/files路径映射到本地存储目录
        var fileStorageConfig = new FileStorageConfig();
        app.Configuration.GetSection("FileStorage").Bind(fileStorageConfig);

        if (fileStorageConfig.Type == FileStorageType.Local)
        {
            // 获取本地存储路径，默认为"wwwroot/files"
            var localPath = fileStorageConfig.LocalPath ?? "wwwroot/files";
            var filesPath = Path.Combine(app.Environment.ContentRootPath, localPath);
            // 确保目录存在
            if (!Directory.Exists(filesPath))
            {
                Directory.CreateDirectory(filesPath);
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(filesPath),
                RequestPath = "/files"
            });
        }

        app.UseRouting();

        // 添加异常处理中间件
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // 从配置文件中读取日志中间件选项
        var loggingOptions = new LoggingOptions();
        app.Configuration.GetSection("LoggingMiddleware").Bind(loggingOptions);

        // 启用请求日志中间件
        app.UseMiddleware<LoggingMiddleware>(loggingOptions);

        // 添加认证和授权中间件
        app.UseAuthentication();
        app.UseAuthorization();

        // 添加权限检查中间件
        app.UsePermissionCheck();
    }
}
