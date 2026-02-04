namespace CasualAdmin.Infrastructure.Data;

using System.Data;
using CasualAdmin.Domain.Infrastructure.Data;
using CasualAdmin.Infrastructure.Data.Context;
using CasualAdmin.Infrastructure.Data.Repositories;

/// <summary>
/// 工作单元实现，用于管理事务和协调多个仓储
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly SqlSugarContext _context;
    private readonly Dictionary<Type, object> _repositories = new();
    private bool _disposed;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="context">SqlSugar上下文</param>
    public UnitOfWork(SqlSugarContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取指定类型的仓储
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>仓储实例</returns>
    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, new()
    {
        var entityType = typeof(TEntity);
        if (!_repositories.TryGetValue(entityType, out var repository))
        {
            repository = new Repository<TEntity>(_context);
            _repositories[entityType] = repository;
        }

        return (IRepository<TEntity>)repository;
    }

    /// <summary>
    /// 开始事务
    /// </summary>
    /// <param name="isolationLevel">事务隔离级别</param>
    public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        _context.BeginTran(isolationLevel);
    }

    /// <summary>
    /// 提交事务
    /// </summary>
    public async Task CommitAsync()
    {
        await _context.CommitTranAsync();
    }

    /// <summary>
    /// 回滚事务
    /// </summary>
    public void Rollback()
    {
        _context.RollbackTran();
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="disposing">是否手动释放</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // 释放托管资源
                _repositories.Clear();
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}