namespace CasualAdmin.Domain.Entities.System;
using SqlSugar;

/// <summary>
/// 字典项实体
/// </summary>
[SugarTable("sys_dict_items")]
public class SysDictItem : BaseEntity
{
    /// <summary>
    /// 字典项ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
    public Guid DictItemId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 字典ID
    /// </summary>
    public Guid DictId { get; set; }

    /// <summary>
    /// 字典项标签
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// 字典项值
    /// </summary>
    [SugarColumn(Length = 50)]
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
    [SugarColumn(Length = 200)]
    public string Remark { get; set; } = string.Empty;

    /// <summary>
    /// 所属字典
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public SysDict? Dict { get; set; }
}