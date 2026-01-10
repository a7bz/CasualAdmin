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
/// 权限控制器测试
/// </summary>
public class PermissionControllerTests
{
    private readonly Mock<IPermissionService> _permissionServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PermissionController _permissionController;

    /// <summary>
    /// 构造函数，初始化模拟对象和被测控制器
    /// </summary>
    public PermissionControllerTests()
    {
        // 初始化模拟对象
        _permissionServiceMock = new Mock<IPermissionService>();
        _mapperMock = new Mock<IMapper>();

        // 创建被测控制器实例
        _permissionController = new PermissionController(_permissionServiceMock.Object, _mapperMock.Object);
    }

    /// <summary>
    /// 测试获取所有权限方法，当有权限时返回成功结果
    /// </summary>
    [Fact]
    public async Task GetAllPermissions_ShouldReturnSuccessResult_WhenPermissionsExist()
    {
        // Arrange
        var permissionList = new List<SysPermission> { new() };
        var permissionDtoList = new List<SysPermissionDto> { new() };

        _permissionServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(permissionList);
        _mapperMock.Setup(mapper => mapper.Map<List<SysPermissionDto>>(permissionList)).Returns(permissionDtoList);

        // Act
        var result = await _permissionController.GetAllPermissions();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(permissionDtoList, result.Data);
    }

