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
/// 字典控制器测试
/// </summary>
public class DictControllerTests
{
    private readonly Mock<IDictService> _dictServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly DictController _dictController;

    /// <summary>
    /// 构造函数，初始化模拟对象和被测控制器
    /// </summary>
    public DictControllerTests()
    {
        // 初始化模拟对象
        _dictServiceMock = new Mock<IDictService>();
        _mapperMock = new Mock<IMapper>();

        // 创建被测控制器实例
        _dictController = new DictController(_dictServiceMock.Object, _mapperMock.Object);
    }

    /// <summary>
    /// 测试获取所有字典方法，当有字典时返回成功结果
    /// </summary>
    [Fact]
    public async Task GetAllDicts_ShouldReturnSuccessResult_WhenDictsExist()
    {
        // Arrange
        var dictList = new List<SysDict> { new SysDict() };
        var dictDtoList = new List<SysDictDto> { new SysDictDto() };

        _dictServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(dictList);
        _mapperMock.Setup(mapper => mapper.Map<List<SysDictDto>>(dictList)).Returns(dictDtoList);

        // Act
        var result = await _dictController.GetAllDicts();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(dictDtoList, result.Data);
    }

    /// <summary>
    /// 测试根据ID获取字典方法，当字典存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task GetDictById_ShouldReturnSuccessResult_WhenDictExists()
    {
        // Arrange
        var dictId = Guid.NewGuid();
        var dict = new SysDict();
        var dictDto = new SysDictDto();

        _dictServiceMock.Setup(service => service.GetByIdAsync(dictId)).ReturnsAsync(dict);
        _mapperMock.Setup(mapper => mapper.Map<SysDictDto>(dict)).Returns(dictDto);

        // Act
        var result = await _dictController.GetDictById(dictId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(dictDto, result.Data);
    }

    /// <summary>
    /// 测试根据ID获取字典方法，当字典不存在时返回NotFound结果
    /// </summary>
    [Fact]
    public async Task GetDictById_ShouldReturnNotFoundResult_WhenDictDoesNotExist()
    {
        // Arrange
        var dictId = Guid.NewGuid();

        _dictServiceMock.Setup(service => service.GetByIdAsync(dictId)).ReturnsAsync((SysDict?)null);

        // Act
        var result = await _dictController.GetDictById(dictId);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(404, result.Code);
    }

    /// <summary>
    /// 测试根据字典编码获取字典方法，当字典存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task GetDictByCode_ShouldReturnSuccessResult_WhenDictExists()
    {
        // Arrange
        var dictCode = "test_dict";
        var dict = new SysDict();
        var dictDto = new SysDictDto();

        _dictServiceMock.Setup(service => service.GetDictByCodeAsync(dictCode)).ReturnsAsync(dict);
        _mapperMock.Setup(mapper => mapper.Map<SysDictDto>(dict)).Returns(dictDto);

        // Act
        var result = await _dictController.GetDictByCode(dictCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(dictDto, result.Data);
    }

    /// <summary>
    /// 测试根据字典编码获取字典方法，当字典不存在时返回NotFound结果
    /// </summary>
    [Fact]
    public async Task GetDictByCode_ShouldReturnNotFoundResult_WhenDictDoesNotExist()
    {
        // Arrange
        var dictCode = "non_existent_dict";

        _dictServiceMock.Setup(service => service.GetDictByCodeAsync(dictCode)).ReturnsAsync((SysDict?)null);

        // Act
        var result = await _dictController.GetDictByCode(dictCode);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(404, result.Code);
    }

    /// <summary>
    /// 测试创建字典方法，返回成功结果
    /// </summary>
    [Fact]
    public async Task CreateDict_ShouldReturnSuccessResult()
    {
        // Arrange
        var dictCreateDto = new SysDictCreateDto();
        var dict = new SysDict();
        var createdDict = new SysDict();
        var createdDictDto = new SysDictDto();

        _mapperMock.Setup(mapper => mapper.Map<SysDict>(dictCreateDto)).Returns(dict);
        _dictServiceMock.Setup(service => service.CreateAsync(dict)).ReturnsAsync(createdDict);
        _mapperMock.Setup(mapper => mapper.Map<SysDictDto>(createdDict)).Returns(createdDictDto);

        // Act
        var result = await _dictController.CreateDict(dictCreateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(createdDictDto, result.Data);
    }

    /// <summary>
    /// 测试更新字典方法，当字典存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task UpdateDict_ShouldReturnSuccessResult_WhenDictExists()
    {
        // Arrange
        var dictId = Guid.NewGuid();
        var dictUpdateDto = new SysDictUpdateDto();
        var existingDict = new SysDict();
        var updatedDict = new SysDict();
        var updatedDictDto = new SysDictDto();

        _dictServiceMock.Setup(service => service.GetByIdAsync(dictId)).ReturnsAsync(existingDict);
        _dictServiceMock.Setup(service => service.UpdateAsync(existingDict)).ReturnsAsync(updatedDict);
        _mapperMock.Setup(mapper => mapper.Map(It.IsAny<SysDictUpdateDto>(), It.IsAny<SysDict>())).Verifiable();
        _mapperMock.Setup(mapper => mapper.Map<SysDictDto>(updatedDict)).Returns(updatedDictDto);

        // Act
        var result = await _dictController.UpdateDict(dictId, dictUpdateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(updatedDictDto, result.Data);
        _mapperMock.Verify(mapper => mapper.Map(dictUpdateDto, existingDict), Times.Once);
    }

    /// <summary>
    /// 测试更新字典方法，当字典不存在时返回NotFound结果
    /// </summary>
    [Fact]
    public async Task UpdateDict_ShouldReturnNotFoundResult_WhenDictDoesNotExist()
    {
        // Arrange
        var dictId = Guid.NewGuid();
        var dictUpdateDto = new SysDictUpdateDto();

        _dictServiceMock.Setup(service => service.GetByIdAsync(dictId)).ReturnsAsync((SysDict?)null);

        // Act
        var result = await _dictController.UpdateDict(dictId, dictUpdateDto);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(404, result.Code);
    }

    /// <summary>
    /// 测试删除字典方法，当删除成功时返回成功结果
    /// </summary>
    [Fact]
    public async Task DeleteDict_ShouldReturnSuccessResult_WhenDeleteSucceeds()
    {
        // Arrange
        var dictId = Guid.NewGuid();

        _dictServiceMock.Setup(service => service.DeleteAsync(dictId)).ReturnsAsync(true);

        // Act
        var result = await _dictController.DeleteDict(dictId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.True(result.Data);
    }

    /// <summary>
    /// 测试删除字典方法，当删除失败时返回Failed结果
    /// </summary>
    [Fact]
    public async Task DeleteDict_ShouldReturnFailedResult_WhenDeleteFails()
    {
        // Arrange
        var dictId = Guid.NewGuid();

        _dictServiceMock.Setup(service => service.DeleteAsync(dictId)).ReturnsAsync(false);

        // Act
        var result = await _dictController.DeleteDict(dictId);

        // Assert
        Assert.NotNull(result);

        Assert.False(result.Data);
    }
}
