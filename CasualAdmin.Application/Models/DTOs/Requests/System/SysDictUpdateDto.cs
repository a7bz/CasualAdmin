namespace CasualAdmin.Application.Models.DTOs.Requests.System;

/// <summary>
/// 更新字典DTO
/// </summary>
public class SysDictUpdateDto
{
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