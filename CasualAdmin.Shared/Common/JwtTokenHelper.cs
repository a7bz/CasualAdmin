using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CasualAdmin.Shared.Common;

/// <summary>
/// JWT Token工具类，用于从token中获取用户信息
/// </summary>
public static class JwtTokenHelper
{
    /// <summary>
    /// 从JWT Token中提取用户ID
    /// </summary>
    /// <param name="token">JWT Token字符串</param>
    /// <returns>用户ID</returns>
    /// <exception cref="ArgumentNullException">当token为空时抛出</exception>
    /// <exception cref="ArgumentException">当token无效或不包含用户ID时抛出</exception>
    public static Guid GetUserIdFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token));

        var tokenHandler = new JwtSecurityTokenHandler();

        if (tokenHandler.ReadToken(token) is not JwtSecurityToken jwtToken)
            throw new ArgumentException("无效的token");

        var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub) 
            ?? throw new ArgumentException("token中不包含用户ID");
        
        return Guid.Parse(userIdClaim.Value);
    }

    /// <summary>
    /// 从JWT Token中提取邮箱
    /// </summary>
    /// <param name="token">JWT Token字符串</param>
    /// <returns>用户邮箱</returns>
    /// <exception cref="ArgumentNullException">当token为空时抛出</exception>
    /// <exception cref="ArgumentException">当token无效或不包含邮箱时抛出</exception>
    public static string GetEmailFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token));

        var tokenHandler = new JwtSecurityTokenHandler();

        if (tokenHandler.ReadToken(token) is not JwtSecurityToken jwtToken)
            throw new ArgumentException("无效的token");

        var emailClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Email) 
            ?? throw new ArgumentException("token中不包含邮箱");
        
        return emailClaim.Value;
    }

    /// <summary>
    /// 从JWT Token中提取用户名
    /// </summary>
    /// <param name="token">JWT Token字符串</param>
    /// <returns>用户名</returns>
    /// <exception cref="ArgumentNullException">当token为空时抛出</exception>
    /// <exception cref="ArgumentException">当token无效或不包含用户名时抛出</exception>
    public static string GetUsernameFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token));

        var tokenHandler = new JwtSecurityTokenHandler();

        if (tokenHandler.ReadToken(token) is not JwtSecurityToken jwtToken)
            throw new ArgumentException("无效的token");

        var usernameClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Name) 
            ?? throw new ArgumentException("token中不包含用户名");
        
        return usernameClaim.Value;
    }

    /// <summary>
    /// 从JWT Token中提取角色列表
    /// </summary>
    /// <param name="token">JWT Token字符串</param>
    /// <returns>角色列表</returns>
    /// <exception cref="ArgumentNullException">当token为空时抛出</exception>
    /// <exception cref="ArgumentException">当token无效时抛出</exception>
    public static List<string> GetRolesFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token));

        var tokenHandler = new JwtSecurityTokenHandler();

        if (tokenHandler.ReadToken(token) is not JwtSecurityToken jwtToken)
            throw new ArgumentException("无效的token");

        var roleClaims = jwtToken.Claims.Where(claim => claim.Type == ClaimTypes.Role);
        return roleClaims.Select(claim => claim.Value).ToList();
    }

    /// <summary>
    /// 从JWT Token中提取用户信息
    /// </summary>
    /// <param name="token">JWT Token字符串</param>
    /// <returns>用户信息对象</returns>
    /// <exception cref="ArgumentNullException">当token为空时抛出</exception>
    /// <exception cref="ArgumentException">当token无效或缺少必要信息时抛出</exception>
    public static TokenUserInfo GetUserInfoFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token));

        var tokenHandler = new JwtSecurityTokenHandler();

        if (tokenHandler.ReadToken(token) is not JwtSecurityToken jwtToken)
            throw new ArgumentException("无效的token");

        var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub) 
            ?? throw new ArgumentException("token中不包含用户ID");
        
        var emailClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Email) 
            ?? throw new ArgumentException("token中不包含邮箱");
        
        var usernameClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Name) 
            ?? throw new ArgumentException("token中不包含用户名");
        
        var roleClaims = jwtToken.Claims.Where(claim => claim.Type == ClaimTypes.Role);
        var roles = roleClaims.Select(claim => claim.Value).ToList();

        return new TokenUserInfo
        {
            UserId = Guid.Parse(userIdClaim.Value),
            Email = emailClaim.Value,
            Username = usernameClaim.Value,
            Roles = roles
        };
    }

    /// <summary>
    /// 从Authorization头中提取token
    /// </summary>
    /// <param name="authorizationHeader">Authorization头值</param>
    /// <returns>提取的token字符串</returns>
    /// <exception cref="ArgumentNullException">当authorizationHeader为空时抛出</exception>
    /// <exception cref="ArgumentException">当authorizationHeader格式不正确时抛出</exception>
    public static string ExtractTokenFromHeader(string authorizationHeader)
    {
        if (string.IsNullOrEmpty(authorizationHeader))
            throw new ArgumentNullException(nameof(authorizationHeader));

        if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Authorization头格式不正确，应为Bearer token");

        return authorizationHeader.Substring("Bearer ".Length).Trim();
    }
}

/// <summary>
/// Token用户信息对象
/// </summary>
public class TokenUserInfo
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 角色列表
    /// </summary>
    public List<string> Roles { get; set; } = new List<string>();
}