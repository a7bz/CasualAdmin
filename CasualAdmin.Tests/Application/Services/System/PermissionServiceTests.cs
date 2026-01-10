namespace CasualAdmin.Tests.Application.Services.System;
using CasualAdmin.Application.Interfaces.Base;
using CasualAdmin.Application.Interfaces.Events;
using CasualAdmin.Application.Interfaces.Services;
using CasualAdmin.Application.Services.System;
using CasualAdmin.Domain.Entities.System;
using Moq;
using Xunit;

/// <summary>
/// 权限服务测试类
/// </summary>
public class PermissionServiceTests
{
    private readonly Mock<IRepository<SysPermission>> _permissionRepositoryMock;
    private readonly Mock<IValidationService> _validationServiceMock;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly PermissionService _permissionService;

    /// <summary>
    /// 测试类构造函数，初始化模拟对象和被测服务
    /// </summary>
    public PermissionServiceTests()
    {
        // 初始化模拟对象
        _permissionRepositoryMock = new Mock<IRepository<SysPermission>>();
        _validationServiceMock = new Mock<IValidationService>();
        _eventBusMock = new Mock<IEventBus>();

        // 配置模拟验证服务
        _validationServiceMock.Setup(v => v.ValidateAndThrow(It.IsAny<SysPermission>())).Verifiable();

        // 创建被测服务实例
        _permissionService = new PermissionService(_permissionRepositoryMock.Object, _validationServiceMock.Object, _eventBusMock.Object);
    }

    /// <summary>
    /// 测试根据ID获取权限方法，当权限存在时返回正确的权限
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnPermission_WhenPermissionExists()
    {
        // Arrange
        var permissionId = Guid.NewGuid();
        var expectedPermission = new SysPermission
        {
            PermissionId = permissionId,
            PermissionName = "Test Permission",
            PermissionCode = "test.permission",
            Remark = "Test permission description"
        };

        _permissionRepositoryMock.Setup(r => r.GetByIdAsync(permissionId)).ReturnsAsync(expectedPermission);

        // Act
        var actualPermission = await _permissionService.GetByIdAsync(permissionId);

        // Assert
        Assert.NotNull(actualPermission);
        Assert.Equal(expectedPermission.PermissionId, actualPermission.PermissionId);
        Assert.Equal("Test Permission", actualPermission.PermissionName);
        Assert.Equal("test.permission", actualPermission.PermissionCode);
        Assert.Equal("Test permission description", actualPermission.Remark);
        _permissionRepositoryMock.Verify(r => r.GetByIdAsync(permissionId), Times.Once);
    }

    /// <summary>
    /// 测试获取所有权限方法，返回所有权限列表
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPermissions()
    {
        // Arrange
        var permission1 = new SysPermission
        {
            PermissionName = "Permission 1",
            PermissionCode = "perm.1",
            Remark = "Permission 1 description"
        };

        var permission2 = new SysPermission
        {
            PermissionName = "Permission 2",
            PermissionCode = "perm.2",
            Remark = "Permission 2 description"
        };

        var permissions = new List<SysPermission> { permission1, permission2 };

        _permissionRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(permissions);

        // Act
        var result = await _permissionService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(permissions.Count, result.Count);
        Assert.Equal("Permission 1", result[0].PermissionName);
        Assert.Equal("Permission 2", result[1].PermissionName);
        _permissionRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    /// <summary>
    /// 测试创建权限方法，确保能够创建权限
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldCreatePermission()
    {
        // Arrange
        var permission = new SysPermission();
        permission.PermissionName = "New Permission";
        permission.PermissionCode = "new.permission";
        permission.Remark = "New permission description";

        var createdPermission = new SysPermission();
        createdPermission.PermissionId = Guid.NewGuid();
        createdPermission.PermissionName = "New Permission";
        createdPermission.PermissionCode = "new.permission";
        createdPermission.Remark = "New permission description";

        _permissionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<SysPermission>())).ReturnsAsync(createdPermission);

        // Act
        var result = await _permissionService.CreateAsync(permission);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdPermission.PermissionId, result.PermissionId);
        Assert.Equal(createdPermission.PermissionName, result.PermissionName);
        Assert.Equal(createdPermission.PermissionCode, result.PermissionCode);
        Assert.NotEqual(default, result.CreatedAt);
        Assert.NotEqual(default, result.UpdatedAt);
        _permissionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SysPermission>()), Times.Once);
    }

    /// <summary>
    /// 测试更新权限方法，当权限存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ShouldUpdatePermission_WhenPermissionExists()
    {
        // Arrange
        var existingPermission = new SysPermission();
        existingPermission.PermissionId = Guid.NewGuid();
        existingPermission.PermissionName = "Old Permission";
        existingPermission.PermissionCode = "old.permission";
        existingPermission.Remark = "Old permission description";

        var updatedPermission = new SysPermission();
        updatedPermission.PermissionId = existingPermission.PermissionId;
        updatedPermission.PermissionName = "Updated Permission";
        updatedPermission.PermissionCode = "updated.permission";
        updatedPermission.Remark = "Updated permission description";

        var savedPermission = new SysPermission();
        savedPermission.PermissionId = existingPermission.PermissionId;
        savedPermission.PermissionName = "Updated Permission";
        savedPermission.PermissionCode = "updated.permission";
        savedPermission.Remark = "Updated permission description";

        _permissionRepositoryMock.Setup(r => r.GetByIdAsync(existingPermission.PermissionId)).ReturnsAsync(existingPermission);
        _permissionRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<SysPermission>())).ReturnsAsync(savedPermission);

        // Act
        var result = await _permissionService.UpdateAsync(updatedPermission);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(savedPermission.PermissionId, result.PermissionId);
        Assert.Equal("Updated Permission", result.PermissionName);
        Assert.Equal("updated.permission", result.PermissionCode);
        _permissionRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SysPermission>()), Times.Once);
    }

    /// <summary>
    /// 测试删除权限方法，当权限存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenPermissionExists()
    {
        // Arrange
        var permissionId = Guid.NewGuid();
        _permissionRepositoryMock.Setup(r => r.DeleteAsync(permissionId)).ReturnsAsync(true);

        // Act
        var result = await _permissionService.DeleteAsync(permissionId);

        // Assert
        Assert.True(result);
        _permissionRepositoryMock.Verify(r => r.DeleteAsync(permissionId), Times.Once);
    }

    /// <summary>
    /// 测试删除权限方法，当权限不存在时返回失败结果
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenPermissionDoesNotExist()
    {
        // Arrange
        var permissionId = Guid.NewGuid();
        _permissionRepositoryMock.Setup(r => r.DeleteAsync(permissionId)).ReturnsAsync(false);

        // Act
        var result = await _permissionService.DeleteAsync(permissionId);

        // Assert
        Assert.False(result);
        _permissionRepositoryMock.Verify(r => r.DeleteAsync(permissionId), Times.Once);
    }
}
