namespace CasualAdmin.API.Middleware;

using System.Security.Claims;
using System.Text.Json;
using CasualAdmin.API.Configurations;
using CasualAdmin.API.Filters;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Domain.Infrastructure.Services;
using CasualAdmin.Shared.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

            // 从Token中直接读取角色（避免查询sys_user_roles表）
            var roleClaims = context.User.FindAll(ClaimTypes.Role);
            var roleNames = roleClaims.Select(c => c.Value).ToList();

            List<string> permissionCodes = new List<string>();

            // 优先从Token中读取权限（新Token）
            var permissionClaims = context.User.FindAll("permission");
            var tokenPermissions = permissionClaims.Select(c => c.Value).ToList();

            if (tokenPermissions.Count > 0)
            {
                // Token中包含权限，直接使用
                permissionCodes = tokenPermissions;
            }
            else
            {
                // Token中不包含权限，回退到缓存和数据库查询（兼容旧Token）
                var cacheService = context.RequestServices.GetRequiredService<ICacheService>();
                var cacheKey = $"user_permissions:{userId}:{string.Join(",", roleNames.OrderBy(r => r))}";

                // 尝试从缓存获取权限
                var cachedPermissions = await cacheService.GetAsync<List<string>>(cacheKey);
                if (cachedPermissions != null)
                {
                    permissionCodes = cachedPermissions;
                }
                else
                {
                    // 缓存未命中，查询数据库
                    var permissionService = context.RequestServices.GetRequiredService<IPermissionService>();
                    var roleService = context.RequestServices.GetRequiredService<IRoleService>();

                    // 如果Token中没有角色信息，从数据库查询（兼容旧Token）
                    if (roleNames.Count == 0)
                    {
                        var roles = await roleService.GetRolesByUserIdAsync(userId);
                        roleNames = roles.Select(r => r.Name).ToList();
                    }

                    if (roleNames.Count > 0)
                    {
                        // 根据角色名称查询角色ID，然后获取权限
                        var roleEntities = await roleService.GetRolesByNamesAsync(roleNames);
                        if (roleEntities.Count > 0)
                        {
                            // 获取所有角色的权限（批量查询，避免N+1问题）
                            var roleIds = roleEntities.Select(r => r.RoleId).ToList();
                            var permissions = await permissionService.GetPermissionsByRoleIdsAsync(roleIds);

                            // 提取权限代码
                            permissionCodes = permissions.Select(p => p.PermissionCode).Distinct().ToList();

                            // 缓存权限信息，缓存30分钟
                            await cacheService.SetAsync(cacheKey, permissionCodes, TimeSpan.FromMinutes(30));
                        }
                    }
                }
            }

            // 将权限信息存储到HttpContext中，供后续使用
            context.Items["UserPermissions"] = permissionCodes;

            // 检查endpoint是否有权限要求特性
            endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                // 检查是否有RequirePermissionAttribute（需要特定权限）
                var requiredPermission = endpoint.Metadata.GetMetadata<RequirePermissionAttribute>();
                if (requiredPermission != null)
                {
                    if (!CasualAdmin.Shared.Helpers.PermissionHelper.HasPermission(permissionCodes, requiredPermission.Permission))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        var response = ApiResponse<object>.Forbidden($"需要权限: {requiredPermission.Permission}");
                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                        return;
                    }
                }

                // 检查是否有RequireAnyPermissionAttribute（需要满足任意一个权限）
                var requireAnyPermission = endpoint.Metadata.GetMetadata<RequireAnyPermissionAttribute>();
                if (requireAnyPermission != null)
                {
                    var hasAnyPermission = requireAnyPermission.Permissions.Any(perm =>
                        CasualAdmin.Shared.Helpers.PermissionHelper.HasPermission(permissionCodes, perm));

                    if (!hasAnyPermission)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        var response = ApiResponse<object>.Forbidden($"需要以下任意权限之一: {string.Join(", ", requireAnyPermission.Permissions)}");
                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                        return;
                    }
                }

                // 检查是否有RequireAllPermissionsAttribute（需要满足所有权限）
                var requireAllPermissions = endpoint.Metadata.GetMetadata<RequireAllPermissionsAttribute>();
                if (requireAllPermissions != null)
                {
                    var hasAllPermissions = requireAllPermissions.Permissions.All(perm =>
                        CasualAdmin.Shared.Helpers.PermissionHelper.HasPermission(permissionCodes, perm));

                    if (!hasAllPermissions)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        var response = ApiResponse<object>.Forbidden($"需要以下所有权限: {string.Join(", ", requireAllPermissions.Permissions)}");
                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                        return;
                    }
                }
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<PermissionCheckMiddleware>>();
            logger.LogError(ex, "权限检查失败，请求路径: {Path}", context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            var response = ApiResponse<object>.Failed("权限检查失败，请联系管理员", 500);
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