namespace CasualAdmin.API.Filters;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// 权限验证特性
/// 用于标记需要特定权限才能访问的API接口
/// 权限格式：模块:功能:操作（如 system:user:add）
/// 支持通配符：* 表示任意层级或任意值
/// </summary>
/// <example>
/// [RequirePermission("system:user:add")]
/// [RequirePermission("system:user:*")]
/// [RequirePermission("system:*:*")]
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : Attribute, IAuthorizationRequirement
{
    /// <summary>
    /// 需要的权限编码
    /// </summary>
    public string Permission { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="permission">权限编码</param>
    public RequirePermissionAttribute(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
        {
            throw new ArgumentException("权限编码不能为空", nameof(permission));
        }

        Permission = permission;
    }
}

/// <summary>
/// 多权限验证特性（满足任意一个即可）
/// </summary>
/// <example>
/// [RequireAnyPermission("system:user:add", "system:user:edit")]
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireAnyPermissionAttribute : Attribute, IAuthorizationRequirement
{
    /// <summary>
    /// 需要的权限编码列表（满足任意一个即可）
    /// </summary>
    public string[] Permissions { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="permissions">权限编码列表</param>
    public RequireAnyPermissionAttribute(params string[] permissions)
    {
        if (permissions == null || permissions.Length == 0)
        {
            throw new ArgumentException("权限编码列表不能为空", nameof(permissions));
        }

        Permissions = permissions;
    }
}

/// <summary>
/// 多权限验证特性（需要满足所有权限）
/// </summary>
/// <example>
/// [RequireAllPermissions("system:user:view", "system:dept:view")]
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireAllPermissionsAttribute : Attribute, IAuthorizationRequirement
{
    /// <summary>
    /// 需要的权限编码列表（需要满足所有）
    /// </summary>
    public string[] Permissions { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="permissions">权限编码列表</param>
    public RequireAllPermissionsAttribute(params string[] permissions)
    {
        if (permissions == null || permissions.Length == 0)
        {
            throw new ArgumentException("权限编码列表不能为空", nameof(permissions));
        }

        Permissions = permissions;
    }
}