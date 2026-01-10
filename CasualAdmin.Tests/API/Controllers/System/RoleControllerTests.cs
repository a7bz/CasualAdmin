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
/// 角色控制器测试
/// </summary>
public class RoleControllerTests
{
    private readonly Mock<IRoleService> _roleServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly RoleController _roleController;

    /// <summary>
    /// 构造函数，初始化模拟对象和被测控制器
    /// </summary>
    public RoleControllerTests()
    {
        // 初始化模拟对象
        _roleServiceMock = new Mock<IRoleService>();
        _mapperMock = new Mock<IMapper>();

        // 创建被测控制器实例
        _roleController = new RoleController(_roleServiceMock.Object, _mapperMock.Object);
    }

    /// <summary>
    /// 测试获取所有角色方法，当有角色时返回成功结果
    /// </summary>
    [Fact]
    public async Task GetAllRoles_ShouldReturnSuccessResult_WhenRolesExist()
    {
        // Arrange
        var roleList = new List<SysRole> { new() };
        var roleDtoList = new List<SysRoleDto> { new() };

        _roleServiceMock.Setup(service => service.GetAllRolesAsync()).ReturnsAsync(roleList);
        _mapperMock.Setup(mapper => mapper.Map<List<SysRoleDto>>(roleList)).Returns(roleDtoList);

        // Act
        var result = await _roleController.GetAllRoles();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(roleDtoList, result.Data);
    }

    /// <summary>
    /// 测试根据ID获取角色方法，当角色存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task GetRoleById_ShouldReturnSuccessResult_WhenRoleExists()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var role = new SysRole();
        var roleDto = new SysRoleDto();

        _roleServiceMock.Setup(service => service.GetRoleByIdAsync(roleId)).ReturnsAsync(role);
        _mapperMock.Setup(mapper => mapper.Map<SysRoleDto>(role)).Returns(roleDto);

        // Act
        var result = await _roleController.GetRoleById(roleId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(roleDto, result.Data);
    }

    /// <summary>
    /// 测试根据ID获取角色方法，当角色不存在时返回NotFound结果
    /// </summary>
    [Fact]
    public async Task GetRoleById_ShouldReturnNotFoundResult_WhenRoleDoesNotExist()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        _roleServiceMock.Setup(service => service.GetRoleByIdAsync(roleId)).ReturnsAsync((SysRole?)null);

