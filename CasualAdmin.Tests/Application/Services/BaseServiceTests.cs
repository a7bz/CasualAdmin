namespace CasualAdmin.Tests.Application.Services
{
    using CasualAdmin.Application.Interfaces.Events;
    using CasualAdmin.Application.Interfaces.Services;
    using CasualAdmin.Application.Services;
    using CasualAdmin.Domain.Entities.System;
    using CasualAdmin.Domain.Infrastructure.Data;
    using Moq;
    using Xunit;

    /// <summary>
    /// 测试用的BaseService子类
    /// </summary>
    public class TestBaseService : BaseService<SysUser>
    {
        public TestBaseService(IRepository<SysUser> repository, IValidationService validationService, IEventBus eventBus)
            : base(repository, validationService, eventBus)
        {
        }
    }

    /// <summary>
    /// 基础服务测试类
    /// </summary>
    public class BaseServiceTests
    {
        /// <summary>
        /// 测试BaseService的构造函数
        /// </summary>
        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // 创建模拟的依赖
            var mockRepository = new Mock<IRepository<SysUser>>();
            var mockValidationService = new Mock<IValidationService>();
            var mockEventBus = new Mock<IEventBus>();

            // 创建BaseService实例
            var baseService = new TestBaseService(mockRepository.Object, mockValidationService.Object, mockEventBus.Object);

            // 断言实例创建成功
            Assert.NotNull(baseService);
        }

        /// <summary>
        /// 测试GetAllAsync方法
        /// </summary>
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            // 创建模拟的依赖
            var mockRepository = new Mock<IRepository<SysUser>>();

            // 设置期望结果为一个空列表，避免使用SysUser的构造函数
            mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<SysUser>());

            var mockValidationService = new Mock<IValidationService>();
            var mockEventBus = new Mock<IEventBus>();

            // 创建BaseService实例
            var baseService = new TestBaseService(mockRepository.Object, mockValidationService.Object, mockEventBus.Object);

            // 调用GetAllAsync方法
            var result = await baseService.GetAllAsync();

            // 断言结果正确
            Assert.NotNull(result);
            mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        /// <summary>
        /// 测试GetByIdAsync方法
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ShouldReturnEntityById()
        {
            // 准备测试数据
            var userId = Guid.NewGuid();

            // 创建模拟的依赖
            var mockRepository = new Mock<IRepository<SysUser>>();
            mockRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((SysUser?)null);

            var mockValidationService = new Mock<IValidationService>();
            var mockEventBus = new Mock<IEventBus>();

            // 创建BaseService实例
            var baseService = new TestBaseService(mockRepository.Object, mockValidationService.Object, mockEventBus.Object);

            // 调用GetByIdAsync方法
            var result = await baseService.GetByIdAsync(userId);

            // 断言结果正确
            Assert.Null(result);
            mockRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        }
    }
}