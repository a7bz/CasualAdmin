namespace CasualAdmin.Application.Commands.Auth;

/// <summary>
/// 登录命令
/// </summary>
public class LoginCommand
{
    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
