namespace CasualAdmin.Domain.Infrastructure.Events;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CasualAdmin.Domain.Events;

/// <summary>
/// 事件存储接口，用于持久化存储领域事件
/// </summary>
public interface IEventStore
{
    /// <summary>
    /// 存储领域事件
    /// </summary>
    /// <param name="domainEvent">领域事件</param>
    /// <returns>任务</returns>
    Task StoreAsync(IDomainEvent domainEvent);

    /// <summary>
    /// 批量存储领域事件
    /// </summary>
    /// <param name="domainEvents">领域事件列表</param>
    /// <returns>任务</returns>
    Task StoreAsync(IEnumerable<IDomainEvent> domainEvents);

    /// <summary>
    /// 获取所有事件
    /// </summary>
    /// <returns>事件列表</returns>
    Task<IEnumerable<IDomainEvent>> GetAllEventsAsync();

    /// <summary>
    /// 根据事件类型获取事件
    /// </summary>
    /// <typeparam name="TDomainEvent">事件类型</typeparam>
    /// <returns>事件列表</returns>
    Task<IEnumerable<TDomainEvent>> GetEventsByTypeAsync<TDomainEvent>() where TDomainEvent : IDomainEvent;

    /// <summary>
    /// 根据时间范围获取事件
    /// </summary>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <returns>事件列表</returns>
    Task<IEnumerable<IDomainEvent>> GetEventsByTimeRangeAsync(DateTime startTime, DateTime endTime);
}