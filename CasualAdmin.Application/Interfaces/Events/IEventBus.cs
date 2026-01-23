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
    /// <returns>任务</returns>
    Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;

    /// <summary>
    /// 发布多个领域事件
    /// </summary>
    /// <param name="domainEvents">领域事件列表</param>
    /// <returns>任务</returns>
    Task PublishAsync(IEnumerable<IDomainEvent> domainEvents);

    /// <summary>
    /// 重播所有事件
    /// </summary>
    /// <returns>任务</returns>
    Task ReplayAllEventsAsync();

    /// <summary>
    /// 根据事件类型重播事件
    /// </summary>
    /// <typeparam name="TDomainEvent">事件类型</typeparam>
    /// <returns>任务</returns>
    Task ReplayEventsByTypeAsync<TDomainEvent>() where TDomainEvent : IDomainEvent;
}
