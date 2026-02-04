namespace CasualAdmin.Application.Interfaces.Base;
using global::System.Linq.Expressions;

/// <summary>
/// 基础服务接口，定义服务层核心业务方法
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
public interface IBaseService<TEntity> where TEntity : class, new()
{
    /// <summary>
    /// 获取所有实体
    /// </summary>
    /// <returns>实体列表</returns>
    Task<List<TEntity>> GetAllAsync();

    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <returns>实体对象，不存在则返回null</returns>
    Task<TEntity?> GetByIdAsync(Guid id);

    /// <summary>
    /// 根据条件查询实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns>实体列表</returns>
    Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 根据条件查询单个实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns>实体对象，不存在则返回null</returns>
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <param name="page">页码，从1开始</param>
    /// <param name="pageSize">每页大小</param>
    /// <returns>实体列表和总记录数</returns>
    Task<(List<TEntity>, int)> GetPagedAsync(Expression<Func<TEntity, bool>> predicate, int page, int pageSize);

    /// <summary>
    /// 按条件计数
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns>符合条件的记录数</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 按条件判断是否存在
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns>是否存在符合条件的记录</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 创建实体（包含业务逻辑验证和处理）
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>创建的实体对象，创建失败则返回null</returns>
    Task<TEntity?> CreateAsync(TEntity entity);

    /// <summary>
    /// 更新实体（包含业务逻辑验证和处理）
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>更新的实体对象</returns>
    Task<TEntity?> UpdateAsync(TEntity entity);

    /// <summary>
    /// 删除实体（包含业务逻辑验证和处理）
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <returns>删除结果</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 批量删除实体（包含业务逻辑验证和处理）
    /// </summary>
    /// <param name="ids">实体ID列表</param>
    /// <returns>删除结果</returns>
    Task<bool> DeleteRangeAsync(List<Guid> ids);
}