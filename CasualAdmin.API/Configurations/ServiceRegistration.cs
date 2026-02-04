namespace CasualAdmin.API.Configurations;

using System.Reflection;
using CasualAdmin.Application.Interfaces.Events;
using CasualAdmin.Application.Services;
using CasualAdmin.Domain.Infrastructure.Data;
using CasualAdmin.Domain.Infrastructure.Events;
using CasualAdmin.Domain.Infrastructure.Services;
using CasualAdmin.Infrastructure.Data;
using CasualAdmin.Infrastructure.Data.Repositories;
using CasualAdmin.Infrastructure.Factories;
using CasualAdmin.Infrastructure.FileStorage;
using CasualAdmin.Infrastructure.Services;
using Microsoft.AspNetCore.ResponseCompression;


public static class ServiceRegistration
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 注册响应压缩服务
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
            {
                "application/json",
                "application/javascript",
                "text/html",
                "text/css",
                "text/plain",
                "text/xml",
                "application/xml",
                "application/xml+rss"
            });
        });

        // 配置 Brotli 压缩级别
        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = System.IO.Compression.CompressionLevel.Optimal;
        });

        // 配置 Gzip 压缩级别
        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = System.IO.Compression.CompressionLevel.Optimal;
        });

        // 注册仓储
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // 注册工作单元
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // 注册事件总线 - 使用工厂模式创建，因为需要两个构造参数
        services.AddScoped<IEventBus>(sp =>
        {
            var serviceProvider = sp.GetRequiredService<IServiceProvider>();
            var eventStore = sp.GetRequiredService<IEventStore>();
            return new EventBus(serviceProvider, eventStore);
        });

        // 自动注册所有实现了特定接口的服务 - 使用 Scrutor 简化
        RegisterServicesFromAssembly(services, Assembly.Load("CasualAdmin.Application"));
        RegisterServicesFromAssembly(services, Assembly.Load("CasualAdmin.Infrastructure"));

        // 注册文件存储服务
        RegisterFileStorageService(services, configuration);

        // 注册缓存服务
        services.AddCacheService(configuration);

        // 注册事件存储服务
        services.AddScoped<IEventStore, FileEventStore>();
    }

    private static void RegisterFileStorageService(IServiceCollection services, IConfiguration configuration)
    {
        // 绑定配置
        var fileStorageConfig = new FileStorageConfig();
        configuration.GetSection("FileStorage").Bind(fileStorageConfig);

        // 注册配置
        services.AddSingleton(fileStorageConfig);

        // 根据配置的Type选择对应的文件存储服务实现
        switch (fileStorageConfig.Type)
        {
            case FileStorageType.S3:
                services.AddSingleton<IFileStorageService, S3FileStorageService>();
                break;
            case FileStorageType.Local:
            default:
                services.AddSingleton<IFileStorageService, LocalFileStorageService>();
                break;
        }
    }

    private static void RegisterServicesFromAssembly(IServiceCollection services, Assembly assembly)
    {
        // 使用 Scrutor 简化服务注册逻辑
        services.Scan(scan => scan
            .FromAssemblyDependencies(assembly)
                // 查找所有以 Service 结尾的非抽象类（排除 CacheService 和 RsaEncryptionService）
                .AddClasses(classes => classes
                    .Where(type =>
                        type.Name.EndsWith("Service") &&
                        !type.Name.Contains("CacheService") &&
                        type.Name != "RsaEncryptionService"))
                // 映射到所有实现的接口
                .AsImplementedInterfaces()
                // 默认使用 Scoped 生命周期
                .WithScopedLifetime()
                // 特殊处理：RsaEncryptionService 使用 Singleton 生命周期
                .AddClasses(classes => classes
                    .Where(type => type.Name == "RsaEncryptionService"))
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
        );
    }
}
