namespace CasualAdmin.Application.Models.DTOs.Responses.System;

/// <summary>
/// 字典项基本信息DTO
/// </summary>
public class SysDictItemDto
{
    /// <summary>
    /// 字典项ID
    /// </summary>
    public Guid? DictItemId { get; set; }

    /// <summary>
    /// 字典ID
    /// </summary>
    public Guid? DictId { get; set; }

    /// <summary>
    /// 字典项标签
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// 字典项值
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// 状态：0-禁用，1-启用
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int? Sort { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}