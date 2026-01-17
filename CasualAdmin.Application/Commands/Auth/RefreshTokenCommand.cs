namespace CasualAdmin.Application.Commands.Auth;

/// <summary>
/// 刷新Token命令
/// </summary>
public class RefreshTokenCommand
{
    /// <summary>
    /// 旧的JWT Token
    /// </summary>
    public string? Token { get; set; }
}