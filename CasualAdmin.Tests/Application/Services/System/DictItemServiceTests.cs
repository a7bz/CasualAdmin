namespace CasualAdmin.Tests.Application.Services.System;

using CasualAdmin.Application.Interfaces.Events;
using CasualAdmin.Application.Interfaces.Services;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Services.System;
using CasualAdmin.Domain.Entities.System;
using CasualAdmin.Domain.Infrastructure.Data;
using CasualAdmin.Domain.Infrastructure.Services;
using global::System.Linq.Expressions;
using Moq;
using Xunit;

/// <summary>
/// 字典项服务测试类
/// </summary>
public class DictItemServiceTests
{
    private readonly Mock<IRepository<SysDictItem>> _dictItemRepositoryMock;
    private readonly Mock<IValidationService> _validationServiceMock;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly Mock<IDictService> _dictServiceMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly DictItemService _dictItemService;

    /// <summary>
    /// 测试类构造函数，初始化模拟对象和被测服务
    /// </summary>
    public DictItemServiceTests()
    {
        // 初始化模拟对象
        _dictItemRepositoryMock = new Mock<IRepository<SysDictItem>>();
        _validationServiceMock = new Mock<IValidationService>();
        _eventBusMock = new Mock<IEventBus>();
        _dictServiceMock = new Mock<IDictService>();
        _cacheServiceMock = new Mock<ICacheService>();

        // 配置模拟验证服务
        _validationServiceMock.Setup(v => v.ValidateAndThrow(It.IsAny<SysDictItem>())).Verifiable();

        // 创建被测服务实例
        _dictItemService = new DictItemService(_dictItemRepositoryMock.Object, _validationServiceMock.Object, _eventBusMock.Object, _dictServiceMock.Object, _cacheServiceMock.Object);
    }

    /// <summary>
    /// 测试根据ID获取字典项方法，当字典项存在时返回正确的字典项
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnDictItem_WhenDictItemExists()
    {
        // Arrange
        var dictItemId = Guid.NewGuid();
        var dictId = Guid.NewGuid();
        var expectedDictItem = new SysDictItem
        {
            DictItemId = dictItemId,
            Label = "Test Label",
            Value = "test-value",
            DictId = dictId,
            Sort = 1
        };

        _dictItemRepositoryMock.Setup(r => r.GetByIdAsync(dictItemId)).ReturnsAsync(expectedDictItem);

        // Act
        var actualDictItem = await _dictItemService.GetByIdAsync(dictItemId);

        // Assert
        Assert.NotNull(actualDictItem);
        Assert.Equal(expectedDictItem.DictItemId, actualDictItem.DictItemId);
        Assert.Equal("Test Label", actualDictItem.Label);
        Assert.Equal("test-value", actualDictItem.Value);
        Assert.Equal(1, actualDictItem.Sort);
        _dictItemRepositoryMock.Verify(r => r.GetByIdAsync(dictItemId), Times.Once);
    }

