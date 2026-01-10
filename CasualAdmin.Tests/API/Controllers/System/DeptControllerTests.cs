namespace CasualAdmin.Tests.API.Controllers.System;

using AutoMapper;
using CasualAdmin.API.Controllers.System;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Models.DTOs.Requests.System;
using CasualAdmin.Application.Models.DTOs.Responses.System;
using CasualAdmin.Domain.Entities.System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

/// <summary>
/// 部门控制器测试
/// </summary>
public class DeptControllerTests
{
    private readonly Mock<IDeptService> _deptServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly DeptController _deptController;

    /// <summary>
    /// 构造函数，初始化模拟对象和被测控制器
    /// </summary>
    public DeptControllerTests()
    {
        // 初始化模拟对象
        _deptServiceMock = new Mock<IDeptService>();
        _mapperMock = new Mock<IMapper>();

        // 创建被测控制器实例
        _deptController = new DeptController(_deptServiceMock.Object, _mapperMock.Object);
    }

    /// <summary>
    /// 测试获取所有部门方法，当有部门时返回成功结果
    /// </summary>
    [Fact]
    public async Task GetAllDepts_ShouldReturnSuccessResult_WhenDeptsExist()
    {
        // Arrange
        var deptList = new List<SysDept> { new SysDept() };
        var deptDtoList = new List<SysDeptDto> { new SysDeptDto() };

        _deptServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(deptList);
        _mapperMock.Setup(mapper => mapper.Map<List<SysDeptDto>>(deptList)).Returns(deptDtoList);

        // Act
        var result = await _deptController.GetAllDepts();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(deptDtoList, result.Data);
    }

