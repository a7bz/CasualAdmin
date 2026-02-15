namespace CasualAdmin.Shared.Common;

using CasualAdmin.Shared.Localization;

/// <summary>
/// API响应结果
/// </summary>
/// <typeparam name="T">响应数据类型</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// 状态码
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 响应数据
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// 时间戳（毫秒）
    /// </summary>
    public long Timestamp { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public ApiResponse()
    {
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="code">状态码</param>
    /// <param name="message">响应消息</param>
    /// <param name="data">响应数据</param>
    public ApiResponse(int code, string message, T? data = default)
    {
        Code = code;
        Message = message;
        Data = data;
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 成功响应
    /// </summary>
    /// <param name="data">响应数据</param>
    /// <param name="message">响应消息</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse<T> Success(T? data = default, string message = "操作成功")
    {
        return new ApiResponse<T>(200, message, data);
    }

    /// <summary>
    /// 失败响应
    /// </summary>
    /// <param name="message">响应消息</param>
    /// <param name="code">状态码</param>
    /// <param name="data">响应数据</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse<T> Failed(string message = "操作失败", int code = 500, T? data = default)
    {
        return new ApiResponse<T>(code, message, data);
    }

    /// <summary>
    /// 参数错误响应
    /// </summary>
    /// <param name="message">响应消息</param>
    /// <param name="data">响应数据</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse<T> BadRequest(string message = "参数错误", T? data = default)
    {
        return new ApiResponse<T>(400, message, data);
    }

    /// <summary>
    /// 未授权响应
    /// </summary>
    /// <param name="message">响应消息</param>
    /// <param name="data">响应数据</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse<T> Unauthorized(string message = "未授权", T? data = default)
    {
        return new ApiResponse<T>(401, message, data);
    }

    /// <summary>
    /// 禁止访问响应
    /// </summary>
    /// <param name="message">响应消息</param>
    /// <param name="data">响应数据</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse<T> Forbidden(string message = "禁止访问", T? data = default)
    {
        return new ApiResponse<T>(403, message, data);
    }

    /// <summary>
    /// 资源不存在响应
    /// </summary>
    /// <param name="message">响应消息</param>
    /// <param name="data">响应数据</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse<T> NotFound(string message = "资源不存在", T? data = default)
    {
        return new ApiResponse<T>(404, message, data);
    }

    /// <summary>
    /// 本地化成功响应
    /// </summary>
    /// <param name="localizationService">本地化服务</param>
    /// <param name="resourceKey">资源键</param>
    /// <param name="data">响应数据</param>
    /// <param name="args">参数</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse<T> LocalizeSuccess(ILocalizationService localizationService, string resourceKey, T? data = default, params object[] args)
    {
        var message = args.Length > 0
            ? localizationService.GetString(resourceKey, args)
            : localizationService.GetString(resourceKey);
        return new ApiResponse<T>(200, message, data);
    }

    /// <summary>
    /// 本地化失败响应
    /// </summary>
    /// <param name="localizationService">本地化服务</param>
    /// <param name="resourceKey">资源键</param>
    /// <param name="code">状态码</param>
    /// <param name="data">响应数据</param>
    /// <param name="args">参数</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse<T> LocalizeFailed(ILocalizationService localizationService, string resourceKey, int code = 500, T? data = default, params object[] args)
    {
        var message = args.Length > 0
            ? localizationService.GetString(resourceKey, args)
            : localizationService.GetString(resourceKey);
        return new ApiResponse<T>(code, message, data);
    }

    /// <summary>
    /// 本地化参数错误响应
    /// </summary>
    /// <param name="localizationService">本地化服务</param>
    /// <param name="resourceKey">资源键</param>
    /// <param name="data">响应数据</param>
    /// <param name="args">参数</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse<T> LocalizeBadRequest(ILocalizationService localizationService, string resourceKey, T? data = default, params object[] args)
    {
        var message = args.Length > 0
            ? localizationService.GetString(resourceKey, args)
            : localizationService.GetString(resourceKey);
        return new ApiResponse<T>(400, message, data);
    }

    /// <summary>
    /// 本地化未授权响应
    /// </summary>
    /// <param name="localizationService">本地化服务</param>
    /// <param name="resourceKey">资源键</param>
    /// <param name="data">响应数据</param>
    /// <param name="args">参数</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse<T> LocalizeUnauthorized(ILocalizationService localizationService, string resourceKey, T? data = default, params object[] args)
    {
        var message = args.Length > 0
            ? localizationService.GetString(resourceKey, args)
            : localizationService.GetString(resourceKey);
        return new ApiResponse<T>(401, message, data);
    }

    /// <summary>
    /// 本地化禁止访问响应
    /// </summary>
    /// <param name="localizationService">本地化服务</param>
    /// <param name="resourceKey">资源键</param>
    /// <param name="data">响应数据</param>
    /// <param name="args">参数</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse<T> LocalizeForbidden(ILocalizationService localizationService, string resourceKey, T? data = default, params object[] args)
    {
        var message = args.Length > 0
            ? localizationService.GetString(resourceKey, args)
            : localizationService.GetString(resourceKey);
        return new ApiResponse<T>(403, message, data);
    }

    /// <summary>
    /// 本地化资源不存在响应
    /// </summary>
    /// <param name="localizationService">本地化服务</param>
    /// <param name="resourceKey">资源键</param>
    /// <param name="data">响应数据</param>
    /// <param name="args">参数</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse<T> LocalizeNotFound(ILocalizationService localizationService, string resourceKey, T? data = default, params object[] args)
    {
        var message = args.Length > 0
            ? localizationService.GetString(resourceKey, args)
            : localizationService.GetString(resourceKey);
        return new ApiResponse<T>(404, message, data);
    }
}

/// <summary>
/// API响应结果（无数据）
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public ApiResponse() : base()
    { }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="code">状态码</param>
    /// <param name="message">响应消息</param>
    public ApiResponse(int code, string message) : base(code, message)
    { }

    /// <summary>
    /// 成功响应
    /// </summary>
    /// <param name="message">响应消息</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse Success(string message = "操作成功")
    {
        return new ApiResponse(200, message);
    }

    /// <summary>
    /// 失败响应
    /// </summary>
    /// <param name="message">响应消息</param>
    /// <param name="code">状态码</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse Failed(string message = "操作失败", int code = 500)
    {
        return new ApiResponse(code, message);
    }

    /// <summary>
    /// 参数错误响应
    /// </summary>
    /// <param name="message">响应消息</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse BadRequest(string message = "参数错误")
    {
        return new ApiResponse(400, message);
    }

    /// <summary>
    /// 未授权响应
    /// </summary>
    /// <param name="message">响应消息</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse Unauthorized(string message = "未授权")
    {
        return new ApiResponse(401, message);
    }

    /// <summary>
    /// 禁止访问响应
    /// </summary>
    /// <param name="message">响应消息</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse Forbidden(string message = "禁止访问")
    {
        return new ApiResponse(403, message);
    }

    /// <summary>
    /// 资源不存在响应
    /// </summary>
    /// <param name="message">响应消息</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse NotFound(string message = "资源不存在")
    {
        return new ApiResponse(404, message);
    }

    /// <summary>
    /// 本地化成功响应
    /// </summary>
    /// <param name="localizationService">本地化服务</param>
    /// <param name="resourceKey">资源键</param>
    /// <param name="args">参数</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse LocalizeSuccess(ILocalizationService localizationService, string resourceKey, params object[] args)
    {
        var message = args.Length > 0
            ? localizationService.GetString(resourceKey, args)
            : localizationService.GetString(resourceKey);
        return new ApiResponse(200, message);
    }

    /// <summary>
    /// 本地化失败响应
    /// </summary>
    /// <param name="localizationService">本地化服务</param>
    /// <param name="resourceKey">资源键</param>
    /// <param name="code">状态码</param>
    /// <param name="args">参数</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse LocalizeFailed(ILocalizationService localizationService, string resourceKey, int code = 500, params object[] args)
    {
        var message = args.Length > 0
            ? localizationService.GetString(resourceKey, args)
            : localizationService.GetString(resourceKey);
        return new ApiResponse(code, message);
    }

    /// <summary>
    /// 本地化参数错误响应
    /// </summary>
    /// <param name="localizationService">本地化服务</param>
    /// <param name="resourceKey">资源键</param>
    /// <param name="args">参数</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse LocalizeBadRequest(ILocalizationService localizationService, string resourceKey, params object[] args)
    {
        var message = args.Length > 0
            ? localizationService.GetString(resourceKey, args)
            : localizationService.GetString(resourceKey);
        return new ApiResponse(400, message);
    }

    /// <summary>
    /// 本地化未授权响应
    /// </summary>
    /// <param name="localizationService">本地化服务</param>
    /// <param name="resourceKey">资源键</param>
    /// <param name="args">参数</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse LocalizeUnauthorized(ILocalizationService localizationService, string resourceKey, params object[] args)
    {
        var message = args.Length > 0
            ? localizationService.GetString(resourceKey, args)
            : localizationService.GetString(resourceKey);
        return new ApiResponse(401, message);
    }

    /// <summary>
    /// 本地化禁止访问响应
    /// </summary>
    /// <param name="localizationService">本地化服务</param>
    /// <param name="resourceKey">资源键</param>
    /// <param name="args">参数</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse LocalizeForbidden(ILocalizationService localizationService, string resourceKey, params object[] args)
    {
        var message = args.Length > 0
            ? localizationService.GetString(resourceKey, args)
            : localizationService.GetString(resourceKey);
        return new ApiResponse(403, message);
    }

    /// <summary>
    /// 本地化资源不存在响应
    /// </summary>
    /// <param name="localizationService">本地化服务</param>
    /// <param name="resourceKey">资源键</param>
    /// <param name="args">参数</param>
    /// <returns>API响应结果</returns>
    public static ApiResponse LocalizeNotFound(ILocalizationService localizationService, string resourceKey, params object[] args)
    {
        var message = args.Length > 0
            ? localizationService.GetString(resourceKey, args)
            : localizationService.GetString(resourceKey);
        return new ApiResponse(404, message);
    }
}
