namespace CasualAdmin.Application.Models.DTOs;

/// <summary>
/// 基础DTO类，包含通用字段
/// </summary>
public abstract class BaseDto
{
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    public bool IsDeleted { get; set; }
}

/// <summary>
/// 树状结构基础DTO
/// </summary>
public abstract class BaseTreeDto<T> : BaseDto
    where T : BaseTreeDto<T>
{
    /// <summary>
    /// 父ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 子节点列表
    /// </summary>
    public List<T> Children { get; set; } = [];
}
