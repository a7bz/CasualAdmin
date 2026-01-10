namespace CasualAdmin.Domain.Attributes;

/// <summary>
/// 实体配置属性，用于标记实体是否支持软删除和多租户
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class EntityConfigAttribute : Attribute
{
    /// <summary>
    /// 是否支持软删除
    /// </summary>
    public bool EnableSoftDelete { get; set; } = true;

    /// <summary>
    /// 是否支持多租户
    /// </summary>
    public bool EnableMultiTenancy { get; set; } = true;
}