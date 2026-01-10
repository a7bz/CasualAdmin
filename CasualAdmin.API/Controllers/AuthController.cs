namespace CasualAdmin.API.Controllers;
using CasualAdmin.Application.Commands.Auth;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Services.System;
using CasualAdmin.Domain.Entities.System;
using CasualAdmin.Shared.Common;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// 认证控制器
/// </summary>
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly IRsaEncryptionService _rsaEncryptionService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userService">用户服务</param>
    /// <param name="authService">认证服务</param>
    /// <param name="rsaEncryptionService">RSA加密服务</param>
    public AuthController(IUserService userService, IAuthService authService, IRsaEncryptionService rsaEncryptionService)
    {
        _userService = userService;
        _authService = authService;
        _rsaEncryptionService = rsaEncryptionService;
    }

    /// <summary>
    /// 获取RSA公钥
    /// </summary>
    /// <returns>RSA公钥</returns>
    [HttpGet("public-key")]
    public ApiResponse<string> GetPublicKey()
    {
        var publicKey = _rsaEncryptionService.GetPublicKey();
        return ApiResponse<string>.Success(publicKey, "获取公钥成功");
    }

    /// <summary>
    /// 用户注册
    /// </summary>
    /// <param name="command">注册命令，包含加密的密码</param>
    /// <returns>注册结果</returns>
    [HttpPost("register")]
    public async Task<ApiResponse<SysUser>> Register([FromBody] RegisterCommand command)
    {
        // 检查邮箱是否已存在
        var existingUser = await _userService.GetUserByEmailAsync(command.Email);
        if (existingUser != null)
        {
            return ApiResponse<SysUser>.BadRequest("邮箱已被注册");
        }

        // 解密密码
        var decryptedPassword = _rsaEncryptionService.Decrypt(command.Password);

        // 创建新用户
        var user = new SysUser();
        user.SetUsername(command.UserName);
        user.SetEmail(command.Email);

        // 使用PasswordHasher加密密码，并生成随机盐值
        var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<SysUser>();
        var salt = Guid.NewGuid().ToString();
        var hashedPassword = passwordHasher.HashPassword(user, decryptedPassword);
        user.SetPassword(hashedPassword, salt);

        var createdUser = await _userService.CreateUserAsync(user);
        if (createdUser == null)
        {
            return ApiResponse<SysUser>.Failed("注册失败");
        }

        return ApiResponse<SysUser>.Success(createdUser, "注册成功");
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="command">登录命令，包含加密的密码</param>
    /// <returns>登录结果，包含Token</returns>
    [HttpPost("login")]
    public async Task<ApiResponse<object>> Login([FromBody] LoginCommand command)
    {
        // 查找用户
        var user = await _userService.GetUserByEmailAsync(command.Email);
        if (user == null)
        {
            return ApiResponse<object>.BadRequest("邮箱或密码错误");
        }

        // 解密密码
        var decryptedPassword = _rsaEncryptionService.Decrypt(command.Password);

        // 验证密码
        if (!_userService.VerifyPassword(user, decryptedPassword))
        {
            return ApiResponse<object>.BadRequest("邮箱或密码错误");
        }

        // 生成Token
        var token = await _authService.GenerateJwtToken(user);

        return ApiResponse<object>.Success(new
        {
            User = user,
            Token = token
        }, "登录成功");
    }
}