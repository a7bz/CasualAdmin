namespace CasualAdmin.Tests.Application.Services
{
    using CasualAdmin.Application.Services;
    using Moq;
    using Xunit;

    /// <summary>
    /// 测试用实体类
    /// </summary>
    public class TestEntity
    {
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// 验证服务测试类
    /// </summary>
    public class ValidationServiceTests
    {
        /// <summary>
        /// 测试验证服务的构造函数
        /// </summary>
        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // 创建模拟的IServiceProvider
            var mockServiceProvider = new Mock<IServiceProvider>();

            // 创建ValidationService实例
            var validationService = new ValidationService(mockServiceProvider.Object);

            // 断言实例创建成功
            Assert.NotNull(validationService);
        }

        /// <summary>
        /// 测试验证空实体
        /// </summary>
        [Fact]
        public void Validate_NullEntity_ShouldReturnFalse()
        {
            // 创建模拟的IServiceProvider
            var mockServiceProvider = new Mock<IServiceProvider>();

            // 创建ValidationService实例
            var validationService = new ValidationService(mockServiceProvider.Object);

            // 调用验证方法
            var (IsValid, Errors) = validationService.Validate<TestEntity>(null);

            // 断言验证结果正确
            Assert.False(IsValid);
            Assert.Contains("实体不能为空", Errors);
        }

        /// <summary>
        /// 测试验证有效实体
        /// </summary>
        [Fact]
        public void Validate_ValidEntity_ShouldReturnTrue()
        {
            // 创建模拟的IServiceProvider
            var mockServiceProvider = new Mock<IServiceProvider>();

            // 创建ValidationService实例
            var validationService = new ValidationService(mockServiceProvider.Object);

            // 创建测试实体
            var testEntity = new TestEntity { Name = "test" };

            // 调用验证方法
            var (IsValid, Errors) = validationService.Validate(testEntity);

            // 断言验证结果正确
            Assert.True(IsValid);
            Assert.Empty(Errors);
        }
    }
}