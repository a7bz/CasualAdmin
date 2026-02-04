namespace CasualAdmin.Shared.Helpers;

/// <summary>
/// 权限帮助类，提供权限验证和通配符匹配功能
/// 权限格式：模块:功能:操作（如 system:user:add）
/// 支持通配符：* 表示任意层级或任意值
/// </summary>
public static class PermissionHelper
{
    /// <summary>
    /// 验证用户是否拥有指定权限
    /// </summary>
    /// <param name="userPermissions">用户拥有的权限列表</param>
    /// <param name="requiredPermission">需要的权限</param>
    /// <returns>true表示有权限，false表示无权限</returns>
    public static bool HasPermission(IEnumerable<string> userPermissions, string requiredPermission)
    {
        if (userPermissions == null || !userPermissions.Any())
        {
            return false;
        }

        // 超级管理员权限
        if (userPermissions.Contains("*:*:*"))
        {
            return true;
        }

        // 精确匹配
        if (userPermissions.Contains(requiredPermission))
        {
            return true;
        }

        // 通配符匹配
        return userPermissions.Any(userPerm => MatchPermission(userPerm, requiredPermission));
    }

    /// <summary>
    /// 检查权限是否匹配（支持通配符）
    /// </summary>
    /// <param name="pattern">权限模式，可能包含通配符</param>
    /// <param name="permission">要匹配的权限</param>
    /// <returns>true表示匹配，false表示不匹配</returns>
    public static bool MatchPermission(string pattern, string permission)
    {
        if (string.IsNullOrEmpty(pattern) || string.IsNullOrEmpty(permission))
        {
            return false;
        }

        // 完全匹配
        if (pattern.Equals(permission, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var patternParts = pattern.Split(':');
        var permissionParts = permission.Split(':');

        // 层级必须相同
        if (patternParts.Length != permissionParts.Length)
        {
            return false;
        }

        // 逐层匹配
        for (int i = 0; i < patternParts.Length; i++)
        {
            var patternPart = patternParts[i];
            var permissionPart = permissionParts[i];

            // 通配符匹配任意值
            if (patternPart != "*" && !patternPart.Equals(permissionPart, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 解析权限编码，获取模块、功能、操作
    /// </summary>
    /// <param name="permissionCode">权限编码</param>
    /// <returns>元组 (module, feature, action)</returns>
    public static (string module, string feature, string action) ParsePermissionCode(string permissionCode)
    {
        if (string.IsNullOrEmpty(permissionCode))
        {
            return ("", "", "");
        }

        var parts = permissionCode.Split(':');

        return parts.Length switch
        {
            1 => (parts[0], "", ""),
            2 => (parts[0], parts[1], ""),
            >= 3 => (parts[0], parts[1], parts[2]),
            _ => ("", "", "")
        };
    }

    /// <summary>
    /// 生成权限编码
    /// </summary>
    /// <param name="module">模块</param>
    /// <param name="feature">功能</param>
    /// <param name="action">操作</param>
    /// <returns>权限编码</returns>
    public static string GeneratePermissionCode(string module, string feature = "*", string action = "*")
    {
        if (string.IsNullOrEmpty(module))
        {
            return "*:*:*";
        }

        return string.IsNullOrEmpty(feature)
            ? $"{module}:*:*"
            : string.IsNullOrEmpty(action)
                ? $"{module}:{feature}:*"
                : $"{module}:{feature}:{action}";
    }

    /// <summary>
    /// 获取用户在某模块下的所有权限
    /// </summary>
    /// <param name="userPermissions">用户权限列表</param>
    /// <param name="module">模块名称</param>
    /// <returns>该模块下的权限列表</returns>
    public static List<string> GetModulePermissions(IEnumerable<string> userPermissions, string module)
    {
        if (userPermissions == null || !userPermissions.Any())
        {
            return new List<string>();
        }

        var modulePrefix = $"{module}:";
        return userPermissions
            .Where(p => p.StartsWith(modulePrefix, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// 检查用户是否有模块的任意权限
    /// </summary>
    /// <param name="userPermissions">用户权限列表</param>
    /// <param name="module">模块名称</param>
    /// <returns>true表示有权限，false表示无权限</returns>
    public static bool HasAnyModulePermission(IEnumerable<string> userPermissions, string module)
    {
        if (userPermissions == null || !userPermissions.Any())
        {
            return false;
        }

        // 检查是否有模块级别的通配符权限
        var moduleWildcard = $"{module}:*:*";
        if (userPermissions.Contains(moduleWildcard, StringComparer.OrdinalIgnoreCase))
        {
            return true;
        }

        // 检查是否有该模块下的具体权限
        return userPermissions.Any(p => p.StartsWith($"{module}:", StringComparison.OrdinalIgnoreCase));
    }
}