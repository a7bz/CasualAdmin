namespace CasualAdmin.Tests.API.Controllers;

using AutoMapper;
using CasualAdmin.API.Controllers;
using CasualAdmin.Application.Commands.Auth;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Models.DTOs.Responses.System;
using CasualAdmin.Domain.Entities.System;
using Moq;
using Xunit;

/// <summary>
/// 认证控制器测试
/// </summary>
public class AuthControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IRsaEncryptionService> _rsaEncryptionServiceMock;
    private readonly Mock<IRoleService> _roleServiceMock;
    private readonly Mock<IPermissionService> _permissionServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AuthController _authController;

    /// <summary>
    /// 构造函数，初始化模拟对象和被测控制器
    /// </summary>
    public AuthControllerTests()
    {
        // 初始化模拟对象
        _userServiceMock = new Mock<IUserService>();
        _authServiceMock = new Mock<IAuthService>();
        _rsaEncryptionServiceMock = new Mock<IRsaEncryptionService>();
        _roleServiceMock = new Mock<IRoleService>();
        _permissionServiceMock = new Mock<IPermissionService>();
        _mapperMock = new Mock<IMapper>();

        // 设置RSA解密服务的模拟行为
        _rsaEncryptionServiceMock.Setup(service => service.Decrypt(It.IsAny<string>())).Returns("Test@123");
        _rsaEncryptionServiceMock.Setup(service => service.GetPublicKey()).Returns("test_public_key");

        // 设置Mapper的模拟行为，将SysUser转换为SysUserDto
        _mapperMock.Setup(mapper => mapper.Map<SysUserDto>(It.IsAny<SysUser>())).Returns(new SysUserDto());

        // 设置RoleService的模拟行为，返回空角色列表
        _roleServiceMock.Setup(service => service.GetRolesByUserIdAsync(It.IsAny<Guid>())).ReturnsAsync([]);

        // 设置PermissionService的模拟行为，返回空权限列表
        _permissionServiceMock.Setup(service => service.GetPermissionsByRoleIdAsync(It.IsAny<Guid>())).ReturnsAsync([]);

        // 创建被测控制器实例
        _authController = new AuthController(_userServiceMock.Object, _authServiceMock.Object, _rsaEncryptionServiceMock.Object, _roleServiceMock.Object, _permissionServiceMock.Object, _mapperMock.Object);
    }

    /// <summary>
    /// 测试获取RSA公钥方法，返回成功结果
    /// </summary>
    [Fact]
    public void GetPublicKey_ShouldReturnSuccessResult()
    {
        // Act
        var result = _authController.GetPublicKey();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
    }

    /// <summary>
    /// 测试注册方法，当邮箱不存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task Register_ShouldReturnSuccessResult_WhenEmailDoesNotExist()
    {
        // Arrange
        var registerCommand = new RegisterCommand
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "encrypted_password"
        };

        _userServiceMock.Setup(service => service.GetUserByEmailAsync(registerCommand.Email)).ReturnsAsync((SysUser?)null);
        _userServiceMock.Setup(service => service.CreateUserAsync(It.IsAny<SysUser>())).ReturnsAsync(new SysUser());

        // Act
        var result = await _authController.Register(registerCommand);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.NotNull(result.Data);
    }

    /// <summary>
    /// 测试注册方法，当邮箱已存在时返回BadRequest结果
    /// </summary>
    [Fact]
    public async Task Register_ShouldReturnBadRequestResult_WhenEmailAlreadyExists()
    {
        // Arrange
        var registerCommand = new RegisterCommand
        {
            UserName = "testuser",
            Email = "existing@example.com",
            Password = "encrypted_password"
        };

        _userServiceMock.Setup(service => service.GetUserByEmailAsync(registerCommand.Email)).ReturnsAsync(new SysUser());

        // Act
        var result = await _authController.Register(registerCommand);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.Code);
    }

    /// <summary>
    /// 测试登录方法，当邮箱和密码正确时返回成功结果
    /// </summary>
    [Fact]
    public async Task Login_ShouldReturnSuccessResult_WhenEmailAndPasswordAreCorrect()
    {
        // Arrange
        var loginCommand = new LoginCommand
        {
            Account = "test@example.com",
            Password = "encrypted_password"
        };

        var existingUser = new SysUser();
        _userServiceMock.Setup(service => service.GetUserByUsernameAsync(loginCommand.Account)).ReturnsAsync(existingUser);
        _userServiceMock.Setup(service => service.VerifyPassword(existingUser, It.IsAny<string>())).Returns(true);
        _authServiceMock.Setup(service => service.GenerateJwtToken(existingUser)).ReturnsAsync("test_token");

        // Act
        var result = await _authController.Login(loginCommand);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.NotNull(result.Data);
    }

    /// <summary>
    /// 测试登录方法，当邮箱不存在时返回BadRequest结果
    /// </summary>
    [Fact]
    public async Task Login_ShouldReturnBadRequestResult_WhenEmailDoesNotExist()
    {
        // Arrange
        var loginCommand = new LoginCommand
        {
            Account = "non_existent@example.com",
            Password = "encrypted_password"
        };

        _userServiceMock.Setup(service => service.GetUserByUsernameAsync(loginCommand.Account)).ReturnsAsync((SysUser?)null);
        _userServiceMock.Setup(service => service.GetUserByEmailAsync(loginCommand.Account)).ReturnsAsync((SysUser?)null);

        // Act
        var result = await _authController.Login(loginCommand);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.Code);
    }

    /// <summary>
    /// 测试登录方法，当密码不正确时返回BadRequest结果
    /// </summary>
    [Fact]
    public async Task Login_ShouldReturnBadRequestResult_WhenPasswordIsIncorrect()
    {
        // Arrange
        var loginCommand = new LoginCommand
        {
            Account = "test@example.com",
            Password = "wrong_encrypted_password"
        };

        var existingUser = new SysUser();
        _userServiceMock.Setup(service => service.GetUserByUsernameAsync(loginCommand.Account)).ReturnsAsync(existingUser);
        _userServiceMock.Setup(service => service.VerifyPassword(existingUser, It.IsAny<string>())).Returns(false);

        // Act
        var result = await _authController.Login(loginCommand);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.Code);
    }
}