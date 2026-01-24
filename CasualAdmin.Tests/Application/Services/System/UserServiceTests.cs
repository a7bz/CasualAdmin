namespace CasualAdmin.Tests.Application.Services.System;
using CasualAdmin.Application.Interfaces.Base;
using CasualAdmin.Application.Interfaces.Events;
using CasualAdmin.Application.Interfaces.Services;
using CasualAdmin.Application.Services.System;
using CasualAdmin.Domain.Entities.System;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

/// <summary>
/// 用户服务测试类
/// </summary>
public class UserServiceTests
{
    private readonly Mock<IRepository<SysUser>> _userRepositoryMock;
    private readonly Mock<IValidationService> _validationServiceMock;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly UserService _userService;

    /// <summary>
    /// 测试类构造函数，初始化模拟对象和被测服务
    /// </summary>
    public UserServiceTests()
    {
        // 初始化模拟对象
        _userRepositoryMock = new Mock<IRepository<SysUser>>();
        _validationServiceMock = new Mock<IValidationService>();
        _eventBusMock = new Mock<IEventBus>();

        // 配置模拟验证服务
        _validationServiceMock.Setup(v => v.ValidateAndThrow(It.IsAny<SysUser>())).Verifiable();

        // 创建被测服务实例
        _userService = new UserService(_userRepositoryMock.Object, _validationServiceMock.Object, _eventBusMock.Object);
    }

    /// <summary>
    /// 测试根据ID获取用户方法，当用户存在时返回正确的用户
    /// </summary>
    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedUser = new SysUser();
        expectedUser.SetUsername("testuser");
        expectedUser.SetEmail("test@example.com");

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(expectedUser);

        // Act
        var actualUser = await _userService.GetUserByIdAsync(userId);

