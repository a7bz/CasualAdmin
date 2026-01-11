namespace CasualAdmin.Domain.Common;

/// <summary>
/// 通用状态枚举
/// </summary>
public enum Status
{
    /// <summary>
    /// 禁用
    /// </summary>
    Disabled = 0,
    /// <summary>
    /// 启用
    /// </summary>
    Enabled = 1
}

/// <summary>
/// 性别枚举
/// </summary>
public enum Gender
{
    /// <summary>
    /// 未知
    /// </summary>
    Unknown = 0,
    /// <summary>
    /// 男
    /// </summary>
    Male = 1,
    /// <summary>
    /// 女
    /// </summary>
    Female = 2
}

/// <summary>
/// 权限类型枚举
/// </summary>
public enum PermissionType
{
    /// <summary>
    /// 菜单权限
    /// </summary>
    Menu = 0,
    /// <summary>
    /// 功能权限
    /// </summary>
    Function = 1
}
