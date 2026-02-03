namespace CasualAdmin.Tests.Infrastructure.Services
{
    using CasualAdmin.Infrastructure.Cache;
    using CasualAdmin.Infrastructure.Services;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    /// <summary>
    /// Redis缓存服务测试类
    /// </summary>
    public class RedisCacheServiceTests
    {
        /// <summary>
        /// 测试Redis缓存服务的构造函数，使用有效配置
        /// </summary>
        [Fact]
        public void Constructor_WithValidConfiguration_ShouldInitializeCorrectly()
        {
            // 创建缓存配置选项
            var cacheOptions = new CacheOptions
            {
                Enabled = true,
                Redis = new RedisOptions
                {
                    Host = "localhost",
                    Port = 6379,
                    Password = "",
                    Database = 0,
                    KeyPrefix = "casualadmin:",
                    Ssl = false,
                    AbortConnect = false,
                    ConnectTimeout = 5000,
                    SyncTimeout = 5000
                }
            };

            // 创建模拟的IOptions
            var mockOptions = new Mock<IOptions<CacheOptions>>();
            mockOptions.Setup(x => x.Value).Returns(cacheOptions);

            // 创建RedisCacheService实例
            var redisCacheService = new RedisCacheService(mockOptions.Object);

            // 断言实例创建成功
            Assert.NotNull(redisCacheService);
        }
    }
}