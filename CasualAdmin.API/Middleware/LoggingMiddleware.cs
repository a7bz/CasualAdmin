namespace CasualAdmin.API.Middleware;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Hosting;

/// <summary>
/// 日志中间件
/// </summary>
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;
    private readonly LoggingOptions _options;
    private readonly IHostEnvironment? _environment;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="next">下一个中间件</param>
    /// <param name="logger">日志记录器</param>
    /// <param name="environment">环境信息（可选）</param>
    /// <param name="options">日志配置选项（可选）</param>
    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger, IHostEnvironment? environment = null, LoggingOptions? options = null)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
        _options = options ?? new LoggingOptions();
    }

    /// <summary>
    /// 中间件执行方法
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <returns>Task</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // 检查是否需要排除当前路径或方法
        if (ShouldExcludeRequest(context.Request))
        {
            await _next(context);
            return;
        }

        // 记录请求开始
        var startTime = Stopwatch.GetTimestamp();
        _logger.LogInformation("Request started: {Method} {Path}", context.Request.Method, context.Request.Path);

        // 记录请求体（仅在开发环境或明确配置时）
        if (_options.IncludeRequestBody && (_environment?.IsDevelopment() ?? false))
        {
            var requestBody = await ReadRequestBodyAsync(context.Request);
            if (!string.IsNullOrEmpty(requestBody))
            {
                _logger.LogInformation("Request body: {RequestBody}", requestBody);
            }
        }

        // 只在需要记录响应体时创建内存流
        Stream originalResponseBodyStream = context.Response.Body;
        MemoryStream? responseBodyStream = null;

        try
        {
            if (_options.IncludeResponseBody && (_environment?.IsDevelopment() ?? false))
            {
                // 只在需要时创建内存流
                responseBodyStream = new MemoryStream();
                context.Response.Body = responseBodyStream;
            }

            // 执行下一个中间件
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing the request: {Method} {Path}", context.Request.Method, context.Request.Path);
            throw;
        }
        finally
        {
            // 记录响应信息
            var elapsedTime = Stopwatch.GetElapsedTime(startTime);
            // 优化时间显示格式，使用秒和毫秒，最多三位小数
            var formattedElapsedTime = $"{elapsedTime.TotalSeconds:F3}s";
            _logger.LogInformation("Request completed: {Method} {Path} {StatusCode} in {ElapsedTime}",
                context.Request.Method, context.Request.Path, context.Response.StatusCode, formattedElapsedTime);

            // 记录响应体（仅在开发环境或明确配置时）
            if (_options.IncludeResponseBody && (_environment?.IsDevelopment() ?? false) && responseBodyStream != null)
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseBody = await ReadResponseBodyAsync(context.Response);
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                if (!string.IsNullOrEmpty(responseBody))
                {
                    _logger.LogInformation("Response body: {ResponseBody}", responseBody);
                }

                // 复制响应到原始流
                await responseBodyStream.CopyToAsync(originalResponseBodyStream);
                context.Response.Body = originalResponseBodyStream;
            }
        }
    }

    /// <summary>
    /// 检查是否需要排除当前请求
    /// </summary>
    /// <param name="request">HTTP请求</param>
    /// <returns>是否需要排除</returns>
    private bool ShouldExcludeRequest(HttpRequest request)
    {
        // 检查请求方法
        if (_options.ExcludeMethods.Any(method => method.Equals(request.Method, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        // 检查请求路径
        if (_options.ExcludePaths.Any(path => request.Path.StartsWithSegments(path, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 读取请求体
    /// </summary>
    /// <param name="request">HTTP请求</param>
    /// <returns>请求体内容</returns>
    private async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        if (request.ContentLength == null || request.ContentLength == 0)
        {
            return string.Empty;
        }

        if (request.ContentLength > _options.RequestBodySizeLimit)
        {
            return $"[Request body too large, size: {request.ContentLength}, limit: {_options.RequestBodySizeLimit}]";
        }

        request.EnableBuffering();
        var body = await new StreamReader(request.Body, encoding: Encoding.UTF8).ReadToEndAsync();
        request.Body.Seek(0, SeekOrigin.Begin);
        return body;
    }

    /// <summary>
    /// 读取响应体
    /// </summary>
    /// <param name="response">HTTP响应</param>
    /// <returns>响应体内容</returns>
    private async Task<string> ReadResponseBodyAsync(HttpResponse response)
    {
        if (response.ContentLength == null || response.ContentLength == 0)
        {
            return string.Empty;
        }

        if (response.ContentLength > _options.ResponseBodySizeLimit)
        {
            return $"[Response body too large, size: {response.ContentLength}, limit: {_options.ResponseBodySizeLimit}]";
        }

        var body = await new StreamReader(response.Body, encoding: Encoding.UTF8).ReadToEndAsync();
        return body;
    }
}