namespace CasualAdmin.API.Middleware;

using System.Collections.Concurrent;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

/// <summary>
/// API 速率限制中间件
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly RateLimitingOptions _options;
    private readonly ConcurrentDictionary<string, RateLimitCounter> _rateLimitCounters;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="next">下一个中间件</param>
    /// <param name="logger">日志记录器</param>
    /// <param name="options">速率限制选项（可选）</param>
    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, RateLimitingOptions? options = null)
    {
        _next = next;
        _logger = logger;
        _options = options ?? new RateLimitingOptions();
        _rateLimitCounters = new ConcurrentDictionary<string, RateLimitCounter>();
    }

    /// <summary>
    /// 中间件执行方法
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <returns>任务</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // 检查是否需要跳过速率限制
        if (ShouldSkipRateLimiting(context))
        {
            await _next(context);
            return;
        }

        // 获取请求标识（IP地址或用户ID）
        var requestKey = GetRequestKey(context);

        // 检查速率限制
        if (!IsRequestAllowed(requestKey))
        {
            // 返回 429 Too Many Requests
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.Headers["RetryAfter"] = _options.WindowSeconds.ToString();
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\": \"Too many requests\", \"message\": \"Rate limit exceeded. Please try again later.\"}");
            _logger.LogWarning("Rate limit exceeded for {RequestKey}", requestKey);
            return;
        }

        // 执行下一个中间件
        await _next(context);
    }

    /// <summary>
    /// 检查是否需要跳过速率限制
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <returns>是否跳过</returns>
    private bool ShouldSkipRateLimiting(HttpContext context)
    {
        // 跳过特定路径
        var path = context.Request.Path.Value?.ToLower();
        if (_options.ExcludePaths.Any(excludePath => path?.StartsWith(excludePath) ?? false))
        {
            return true;
        }

        // 跳过特定HTTP方法
        if (_options.ExcludeMethods.Contains(context.Request.Method))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 获取请求标识
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <returns>请求标识</returns>
    private string GetRequestKey(HttpContext context)
    {
        // 优先使用用户ID（如果已认证）
        if (context.User.Identity?.IsAuthenticated ?? false)
        {
            var userId = context.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                return $"user:{userId}";
            }
        }

        // 否则使用IP地址
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return $"ip:{ipAddress}";
    }

    /// <summary>
    /// 检查请求是否允许
    /// </summary>
    /// <param name="requestKey">请求标识</param>
    /// <returns>是否允许</returns>
    private bool IsRequestAllowed(string requestKey)
    {
        var now = DateTime.UtcNow;

        // 获取或创建速率限制计数器
        var counter = _rateLimitCounters.GetOrAdd(requestKey, _ => new RateLimitCounter
        {
            LastRequestTime = now,
            RequestCount = 0
        });

        // 重置过期的计数器
        if ((now - counter.LastRequestTime).TotalSeconds > _options.WindowSeconds)
        {
            counter.RequestCount = 0;
            counter.LastRequestTime = now;
        }

        // 检查是否超过限制
        if (counter.RequestCount >= _options.MaxRequests)
        {
            return false;
        }

        // 增加请求计数
        counter.RequestCount++;
        counter.LastRequestTime = now;

        return true;
    }
}

/// <summary>
/// 速率限制选项
/// </summary>
public class RateLimitingOptions
{
    /// <summary>
    /// 时间窗口（秒）
    /// </summary>
    public int WindowSeconds { get; set; } = 60;

    /// <summary>
    /// 时间窗口内最大请求数
    /// </summary>
    public int MaxRequests { get; set; } = 100;

    /// <summary>
    /// 排除的路径
    /// </summary>
    public List<string> ExcludePaths { get; set; } = ["/swagger", "/health", "/files"];

    /// <summary>
    /// 排除的HTTP方法
    /// </summary>
    public List<string> ExcludeMethods { get; set; } = [];
}

/// <summary>
/// 速率限制计数器
/// </summary>
internal class RateLimitCounter
{
    /// <summary>
    /// 最后请求时间
    /// </summary>
    public DateTime LastRequestTime { get; set; }

    /// <summary>
    /// 请求计数
    /// </summary>
    public int RequestCount { get; set; }
}

/// <summary>
/// 速率限制中间件扩展方法
/// </summary>
public static class RateLimitingMiddlewareExtensions
{
    /// <summary>
    /// 使用速率限制中间件
    /// </summary>
    /// <param name="app">应用构建器</param>
    /// <param name="options">速率限制选项</param>
    /// <returns>应用构建器</returns>
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app, RateLimitingOptions? options = null)
    {
        if (options != null)
        {
            return app.UseMiddleware<RateLimitingMiddleware>(options);
        }
        return app.UseMiddleware<RateLimitingMiddleware>();
    }
}
