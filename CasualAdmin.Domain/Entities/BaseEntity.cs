namespace CasualAdmin.Domain.Entities;
using SqlSugar;

/// <summary>
/// 基础实体类，包含多租户支持
/// </summary>
[SugarIndex("idx_is_deleted", nameof(IsDeleted), OrderByType.Asc)]
[SugarIndex("idx_tenant_id", nameof(TenantId), OrderByType.Asc)]
public abstract class BaseEntity
{
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 是否删除
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// 租户ID，用于多租户隔离
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "租户ID")]
    public Guid? TenantId { get; set; }
}