        // Assert
        Assert.NotNull(actualUser);
        Assert.Equal(expectedUser.UserId, actualUser.UserId);
        Assert.Equal("testuser", actualUser.Username);
        Assert.Equal("test@example.com", actualUser.Email);
        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
    }

    /// <summary>
    /// 测试获取所有用户方法，返回所有用户列表
    /// </summary>
    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var user1 = new SysUser();
        user1.SetUsername("user1");
        user1.SetEmail("user1@example.com");

        var user2 = new SysUser();
        user2.SetUsername("user2");
        user2.SetEmail("user2@example.com");

        var users = new List<SysUser> { user1, user2 };

        _userRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(users.Count, result.Count);
        for (int i = 0; i < users.Count; i++)
        {
            Assert.Equal(users[i].UserId, result[i].UserId);
            Assert.Equal(users[i].Username, result[i].Username);
            Assert.Equal(users[i].Email, result[i].Email);
        }
        _userRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    /// <summary>
    /// 测试根据邮箱获取用户方法，当用户存在时返回正确的用户
    /// </summary>
    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var email = "test@example.com";
        var user = new SysUser();
        user.SetUsername("testuser");
        user.SetEmail(email);

        _userRepositoryMock.Setup(r => r.FirstOrDefaultAsync(u => u.Email == email)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByEmailAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.UserId, result.UserId);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal(user.Email, result.Email);
        _userRepositoryMock.Verify(r => r.FirstOrDefaultAsync(u => u.Email == email), Times.Once);
    }

    /// <summary>
    /// 测试根据邮箱获取用户方法，当用户不存在时返回null
    /// </summary>
    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var email = "nonexistent@example.com";

        _userRepositoryMock.Setup(r => r.FirstOrDefaultAsync(u => u.Email == email)).ReturnsAsync((SysUser?)null);

        // Act
        var result = await _userService.GetUserByEmailAsync(email);

        // Assert
        Assert.Null(result);
        _userRepositoryMock.Verify(r => r.FirstOrDefaultAsync(u => u.Email == email), Times.Once);
    }

    /// <summary>
    /// 测试创建用户方法，确保能够创建用户并正确加密密码
    /// </summary>
    [Fact]
    public async Task CreateUserAsync_ShouldCreateUser_WithHashedPassword()
    {
        // Arrange
        var user = new SysUser();
        user.SetUsername("testuser");
        user.SetEmail("test@example.com");
        user.SetPassword("Password123!", "test-salt"); // 暂时设置明文密码，会在CreateUserAsync中被哈希

        var createdUser = new SysUser();
        createdUser.SetUsername("testuser");
        createdUser.SetEmail("test@example.com");
        createdUser.SetPassword("hashedPassword", "test-salt");

        _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<SysUser>())).ReturnsAsync(createdUser);

        // Act
        var result = await _userService.CreateUserAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdUser.UserId, result.UserId);
        Assert.Equal(createdUser.Username, result.Username);
        Assert.Equal(createdUser.Email, result.Email);
        Assert.NotEqual("Password123!", result.Password); // 密码应该已经被哈希
        Assert.NotNull(result.Password);
        Assert.NotEmpty(result.Password);
        Assert.NotEqual(default, result.CreatedAt);
        Assert.NotEqual(default, result.UpdatedAt);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SysUser>()), Times.Once);
    }

    /// <summary>
    /// 测试更新用户方法，确保能够更新用户信息
    /// </summary>
    [Fact]
    public async Task UpdateUserAsync_ShouldUpdateUser_WhenUserExists()
    {
        // Arrange
        var existingUser = new SysUser();
        existingUser.SetUsername("olduser");
        existingUser.SetEmail("old@example.com");
        existingUser.SetPassword("oldHashedPassword", "test-salt");

        // 使用与existingUser相同的UserId，这样GetByIdAsync才能找到existingUser
        var updatedUser = existingUser;
        updatedUser.SetUsername("newuser");
        updatedUser.SetEmail("new@example.com");
        updatedUser.SetPassword("NewPassword123!", "test-salt");

        var savedUser = new SysUser();
        savedUser.SetUsername("newuser");
        savedUser.SetEmail("new@example.com");
        savedUser.SetPassword("newHashedPassword", "test-salt");

        // 使用existingUser的UserId设置mock，这样GetByIdAsync才能找到existingUser
        _userRepositoryMock.Setup(r => r.GetByIdAsync(existingUser.UserId)).ReturnsAsync(existingUser);
        _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<SysUser>())).ReturnsAsync(savedUser);

        // Act
        var result = await _userService.UpdateUserAsync(updatedUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(savedUser.Username, result.Username);
        Assert.Equal(savedUser.Email, result.Email);
        Assert.Equal(savedUser.Password, result.Password);
        Assert.NotEqual("NewPassword123!", result.Password); // 密码应该已经被哈希
        Assert.NotEqual("oldHashedPassword", result.Password); // 密码应该已经被更新
        _userRepositoryMock.Verify(r => r.GetByIdAsync(existingUser.UserId), Times.Once);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SysUser>()), Times.Once);
    }

    /// <summary>
    /// 测试更新用户方法，当用户不存在时返回null
    /// </summary>
    [Fact]
    public async Task UpdateUserAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new SysUser();
        user.SetUsername("testuser");
        user.SetEmail("test@example.com");

        // 确保mock返回null，模拟用户不存在
        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.UserId)).ReturnsAsync((SysUser?)null);

        // Act
        var result = await _userService.UpdateUserAsync(user);

        // Assert
        Assert.Null(result);
        _userRepositoryMock.Verify(r => r.GetByIdAsync(user.UserId), Times.Once);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SysUser>()), Times.Never);
    }

    /// <summary>
    /// 测试更新用户方法，当不提供新密码时保持原有密码
    /// </summary>
    [Fact]
    public async Task UpdateUserAsync_ShouldKeepOriginalPassword_WhenNoNewPasswordProvided()
    {
        // Arrange
        var existingPassword = "oldHashedPassword";
        var existingUser = new SysUser();
        existingUser.SetUsername("olduser");
        existingUser.SetEmail("old@example.com");
        existingUser.SetPassword(existingPassword, "test-salt");

        // 使用与existingUser相同的UserId，这样GetByIdAsync才能找到existingUser
        var updatedUser = existingUser;
        updatedUser.SetUsername("newuser");
        updatedUser.SetEmail("new@example.com");
        // 不设置新密码，测试原有密码是否会被保持

        var savedUser = new SysUser();
        savedUser.SetUsername("newuser");
        savedUser.SetEmail("new@example.com");
        savedUser.SetPassword(existingPassword, "test-salt"); // 应该保持原有密码

        // 使用existingUser的UserId设置mock，这样GetByIdAsync才能找到existingUser
        _userRepositoryMock.Setup(r => r.GetByIdAsync(existingUser.UserId)).ReturnsAsync(existingUser);
        _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<SysUser>())).ReturnsAsync(savedUser);

        // Act
        var result = await _userService.UpdateUserAsync(updatedUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(savedUser.Username, result.Username);
        Assert.Equal(savedUser.Email, result.Email);
        Assert.Equal(existingPassword, result.Password); // 应该保持原有密码
        _userRepositoryMock.Verify(r => r.GetByIdAsync(existingUser.UserId), Times.Once);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SysUser>()), Times.Once);
    }

    /// <summary>
    /// 测试删除用户方法，当用户存在时返回true
    /// </summary>
    [Fact]
    public async Task DeleteUserAsync_ShouldReturnTrue_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepositoryMock.Setup(r => r.DeleteAsync(userId)).ReturnsAsync(true);

        // Act
        var result = await _userService.DeleteUserAsync(userId);

        // Assert
        Assert.True(result);
        _userRepositoryMock.Verify(r => r.DeleteAsync(userId), Times.Once);
    }

    /// <summary>
    /// 测试删除用户方法，当用户不存在时返回false
    /// </summary>
    [Fact]
    public async Task DeleteUserAsync_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepositoryMock.Setup(r => r.DeleteAsync(userId)).ReturnsAsync(false);

        // Act
        var result = await _userService.DeleteUserAsync(userId);

        // Assert
        Assert.False(result);
        _userRepositoryMock.Verify(r => r.DeleteAsync(userId), Times.Once);
    }

    /// <summary>
    /// 测试验证密码方法，当密码正确时返回true
    /// </summary>
    [Fact]
    public void VerifyPassword_ShouldReturnTrue_WhenPasswordIsCorrect()
    {
        // Arrange
        var user = new SysUser();
        user.SetUsername("testuser");
        user.SetPassword("hashedPassword", "test-salt");

        var password = "Password123!";

        // 模拟密码验证成功
        var passwordHasher = new PasswordHasher<SysUser>();
        var hashedPassword = passwordHasher.HashPassword(user, password);
        user.SetPassword(hashedPassword, "test-salt");

        // Act
        var result = _userService.VerifyPassword(user, password);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 测试验证密码方法，当密码不正确时返回false
    /// </summary>
    [Fact]
    public void VerifyPassword_ShouldReturnFalse_WhenPasswordIsIncorrect()
    {
        // Arrange
        var user = new SysUser();
        user.SetUsername("testuser");
        user.SetPassword("hashedPassword", "test-salt");

        var correctPassword = "Password123!";
        var incorrectPassword = "WrongPassword!";

        // 模拟密码验证
        var passwordHasher = new PasswordHasher<SysUser>();
        var hashedPassword = passwordHasher.HashPassword(user, correctPassword);
        user.SetPassword(hashedPassword, "test-salt");

        // Act
        var result = _userService.VerifyPassword(user, incorrectPassword);

        // Assert
        Assert.False(result);
    }
}
