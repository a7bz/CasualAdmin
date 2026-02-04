namespace CasualAdmin.Infrastructure.Services
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using CasualAdmin.Application.Interfaces.Services;
    using CasualAdmin.Infrastructure.Cache;
    using Microsoft.Extensions.Logging;

    using Microsoft.Extensions.Options;
    using StackExchange.Redis;

    /// <summary>
    /// Redis缓存服务实现
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _redisDatabase;
        private readonly ConnectionMultiplexer _redisConnection;
        private readonly string _keyPrefix;
        private readonly ILogger<RedisCacheService> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">缓存配置选项</param>
        /// <param name="logger">日志记录器</param>
        public RedisCacheService(IOptions<CacheOptions> options, ILogger<RedisCacheService> logger)
        {
            var redisOptions = options.Value.Redis;
            _keyPrefix = redisOptions.KeyPrefix;
            _logger = logger;

            var configurationOptions = ConfigurationOptions.Parse(redisOptions.BuildConnectionString());
            configurationOptions.ConnectTimeout = redisOptions.ConnectTimeout;
            configurationOptions.ConnectRetry = 3;

            _logger.LogInformation("正在连接 Redis 服务器: {Host}:{Port}, 数据库: {Database}, 超时时间: {Timeout}ms",
                redisOptions.Host, redisOptions.Port, redisOptions.Database, redisOptions.ConnectTimeout);

            try
            {
                _redisConnection = ConnectionMultiplexer.Connect(configurationOptions);
                _redisConnection.ConnectionFailed += OnConnectionFailed;
                _redisDatabase = _redisConnection.GetDatabase(redisOptions.Database);
                _logger.LogInformation("Redis 连接成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis 连接失败: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 连接失败回调
        /// </summary>
        private void OnConnectionFailed(object? sender, ConnectionFailedEventArgs e)
        {
            if (e.Exception is TimeoutException)
            {
                _logger.LogWarning("Redis 连接超时: {EndPoint}, 异常: {Message}", e.EndPoint, e.Exception?.Message);
            }
            else
            {
                _logger.LogError(e.Exception, "Redis 连接失败: {EndPoint}, 异常: {Message}", e.EndPoint, e.Exception?.Message);
            }
        }

        /// <summary>
        /// 获取带前缀的键
        /// </summary>
        private string GetPrefixedKey(string key) => $"{_keyPrefix}{key}";

        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <typeparam name="T">缓存值类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值，不存在则返回默认值</returns>
        public T? Get<T>(string key)
        {
            var value = _redisDatabase.StringGet(GetPrefixedKey(key));
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
            var value = await _redisDatabase.StringGetAsync(GetPrefixedKey(key));
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
            _redisDatabase.StringSet(GetPrefixedKey(key), jsonValue, expiration);
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
            await _redisDatabase.StringSetAsync(GetPrefixedKey(key), jsonValue, expiration);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        public void Remove(string key)
        {
            _redisDatabase.KeyDelete(GetPrefixedKey(key));
        }

        /// <summary>
        /// 异步删除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>任务</returns>
        public async Task RemoveAsync(string key)
        {
            await _redisDatabase.KeyDeleteAsync(GetPrefixedKey(key));
        }

        /// <summary>
        /// 检查缓存是否存在
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>是否存在</returns>
        public bool Exists(string key)
        {
            return _redisDatabase.KeyExists(GetPrefixedKey(key));
        }

        /// <summary>
        /// 异步检查缓存是否存在
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>是否存在</returns>
        public async Task<bool> ExistsAsync(string key)
        {
            return await _redisDatabase.KeyExistsAsync(GetPrefixedKey(key));
        }
    }
}