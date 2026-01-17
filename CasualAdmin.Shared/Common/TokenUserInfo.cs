namespace CasualAdmin.Shared.Common;

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
    public List<string> Roles { get; set; } = [];
}