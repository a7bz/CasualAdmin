namespace CasualAdmin.Tests.API.Controllers.System;

using AutoMapper;
using CasualAdmin.API.Controllers.System;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Models.DTOs.Requests.System;
using CasualAdmin.Application.Models.DTOs.Responses.System;
using CasualAdmin.Domain.Entities.System;
using Moq;
using Xunit;

/// <summary>
/// 字典项控制器测试
/// </summary>
public class DictItemControllerTests
{
    private readonly Mock<IDictItemService> _dictItemServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly DictItemController _dictItemController;

    /// <summary>
    /// 构造函数，初始化模拟对象和被测控制器
    /// </summary>
    public DictItemControllerTests()
    {
        // 初始化模拟对象
        _dictItemServiceMock = new Mock<IDictItemService>();
        _mapperMock = new Mock<IMapper>();

        // 创建被测控制器实例
        _dictItemController = new DictItemController(_dictItemServiceMock.Object, _mapperMock.Object);
    }

    /// <summary>
    /// 测试获取所有字典项方法，当有字典项时返回成功结果
    /// </summary>
    [Fact]
    public async Task GetAllDictItems_ShouldReturnSuccessResult_WhenDictItemsExist()
    {
        // Arrange
        var dictItemList = new List<SysDictItem> { new SysDictItem() };
        var dictItemDtoList = new List<SysDictItemDto> { new SysDictItemDto() };

        _dictItemServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(dictItemList);
        _mapperMock.Setup(mapper => mapper.Map<List<SysDictItemDto>>(dictItemList)).Returns(dictItemDtoList);

        // Act
        var result = await _dictItemController.GetAllDictItems();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(dictItemDtoList, result.Data);
    }

