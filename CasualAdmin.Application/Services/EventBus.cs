namespace CasualAdmin.Application.Services;
using CasualAdmin.Application.Interfaces.Events;
using CasualAdmin.Domain.Events;
using global::System.Reflection;

/// <summary>
/// 事件总线实现，用于发布和处理领域事件
/// </summary>
public class EventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, List<Type>> _eventHandlers = new();

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serviceProvider">服务提供器，用于解析事件处理器</param>
    public EventBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        // 扫描并注册所有事件处理器
        RegisterEventHandlers();
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
            var eventType = domainEvent.GetType();
            if (_eventHandlers.TryGetValue(eventType, out var handlerTypes))
            {
                foreach (var handlerType in handlerTypes)
                {
                    // 创建事件处理器实例
                    var handler = _serviceProvider.GetService(handlerType) as IDomainEventHandler<IDomainEvent>;
                    if (handler != null)
                    {
                        // 调用事件处理器
                        await handler.HandleAsync(domainEvent);
                    }
                }
            }
        }
    }
}