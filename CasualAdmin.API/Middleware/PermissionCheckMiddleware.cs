namespace CasualAdmin.API.Middleware;
using System.Security.Claims;
using System.Text.Json;
using CasualAdmin.API.Configurations;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Shared.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 权限检查中间件
/// </summary>
public class PermissionCheckMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AuthorizationExcludePaths _authorizationExcludePaths;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="next">下一个中间件</param>
    /// <param name="authorizationExcludePaths">授权排除路径配置</param>
    public PermissionCheckMiddleware(RequestDelegate next, AuthorizationExcludePaths authorizationExcludePaths)
    {
        _next = next;
        _authorizationExcludePaths = authorizationExcludePaths;
    }

    /// <summary>
    /// 执行中间件
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <returns>任务</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // 跳过授权排除路径
        if (_authorizationExcludePaths.ShouldSkipAuthorization(context.Request.Path))
        {
            await _next(context);
            return;
        }

        // 检查是否有AllowAnonymous属性
        var endpoint = context.GetEndpoint();
        if (endpoint != null)
        {
            var allowAnonymous = endpoint.Metadata.GetMetadata<IAllowAnonymous>();
            if (allowAnonymous != null)
            {
                await _next(context);
                return;
            }
        }

        // 检查用户是否已认证
        if (context.User == null || context.User.Identity == null || !context.User.Identity.IsAuthenticated)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var response = ApiResponse<object>.Unauthorized("用户未认证");
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            return;
        }

        try
        {
            // 获取用户ID
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier) ??
                             context.User.FindFirst("sub");

            if (userIdClaim == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var response = ApiResponse<object>.Unauthorized("Token中不包含用户ID");
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                return;
            }

            var userId = Guid.Parse(userIdClaim.Value);

            // 获取权限服务
            var permissionService = context.RequestServices.GetRequiredService<IPermissionService>();
            var userService = context.RequestServices.GetRequiredService<IUserService>();
            var roleService = context.RequestServices.GetRequiredService<IRoleService>();

            // 获取用户角色
            var roles = await roleService.GetRolesByUserIdAsync(userId);
            if (roles.Count == 0)
            {
                // 用户没有角色，允许访问（权限控制交给具体的控制器方法）
                await _next(context);
                return;
            }

            // 获取所有角色的权限
            var permissions = new List<Domain.Entities.System.SysPermission>();
            foreach (var role in roles)
            {
                var rolePermissions = await permissionService.GetPermissionsByRoleIdAsync(role.RoleId);
                permissions.AddRange(rolePermissions);
            }

            // 提取权限代码
            var permissionCodes = permissions.Select(p => p.PermissionCode).Distinct().ToList();

            // 将权限信息存储到HttpContext中，供后续使用
            context.Items["UserPermissions"] = permissionCodes;

            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            var response = ApiResponse<object>.Failed("权限检查失败: " + ex.Message, 500);
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}

/// <summary>
/// 权限检查中间件扩展
/// </summary>
public static class PermissionCheckMiddlewareExtensions
{
    /// <summary>
    /// 使用权限检查中间件
    /// </summary>
    /// <param name="app">应用构建器</param>
    /// <returns>应用构建器</returns>
    public static IApplicationBuilder UsePermissionCheck(this IApplicationBuilder app)
    {
        return app.UseMiddleware<PermissionCheckMiddleware>();
    }
}