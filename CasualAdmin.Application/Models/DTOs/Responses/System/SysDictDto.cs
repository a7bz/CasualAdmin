namespace CasualAdmin.Application.Models.DTOs.Responses.System;

/// <summary>
/// 字典基本信息DTO
/// </summary>
public class SysDictDto
{
    /// <summary>
    /// 字典ID
    /// </summary>
    public Guid DictId { get; set; }

    /// <summary>
    /// 字典名称
    /// </summary>
    public string DictName { get; set; } = string.Empty;

    /// <summary>
    /// 字典编码
    /// </summary>
    public string DictCode { get; set; } = string.Empty;

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