    /// <summary>
    /// 测试根据ID获取字典项方法，当字典项存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task GetDictItemById_ShouldReturnSuccessResult_WhenDictItemExists()
    {
        // Arrange
        var dictItemId = Guid.NewGuid();
        var dictItem = new SysDictItem();
        var dictItemDto = new SysDictItemDto();

        _dictItemServiceMock.Setup(service => service.GetByIdAsync(dictItemId)).ReturnsAsync(dictItem);
        _mapperMock.Setup(mapper => mapper.Map<SysDictItemDto>(dictItem)).Returns(dictItemDto);

        // Act
        var result = await _dictItemController.GetDictItemById(dictItemId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(dictItemDto, result.Data);
    }

    /// <summary>
    /// 测试根据ID获取字典项方法，当字典项不存在时返回NotFound结果
    /// </summary>
    [Fact]
    public async Task GetDictItemById_ShouldReturnNotFoundResult_WhenDictItemDoesNotExist()
    {
        // Arrange
        var dictItemId = Guid.NewGuid();

        _dictItemServiceMock.Setup(service => service.GetByIdAsync(dictItemId)).ReturnsAsync((SysDictItem?)null);

        // Act
        var result = await _dictItemController.GetDictItemById(dictItemId);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(404, result.Code);
    }

    /// <summary>
    /// 测试根据字典编码获取字典项列表方法，当有字典项时返回成功结果
    /// </summary>
    [Fact]
    public async Task GetDictItemsByDictCode_ShouldReturnSuccessResult_WhenDictItemsExist()
    {
        // Arrange
        var dictCode = "test_dict";
        var dictItemList = new List<SysDictItem> { new SysDictItem() };
        var dictItemDtoList = new List<SysDictItemDto> { new SysDictItemDto() };

        _dictItemServiceMock.Setup(service => service.GetDictItemsByDictCodeAsync(dictCode)).ReturnsAsync(dictItemList);
        _mapperMock.Setup(mapper => mapper.Map<List<SysDictItemDto>>(dictItemList)).Returns(dictItemDtoList);

        // Act
        var result = await _dictItemController.GetDictItemsByDictCode(dictCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(dictItemDtoList, result.Data);
    }

    /// <summary>
    /// 测试创建字典项方法，返回成功结果
    /// </summary>
    [Fact]
    public async Task CreateDictItem_ShouldReturnSuccessResult()
    {
        // Arrange
        var dictItemCreateDto = new SysDictItemCreateDto();
        var dictItem = new SysDictItem();
        var createdDictItem = new SysDictItem();
        var createdDictItemDto = new SysDictItemDto();

        _mapperMock.Setup(mapper => mapper.Map<SysDictItem>(dictItemCreateDto)).Returns(dictItem);
        _dictItemServiceMock.Setup(service => service.CreateAsync(dictItem)).ReturnsAsync(createdDictItem);
        _mapperMock.Setup(mapper => mapper.Map<SysDictItemDto>(createdDictItem)).Returns(createdDictItemDto);

        // Act
        var result = await _dictItemController.CreateDictItem(dictItemCreateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(createdDictItemDto, result.Data);
    }

    /// <summary>
    /// 测试更新字典项方法，当字典项存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task UpdateDictItem_ShouldReturnSuccessResult_WhenDictItemExists()
    {
        // Arrange
        var dictItemId = Guid.NewGuid();
        var dictItemUpdateDto = new SysDictItemUpdateDto();
        var existingDictItem = new SysDictItem();
        var updatedDictItem = new SysDictItem();
        var updatedDictItemDto = new SysDictItemDto();

        _dictItemServiceMock.Setup(service => service.GetByIdAsync(dictItemId)).ReturnsAsync(existingDictItem);
        _dictItemServiceMock.Setup(service => service.UpdateAsync(existingDictItem)).ReturnsAsync(updatedDictItem);
        _mapperMock.Setup(mapper => mapper.Map(It.IsAny<SysDictItemUpdateDto>(), It.IsAny<SysDictItem>())).Verifiable();
        _mapperMock.Setup(mapper => mapper.Map<SysDictItemDto>(updatedDictItem)).Returns(updatedDictItemDto);

        // Act
        var result = await _dictItemController.UpdateDictItem(dictItemId, dictItemUpdateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(updatedDictItemDto, result.Data);
        _mapperMock.Verify(mapper => mapper.Map(dictItemUpdateDto, existingDictItem), Times.Once);
    }

    /// <summary>
    /// 测试更新字典项方法，当字典项不存在时返回NotFound结果
    /// </summary>
    [Fact]
    public async Task UpdateDictItem_ShouldReturnNotFoundResult_WhenDictItemDoesNotExist()
    {
        // Arrange
        var dictItemId = Guid.NewGuid();
        var dictItemUpdateDto = new SysDictItemUpdateDto();

        _dictItemServiceMock.Setup(service => service.GetByIdAsync(dictItemId)).ReturnsAsync((SysDictItem?)null);

        // Act
        var result = await _dictItemController.UpdateDictItem(dictItemId, dictItemUpdateDto);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(404, result.Code);
    }

    /// <summary>
    /// 测试删除字典项方法，当删除成功时返回成功结果
    /// </summary>
    [Fact]
    public async Task DeleteDictItem_ShouldReturnSuccessResult_WhenDeleteSucceeds()
    {
        // Arrange
        var dictItemId = Guid.NewGuid();

        _dictItemServiceMock.Setup(service => service.DeleteAsync(dictItemId)).ReturnsAsync(true);

        // Act
        var result = await _dictItemController.DeleteDictItem(dictItemId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.True(result.Data);
    }

    /// <summary>
    /// 测试删除字典项方法，当删除失败时返回Failed结果
    /// </summary>
    [Fact]
    public async Task DeleteDictItem_ShouldReturnFailedResult_WhenDeleteFails()
    {
        // Arrange
        var dictItemId = Guid.NewGuid();

        _dictItemServiceMock.Setup(service => service.DeleteAsync(dictItemId)).ReturnsAsync(false);

        // Act
        var result = await _dictItemController.DeleteDictItem(dictItemId);

        // Assert
        Assert.NotNull(result);

        Assert.False(result.Data);
    }
}
