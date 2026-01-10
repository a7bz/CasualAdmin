namespace CasualAdmin.Domain.Events;

/// <summary>
/// 领域事件接口，所有领域事件都需要实现此接口
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// 事件ID
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// 事件发生时间
    /// </summary>
    DateTime OccurredOn { get; }
}