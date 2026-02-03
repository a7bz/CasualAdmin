namespace CasualAdmin.Tests.Infrastructure.Services
{
    using CasualAdmin.Infrastructure.Services;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using Xunit;

    /// <summary>
    /// Redis缓存服务测试类
    /// </summary>
    public class RedisCacheServiceTests
    {
        /// <summary>
        /// 测试Redis缓存服务的构造函数，使用无效配置
        /// </summary>
        [Fact]
        public void Constructor_WithInvalidConfiguration_ShouldThrowException()
        {
            // 创建模拟的IConfiguration，不包含Redis配置
            var mockConfiguration = new Mock<IConfiguration>();

            // 断言构造函数抛出异常（实际会抛出NullReferenceException）
            Assert.Throws<NullReferenceException>(() => new RedisCacheService(mockConfiguration.Object));
        }

        /// <summary>
        /// 测试Redis缓存服务的构造函数，使用有效配置但带有abortConnect=false以避免实际连接
        /// </summary>
        [Fact]
        public void Constructor_WithValidConfiguration_ShouldInitializeCorrectly()
        {
            // 创建配置构建器
            var configurationBuilder = new ConfigurationBuilder();

            // 添加测试配置，使用abortConnect=false避免实际连接Redis服务器
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?> {
                { "Redis:ConnectionString", "localhost:6379,abortConnect=false" }
            });

            // 构建配置
            var configuration = configurationBuilder.Build();

            // 创建RedisCacheService实例
            var redisCacheService = new RedisCacheService(configuration);

            // 断言实例创建成功
            Assert.NotNull(redisCacheService);
        }
    }
}