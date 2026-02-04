namespace CasualAdmin.Tests.Application.Services.System;

using CasualAdmin.Application.Interfaces.Events;
using CasualAdmin.Application.Interfaces.Services;
using CasualAdmin.Application.Services.System;
using CasualAdmin.Domain.Entities.System;
using CasualAdmin.Domain.Infrastructure.Data;
using CasualAdmin.Domain.Infrastructure.Services;
using Moq;
using Xunit;

/// <summary>
/// 字典服务测试
/// </summary>
public class DictServiceTests
{
    private readonly Mock<IRepository<SysDict>> _dictRepositoryMock;
    private readonly Mock<IRepository<SysDictItem>> _dictItemRepositoryMock;
    private readonly Mock<IValidationService> _validationServiceMock;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly DictService _dictService;

    /// <summary>
    /// 测试类构造函数，初始化模拟对象和被测服务
    /// </summary>
    public DictServiceTests()
    {
        // 初始化模拟对象
        _dictRepositoryMock = new Mock<IRepository<SysDict>>();
        _dictItemRepositoryMock = new Mock<IRepository<SysDictItem>>();
        _validationServiceMock = new Mock<IValidationService>();
        _eventBusMock = new Mock<IEventBus>();
        _cacheServiceMock = new Mock<ICacheService>();

        // 配置模拟验证服务
        _validationServiceMock.Setup(v => v.ValidateAndThrow(It.IsAny<SysDict>())).Verifiable();

        // 创建被测服务实例
        _dictService = new DictService(_dictRepositoryMock.Object, _validationServiceMock.Object, _eventBusMock.Object, _dictItemRepositoryMock.Object, _cacheServiceMock.Object);
    }

    /// <summary>
    /// 测试根据ID获取字典方法，当字典存在时返回正确的字典
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnDict_WhenDictExists()
    {
        // Arrange
        var dictId = Guid.NewGuid();
        var expectedDict = new SysDict
        {
            DictId = dictId,
            DictName = "Test Dict",
            DictCode = "test.dict",
            Remark = "Test dictionary description"
        };

        _dictRepositoryMock.Setup(r => r.GetByIdAsync(dictId)).ReturnsAsync(expectedDict);

        // Act
        var actualDict = await _dictService.GetByIdAsync(dictId);

        // Assert
        Assert.NotNull(actualDict);
        Assert.Equal(expectedDict.DictId, actualDict.DictId);
        Assert.Equal("Test Dict", actualDict.DictName);
        Assert.Equal("test.dict", actualDict.DictCode);
        Assert.Equal("Test dictionary description", actualDict.Remark);
        _dictRepositoryMock.Verify(r => r.GetByIdAsync(dictId), Times.Once);
    }

    /// <summary>
    /// 测试获取所有字典方法，返回所有字典列表
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllDicts()
    {
        // Arrange
        var dict1 = new SysDict
        {
            DictName = "Dict 1",
            DictCode = "dict.1",
            Remark = "Dict 1 description"
        };

        var dict2 = new SysDict
        {
            DictName = "Dict 2",
            DictCode = "dict.2",
            Remark = "Dict 2 description"
        };

        var dicts = new List<SysDict> { dict1, dict2 };

        _dictRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(dicts);

        // Act
        var result = await _dictService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dicts.Count, result.Count);
        Assert.Equal("Dict 1", result[0].DictName);
        Assert.Equal("Dict 2", result[1].DictName);
        _dictRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    /// <summary>
    /// 测试根据字典编码获取字典方法，当字典存在时返回正确的字典
    /// </summary>
    [Fact]
    public async Task GetDictByCodeAsync_ShouldReturnDict_WhenDictExists()
    {
        // Arrange
        var dictCode = "test.dict";
        var expectedDict = new SysDict
        {
            DictName = "Test Dict",
            DictCode = dictCode,
            Remark = "Test dictionary description"
        };

        _dictRepositoryMock.Setup(r => r.FindAsync(d => d.DictCode == dictCode)).ReturnsAsync(new List<SysDict> { expectedDict });

        // Act
        var actualDict = await _dictService.GetDictByCodeAsync(dictCode);

        // Assert
        Assert.NotNull(actualDict);
        Assert.Equal(expectedDict.DictId, actualDict.DictId);
        Assert.Equal("Test Dict", actualDict.DictName);
        Assert.Equal(dictCode, actualDict.DictCode);
        _dictRepositoryMock.Verify(r => r.FindAsync(d => d.DictCode == dictCode), Times.Once);
    }

