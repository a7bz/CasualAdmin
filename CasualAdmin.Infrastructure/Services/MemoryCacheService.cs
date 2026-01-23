namespace CasualAdmin.Infrastructure.Services
{
    using System;
    using System.Threading.Tasks;
    using CasualAdmin.Application.Interfaces.Services;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// 内存缓存服务实现
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="memoryCache">内存缓存实例</param>
        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <typeparam name="T">缓存值类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值，不存在则返回默认值</returns>
        public T Get<T>(string key)
        {
            return _memoryCache.TryGetValue(key, out T value) ? value : default;
        }

        /// <summary>
        /// 异步获取缓存值
        /// </summary>
        /// <typeparam name="T">缓存值类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值，不存在则返回默认值</returns>
        public Task<T> GetAsync<T>(string key)
        {
            return Task.FromResult(Get<T>(key));
        }

        /// <summary>
        /// 设置缓存值
        /// </summary>
        /// <typeparam name="T">缓存值类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expiration">过期时间</param>
        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            _memoryCache.Set(key, value, expiration);
        }

        /// <summary>
        /// 异步设置缓存值
        /// </summary>
        /// <typeparam name="T">缓存值类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expiration">过期时间</param>
        /// <returns>任务</returns>
        public Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            Set(key, value, expiration);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        /// <summary>
        /// 异步删除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>任务</returns>
        public Task RemoveAsync(string key)
        {
            Remove(key);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 检查缓存是否存在
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>是否存在</returns>
        public bool Exists(string key)
        {
            return _memoryCache.TryGetValue(key, out _);
        }

        /// <summary>
        /// 异步检查缓存是否存在
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>是否存在</returns>
        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(Exists(key));
        }
    }
}