namespace CasualAdmin.Domain.Entities.System;
using CasualAdmin.Domain.Common;
using SqlSugar;

/// <summary>
/// 字典实体
/// </summary>
[SugarTable("sys_dicts")]
public class SysDict : BaseEntity
{
    /// <summary>
    /// 字典ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
    public Guid DictId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 字典名称
    /// </summary>
    [SugarColumn(Length = 50)]
    public string DictName { get; set; } = string.Empty;

    /// <summary>
    /// 字典编码
    /// </summary>
    [SugarColumn(Length = 50)]
    public string DictCode { get; set; } = string.Empty;

    /// <summary>
    /// 状态
    /// </summary>
    public Status Status { get; set; } = Status.Enabled;

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
    /// 字典项列表
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysDictItem> DictItems { get; set; } = [];
}