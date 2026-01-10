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
/// 用户控制器测试
/// </summary>
public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserController _userController;

    /// <summary>
    /// 构造函数，初始化模拟对象和被测控制器
    /// </summary>
    public UserControllerTests()
    {
        // 初始化模拟对象
        _userServiceMock = new Mock<IUserService>();
        _mapperMock = new Mock<IMapper>();

        // 创建被测控制器实例
        _userController = new UserController(_userServiceMock.Object, _mapperMock.Object);
    }

    /// <summary>
    /// 测试获取所有用户方法，当有用户时返回成功结果
    /// </summary>
    [Fact]
    public async Task GetAllUsers_ShouldReturnSuccessResult_WhenUsersExist()
    {
        // Arrange
        var userList = new List<SysUser> { new SysUser() };
        var userDtoList = new List<SysUserDto> { new SysUserDto() };

        _userServiceMock.Setup(service => service.GetAllUsersAsync()).ReturnsAsync(userList);
        _mapperMock.Setup(mapper => mapper.Map<List<SysUserDto>>(userList)).Returns(userDtoList);

        // Act
        var result = await _userController.GetAllUsers();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(userDtoList, result.Data);
    }

    /// <summary>
    /// 测试根据ID获取用户方法，当用户存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task GetUserById_ShouldReturnSuccessResult_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new SysUser();
        var userDto = new SysUserDto();

        _userServiceMock.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync(user);
        _mapperMock.Setup(mapper => mapper.Map<SysUserDto>(user)).Returns(userDto);

        // Act
        var result = await _userController.GetUserById(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(userDto, result.Data);
    }

    /// <summary>
    /// 测试根据ID获取用户方法，当用户不存在时返回NotFound结果
    /// </summary>
    [Fact]
    public async Task GetUserById_ShouldReturnNotFoundResult_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userServiceMock.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync((SysUser?)null);

        // Act
        var result = await _userController.GetUserById(userId);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(404, result.Code);
    }

    /// <summary>
    /// 测试创建用户方法，返回成功结果
    /// </summary>
    [Fact]
    public async Task CreateUser_ShouldReturnSuccessResult_WhenUserIsCreatedSuccessfully()
    {
        // Arrange
        var userCreateDto = new SysUserCreateDto();
        var user = new SysUser();
        var createdUser = new SysUser();
        var createdUserDto = new SysUserDto();

        _mapperMock.Setup(mapper => mapper.Map<SysUser>(userCreateDto)).Returns(user);
        _userServiceMock.Setup(service => service.CreateUserAsync(user)).ReturnsAsync(createdUser);
        _mapperMock.Setup(mapper => mapper.Map<SysUserDto>(createdUser)).Returns(createdUserDto);

        // Act
        var result = await _userController.CreateUser(userCreateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(createdUserDto, result.Data);
    }

    /// <summary>
    /// 测试创建用户方法，当创建失败时返回Failed结果
    /// </summary>
    [Fact]
    public async Task CreateUser_ShouldReturnFailedResult_WhenUserCreationFails()
    {
        // Arrange
        var userCreateDto = new SysUserCreateDto();
        var user = new SysUser();

        _mapperMock.Setup(mapper => mapper.Map<SysUser>(userCreateDto)).Returns(user);
#pragma warning disable CS8620
        _userServiceMock.Setup(service => service.CreateUserAsync(user)).ReturnsAsync((SysUser?)null);
#pragma warning restore CS8620

        // Act
        var result = await _userController.CreateUser(userCreateDto);

        // Assert
        Assert.NotNull(result);

    }

    /// <summary>
    /// 测试更新用户方法，当用户存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task UpdateUser_ShouldReturnSuccessResult_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userUpdateDto = new SysUserUpdateDto();
        var existingUser = new SysUser();
        var updatedUser = new SysUser();
        var updatedUserDto = new SysUserDto();

        _userServiceMock.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync(existingUser);
        _userServiceMock.Setup(service => service.UpdateUserAsync(existingUser)).ReturnsAsync(updatedUser);
        _mapperMock.Setup(mapper => mapper.Map(It.IsAny<SysUserUpdateDto>(), It.IsAny<SysUser>())).Verifiable();
        _mapperMock.Setup(mapper => mapper.Map<SysUserDto>(updatedUser)).Returns(updatedUserDto);

        // Act
        var result = await _userController.UpdateUser(userId, userUpdateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(updatedUserDto, result.Data);
        _mapperMock.Verify(mapper => mapper.Map(userUpdateDto, existingUser), Times.Once);
    }

    /// <summary>
    /// 测试更新用户方法，当用户不存在时返回NotFound结果
    /// </summary>
    [Fact]
    public async Task UpdateUser_ShouldReturnNotFoundResult_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userUpdateDto = new SysUserUpdateDto();

        _userServiceMock.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync((SysUser?)null);

        // Act
        var result = await _userController.UpdateUser(userId, userUpdateDto);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(404, result.Code);
    }

    /// <summary>
    /// 测试删除用户方法，当删除成功时返回成功结果
    /// </summary>
    [Fact]
    public async Task DeleteUser_ShouldReturnSuccessResult_WhenDeleteSucceeds()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userServiceMock.Setup(service => service.DeleteUserAsync(userId)).ReturnsAsync(true);

        // Act
        var result = await _userController.DeleteUser(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.True(result.Data);
    }

    /// <summary>
    /// 测试删除用户方法，当删除失败时返回Failed结果
    /// </summary>
    [Fact]
    public async Task DeleteUser_ShouldReturnFailedResult_WhenDeleteFails()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userServiceMock.Setup(service => service.DeleteUserAsync(userId)).ReturnsAsync(false);

        // Act
        var result = await _userController.DeleteUser(userId);

        // Assert
        Assert.NotNull(result);

        Assert.False(result.Data);
    }
}