    /// <summary>
    /// 测试根据ID获取权限方法，当权限存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task GetPermissionById_ShouldReturnSuccessResult_WhenPermissionExists()
    {
        // Arrange
        var permissionId = Guid.NewGuid();
        var permission = new SysPermission();
        var permissionDto = new SysPermissionDto();

        _permissionServiceMock.Setup(service => service.GetByIdAsync(permissionId)).ReturnsAsync(permission);
        _mapperMock.Setup(mapper => mapper.Map<SysPermissionDto>(permission)).Returns(permissionDto);

        // Act
        var result = await _permissionController.GetPermissionById(permissionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(permissionDto, result.Data);
    }

    /// <summary>
    /// 测试根据ID获取权限方法，当权限不存在时返回NotFound结果
    /// </summary>
    [Fact]
    public async Task GetPermissionById_ShouldReturnNotFoundResult_WhenPermissionDoesNotExist()
    {
        // Arrange
        var permissionId = Guid.NewGuid();

        _permissionServiceMock.Setup(service => service.GetByIdAsync(permissionId)).ReturnsAsync((SysPermission?)null);

        // Act
        var result = await _permissionController.GetPermissionById(permissionId);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(404, result.Code);
    }

    /// <summary>
    /// 测试根据角色ID获取权限列表方法，返回成功结果
    /// </summary>
    [Fact]
    public async Task GetPermissionsByRoleId_ShouldReturnSuccessResult()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var permissionList = new List<SysPermission> { new() };
        var permissionDtoList = new List<SysPermissionDto> { new() };

        _permissionServiceMock.Setup(service => service.GetPermissionsByRoleIdAsync(roleId)).ReturnsAsync(permissionList);
        _mapperMock.Setup(mapper => mapper.Map<List<SysPermissionDto>>(permissionList)).Returns(permissionDtoList);

        // Act
        var result = await _permissionController.GetPermissionsByRoleId(roleId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(permissionDtoList, result.Data);
    }

    /// <summary>
    /// 测试根据菜单ID获取权限列表方法，返回成功结果
    /// </summary>
    [Fact]
    public async Task GetPermissionsByMenuId_ShouldReturnSuccessResult()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        var permissionList = new List<SysPermission> { new() };
        var permissionDtoList = new List<SysPermissionDto> { new() };

        _permissionServiceMock.Setup(service => service.GetPermissionsByMenuIdAsync(menuId)).ReturnsAsync(permissionList);
        _mapperMock.Setup(mapper => mapper.Map<List<SysPermissionDto>>(permissionList)).Returns(permissionDtoList);

        // Act
        var result = await _permissionController.GetPermissionsByMenuId(menuId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(permissionDtoList, result.Data);
    }

    /// <summary>
    /// 测试创建权限方法，返回成功结果
    /// </summary>
    [Fact]
    public async Task CreatePermission_ShouldReturnSuccessResult()
    {
        // Arrange
        var permissionCreateDto = new SysPermissionCreateDto();
        var permission = new SysPermission();
        var createdPermission = new SysPermission();
        var createdPermissionDto = new SysPermissionDto();

        _mapperMock.Setup(mapper => mapper.Map<SysPermission>(permissionCreateDto)).Returns(permission);
        _permissionServiceMock.Setup(service => service.CreateAsync(permission)).ReturnsAsync(createdPermission);
        _mapperMock.Setup(mapper => mapper.Map<SysPermissionDto>(createdPermission)).Returns(createdPermissionDto);

        // Act
        var result = await _permissionController.CreatePermission(permissionCreateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(createdPermissionDto, result.Data);
    }

    /// <summary>
    /// 测试更新权限方法，当权限存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task UpdatePermission_ShouldReturnSuccessResult_WhenPermissionExists()
    {
        // Arrange
        var permissionId = Guid.NewGuid();
        var permissionUpdateDto = new SysPermissionUpdateDto();
        var existingPermission = new SysPermission();
        var updatedPermission = new SysPermission();
        var updatedPermissionDto = new SysPermissionDto();

        _permissionServiceMock.Setup(service => service.GetByIdAsync(permissionId)).ReturnsAsync(existingPermission);
        _permissionServiceMock.Setup(service => service.UpdateAsync(existingPermission)).ReturnsAsync(updatedPermission);
        _mapperMock.Setup(mapper => mapper.Map(It.IsAny<SysPermissionUpdateDto>(), It.IsAny<SysPermission>())).Verifiable();
        _mapperMock.Setup(mapper => mapper.Map<SysPermissionDto>(updatedPermission)).Returns(updatedPermissionDto);

        // Act
        var result = await _permissionController.UpdatePermission(permissionId, permissionUpdateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(updatedPermissionDto, result.Data);
        _mapperMock.Verify(mapper => mapper.Map(permissionUpdateDto, existingPermission), Times.Once);
    }

    /// <summary>
    /// 测试更新权限方法，当权限不存在时返回NotFound结果
    /// </summary>
    [Fact]
    public async Task UpdatePermission_ShouldReturnNotFoundResult_WhenPermissionDoesNotExist()
    {
        // Arrange
        var permissionId = Guid.NewGuid();
        var permissionUpdateDto = new SysPermissionUpdateDto();

        _permissionServiceMock.Setup(service => service.GetByIdAsync(permissionId)).ReturnsAsync((SysPermission?)null);

        // Act
        var result = await _permissionController.UpdatePermission(permissionId, permissionUpdateDto);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(404, result.Code);
    }

    /// <summary>
    /// 测试更新权限方法，当更新失败时返回Failed结果
    /// </summary>
    [Fact]
    public async Task UpdatePermission_ShouldReturnFailedResult_WhenUpdateFails()
    {
        // Arrange
        var permissionId = Guid.NewGuid();
        var permissionUpdateDto = new SysPermissionUpdateDto();
        var existingPermission = new SysPermission();

        _permissionServiceMock.Setup(service => service.GetByIdAsync(permissionId)).ReturnsAsync(existingPermission);
        _permissionServiceMock.Setup(service => service.UpdateAsync(existingPermission)).ReturnsAsync((SysPermission?)null);
        _mapperMock.Setup(mapper => mapper.Map(It.IsAny<SysPermissionUpdateDto>(), It.IsAny<SysPermission>())).Verifiable();

        // Act
        var result = await _permissionController.UpdatePermission(permissionId, permissionUpdateDto);

        // Assert
        Assert.NotNull(result);

        _mapperMock.Verify(mapper => mapper.Map(permissionUpdateDto, existingPermission), Times.Once);
    }

    /// <summary>
    /// 测试删除权限方法，当删除成功时返回成功结果
    /// </summary>
    [Fact]
    public async Task DeletePermission_ShouldReturnSuccessResult_WhenDeleteSucceeds()
    {
        // Arrange
        var permissionId = Guid.NewGuid();

        _permissionServiceMock.Setup(service => service.DeleteAsync(permissionId)).ReturnsAsync(true);

        // Act
        var result = await _permissionController.DeletePermission(permissionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.True(result.Data);
    }

    /// <summary>
    /// 测试删除权限方法，当删除失败时返回Failed结果
    /// </summary>
    [Fact]
    public async Task DeletePermission_ShouldReturnFailedResult_WhenDeleteFails()
    {
        // Arrange
        var permissionId = Guid.NewGuid();

        _permissionServiceMock.Setup(service => service.DeleteAsync(permissionId)).ReturnsAsync(false);

        // Act
        var result = await _permissionController.DeletePermission(permissionId);

        // Assert
        Assert.NotNull(result);

        Assert.False(result.Data);
    }
}