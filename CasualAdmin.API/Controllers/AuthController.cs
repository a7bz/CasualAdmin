namespace CasualAdmin.API.Controllers;
using AutoMapper;
using CasualAdmin.Application.Commands.Auth;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Models.DTOs.Responses.System;
using CasualAdmin.Domain.Entities.System;
using CasualAdmin.Shared.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using global::System.Text.RegularExpressions;

/// <summary>
/// 认证控制器
/// </summary>
[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly IRsaEncryptionService _rsaEncryptionService;
    private readonly IRoleService _roleService;
    private readonly IPermissionService _permissionService;
    private readonly IMapper _mapper;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userService">用户服务</param>
    /// <param name="authService">认证服务</param>
    /// <param name="rsaEncryptionService">RSA加密服务</param>
    /// <param name="roleService">角色服务</param>
    /// <param name="permissionService">权限服务</param>
    /// <param name="mapper">对象映射服务</param>
    public AuthController(IUserService userService, IAuthService authService, IRsaEncryptionService rsaEncryptionService, IRoleService roleService, IPermissionService permissionService, IMapper mapper)
    {
        _userService = userService;
        _authService = authService;
        _rsaEncryptionService = rsaEncryptionService;
        _roleService = roleService;
        _permissionService = permissionService;
        _mapper = mapper;
    }

    /// <summary>
    /// 验证密码复杂度要求
    /// </summary>
    /// <param name="password">密码</param>
    /// <returns>是否符合复杂度要求</returns>
    private bool ValidatePasswordComplexity(string password)
    {
        // 使用正则表达式验证密码复杂度：至少8位，包含大小写字母、数字和特殊字符
        var pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$";
        return Regex.IsMatch(password, pattern);
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
    /// <returns>注册结果，包含用户信息和Token</returns>
    [HttpPost("register")]
    public async Task<ApiResponse<object>> Register([FromBody] RegisterCommand command)
    {
        // 检查邮箱是否已存在
        var existingUser = await _userService.GetUserByEmailAsync(command.Email);
        if (existingUser != null)
        {
            return ApiResponse<object>.BadRequest("邮箱已被注册");
        }

        // 解密密码
        var decryptedPassword = _rsaEncryptionService.Decrypt(command.Password);

        // 验证密码复杂度
        if (!ValidatePasswordComplexity(decryptedPassword))
        {
            return ApiResponse<object>.BadRequest("密码长度不能少于8位，必须包含大小写字母、数字和特殊字符");
        }

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
            return ApiResponse<object>.Failed("注册失败");
        }

        // 生成Token
        var token = await _authService.GenerateJwtToken(createdUser);
        // 将用户实体转换为DTO，避免返回嵌套的部门信息
        var userDto = _mapper.Map<SysUserDto>(createdUser);

        // 获取用户角色列表
        var roles = await _roleService.GetRolesByUserIdAsync(createdUser.UserId);

        // 将角色列表转换为DTO，只返回必要的信息
        var roleDtos = _mapper.Map<List<SysRoleDto>>(roles);

        // 批量获取所有角色的权限，避免N+1查询问题
        var permissionKeys = new List<string>();
        if (roles.Count > 0)
        {
            var roleIds = roles.Select(r => r.RoleId).ToList();
            var permissions = await _permissionService.GetPermissionsByRoleIdsAsync(roleIds);
            permissionKeys = permissions.Select(p => p.PermissionCode).Distinct().ToList();
        }

        return ApiResponse<object>.Success(new
        {
            User = userDto,
            Token = token,
            Roles = roleDtos,
            PermissionKeys = permissionKeys
        }, "注册成功");
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="command">登录命令，包含加密的密码</param>
    /// <returns>登录结果，包含Token</returns>
    [HttpPost("login")]
    public async Task<ApiResponse<object>> Login([FromBody] LoginCommand command)
    {
        // 查找用户（支持用户名或邮箱）
        var user = await _userService.GetUserByUsernameAsync(command.Account);
        if (user == null)
        {
            // 如果用户名不存在，尝试通过邮箱查找
            user = await _userService.GetUserByEmailAsync(command.Account);
        }

        if (user == null)
        {
            return ApiResponse<object>.BadRequest("账号或密码错误");
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
        // 将用户实体转换为DTO，避免返回嵌套的部门信息
        var userDto = _mapper.Map<SysUserDto>(user);

        // 获取用户角色列表
        var roles = await _roleService.GetRolesByUserIdAsync(user.UserId);

        // 将角色列表转换为DTO，只返回必要的信息
        var roleDtos = _mapper.Map<List<SysRoleDto>>(roles);

        // 批量获取所有角色的权限，避免N+1查询问题
        var permissionKeys = new List<string>();
        if (roles.Count > 0)
        {
            var roleIds = roles.Select(r => r.RoleId).ToList();
            var permissions = await _permissionService.GetPermissionsByRoleIdsAsync(roleIds);
            permissionKeys = permissions.Select(p => p.PermissionCode).Distinct().ToList();
        }

        return ApiResponse<object>.Success(new
        {
            User = userDto,
            Token = token,
            Roles = roleDtos,
            PermissionKeys = permissionKeys
        }, "登录成功");
    }

    /// <summary>
    /// 刷新Token
    /// </summary>
    /// <param name="refreshTokenCommand">刷新Token命令，包含旧Token</param>
    /// <returns>新Token</returns>
    [HttpPost("refresh-token")]
    public async Task<ApiResponse<object>> RefreshToken([FromBody] RefreshTokenCommand refreshTokenCommand)
    {
        try
        {
            if (string.IsNullOrEmpty(refreshTokenCommand.Token))
            {
                return ApiResponse<object>.BadRequest("Token不能为空");
            }

            var newToken = await _authService.RefreshJwtToken(refreshTokenCommand.Token);
            return ApiResponse<object>.Success(new
            {
                Token = newToken
            }, "Token刷新成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<object>.BadRequest(ex.Message);
        }
    }
}