namespace CasualAdmin.Application.Models.DTOs.Requests.System;

/// <summary>
/// 更新角色DTO
/// </summary>
public class SysRoleUpdateDto
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色代码
    /// </summary>
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// 角色类型：0-系统角色，1-租户角色
    /// </summary>
    public int RoleType { get; set; } = 1;

    /// <summary>
    /// 角色描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 状态：0-禁用，1-启用
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; } = 0;
}
