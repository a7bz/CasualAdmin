namespace CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Domain.Entities.System;

/// <summary>
/// 认证服务接口
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 生成JWT Token
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <returns>JWT Token</returns>
    Task<string> GenerateJwtToken(SysUser user);

    /// <summary>
    /// 验证JWT Token
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>验证结果</returns>
    bool ValidateJwtToken(string token);

    /// <summary>
    /// 从Token中获取用户ID
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>用户ID</returns>
    Guid GetUserIdFromToken(string token);
}
