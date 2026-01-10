namespace CasualAdmin.Application.Interfaces.Base;
using global::System.Data;

/// <summary>
/// 工作单元接口，用于管理事务和协调多个仓储
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// 获取指定类型的仓储
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>仓储实例</returns>
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, new();

    /// <summary>
    /// 开始事务
    /// </summary>
    /// <param name="isolationLevel">事务隔离级别</param>
    void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

    /// <summary>
    /// 提交事务
    /// </summary>
    Task CommitAsync();

    /// <summary>
    /// 回滚事务
    /// </summary>
    void Rollback();
}