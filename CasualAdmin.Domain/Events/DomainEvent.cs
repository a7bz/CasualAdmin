namespace CasualAdmin.Domain.Events;

/// <summary>
/// 领域事件基类，所有领域事件都可以继承此类
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    /// <summary>
    /// 构造函数
    /// </summary>
    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.Now;
    }

    /// <summary>
    /// 事件ID
    /// </summary>
    public Guid EventId { get; }

    /// <summary>
    /// 事件发生时间
    /// </summary>
    public DateTime OccurredOn { get; }
}