namespace CasualAdmin.Application.Services.System;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Domain.Entities.System;
using global::System.IdentityModel.Tokens.Jwt;
using global::System.Security.Claims;
using global::System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// 认证服务实现
/// </summary>
public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IRoleService _roleService;
    private readonly IUserService _userService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="configuration">配置服务</param>
    /// <param name="roleService">角色服务</param>
    /// <param name="userService">用户服务</param>
    public AuthService(IConfiguration configuration, IRoleService roleService, IUserService userService)
    {
        _configuration = configuration;
        _roleService = roleService;
        _userService = userService;
    }

    /// <summary>
    /// 生成JWT Token
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <returns>JWT Token</returns>
    public async Task<string> GenerateJwtToken(SysUser user)
    {
        // 获取用户角色列表
        var roles = await _roleService.GetRolesByUserIdAsync(user.UserId);
        return await GenerateJwtToken(user, roles);
    }

    /// <summary>
    /// 生成JWT Token（使用预查询的角色列表，避免重复查询数据库）
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="roles">用户角色列表</param>
    /// <returns>JWT Token</returns>
    public async Task<string> GenerateJwtToken(SysUser user, List<SysRole> roles)
    {
        // 创建基础Claims
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name, user.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // 添加角色Claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }

        var jwtKey = _configuration["Jwt:Key"] ?? string.Empty;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpireHours"])),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// 验证JWT Token
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>验证结果</returns>
    public bool ValidateJwtToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtKey = _configuration["Jwt:Key"] ?? string.Empty;
        var key = Encoding.UTF8.GetBytes(jwtKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 刷新JWT Token
    /// </summary>
    /// <param name="token">旧的JWT Token</param>
    /// <returns>新的JWT Token</returns>
    public async Task<string> RefreshJwtToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token), "Token不能为空");

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtKey = _configuration["Jwt:Key"] ?? string.Empty;
        var key = Encoding.UTF8.GetBytes(jwtKey);

        try
        {
            // 验证旧token
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            // 从Claims中获取用户ID
            var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub) ?? throw new SecurityTokenException("Token中不包含用户ID");
            var userId = Guid.Parse(userIdClaim.Value);

            // 获取用户信息
            var user = await _userService.GetUserByIdAsync(userId) ?? throw new SecurityTokenException("用户不存在");

            // 生成新token
            return await GenerateJwtToken(user);
        }
        catch (Exception ex)
        {
            throw new SecurityTokenException("Token刷新失败", ex);
        }
    }
}