    /// <summary>
    /// 测试根据ID获取部门方法，当部门存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task GetDeptById_ShouldReturnSuccessResult_WhenDeptExists()
    {
        // Arrange
        var deptId = Guid.NewGuid();
        var dept = new SysDept();
        var deptDto = new SysDeptDto();

        _deptServiceMock.Setup(service => service.GetByIdAsync(deptId)).ReturnsAsync(dept);
        _mapperMock.Setup(mapper => mapper.Map<SysDeptDto>(dept)).Returns(deptDto);

        // Act
        var result = await _deptController.GetDeptById(deptId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(deptDto, result.Data);
    }

    /// <summary>
    /// 测试根据ID获取部门方法，当部门不存在时返回NotFound结果
    /// </summary>
    [Fact]
    public async Task GetDeptById_ShouldReturnNotFoundResult_WhenDeptDoesNotExist()
    {
        // Arrange
        var deptId = Guid.NewGuid();

        _deptServiceMock.Setup(service => service.GetByIdAsync(deptId)).ReturnsAsync((SysDept?)null);

        // Act
        var result = await _deptController.GetDeptById(deptId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.Code);
    }

    /// <summary>
    /// 测试获取部门树方法，返回成功结果
    /// </summary>
    [Fact]
    public async Task GetDeptTree_ShouldReturnSuccessResult()
    {
        // Arrange
        var deptTree = new List<SysDept> { new SysDept() };
        var deptTreeDto = new List<SysDeptTreeDto> { new SysDeptTreeDto() };

        _deptServiceMock.Setup(service => service.GetDeptTreeAsync()).ReturnsAsync(deptTree);
        _mapperMock.Setup(mapper => mapper.Map<List<SysDeptTreeDto>>(deptTree)).Returns(deptTreeDto);

        // Act
        var result = await _deptController.GetDeptTree();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(deptTreeDto, result.Data);
    }

    /// <summary>
    /// 测试根据父部门ID获取子部门列表方法，返回成功结果
    /// </summary>
    [Fact]
    public async Task GetChildrenByParentId_ShouldReturnSuccessResult()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var childrenList = new List<SysDept> { new SysDept() };
        var childrenDtoList = new List<SysDeptDto> { new SysDeptDto() };

        _deptServiceMock.Setup(service => service.GetChildrenByParentIdAsync(parentId)).ReturnsAsync(childrenList);
        _mapperMock.Setup(mapper => mapper.Map<List<SysDeptDto>>(childrenList)).Returns(childrenDtoList);

        // Act
        var result = await _deptController.GetChildrenByParentId(parentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(childrenDtoList, result.Data);
    }

    /// <summary>
    /// 测试创建部门方法，返回成功结果
    /// </summary>
    [Fact]
    public async Task CreateDept_ShouldReturnSuccessResult()
    {
        // Arrange
        var deptCreateDto = new SysDeptCreateDto();
        var dept = new SysDept();
        var createdDept = new SysDept();
        var createdDeptDto = new SysDeptDto();

        _mapperMock.Setup(mapper => mapper.Map<SysDept>(deptCreateDto)).Returns(dept);
        _deptServiceMock.Setup(service => service.CreateAsync(dept)).ReturnsAsync(createdDept);
        _mapperMock.Setup(mapper => mapper.Map<SysDeptDto>(createdDept)).Returns(createdDeptDto);

        // Act
        var result = await _deptController.CreateDept(deptCreateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(createdDeptDto, result.Data);
    }

    /// <summary>
    /// 测试更新部门方法，当部门存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task UpdateDept_ShouldReturnSuccessResult_WhenDeptExists()
    {
        // Arrange
        var deptId = Guid.NewGuid();
        var deptUpdateDto = new SysDeptUpdateDto();
        var existingDept = new SysDept();
        var updatedDept = new SysDept();
        var updatedDeptDto = new SysDeptDto();

        _deptServiceMock.Setup(service => service.GetByIdAsync(deptId)).ReturnsAsync(existingDept);
        _deptServiceMock.Setup(service => service.UpdateAsync(existingDept)).ReturnsAsync(updatedDept);
        _mapperMock.Setup(mapper => mapper.Map(It.IsAny<SysDeptUpdateDto>(), It.IsAny<SysDept>())).Verifiable();
        _mapperMock.Setup(mapper => mapper.Map<SysDeptDto>(updatedDept)).Returns(updatedDeptDto);

        // Act
        var result = await _deptController.UpdateDept(deptId, deptUpdateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(updatedDeptDto, result.Data);
        _mapperMock.Verify(mapper => mapper.Map(deptUpdateDto, existingDept), Times.Once);
    }

    /// <summary>
    /// 测试更新部门方法，当部门不存在时返回NotFound结果
    /// </summary>
    [Fact]
    public async Task UpdateDept_ShouldReturnNotFoundResult_WhenDeptDoesNotExist()
    {
        // Arrange
        var deptId = Guid.NewGuid();
        var deptUpdateDto = new SysDeptUpdateDto();

        _deptServiceMock.Setup(service => service.GetByIdAsync(deptId)).ReturnsAsync((SysDept?)null);

        // Act
        var result = await _deptController.UpdateDept(deptId, deptUpdateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.Code);
    }

    /// <summary>
    /// 测试删除部门方法，当删除成功时返回成功结果
    /// </summary>
    [Fact]
    public async Task DeleteDept_ShouldReturnSuccessResult_WhenDeleteSucceeds()
    {
        // Arrange
        var deptId = Guid.NewGuid();

        _deptServiceMock.Setup(service => service.DeleteAsync(deptId)).ReturnsAsync(true);

        // Act
        var result = await _deptController.DeleteDept(deptId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.True(result.Data);
    }

    /// <summary>
    /// 测试删除部门方法，当删除失败时返回Failed结果
    /// </summary>
    [Fact]
    public async Task DeleteDept_ShouldReturnFailedResult_WhenDeleteFails()
    {
        // Arrange
        var deptId = Guid.NewGuid();

        _deptServiceMock.Setup(service => service.DeleteAsync(deptId)).ReturnsAsync(false);

        // Act
        var result = await _deptController.DeleteDept(deptId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(500, result.Code);
        Assert.False(result.Data);
    }
}
