namespace CasualAdmin.Application.Interfaces.Events;
using CasualAdmin.Domain.Events;

/// <summary>
/// 事件总线接口，用于发布领域事件
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// 发布领域事件
    /// </summary>
    /// <typeparam name="TDomainEvent">领域事件类型</typeparam>
    /// <param name="domainEvent">领域事件实例</param>
    Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;

    /// <summary>
    /// 发布多个领域事件
    /// </summary>
    /// <param name="domainEvents">领域事件列表</param>
    Task PublishAsync(IEnumerable<IDomainEvent> domainEvents);
}