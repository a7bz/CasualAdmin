namespace CasualAdmin.API.Middleware;
using CasualAdmin.Shared.Common;

/// <summary>
/// 异常处理中间件
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="next">下一个中间件</param>
    /// <param name="logger">日志记录器</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// 中间件执行方法
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <returns>Task</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // 执行下一个中间件
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing the request: {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// 处理异常
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <param name="exception">异常对象</param>
    /// <returns>Task</returns>
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = exception switch
        {
            ArgumentNullException => new ApiResponse<object>(StatusCodes.Status400BadRequest, "请求参数不能为空"),
            ArgumentException => new ApiResponse<object>(StatusCodes.Status400BadRequest, "请求参数无效"),
            KeyNotFoundException => new ApiResponse<object>(StatusCodes.Status404NotFound, "请求的资源不存在"),
            UnauthorizedAccessException => new ApiResponse<object>(StatusCodes.Status401Unauthorized, "未授权访问"),
            _ => new ApiResponse<object>(StatusCodes.Status500InternalServerError, "服务器内部错误，请稍后重试")
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}