    /// <summary>
    /// 测试根据字典编码获取字典方法，当字典不存在时返回null
    /// </summary>
    [Fact]
    public async Task GetDictByCodeAsync_ShouldReturnNull_WhenDictDoesNotExist()
    {
        // Arrange
        var dictCode = "non.existent.dict";
        _dictRepositoryMock.Setup(r => r.FindAsync(d => d.DictCode == dictCode)).ReturnsAsync(new List<SysDict>());

        // Act
        var actualDict = await _dictService.GetDictByCodeAsync(dictCode);

        // Assert
        Assert.Null(actualDict);
        _dictRepositoryMock.Verify(r => r.FindAsync(d => d.DictCode == dictCode), Times.Once);
    }

    /// <summary>
    /// 测试创建字典方法，确保能够创建字典
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldCreateDict()
    {
        // Arrange
        var dict = new SysDict
        {
            DictName = "New Dict",
            DictCode = "new.dict",
            Remark = "New dictionary description"
        };

        var createdDict = new SysDict
        {
            DictId = Guid.NewGuid(),
            DictName = "New Dict",
            DictCode = "new.dict",
            Remark = "New dictionary description",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _dictRepositoryMock.Setup(r => r.AddAsync(It.IsAny<SysDict>())).ReturnsAsync(createdDict);

        // Act
        var result = await _dictService.CreateAsync(dict);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdDict.DictId, result.DictId);
        Assert.Equal(createdDict.DictName, result.DictName);
        Assert.Equal(createdDict.DictCode, result.DictCode);
        Assert.NotEqual(default, result.CreatedAt);
        Assert.NotEqual(default, result.UpdatedAt);
        _dictRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SysDict>()), Times.Once);
    }

    /// <summary>
    /// 测试更新字典方法，当字典存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ShouldUpdateDict_WhenDictExists()
    {
        // Arrange
        var existingDict = new SysDict
        {
            DictName = "Old Dict",
            DictCode = "old.dict",
            Remark = "Old dictionary description"
        };

        var updatedDict = new SysDict
        {
            DictId = existingDict.DictId,
            DictName = "Updated Dict",
            DictCode = "updated.dict",
            Remark = "Updated dictionary description"
        };

        var savedDict = new SysDict
        {
            DictId = existingDict.DictId,
            DictName = "Updated Dict",
            DictCode = "updated.dict",
            Remark = "Updated dictionary description"
        };

        _dictRepositoryMock.Setup(r => r.GetByIdAsync(existingDict.DictId)).ReturnsAsync(existingDict);
        _dictRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<SysDict>())).ReturnsAsync(savedDict);

        // Act
        var result = await _dictService.UpdateAsync(updatedDict);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(savedDict.DictId, result.DictId);
        Assert.Equal("Updated Dict", result.DictName);
        Assert.Equal("updated.dict", result.DictCode);
        _dictRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SysDict>()), Times.Once);
    }

    /// <summary>
    /// 测试删除字典方法，当字典存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenDictExists()
    {
        // Arrange
        var dictId = Guid.NewGuid();
        _dictRepositoryMock.Setup(r => r.DeleteAsync(dictId)).ReturnsAsync(true);

        // Act
        var result = await _dictService.DeleteAsync(dictId);

        // Assert
        Assert.True(result);
        _dictRepositoryMock.Verify(r => r.DeleteAsync(dictId), Times.Once);
    }

    /// <summary>
    /// 测试删除字典方法，当字典不存在时返回失败结果
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenDictDoesNotExist()
    {
        // Arrange
        var dictId = Guid.NewGuid();
        _dictRepositoryMock.Setup(r => r.DeleteAsync(dictId)).ReturnsAsync(false);

        // Act
        var result = await _dictService.DeleteAsync(dictId);

        // Assert
        Assert.False(result);
        _dictRepositoryMock.Verify(r => r.DeleteAsync(dictId), Times.Once);
    }
}
