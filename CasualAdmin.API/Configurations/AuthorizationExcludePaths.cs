namespace CasualAdmin.API.Configurations;

/// <summary>
/// 授权排除路径配置
/// </summary>
public class AuthorizationExcludePaths
{
    /// <summary>
    /// 跳过授权的路径列表
    /// </summary>
    public List<string> Paths { get; set; } =
    [
        "/Auth",
        "/swagger",
    ];

    /// <summary>
    /// 检查路径是否需要跳过授权
    /// </summary>
    /// <param name="path">请求路径</param>
    /// <returns>是否跳过授权</returns>
    public bool ShouldSkipAuthorization(string path)
    {
        return Paths.Any(excludePath => path.StartsWith(excludePath, StringComparison.OrdinalIgnoreCase));
    }
}