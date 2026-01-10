namespace CasualAdmin.API.Configurations;
using System.Reflection;
using CasualAdmin.Application.Interfaces.Base;
using CasualAdmin.Application.Interfaces.Events;
using CasualAdmin.Application.Interfaces.Services;
using CasualAdmin.Application.Services;
using CasualAdmin.Infrastructure.Data;
using CasualAdmin.Infrastructure.Data.Repositories;
using CasualAdmin.Infrastructure.FileStorage;

public static class ServiceRegistration
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 注册仓储
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // 注册工作单元
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // 注册事件总线
        services.AddScoped<IEventBus, EventBus>();

        // 自动注册所有实现了特定接口的服务
        // 获取Application程序集
        var applicationAssembly = Assembly.Load("CasualAdmin.Application");
        var infrastructureAssembly = Assembly.Load("CasualAdmin.Infrastructure");

        // 扫描所有服务实现类 - Application层
        RegisterServicesFromAssembly(services, applicationAssembly);

        // 扫描所有服务实现类 - Infrastructure层
        RegisterServicesFromAssembly(services, infrastructureAssembly);

        // 注册文件存储服务
        RegisterFileStorageService(services, configuration);
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
        // 只注册名称以Service结尾的类，避免注册编译器生成的类型
        var serviceImplementations = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"));

        foreach (var implementationType in serviceImplementations)
        {
            // 获取实现的所有接口
            var interfaces = implementationType.GetInterfaces()
                .Where(i => i.Name.EndsWith("Service"));

            if (interfaces.Any())
            {
                // 如果有实现接口，则按接口注册
                foreach (var interfaceType in interfaces)
                {
                    // 注册为Scoped服务
                    services.AddScoped(interfaceType, implementationType);
                }
            }
            else
            {
                // 如果没有实现接口，则按具体类注册
                services.AddScoped(implementationType);
            }
        }
    }
}
