namespace CasualAdmin.Application.Models.DTOs.Requests.System;

/// <summary>
/// 创建字典项DTO
/// </summary>
public class SysDictItemCreateDto
{
    /// <summary>
    /// 字典ID
    /// </summary>
    public Guid DictId { get; set; }

    /// <summary>
    /// 字典项标签
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// 字典项值
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 状态：0-禁用，1-启用
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; } = string.Empty;
}