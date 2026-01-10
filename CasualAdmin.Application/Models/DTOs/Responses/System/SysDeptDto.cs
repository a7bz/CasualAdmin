namespace CasualAdmin.Application.Models.DTOs.Responses.System;

/// <summary>
/// 部门基本信息DTO
/// </summary>
public class SysDeptDto
{
    /// <summary>
    /// 部门ID
    /// </summary>
    public Guid DeptId { get; set; }

    /// <summary>
    /// 父部门ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string DeptName { get; set; } = string.Empty;

    /// <summary>
    /// 部门编码
    /// </summary>
    public string DeptCode { get; set; } = string.Empty;

    /// <summary>
    /// 部门负责人
    /// </summary>
    public string Leader { get; set; } = string.Empty;

    /// <summary>
    /// 联系电话
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态：0-禁用，1-启用
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; } = string.Empty;
}