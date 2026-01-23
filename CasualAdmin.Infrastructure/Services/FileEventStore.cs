namespace CasualAdmin.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CasualAdmin.Application.Interfaces.Events;
using CasualAdmin.Domain.Events;

/// <summary>
/// 文件事件存储实现
/// </summary>
public class FileEventStore : IEventStore
{
    private readonly string _storagePath;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="storagePath">存储路径</param>
    public FileEventStore(string storagePath = "event-store")
    {
        _storagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, storagePath);
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            IgnoreNullValues = false
        };

        // 确保存储目录存在
        Directory.CreateDirectory(_storagePath);
    }

    /// <summary>
    /// 存储领域事件
    /// </summary>
    /// <param name="domainEvent">领域事件</param>
    /// <returns>任务</returns>
    public async Task StoreAsync(IDomainEvent domainEvent)
    {
        await StoreAsync(new List<IDomainEvent> { domainEvent });
    }

    /// <summary>
    /// 批量存储领域事件
    /// </summary>
    /// <param name="domainEvents">领域事件列表</param>
    /// <returns>任务</returns>
    public async Task StoreAsync(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            var eventType = domainEvent.GetType().FullName;
            var eventId = domainEvent.EventId;
            var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{eventId}_{eventType!.Replace(".", "_")}.json";
            var filePath = Path.Combine(_storagePath, fileName);

            var eventData = new EventData
            {
                EventId = domainEvent.EventId,
                EventType = eventType,
                OccurredOn = domainEvent.OccurredOn,
                EventDataJson = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), _jsonOptions)
            };

            await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(eventData, _jsonOptions));
        }
    }

    /// <summary>
    /// 获取所有事件
    /// </summary>
    /// <returns>事件列表</returns>
    public async Task<IEnumerable<IDomainEvent>> GetAllEventsAsync()
    {
        var events = new List<IDomainEvent>();
        var files = Directory.GetFiles(_storagePath, "*.json");

        foreach (var file in files)
        {
            try
            {
                var json = await File.ReadAllTextAsync(file);
                var eventData = JsonSerializer.Deserialize<EventData>(json, _jsonOptions);
                if (eventData != null)
                {
                    var eventType = Type.GetType(eventData.EventType);
                    if (eventType != null)
                    {
                        var domainEvent = JsonSerializer.Deserialize(eventData.EventDataJson, eventType, _jsonOptions) as IDomainEvent;
                        if (domainEvent != null)
                        {
                            events.Add(domainEvent);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading event file {file}: {ex.Message}");
            }
        }

        return events;
    }

    /// <summary>
    /// 根据事件类型获取事件
    /// </summary>
    /// <typeparam name="TDomainEvent">事件类型</typeparam>
    /// <returns>事件列表</returns>
    public async Task<IEnumerable<TDomainEvent>> GetEventsByTypeAsync<TDomainEvent>() where TDomainEvent : IDomainEvent
    {
        var allEvents = await GetAllEventsAsync();
        return allEvents.OfType<TDomainEvent>();
    }

    /// <summary>
    /// 根据时间范围获取事件
    /// </summary>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <returns>事件列表</returns>
    public async Task<IEnumerable<IDomainEvent>> GetEventsByTimeRangeAsync(DateTime startTime, DateTime endTime)
    {
        var allEvents = await GetAllEventsAsync();
        return allEvents.Where(e => e.OccurredOn >= startTime && e.OccurredOn <= endTime);
    }

    /// <summary>
    /// 事件数据结构
    /// </summary>
    private class EventData
    {
        /// <summary>
        /// 事件ID
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// 事件类型
        /// </summary>
        public string EventType { get; set; } = string.Empty;

        /// <summary>
        /// 事件发生时间
        /// </summary>
        public DateTime OccurredOn { get; set; }

        /// <summary>
        /// 事件数据JSON
        /// </summary>
        public string EventDataJson { get; set; } = string.Empty;
    }


}
