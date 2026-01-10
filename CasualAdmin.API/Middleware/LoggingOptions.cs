namespace CasualAdmin.API.Middleware;

/// <summary>
/// 日志中间件配置选项
/// </summary>
public class LoggingOptions
{
    /// <summary>
    /// 是否记录请求体
    /// </summary>
    public bool IncludeRequestBody { get; set; } = true;

    /// <summary>
    /// 是否记录响应体
    /// </summary>
    public bool IncludeResponseBody { get; set; } = true;

    /// <summary>
    /// 请求体大小限制（字节），超过此大小的请求体将不会被记录
    /// </summary>
    public int RequestBodySizeLimit { get; set; } = 1024 * 1024; // 默认1MB

    /// <summary>
    /// 响应体大小限制（字节），超过此大小的响应体将不会被记录
    /// </summary>
    public int ResponseBodySizeLimit { get; set; } = 1024 * 1024; // 默认1MB

    /// <summary>
    /// 排除记录日志的路径
    /// </summary>
    public List<string> ExcludePaths { get; set; } = new List<string>();

    /// <summary>
    /// 排除记录日志的HTTP方法
    /// </summary>
    public List<string> ExcludeMethods { get; set; } = new List<string>();
}
