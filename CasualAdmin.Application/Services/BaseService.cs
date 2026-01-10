namespace CasualAdmin.Application.Services;
using CasualAdmin.Application.Interfaces.Base;
using CasualAdmin.Application.Interfaces.Events;
using CasualAdmin.Application.Interfaces.Services;
using global::System.Linq.Expressions;

/// <summary>
/// 基础服务类，实现IBaseService接口，专注于业务逻辑处理
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
public abstract class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class, new()
{
    protected readonly IRepository<TEntity> _repository;
    protected readonly IValidationService _validationService;
    protected readonly IEventBus _eventBus;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="repository">仓储实例</param>
    /// <param name="validationService">验证服务实例</param>
    /// <param name="eventBus">事件总线实例</param>
    protected BaseService(IRepository<TEntity> repository, IValidationService validationService, IEventBus eventBus)
    {
        _repository = repository;
        _validationService = validationService;
        _eventBus = eventBus;
    }

    /// <summary>
    /// 获取所有实体
    /// </summary>
    /// <returns>实体列表</returns>
    public async Task<List<TEntity>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <returns>实体对象，不存在则返回null</returns>
    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    /// <summary>
    /// 根据条件查询实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns>实体列表</returns>
    public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _repository.FindAsync(predicate);
    }

    /// <summary>
    /// 根据条件查询单个实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns>实体对象，不存在则返回null</returns>
    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _repository.FirstOrDefaultAsync(predicate);
    }

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <param name="page">页码，从1开始</param>
    /// <param name="pageSize">每页大小</param>
    /// <returns>实体列表和总记录数</returns>
    public async Task<(List<TEntity>, int)> GetPagedAsync(Expression<Func<TEntity, bool>> predicate, int page, int pageSize)
    {
        return await _repository.GetPagedAsync(predicate, page, pageSize);
    }

    /// <summary>
    /// 按条件计数
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns>符合条件的记录数</returns>
    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _repository.CountAsync(predicate);
    }

    /// <summary>
    /// 按条件判断是否存在
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns>是否存在符合条件的记录</returns>
    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _repository.AnyAsync(predicate);
    }

    /// <summary>
    /// 创建实体（包含业务逻辑验证和处理）
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>创建的实体对象，创建失败则返回null</returns>
    public async Task<TEntity?> CreateAsync(TEntity entity)
    {
        // 业务验证
        if (!ValidateEntity(entity))
        {
            throw new ArgumentException("实体数据无效");
        }

        // 创建前业务逻辑处理
        var processedEntity = await OnBeforeCreateAsync(entity);

        // 调用仓储保存数据
        var createdEntity = await _repository.AddAsync(processedEntity);

        // 创建后业务逻辑处理
        OnAfterCreate(createdEntity);

        return createdEntity;
    }

    /// <summary>
    /// 更新实体（包含业务逻辑验证和处理）
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>更新的实体对象</returns>
    public async Task<TEntity?> UpdateAsync(TEntity entity)
    {
        // 业务验证
        if (!ValidateEntity(entity))
        {
            throw new ArgumentException("实体数据无效");
        }

        // 更新前业务逻辑处理
        var processedEntity = await OnBeforeUpdateAsync(entity);

        // 调用仓储更新数据
        var updatedEntity = await _repository.UpdateAsync(processedEntity);

        // 更新后业务逻辑处理
        if (updatedEntity != null)
        {
            OnAfterUpdate(updatedEntity);
        }

        return updatedEntity;
    }

    /// <summary>
    /// 删除实体（包含业务逻辑验证和处理）
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <returns>删除结果</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        // 删除前业务逻辑处理
        OnBeforeDelete(id);

        // 调用仓储删除数据
        var result = await _repository.DeleteAsync(id);

        // 删除后业务逻辑处理
        if (result)
        {
            OnAfterDelete(id);
        }

        return result;
    }

    /// <summary>
    /// 验证实体数据是否有效
    /// 默认实现，使用FluentValidation进行验证
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>是否有效</returns>
    protected virtual bool ValidateEntity(TEntity entity)
    {
        // 使用验证服务进行验证
        _validationService.ValidateAndThrow(entity);
        return true;
    }

    /// <summary>
    /// 处理实体创建前的业务逻辑
    /// 默认实现，子类可以重写
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>处理后的实体对象</returns>
    protected virtual TEntity OnBeforeCreate(TEntity entity)
    {
        // 默认实现：直接返回实体，不做任何处理
        return entity;
    }

    /// <summary>
    /// 异步处理实体创建前的业务逻辑
    /// 默认实现，子类可以重写
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>处理后的实体对象</returns>
    protected virtual Task<TEntity> OnBeforeCreateAsync(TEntity entity)
    {
        // 默认实现：调用同步版本的方法，返回已完成的任务
        return Task.FromResult(OnBeforeCreate(entity));
    }

    /// <summary>
    /// 处理实体创建后的业务逻辑
    /// 默认实现，子类可以重写
    /// </summary>
    /// <param name="entity">创建的实体对象</param>
    protected virtual void OnAfterCreate(TEntity entity)
    {
        // 默认实现：不做任何处理
    }

    /// <summary>
    /// 处理实体更新前的业务逻辑
    /// 默认实现，子类可以重写
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>处理后的实体对象</returns>
    protected virtual TEntity OnBeforeUpdate(TEntity entity)
    {
        // 默认实现：直接返回实体，不做任何处理
        return entity;
    }

    /// <summary>
    /// 异步处理实体更新前的业务逻辑
    /// 默认实现，子类可以重写
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>处理后的实体对象</returns>
    protected virtual Task<TEntity> OnBeforeUpdateAsync(TEntity entity)
    {
        // 默认实现：调用同步版本的方法，返回已完成的任务
        return Task.FromResult(OnBeforeUpdate(entity));
    }

    /// <summary>
    /// 处理实体更新后的业务逻辑
    /// 默认实现，子类可以重写
    /// </summary>
    /// <param name="entity">更新后的实体对象</param>
    protected virtual void OnAfterUpdate(TEntity entity)
    {
        // 默认实现：不做任何处理
    }

    /// <summary>
    /// 处理实体删除前的业务逻辑
    /// 默认实现，子类可以重写
    /// </summary>
    /// <param name="id">实体ID</param>
    protected virtual void OnBeforeDelete(Guid id)
    {
        // 默认实现：不做任何处理
    }

    /// <summary>
    /// 处理实体删除后的业务逻辑
    /// 默认实现，子类可以重写
    /// </summary>
    /// <param name="id">实体ID</param>
    protected virtual void OnAfterDelete(Guid id)
    {
        // 默认实现：不做任何处理
    }
}