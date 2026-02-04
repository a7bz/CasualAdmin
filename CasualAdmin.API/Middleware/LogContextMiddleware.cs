namespace CasualAdmin.API.Middleware;
using System.Diagnostics;
using Serilog.Context;

/// <summary>
/// 日志上下文中间件
/// 为日志添加请求 ID、用户 ID、跟踪 ID 等上下文信息
/// </summary>
public class LogContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LogContextMiddleware> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="next">下一个中间件</param>
    /// <param name="logger">日志记录器</param>
    public LogContextMiddleware(RequestDelegate next, ILogger<LogContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// 中间件执行方法
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <returns>任务</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // 生成或获取请求 ID
        var requestId = context.TraceIdentifier;

        // 获取或生成跟踪 ID（用于分布式追踪）
        var traceId = GetOrCreateTraceId(context);

        // 使用 LogContext 设置上下文信息
        using (LogContext.PushProperty("RequestId", requestId))
        using (LogContext.PushProperty("TraceId", traceId))
        using (LogContext.PushProperty("Method", context.Request.Method))
        using (LogContext.PushProperty("Path", context.Request.Path))
        using (LogContext.PushProperty("Host", context.Request.Host.Value))
        using (LogContext.PushProperty("RemoteIp", GetRemoteIpAddress(context)))
        {
            // 如果用户已认证，添加用户信息
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                var userName = context.User.Identity.Name;
                var roles = string.Join(",", context.User.Claims.Where(c => c.Type == "role").Select(c => c.Value));

                if (!string.IsNullOrEmpty(userId))
                {
                    LogContext.PushProperty("UserId", userId);
                }
                if (!string.IsNullOrEmpty(userName))
                {
                    LogContext.PushProperty("UserName", userName);
                }
                if (!string.IsNullOrEmpty(roles))
                {
                    LogContext.PushProperty("Roles", roles);
                }
            }

            // 添加用户代理信息
            var userAgent = context.Request.Headers["UserAgent"].ToString();
            if (!string.IsNullOrEmpty(userAgent))
            {
                LogContext.PushProperty("UserAgent", userAgent);
            }

            // 执行下一个中间件
            await _next(context);
        }
    }

    /// <summary>
    /// 获取或创建跟踪 ID
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <returns>跟踪 ID</returns>
    private static string GetOrCreateTraceId(HttpContext context)
    {
        // 尝试从请求头获取跟踪 ID
        if (context.Request.Headers.TryGetValue("X-Trace-Id", out var traceIdValue))
        {
            return traceIdValue.ToString();
        }

        // 尝试从请求头获取 X-Request-ID
        if (context.Request.Headers.TryGetValue("X-Request-ID", out var requestIdValue))
        {
            return requestIdValue.ToString();
        }

        // 如果没有跟踪 ID，使用 Activity.Current 或生成新的
        var activity = Activity.Current;
        if (activity != null && !string.IsNullOrEmpty(activity.TraceId.ToString()))
        {
            return activity.TraceId.ToString();
        }

        // 生成新的跟踪 ID
        return Guid.NewGuid().ToString();
    }

    /// <summary>
    /// 获取远程 IP 地址
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <returns>IP地址</returns>
    private static string GetRemoteIpAddress(HttpContext context)
    {
        // 尝试从 X-Forwarded-For 获取真实 IP（代理后的真实客户端 IP）
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            var forwardedIps = forwardedFor.ToString().Split(',');
            if (forwardedIps.Length > 0)
            {
                return forwardedIps[0].Trim();
            }
        }

        // 尝试从 X-Real-IP 获取
        if (context.Request.Headers.TryGetValue("X-Real-IP", out var realIp))
        {
            return realIp.ToString();
        }

        // 直接获取连接的远程 IP
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}

/// <summary>
/// 日志上下文中间件扩展方法
/// </summary>
public static class LogContextMiddlewareExtensions
{
    /// <summary>
    /// 使用日志上下文中间件
    /// </summary>
    /// <param name="app">应用构建器</param>
    /// <returns>应用构建器</returns>
    public static IApplicationBuilder UseLogContext(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LogContextMiddleware>();
    }
}