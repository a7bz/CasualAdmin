namespace CasualAdmin.Application.Interfaces.Base;
using global::System.Data;
using global::System.Linq.Expressions;

/// <summary>
/// 数据库上下文接口，抽象数据库操作
/// </summary>
public interface IDbContext : IDisposable
{
    /// <summary>
    /// 当前租户ID
    /// </summary>
    Guid? CurrentTenantId { get; set; }

    /// <summary>
    /// 开始事务
    /// </summary>
    /// <param name="isolationLevel">事务隔离级别</param>
    void BeginTran(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

    /// <summary>
    /// 提交事务
    /// </summary>
    Task CommitTranAsync();

    /// <summary>
    /// 回滚事务
    /// </summary>
    void RollbackTran();

    /// <summary>
    /// 获取查询对象
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>查询对象</returns>
    dynamic Queryable<TEntity>() where TEntity : class, new();

    /// <summary>
    /// 插入实体
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <returns>受影响的行数</returns>
    Task<int> InsertAsync<TEntity>(TEntity entity) where TEntity : class, new();

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <returns>受影响的行数</returns>
    Task<int> UpdateAsync<TEntity>(TEntity entity) where TEntity : class, new();

    /// <summary>
    /// 删除实体
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <returns>受影响的行数</returns>
    Task<int> DeleteAsync<TEntity>(TEntity entity) where TEntity : class, new();

    /// <summary>
    /// 批量插入实体
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="entities">实体列表</param>
    /// <returns>受影响的行数</returns>
    Task<int> InsertRangeAsync<TEntity>(List<TEntity> entities) where TEntity : class, new();

    /// <summary>
    /// 批量更新实体
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="entities">实体列表</param>
    /// <returns>受影响的行数</returns>
    Task<int> UpdateRangeAsync<TEntity>(List<TEntity> entities) where TEntity : class, new();

    /// <summary>
    /// 按条件删除实体
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="predicate">删除条件</param>
    /// <returns>受影响的行数</returns>
    Task<int> DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, new();
}