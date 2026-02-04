namespace CasualAdmin.Application.Services;
using CasualAdmin.Application.Interfaces.Events;
using CasualAdmin.Domain.Events;
using global::System;
using global::System.Reflection;
using global::System.Threading;
using global::System.Threading.Channels;
using global::System.Threading.Tasks;

/// <summary>
/// 事件总线实现，用于发布和处理领域事件
/// 使用 Channel 替代轮询机制，提高性能并减少CPU占用
/// </summary>
public class EventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, List<Type>> _eventHandlers = new();
    private readonly Channel<IDomainEvent> _eventChannel;
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _processingTask;
    private readonly IEventStore _eventStore;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serviceProvider">服务提供器，用于解析事件处理器</param>
    /// <param name="eventStore">事件存储服务</param>
    public EventBus(IServiceProvider serviceProvider, IEventStore eventStore)
    {
        _serviceProvider = serviceProvider;
        _eventStore = eventStore;

        // 创建无界 Channel，支持高并发写入
        _eventChannel = Channel.CreateUnbounded<IDomainEvent>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

        // 扫描并注册所有事件处理器
        RegisterEventHandlers();

        // 启动事件处理后台任务
        _processingTask = Task.Run(() => ProcessEventQueueAsync(_cts.Token), _cts.Token);
    }

    /// <summary>
    /// 扫描并注册所有事件处理器
    /// </summary>
    private void RegisterEventHandlers()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName != null && a.FullName.Contains("CasualAdmin"));

        foreach (var assembly in assemblies)
        {
            var handlerTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)));

            foreach (var handlerType in handlerTypes)
            {
                // 获取事件处理器处理的事件类型
                var eventType = handlerType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>))
                    .GetGenericArguments()[0];

                if (!_eventHandlers.TryGetValue(eventType, out var handlers))
                {
                    handlers = new List<Type>();
                    _eventHandlers[eventType] = handlers;
                }

                handlers.Add(handlerType);
            }
        }
    }

    /// <summary>
    /// 发布领域事件
    /// </summary>
    /// <typeparam name="TDomainEvent">领域事件类型</typeparam>
    /// <param name="domainEvent">领域事件实例</param>
    public async Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
    {
        await PublishAsync(new List<IDomainEvent> { domainEvent });
    }

    /// <summary>
    /// 发布多个领域事件
    /// </summary>
    /// <param name="domainEvents">领域事件列表</param>
    public async Task PublishAsync(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            // 使用 Channel 写入事件，异步处理
            await _eventChannel.Writer.WriteAsync(domainEvent, _cts.Token);
        }
    }

    /// <summary>
    /// 处理事件队列
    /// 使用 Channel 的异步读取机制，替代轮询
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    private async Task ProcessEventQueueAsync(CancellationToken cancellationToken)
    {
        try
        {
            // 使用 Channel.Reader.WaitToReadAsync 替代轮询
            // 当有新事件可用时自动唤醒，无需持续轮询
            await foreach (var domainEvent in _eventChannel.Reader.ReadAllAsync(cancellationToken))
            {
                await ProcessEventAsync(domainEvent);
            }
        }
        catch (OperationCanceledException)
        {
            // 任务被取消，正常退出
        }
        catch (Exception ex)
        {
            // 记录错误
            Console.WriteLine($"Error in event queue processing: {ex.Message}");
        }
    }

    /// <summary>
    /// 处理单个事件
    /// </summary>
    /// <param name="domainEvent">领域事件</param>
    /// <returns>任务</returns>
    private async Task ProcessEventAsync(IDomainEvent domainEvent)
    {
        try
        {
            // 先存储事件，确保事件不丢失
            await _eventStore.StoreAsync(domainEvent);

            // 然后处理事件
            var eventType = domainEvent.GetType();
            if (_eventHandlers.TryGetValue(eventType, out var handlerTypes))
            {
                foreach (var handlerType in handlerTypes)
                {
                    try
                    {
                        // 创建事件处理器实例
                        var handler = _serviceProvider.GetService(handlerType) as IDomainEventHandler<IDomainEvent>;
                        if (handler != null)
                        {
                            // 调用事件处理器
                            await handler.HandleAsync(domainEvent);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 记录错误但继续处理其他处理器
                        Console.WriteLine($"Error handling event with {handlerType.Name}: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // 记录存储事件时的错误
            Console.WriteLine($"Error storing event: {ex.Message}");
        }
    }

    /// <summary>
    /// 重播所有事件
    /// </summary>
    /// <returns>任务</returns>
    public async Task ReplayAllEventsAsync()
    {
        var events = await _eventStore.GetAllEventsAsync();
        foreach (var domainEvent in events)
        {
            // 直接处理事件，不重新存储
            var eventType = domainEvent.GetType();
            if (_eventHandlers.TryGetValue(eventType, out var handlerTypes))
            {
                foreach (var handlerType in handlerTypes)
                {
                    try
                    {
                        var handler = _serviceProvider.GetService(handlerType) as IDomainEventHandler<IDomainEvent>;
                        if (handler != null)
                        {
                            await handler.HandleAsync(domainEvent);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error replaying event with {handlerType.Name}: {ex.Message}");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 根据事件类型重播事件
    /// </summary>
    /// <typeparam name="TDomainEvent">事件类型</typeparam>
    /// <returns>任务</returns>
    public async Task ReplayEventsByTypeAsync<TDomainEvent>() where TDomainEvent : IDomainEvent
    {
        var events = await _eventStore.GetEventsByTypeAsync<TDomainEvent>();
        foreach (var domainEvent in events)
        {
            // 直接处理事件，不重新存储
            var eventType = domainEvent.GetType();
            if (_eventHandlers.TryGetValue(eventType, out var handlerTypes))
            {
                foreach (var handlerType in handlerTypes)
                {
                    try
                    {
                        var handler = _serviceProvider.GetService(handlerType) as IDomainEventHandler<IDomainEvent>;
                        if (handler != null)
                        {
                            await handler.HandleAsync(domainEvent);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error replaying event with {handlerType.Name}: {ex.Message}");
                    }
                }
            }
        }
    }
}