namespace CasualAdmin.API.Middleware;
using System.Security.Authentication;
using CasualAdmin.Shared.Common;
using Microsoft.IdentityModel.Tokens;

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

        // 获取环境变量
        var environment = context.RequestServices.GetRequiredService<IWebHostEnvironment>();

        // 根据异常类型设置状态码和错误信息
        var (statusCode, message) = exception switch
        {
            // 认证授权相关异常
            SecurityTokenExpiredException => (StatusCodes.Status401Unauthorized, "Token已过期"),
            SecurityTokenInvalidSignatureException => (StatusCodes.Status401Unauthorized, "Token签名无效"),
            SecurityTokenInvalidIssuerException => (StatusCodes.Status401Unauthorized, "Token颁发者无效"),
            SecurityTokenInvalidAudienceException => (StatusCodes.Status401Unauthorized, "Token受众无效"),
            AuthenticationException => (StatusCodes.Status401Unauthorized, "认证失败"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "未授权访问"),

            // 其他常见异常
            ArgumentNullException => (StatusCodes.Status400BadRequest, "请求参数不能为空"),
            ArgumentException => (StatusCodes.Status400BadRequest, "请求参数无效"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "请求的资源不存在"),

            // 默认异常
            _ => (StatusCodes.Status500InternalServerError, environment.IsDevelopment()
                ? $"服务器内部错误: {exception.Message}"
                : "服务器内部错误，请稍后重试")
        };

        context.Response.StatusCode = statusCode;
        var response = new ApiResponse<object>(statusCode, message);

        await context.Response.WriteAsJsonAsync(response);
    }
}