    /// <summary>
    /// 测试获取所有字典项方法，返回所有字典项列表
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllDictItems()
    {
        // Arrange
        var dictId = Guid.NewGuid();
        var dictItem1 = new SysDictItem
        {
            Label = "Item 1",
            Value = "item-1",
            DictId = dictId,
            Sort = 1
        };

        var dictItem2 = new SysDictItem
        {
            Label = "Item 2",
            Value = "item-2",
            DictId = dictId,
            Sort = 2
        };

        var dictItems = new List<SysDictItem> { dictItem1, dictItem2 };

        _dictItemRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(dictItems);

        // Act
        var result = await _dictItemService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dictItems.Count, result.Count);
        Assert.Equal("Item 1", result[0].Label);
        Assert.Equal("Item 2", result[1].Label);
        _dictItemRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    /// <summary>
    /// 测试根据字典编码获取字典项列表方法，返回指定字典编码的所有字典项
    /// 验证返回的字典项列表包含正确的字典项信息
    /// </summary>
    [Fact]
    public async Task GetDictItemsByDictCodeAsync_ShouldReturnDictItems()
    {
        // Arrange
        var dictCode = "test.dict";
        var dictId = Guid.NewGuid();
        var dict = new SysDict
        {
            DictId = dictId,
            DictCode = dictCode
        };

        var dictItem1 = new SysDictItem
        {
            Label = "Item 1",
            Value = "item-1",
            DictId = dictId,
            Sort = 1
        };

        var dictItem2 = new SysDictItem
        {
            Label = "Item 2",
            Value = "item-2",
            DictId = dictId,
            Sort = 2
        };

        var dictItems = new List<SysDictItem> { dictItem1, dictItem2 };

        // 配置模拟服务
        _dictServiceMock.Setup(ds => ds.GetDictByCodeAsync(dictCode)).ReturnsAsync(dict);
        _dictItemRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<SysDictItem, bool>>>())).ReturnsAsync(dictItems);

        // Act
        var result = await _dictItemService.GetDictItemsByDictCodeAsync(dictCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dictItems.Count, result.Count);
        Assert.Equal("Item 1", result[0].Label);
        Assert.Equal("Item 2", result[1].Label);
        Assert.All(result, di => Assert.Equal(dictId, di.DictId));
        _dictServiceMock.Verify(ds => ds.GetDictByCodeAsync(dictCode), Times.Once);
        _dictItemRepositoryMock.Verify(r => r.FindAsync(It.IsAny<Expression<Func<SysDictItem, bool>>>()), Times.Once);
    }

    /// <summary>
    /// 测试创建字典项方法，确保能够创建字典项并返回正确的结果
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldCreateDictItem()
    {
        // Arrange
        var dictId = Guid.NewGuid();
        var dictItem = new SysDictItem
        {
            Label = "New Item",
            Value = "new-item",
            DictId = dictId,
            Sort = 1
        };

        var createdDictItem = new SysDictItem
        {
            DictItemId = Guid.NewGuid(),
            Label = "New Item",
            Value = "new-item",
            DictId = dictId,
            Sort = 1
        };

        _dictItemRepositoryMock.Setup(r => r.AddAsync(It.IsAny<SysDictItem>())).ReturnsAsync(createdDictItem);

        // Act
        var result = await _dictItemService.CreateAsync(dictItem);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdDictItem.DictItemId, result.DictItemId);
        Assert.Equal(createdDictItem.Label, result.Label);
        Assert.Equal(createdDictItem.Value, result.Value);
        Assert.Equal(createdDictItem.Sort, result.Sort);
        Assert.NotEqual(default, result.CreatedAt);
        Assert.NotEqual(default, result.UpdatedAt);
        _dictItemRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SysDictItem>()), Times.Once);
    }

    /// <summary>
    /// 测试更新字典项方法，当字典项存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ShouldUpdateDictItem_WhenDictItemExists()
    {
        // Arrange
        var existingDictItem = new SysDictItem
        {
            DictItemId = Guid.NewGuid(),
            Label = "Old Label",
            Value = "old-value",
            DictId = Guid.NewGuid(),
            Sort = 1
        };

        var updatedDictItem = new SysDictItem
        {
            DictItemId = existingDictItem.DictItemId,
            Label = "Updated Label",
            Value = "updated-value",
            DictId = existingDictItem.DictId,
            Sort = 2
        };

        var savedDictItem = new SysDictItem
        {
            DictItemId = existingDictItem.DictItemId,
            Label = "Updated Label",
            Value = "updated-value",
            DictId = existingDictItem.DictId,
            Sort = 2
        };

        _dictItemRepositoryMock.Setup(r => r.GetByIdAsync(existingDictItem.DictItemId)).ReturnsAsync(existingDictItem);
        _dictItemRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<SysDictItem>())).ReturnsAsync(savedDictItem);

        // Act
        var result = await _dictItemService.UpdateAsync(updatedDictItem);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(savedDictItem.DictItemId, result.DictItemId);
        Assert.Equal("Updated Label", result.Label);
        Assert.Equal("updated-value", result.Value);
        Assert.Equal(2, result.Sort);
        _dictItemRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SysDictItem>()), Times.Once);
    }

    /// <summary>
    /// 测试删除字典项方法，当字典项存在时返回成功结果
    /// 验证字典项已从数据库中删除
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenDictItemExists()
    {
        // Arrange
        var dictItemId = Guid.NewGuid();
        _dictItemRepositoryMock.Setup(r => r.DeleteAsync(dictItemId)).ReturnsAsync(true);

        // Act
        var result = await _dictItemService.DeleteAsync(dictItemId);

        // Assert
        Assert.True(result);
        _dictItemRepositoryMock.Verify(r => r.DeleteAsync(dictItemId), Times.Once);
    }

    /// <summary>
    /// 测试删除字典项方法，当字典项不存在时返回失败结果
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenDictItemDoesNotExist()
    {
        // Arrange
        var dictItemId = Guid.NewGuid();
        _dictItemRepositoryMock.Setup(r => r.DeleteAsync(dictItemId)).ReturnsAsync(false);

        // Act
        var result = await _dictItemService.DeleteAsync(dictItemId);

        // Assert
        Assert.False(result);
        _dictItemRepositoryMock.Verify(r => r.DeleteAsync(dictItemId), Times.Once);
    }
}
