namespace CasualAdmin.Infrastructure.Factories
{
    using System;
    using CasualAdmin.Application.Interfaces.Services;
    using CasualAdmin.Infrastructure.Services;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// 缓存服务工厂
    /// </summary>
    public static class CacheServiceFactory
    {
        /// <summary>
        /// 根据配置创建缓存服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">配置对象</param>
        /// <returns>缓存服务实例</returns>
        public static IServiceCollection AddCacheService(this IServiceCollection services, IConfiguration configuration)
        {
            // 检查是否配置了Redis
            var redisConnectionString = configuration.GetValue<string>("Redis:ConnectionString");

            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                // 使用Redis缓存
                services.AddSingleton<ICacheService, RedisCacheService>();
            }
            else
            {
                // 使用内存缓存
                services.AddMemoryCache();
                services.AddSingleton<ICacheService, MemoryCacheService>();
            }

            return services;
        }
    }
}