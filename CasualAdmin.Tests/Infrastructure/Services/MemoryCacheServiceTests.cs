namespace CasualAdmin.Tests.Infrastructure.Services
{
    using CasualAdmin.Infrastructure.Services;
    using Microsoft.Extensions.Caching.Memory;
    using Moq;
    using Xunit;

    /// <summary>
    /// 内存缓存服务测试类
    /// </summary>
    public class MemoryCacheServiceTests
    {
        /// <summary>
        /// 测试内存缓存服务的构造函数
        /// </summary>
        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // 创建模拟的IMemoryCache
            var mockMemoryCache = new Mock<IMemoryCache>();

            // 创建MemoryCacheService实例
            var memoryCacheService = new MemoryCacheService(mockMemoryCache.Object);

            // 断言实例创建成功
            Assert.NotNull(memoryCacheService);
        }

        /// <summary>
        /// 测试获取不存在的缓存
        /// </summary>
        [Fact]
        public async Task GetNonExistentCache_ShouldReturnNull()
        {
            // 创建模拟的IMemoryCache
            var mockMemoryCache = new Mock<IMemoryCache>();

            // 创建一个非null的对象引用
            object? cacheValue = null;

            // 设置TryGetValue返回false，表示缓存不存在
            mockMemoryCache.Setup(cache => cache.TryGetValue(It.IsAny<string>(), out cacheValue)).Returns(false);

            // 创建MemoryCacheService实例
            var memoryCacheService = new MemoryCacheService(mockMemoryCache.Object);

            // 获取不存在的缓存
            var result = await memoryCacheService.GetAsync<string>("nonExistentKey");

            // 断言返回null
            Assert.Null(result);
        }

        /// <summary>
        /// 测试检查缓存是否存在
        /// </summary>
        [Fact]
        public async Task ExistsAsync_ShouldReturnCorrectResult()
        {
            // 创建模拟的IMemoryCache
            var mockMemoryCache = new Mock<IMemoryCache>();

            // 创建非null的对象引用
            object? existingCacheValue = new();
            object? nonExistingCacheValue = null;

            // 设置TryGetValue返回true，表示缓存存在
            mockMemoryCache.Setup(cache => cache.TryGetValue("existingKey", out existingCacheValue)).Returns(true);
            // 设置TryGetValue返回false，表示缓存不存在
            mockMemoryCache.Setup(cache => cache.TryGetValue("nonExistingKey", out nonExistingCacheValue)).Returns(false);

            // 创建MemoryCacheService实例
            var memoryCacheService = new MemoryCacheService(mockMemoryCache.Object);

            // 检查存在的缓存
            var existsResult = await memoryCacheService.ExistsAsync("existingKey");
            // 检查不存在的缓存
            var notExistsResult = await memoryCacheService.ExistsAsync("nonExistingKey");

            // 断言结果正确
            Assert.True(existsResult);
            Assert.False(notExistsResult);
        }
    }
}