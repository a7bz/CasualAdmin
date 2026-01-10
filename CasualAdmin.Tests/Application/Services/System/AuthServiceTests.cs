namespace CasualAdmin.Tests.Application.Services.System;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Services.System;
using CasualAdmin.Domain.Entities.System;
using global::System.IdentityModel.Tokens.Jwt;
using global::System.Security.Claims;
using global::System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

/// <summary>
/// 认证服务测试类
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IRoleService> _roleServiceMock;
    private readonly AuthService _authService;

    /// <summary>
    /// 测试类构造函数，初始化模拟对象和被测服务
    /// </summary>
    public AuthServiceTests()
    {
        // 初始化模拟配置
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.SetupGet(c => c["Jwt:Key"]).Returns("ThisIsASecretKeyForJwtTokenGeneration1234567890");
        _configurationMock.SetupGet(c => c["Jwt:Issuer"]).Returns("CasualAdmin");
        _configurationMock.SetupGet(c => c["Jwt:Audience"]).Returns("CasualAdminUsers");
        _configurationMock.SetupGet(c => c["Jwt:ExpireHours"]).Returns("2");

        // 初始化模拟角色服务
        _roleServiceMock = new Mock<IRoleService>();

        // 创建被测服务实例
        _authService = new AuthService(_configurationMock.Object, _roleServiceMock.Object);
    }

    /// <summary>
    /// 测试生成JWT令牌方法
    /// </summary>
    [Fact]
    public async Task GenerateJwtToken_ShouldReturnValidToken_WhenUserIsValid()
    {
        // Arrange
        var user = new SysUser();
        user.SetUsername("testuser");
        user.SetEmail("test@example.com");

        var roles = new List<SysRole>
        {
            new() { Name = "Admin" },
            new() { Name = "User" }
        };

        _roleServiceMock.Setup(r => r.GetRolesByUserIdAsync(user.UserId)).ReturnsAsync(roles);

        // Act
        var token = await _authService.GenerateJwtToken(user);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        // 验证令牌内容
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        Assert.NotNull(jwtToken);
        Assert.Contains(jwtToken!.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.UserId.ToString());
        Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
        Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.Name && c.Value == user.Username);
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == "User");
    }

    /// <summary>
    /// 测试验证JWT令牌方法，当令牌有效时返回true
    /// </summary>
    [Fact]
    public async Task ValidateJwtToken_ShouldReturnTrue_WhenTokenIsValid()
    {
        // Arrange
        var user = new SysUser();
        user.SetUsername("testuser");
        user.SetEmail("test@example.com");

        _roleServiceMock.Setup(r => r.GetRolesByUserIdAsync(user.UserId)).ReturnsAsync([]);

        var validToken = await _authService.GenerateJwtToken(user);

        // Act
        var result = _authService.ValidateJwtToken(validToken);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 测试验证JWT令牌方法，当令牌无效时返回false
    /// </summary>
    [Fact]
    public void ValidateJwtToken_ShouldReturnFalse_WhenTokenIsInvalid()
    {
        // Arrange
        var invalidToken = "InvalidToken1234567890";

        // Act
        var result = _authService.ValidateJwtToken(invalidToken);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试验证JWT令牌方法，当令牌为空时返回false
    /// </summary>
    [Fact]
    public void ValidateJwtToken_ShouldReturnFalse_WhenTokenIsEmpty()
    {
        // Arrange
        var emptyToken = string.Empty;

        // Act
        var result = _authService.ValidateJwtToken(emptyToken);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试从令牌获取用户ID方法，当令牌有效时返回正确的用户ID
    /// </summary>
    [Fact]
    public async Task GetUserIdFromToken_ShouldReturnCorrectUserId_WhenTokenIsValid()
    {
        // Arrange
        var user = new SysUser();
        user.SetUsername("testuser");
        user.SetEmail("test@example.com");

        _roleServiceMock.Setup(r => r.GetRolesByUserIdAsync(user.UserId)).ReturnsAsync([]);

        var validToken = await _authService.GenerateJwtToken(user);

        // Act
        var actualUserId = _authService.GetUserIdFromToken(validToken);

        // Assert
        Assert.Equal(user.UserId, actualUserId);
    }

    /// <summary>
    /// 测试从令牌获取用户ID方法，当令牌为空时抛出ArgumentNullException
    /// </summary>
    [Fact]
    public void GetUserIdFromToken_ShouldThrowArgumentNullException_WhenTokenIsEmpty()
    {
        // Arrange
        var emptyToken = string.Empty;

        // Act
        Func<Guid> act = () => _authService.GetUserIdFromToken(emptyToken);

        // Assert
        Assert.Throws<ArgumentNullException>(() => _authService.GetUserIdFromToken(emptyToken));
    }

    /// <summary>
    /// 测试从令牌获取用户ID方法，当令牌无效时抛出异常
    /// </summary>
    [Fact]
    public void GetUserIdFromToken_ShouldThrowException_WhenTokenIsInvalid()
    {
        // Arrange
        var invalidToken = "InvalidToken1234567890";

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => _authService.GetUserIdFromToken(invalidToken));
    }

    /// <summary>
    /// 测试从令牌获取用户ID方法，当令牌不包含用户ID声明时抛出ArgumentException
    /// </summary>
    [Fact]
    public void GetUserIdFromToken_ShouldThrowArgumentException_WhenTokenDoesNotContainUserId()
    {
        // Arrange
        // 创建一个不包含sub声明的令牌
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("ThisIsASecretKeyForJwtTokenGeneration1234567890");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = "CasualAdmin",
            Audience = "CasualAdminUsers",
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Claims = new Dictionary<string, object>
            {
                { JwtRegisteredClaimNames.Email, "test@example.com" },
                { JwtRegisteredClaimNames.Name, "testuser" }
            }
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var invalidToken = tokenHandler.WriteToken(token);

        // Act
        Func<Guid> act = () => _authService.GetUserIdFromToken(invalidToken);

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _authService.GetUserIdFromToken(invalidToken));
        Assert.Contains("Token does not contain user ID", exception.Message);
    }
}
