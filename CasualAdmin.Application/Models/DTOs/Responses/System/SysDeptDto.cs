namespace CasualAdmin.Application.Models.DTOs.Responses.System;
using CasualAdmin.Application.Models.DTOs;
using CasualAdmin.Domain.Common;

/// <summary>
/// 部门DTO
/// </summary>
public class SysDeptDto : BaseDto
{
    /// <summary>
    /// 部门ID
    /// </summary>
    public Guid? DeptId { get; set; }

    /// <summary>
    /// 父部门ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string? DeptName { get; set; }

    /// <summary>
    /// 部门编码
    /// </summary>
    public string? DeptCode { get; set; }

    /// <summary>
    /// 部门负责人
    /// </summary>
    public string? Leader { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int? Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public Status? Status { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}