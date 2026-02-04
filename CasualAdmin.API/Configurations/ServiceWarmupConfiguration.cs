namespace CasualAdmin.API.Configurations;

using CasualAdmin.Domain.Infrastructure.Services;
using CasualAdmin.Infrastructure.Data.Context;
using FluentValidation;

public static class ServiceWarmupConfiguration
{
    public static async Task WarmupServicesAsync(this IServiceProvider serviceProvider)
    {

        // 1. 预热 SqlSugar 数据库初始化
        var sqlSugarContext = serviceProvider.GetRequiredService<SqlSugarContext>();
        sqlSugarContext.ExecuteCodeFirst();

        // 2. 预热 FluentValidation 验证器（通过程序集扫描触发加载）
        WarmupFluentValidators();

        // 3. 预热 AutoMapper（触发映射配置初始化）
        WarmupAutoMapper(serviceProvider);

        // 4. 预热 RSA 加密服务（单例）
        WarmupRsaEncryptionService(serviceProvider);

        // 5. 预热缓存服务
        await WarmupCacheServiceAsync(serviceProvider);
    }

    private static void WarmupFluentValidators()
    {
        try
        {
            var assembly = System.Reflection.Assembly.Load("CasualAdmin.Application");
            var validatorTypes = assembly.GetTypes()
                .Where(t => !t.IsAbstract && typeof(IValidator).IsAssignableFrom(t));

            // 触发验证器加载但不实例化
            var validatorTypeNames = string.Join(", ", validatorTypes.Take(5).Select(t => t.Name));
            if (validatorTypes.Count() > 5)
                validatorTypeNames += "...";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FluentValidation 预热跳过: {ex.Message}");
        }
    }

    private static void WarmupAutoMapper(IServiceProvider serviceProvider)
    {
        var mapper = serviceProvider.GetRequiredService<AutoMapper.IMapper>();
        mapper.Map<object>(new { });
    }

    private static void WarmupRsaEncryptionService(IServiceProvider serviceProvider)
    {
        serviceProvider.GetRequiredService<Application.Interfaces.System.IRsaEncryptionService>();
    }

    private static async Task WarmupCacheServiceAsync(IServiceProvider serviceProvider)
    {
        var cacheService = serviceProvider.GetRequiredService<ICacheService>();
        await cacheService.GetAsync<object>("warmup-test");
    }
}