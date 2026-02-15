namespace CasualAdmin.API.Controllers;

using CasualAdmin.Shared.Common;
using CasualAdmin.Shared.Localization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// 本地化示例控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LocalizationController : ControllerBase
{
    private readonly ILocalizationService _localizationService;

    public LocalizationController(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    /// <summary>
    /// 获取当前语言
    /// </summary>
    /// <returns>当前语言代码</returns>
    [HttpGet("current-language")]
    public ApiResponse<string> GetCurrentLanguage()
    {
        var language = _localizationService.GetCurrentLanguage();
        return ApiResponse<string>.Success(language);
    }

    /// <summary>
    /// 获取本地化字符串
    /// </summary>
    /// <param name="key">资源键</param>
    /// <returns>本地化字符串</returns>
    [HttpGet("string")]
    public ApiResponse<string> GetLocalizedString([FromQuery] string key)
    {
        var value = _localizationService.GetString(key);
        return ApiResponse<string>.Success(value);
    }

    /// <summary>
    /// 获取本地化字符串（带参数）
    /// </summary>
    /// <param name="key">资源键</param>
    /// <param name="args">参数</param>
    /// <returns>本地化字符串</returns>
    [HttpGet("string-with-args")]
    public ApiResponse<string> GetLocalizedStringWithArgs([FromQuery] string key, [FromQuery] string[] args)
    {
        var value = _localizationService.GetString(key, args);
        return ApiResponse<string>.Success(value);
    }

    /// <summary>
    /// 成功响应示例（多语言）
    /// </summary>
    /// <returns>API响应</returns>
    [HttpGet("success")]
    public ApiResponse SuccessExample()
    {
        return ApiResponse.LocalizeSuccess(_localizationService, "Common_Success");
    }

    /// <summary>
    /// 失败响应示例（多语言）
    /// </summary>
    /// <returns>API响应</returns>
    [HttpGet("failed")]
    public ApiResponse FailedExample()
    {
        return ApiResponse.LocalizeFailed(_localizationService, "Common_Failed");
    }

    /// <summary>
    /// 参数错误响应示例（多语言）
    /// </summary>
    /// <returns>API响应</returns>
    [HttpGet("bad-request")]
    public ApiResponse BadRequestExample()
    {
        return ApiResponse.LocalizeBadRequest(_localizationService, "Common_BadRequest");
    }

    /// <summary>
    /// 未授权响应示例（多语言）
    /// </summary>
    /// <returns>API响应</returns>
    [HttpGet("unauthorized")]
    public ApiResponse UnauthorizedExample()
    {
        return ApiResponse.LocalizeUnauthorized(_localizationService, "Common_Unauthorized");
    }

    /// <summary>
    /// 禁止访问响应示例（多语言）
    /// </summary>
    /// <returns>API响应</returns>
    [HttpGet("forbidden")]
    public ApiResponse ForbiddenExample()
    {
        return ApiResponse.LocalizeForbidden(_localizationService, "Common_Forbidden");
    }

    /// <summary>
    /// 资源不存在响应示例（多语言）
    /// </summary>
    /// <returns>API响应</returns>
    [HttpGet("not-found")]
    public ApiResponse NotFoundExample()
    {
        return ApiResponse.LocalizeNotFound(_localizationService, "Common_NotFound");
    }

    /// <summary>
    /// 带参数的验证错误示例（多语言）
    /// </summary>
    /// <returns>API响应</returns>
    [HttpGet("validation-required")]
    public ApiResponse ValidationRequiredExample()
    {
        return ApiResponse.LocalizeBadRequest(_localizationService, "Validation_Required", "用户名");
    }

    /// <summary>
    /// 登录成功示例（多语言）
    /// </summary>
    /// <returns>API响应</returns>
    [HttpGet("login-success")]
    public ApiResponse LoginSuccessExample()
    {
        return ApiResponse.LocalizeSuccess(_localizationService, "Auth_LoginSuccess");
    }

    /// <summary>
    /// 用户不存在示例（多语言）
    /// </summary>
    /// <returns>API响应</returns>
    [HttpGet("user-not-found")]
    public ApiResponse UserNotFoundExample()
    {
        return ApiResponse.LocalizeNotFound(_localizationService, "User_NotFound");
    }

    /// <summary>
    /// 邮箱已存在示例（多语言）
    /// </summary>
    /// <returns>API响应</returns>
    [HttpGet("email-exists")]
    public ApiResponse EmailExistsExample()
    {
        return ApiResponse.LocalizeFailed(_localizationService, "Auth_EmailAlreadyExists");
    }

    /// <summary>
    /// 用户创建成功示例（多语言，带数据）
    /// </summary>
    /// <returns>API响应</returns>
    [HttpGet("user-create-success")]
    public ApiResponse<object> UserCreateSuccessExample()
    {
        var userData = new
        {
            UserId = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com"
        };
        return ApiResponse<object>.LocalizeSuccess(_localizationService, "User_CreateSuccess", userData);
    }

    /// <summary>
    /// 设置当前语言
    /// </summary>
    /// <param name="language">语言代码</param>
    /// <returns>API响应</returns>
    [HttpPost("set-language")]
    public ApiResponse SetLanguage([FromBody] string language)
    {
        _localizationService.SetCurrentLanguage(language);
        return ApiResponse.LocalizeSuccess(_localizationService, "Common_Success");
    }
}