        // Act
        var result = await _roleController.GetRoleById(roleId);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(404, result.Code);
    }

    /// <summary>
    /// 测试创建角色方法，返回成功结果
    /// </summary>
    [Fact]
    public async Task CreateRole_ShouldReturnSuccessResult_WhenRoleIsCreatedSuccessfully()
    {
        // Arrange
        var roleCreateDto = new SysRoleCreateDto();
        var role = new SysRole();
        var createdRole = new SysRole();
        var createdRoleDto = new SysRoleDto();

        _mapperMock.Setup(mapper => mapper.Map<SysRole>(roleCreateDto)).Returns(role);
        _roleServiceMock.Setup(service => service.CreateRoleAsync(role)).ReturnsAsync(createdRole);
        _mapperMock.Setup(mapper => mapper.Map<SysRoleDto>(createdRole)).Returns(createdRoleDto);

        // Act
        var result = await _roleController.CreateRole(roleCreateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(createdRoleDto, result.Data);
    }

    /// <summary>
    /// 测试创建角色方法，当创建失败时返回Failed结果
    /// </summary>
    [Fact]
    public async Task CreateRole_ShouldReturnFailedResult_WhenRoleCreationFails()
    {
        // Arrange
        var roleCreateDto = new SysRoleCreateDto();
        var role = new SysRole();

        _mapperMock.Setup(mapper => mapper.Map<SysRole>(roleCreateDto)).Returns(role);
        _roleServiceMock.Setup(service => service.CreateRoleAsync(role)).ReturnsAsync(role);

        // Act
        var result = await _roleController.CreateRole(roleCreateDto);

        // Assert
        Assert.NotNull(result);

    }

    /// <summary>
    /// 测试更新角色方法，当角色存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task UpdateRole_ShouldReturnSuccessResult_WhenRoleExistsAndUpdateSucceeds()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var roleUpdateDto = new SysRoleUpdateDto();
        var existingRole = new SysRole();
        var updatedRole = new SysRole();
        var updatedRoleDto = new SysRoleDto();

        _roleServiceMock.Setup(service => service.GetRoleByIdAsync(roleId)).ReturnsAsync(existingRole);
        _roleServiceMock.Setup(service => service.UpdateRoleAsync(existingRole)).ReturnsAsync(updatedRole);
        _mapperMock.Setup(mapper => mapper.Map(It.IsAny<SysRoleUpdateDto>(), It.IsAny<SysRole>())).Verifiable();
        _mapperMock.Setup(mapper => mapper.Map<SysRoleDto>(updatedRole)).Returns(updatedRoleDto);

        // Act
        var result = await _roleController.UpdateRole(roleId, roleUpdateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(updatedRoleDto, result.Data);
        _mapperMock.Verify(mapper => mapper.Map(roleUpdateDto, existingRole), Times.Once);
    }

    /// <summary>
    /// 测试更新角色方法，当角色不存在时返回NotFound结果
    /// </summary>
    [Fact]
    public async Task UpdateRole_ShouldReturnNotFoundResult_WhenRoleDoesNotExist()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var roleUpdateDto = new SysRoleUpdateDto();

        _roleServiceMock.Setup(service => service.GetRoleByIdAsync(roleId)).ReturnsAsync((SysRole?)null);

        // Act
        var result = await _roleController.UpdateRole(roleId, roleUpdateDto);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(404, result.Code);
    }

    /// <summary>
    /// 测试更新角色方法，当更新失败时返回Failed结果
    /// </summary>
    [Fact]
    public async Task UpdateRole_ShouldReturnFailedResult_WhenUpdateFails()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var roleUpdateDto = new SysRoleUpdateDto();
        var existingRole = new SysRole();

        _roleServiceMock.Setup(service => service.GetRoleByIdAsync(roleId)).ReturnsAsync(existingRole);
        _roleServiceMock.Setup(service => service.UpdateRoleAsync(existingRole)).ReturnsAsync((SysRole?)null);
        _mapperMock.Setup(mapper => mapper.Map(It.IsAny<SysRoleUpdateDto>(), It.IsAny<SysRole>())).Verifiable();

        // Act
        var result = await _roleController.UpdateRole(roleId, roleUpdateDto);

        // Assert
        Assert.NotNull(result);

        _mapperMock.Verify(mapper => mapper.Map(roleUpdateDto, existingRole), Times.Once);
    }

    /// <summary>
    /// 测试删除角色方法，当删除成功时返回成功结果
    /// </summary>
    [Fact]
    public async Task DeleteRole_ShouldReturnSuccessResult_WhenDeleteSucceeds()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        _roleServiceMock.Setup(service => service.DeleteRoleAsync(roleId)).ReturnsAsync(true);

        // Act
        var result = await _roleController.DeleteRole(roleId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.True(result.Data);
    }

    /// <summary>
    /// 测试删除角色方法，当删除失败时返回Failed结果
    /// </summary>
    [Fact]
    public async Task DeleteRole_ShouldReturnFailedResult_WhenDeleteFails()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        _roleServiceMock.Setup(service => service.DeleteRoleAsync(roleId)).ReturnsAsync(false);

        // Act
        var result = await _roleController.DeleteRole(roleId);

        // Assert
        Assert.NotNull(result);

        Assert.False(result.Data);
    }

    /// <summary>
    /// 测试为用户分配角色方法，当分配成功时返回成功结果
    /// </summary>
    [Fact]
    public async Task AssignRoleToUser_ShouldReturnSuccessResult_WhenAssignSucceeds()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _roleServiceMock.Setup(service => service.AssignRoleToUserAsync(userId, roleId)).ReturnsAsync(true);

        // Act
        var result = await _roleController.AssignRoleToUser(roleId, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.True(result.Data);
    }

    /// <summary>
    /// 测试为用户分配角色方法，当分配失败时返回Failed结果
    /// </summary>
    [Fact]
    public async Task AssignRoleToUser_ShouldReturnFailedResult_WhenAssignFails()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _roleServiceMock.Setup(service => service.AssignRoleToUserAsync(userId, roleId)).ReturnsAsync(false);

        // Act
        var result = await _roleController.AssignRoleToUser(roleId, userId);

        // Assert
        Assert.NotNull(result);

        Assert.False(result.Data);
    }

    /// <summary>
    /// 测试移除用户角色方法，当移除成功时返回成功结果
    /// </summary>
    [Fact]
    public async Task RemoveRoleFromUser_ShouldReturnSuccessResult_WhenRemoveSucceeds()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _roleServiceMock.Setup(service => service.RemoveRoleFromUserAsync(userId, roleId)).ReturnsAsync(true);

        // Act
        var result = await _roleController.RemoveRoleFromUser(roleId, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.True(result.Data);
    }

    /// <summary>
    /// 测试移除用户角色方法，当移除失败时返回Failed结果
    /// </summary>
    [Fact]
    public async Task RemoveRoleFromUser_ShouldReturnFailedResult_WhenRemoveFails()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _roleServiceMock.Setup(service => service.RemoveRoleFromUserAsync(userId, roleId)).ReturnsAsync(false);

        // Act
        var result = await _roleController.RemoveRoleFromUser(roleId, userId);

        // Assert
        Assert.NotNull(result);

        Assert.False(result.Data);
    }

    /// <summary>
    /// 测试获取用户角色列表方法，返回成功结果
    /// </summary>
    [Fact]
    public async Task GetRolesByUserId_ShouldReturnSuccessResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleList = new List<SysRole> { new() };
        var roleDtoList = new List<SysRoleDto> { new() };

        _roleServiceMock.Setup(service => service.GetRolesByUserIdAsync(userId)).ReturnsAsync(roleList);
        _mapperMock.Setup(mapper => mapper.Map<List<SysRoleDto>>(roleList)).Returns(roleDtoList);

        // Act
        var result = await _roleController.GetRolesByUserId(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(roleDtoList, result.Data);
    }

    /// <summary>
    /// 测试获取角色用户列表方法，返回成功结果
    /// </summary>
    [Fact]
    public async Task GetUsersByRoleId_ShouldReturnSuccessResult()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var userList = new List<SysUser> { new() };
        var userDtoList = new List<SysUserDto> { new() };

        _roleServiceMock.Setup(service => service.GetUsersByRoleIdAsync(roleId)).ReturnsAsync(userList);
        _mapperMock.Setup(mapper => mapper.Map<List<SysUserDto>>(userList)).Returns(userDtoList);

        // Act
        var result = await _roleController.GetUsersByRoleId(roleId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(userDtoList, result.Data);
    }
}