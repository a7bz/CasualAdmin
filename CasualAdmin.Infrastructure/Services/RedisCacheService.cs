namespace CasualAdmin.Infrastructure.Services
{
    using System;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using CasualAdmin.Application.Interfaces.Services;
    using Microsoft.Extensions.Configuration;
    using StackExchange.Redis;

    /// <summary>
    /// Redis缓存服务实现
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _redisDatabase;
        private readonly ConnectionMultiplexer _redisConnection;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration">配置对象</param>
        public RedisCacheService(IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetValue<string>("Redis:ConnectionString")
                ?? throw new ArgumentNullException("Redis:ConnectionString", "Redis连接字符串未配置");

            _redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
            _redisDatabase = _redisConnection.GetDatabase();
        }

        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <typeparam name="T">缓存值类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值，不存在则返回默认值</returns>
        public T? Get<T>(string key)
        {
            var value = _redisDatabase.StringGet(key);
            if (value.IsNull)
            {
                return default;
            }

            var valueString = value.ToString();
            return JsonSerializer.Deserialize<T>(valueString);
        }

        /// <summary>
        /// 异步获取缓存值
        /// </summary>
        /// <typeparam name="T">缓存值类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值，不存在则返回默认值</returns>
        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _redisDatabase.StringGetAsync(key);
            if (value.IsNull)
            {
                return default;
            }

            var valueString = value.ToString();
            return JsonSerializer.Deserialize<T>(valueString);
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
            var jsonValue = JsonSerializer.Serialize(value);
            _redisDatabase.StringSet(key, jsonValue, expiration);
        }

        /// <summary>
        /// 异步设置缓存值
        /// </summary>
        /// <typeparam name="T">缓存值类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expiration">过期时间</param>
        /// <returns>任务</returns>
        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            var jsonValue = JsonSerializer.Serialize(value);
            await _redisDatabase.StringSetAsync(key, jsonValue, expiration);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        public void Remove(string key)
        {
            _redisDatabase.KeyDelete(key);
        }

        /// <summary>
        /// 异步删除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>任务</returns>
        public async Task RemoveAsync(string key)
        {
            await _redisDatabase.KeyDeleteAsync(key);
        }

        /// <summary>
        /// 检查缓存是否存在
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>是否存在</returns>
        public bool Exists(string key)
        {
            return _redisDatabase.KeyExists(key);
        }

        /// <summary>
        /// 异步检查缓存是否存在
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>是否存在</returns>
        public async Task<bool> ExistsAsync(string key)
        {
            return await _redisDatabase.KeyExistsAsync(key);
        }
    }
}