namespace CasualAdmin.Infrastructure.Data.Repositories;
using System.Linq.Expressions;
using CasualAdmin.Application.Interfaces.Base;
using CasualAdmin.Domain.Entities;
using SqlSugar;

/// <summary>
/// 泛型仓储实现
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
{
    private readonly IDbContext _context;
    private ISugarQueryable<TEntity> _dbSet;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public Repository(IDbContext context)
    {
        _context = context;
        _dbSet = ApplyFilters(_context.Queryable<TEntity>());
    }

    /// <summary>
    /// 应用软删除和多租户过滤
    /// </summary>
    /// <param name="query">原始查询</param>
    /// <returns>应用过滤后的查询</returns>
    private ISugarQueryable<TEntity> ApplyFilters(dynamic query)
    {
        var sugarQuery = (ISugarQueryable<TEntity>)query;
        // 获取实体配置
        var entityConfig = typeof(TEntity).GetCustomAttributes(typeof(Domain.Attributes.EntityConfigAttribute), true)
            .FirstOrDefault() as Domain.Attributes.EntityConfigAttribute ?? new Domain.Attributes.EntityConfigAttribute();

        // 应用软删除过滤
        if (entityConfig.EnableSoftDelete && typeof(TEntity).GetProperty("IsDeleted") != null)
        {
            sugarQuery = sugarQuery.Where("is_deleted = @isDeleted", new { isDeleted = false });
        }

        // 应用多租户过滤
        if (entityConfig.EnableMultiTenancy && typeof(BaseEntity).IsAssignableFrom(typeof(TEntity)) && _context.CurrentTenantId.HasValue)
        {
            var tenantId = _context.CurrentTenantId.Value;
            sugarQuery = sugarQuery.Where("tenant_id = @tenantId", new { tenantId });
        }

        return sugarQuery;
    }

    /// <summary>
    /// 根据主键获取实体
    /// </summary>
    /// <param name="id">主键值</param>
    /// <returns>实体对象，不存在则返回null</returns>
    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        // 获取实体的主键属性
        var primaryKeyProperty = GetPrimaryKeyProperty() ?? throw new ArgumentException($"实体 {typeof(TEntity).Name} 没有主键属性");

        // 动态构建LINQ表达式，避免反射调用，让SqlSugar能够正确解析
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var propertyAccess = Expression.Property(parameter, primaryKeyProperty);
        var constant = Expression.Constant(id);
        var equality = Expression.Equal(propertyAccess, constant);
        var lambda = Expression.Lambda<Func<TEntity, bool>>(equality, parameter);

        // 使用构建的表达式查询
        return await _dbSet.FirstAsync(lambda);
    }

    /// <summary>
    /// 获取实体的主键属性
    /// </summary>
    /// <returns>主键属性信息，可能为null</returns>
    private System.Reflection.PropertyInfo? GetPrimaryKeyProperty()
    {
        // 尝试查找带有SugarColumn.IsPrimaryKey特性的属性（优先级最高，因为是SqlSugar专用）
        var sugarKeyProperty = typeof(TEntity).GetProperties()
            .FirstOrDefault(p =>
            {
                var sugarColumnAttrs = p.GetCustomAttributes(typeof(SqlSugar.SugarColumn), true);
                foreach (var attr in sugarColumnAttrs)
                {
                    if (attr is SqlSugar.SugarColumn sugarColumn && sugarColumn.IsPrimaryKey)
                    {
                        return true;
                    }
                }
                return false;
            });
        if (sugarKeyProperty != null)
        {
            return sugarKeyProperty;
        }

        // 尝试查找带有Key特性的属性
        var keyProperty = typeof(TEntity).GetProperties()
            .FirstOrDefault(p => p.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), true).Length != 0);
        if (keyProperty != null)
        {
            return keyProperty;
        }

        // 尝试查找Id属性（最常见的主键名称）
        var idProperty = typeof(TEntity).GetProperty("Id");
        if (idProperty != null)
        {
            return idProperty;
        }

        // 尝试查找以Id结尾的属性（比如DictItemId）
        var idEndingProperty = typeof(TEntity).GetProperties()
            .FirstOrDefault(p => p.Name.EndsWith("Id") && p.PropertyType == typeof(Guid));
        if (idEndingProperty != null)
        {
            return idEndingProperty;
        }

        // 如果都找不到，返回null
        return null;
    }

    /// <summary>
    /// 获取所有实体
    /// </summary>
    /// <returns>实体列表</returns>
    public async Task<List<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <summary>
    /// 根据条件查询实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns>实体列表</returns>
    public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    /// <summary>
    /// 根据条件查询单个实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns>实体对象，不存在则返回null</returns>
    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).FirstAsync();
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
        var query = _dbSet.Where(predicate);
        var totalCount = await query.CountAsync();
        var list = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (list, totalCount);
    }

    /// <summary>
    /// 按条件计数
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns>符合条件的记录数</returns>
    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).CountAsync();
    }

    /// <summary>
    /// 按条件判断是否存在
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns>是否存在符合条件的记录</returns>
    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).AnyAsync();
    }

    /// <summary>
    /// 添加实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>添加的实体</returns>
    public async Task<TEntity> AddAsync(TEntity entity)
    {
        // 设置创建时间和更新时间
        var now = DateTime.Now;
        var createdAtProperty = entity.GetType().GetProperty("CreatedAt");
        createdAtProperty?.SetValue(entity, now);
        var updatedAtProperty = entity.GetType().GetProperty("UpdatedAt");
        updatedAtProperty?.SetValue(entity, now);

        // 获取实体配置
        var entityConfig = typeof(TEntity).GetCustomAttributes(typeof(Domain.Attributes.EntityConfigAttribute), true)
            .FirstOrDefault() as Domain.Attributes.EntityConfigAttribute ?? new Domain.Attributes.EntityConfigAttribute();

        // 设置租户ID
        if (entityConfig.EnableMultiTenancy && typeof(BaseEntity).IsAssignableFrom(typeof(TEntity)) && _context.CurrentTenantId.HasValue)
        {
            var tenantIdProperty = entity.GetType().GetProperty("TenantId");
            tenantIdProperty?.SetValue(entity, _context.CurrentTenantId.Value);
        }

        await _context.InsertAsync(entity);
        return entity;
    }

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>更新的实体</returns>
    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        // 设置更新时间
        var updatedAtProperty = entity.GetType().GetProperty("UpdatedAt");
        updatedAtProperty?.SetValue(entity, DateTime.Now);

        await _context.UpdateAsync(entity);
        return entity;
    }

    /// <summary>
    /// 批量更新实体
    /// </summary>
    /// <param name="entities">实体列表</param>
    /// <returns>更新的实体数量</returns>
    public async Task<int> UpdateRangeAsync(List<TEntity> entities)
    {
        var now = DateTime.Now;
        foreach (var entity in entities)
        {
            // 设置更新时间
            var updatedAtProperty = entity.GetType().GetProperty("UpdatedAt");
            updatedAtProperty?.SetValue(entity, now);
        }

        return await _context.UpdateRangeAsync(entities);
    }

    /// <summary>
    /// 删除实体（软删除优先）
    /// </summary>
    /// <param name="id">主键值</param>
    /// <returns>删除结果</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        // 尝试软删除
        var entity = await GetByIdAsync(id);
        if (entity == null)
        {
            return false;
        }

        var isDeletedProperty = typeof(TEntity).GetProperty("IsDeleted");
        if (isDeletedProperty != null)
        {
            // 软删除
            isDeletedProperty.SetValue(entity, true);
            await UpdateAsync(entity);
            return true;
        }
        else
        {
            // 硬删除
            return await _context.DeleteAsync(entity) > 0;
        }
    }

    /// <summary>
    /// 批量添加实体
    /// </summary>
    /// <param name="entities">实体列表</param>
    /// <returns>添加的实体数量</returns>
    public async Task<int> AddRangeAsync(List<TEntity> entities)
    {
        var now = DateTime.Now;
        // 获取实体配置
        var entityConfig = typeof(TEntity).GetCustomAttributes(typeof(Domain.Attributes.EntityConfigAttribute), true)
            .FirstOrDefault() as Domain.Attributes.EntityConfigAttribute ?? new Domain.Attributes.EntityConfigAttribute();

        foreach (var entity in entities)
        {
            var createdAtProperty = entity.GetType().GetProperty("CreatedAt");
            createdAtProperty?.SetValue(entity, now);
            var updatedAtProperty = entity.GetType().GetProperty("UpdatedAt");
            updatedAtProperty?.SetValue(entity, now);

            // 设置租户ID
            if (entityConfig.EnableMultiTenancy && typeof(BaseEntity).IsAssignableFrom(typeof(TEntity)) && _context.CurrentTenantId.HasValue)
            {
                var tenantIdProperty = entity.GetType().GetProperty("TenantId");
                tenantIdProperty?.SetValue(entity, _context.CurrentTenantId.Value);
            }
        }

        return await _context.InsertRangeAsync(entities);
    }

    /// <summary>
    /// 批量删除实体
    /// </summary>
    /// <param name="entities">实体列表</param>
    /// <returns>删除的实体数量</returns>
    public async Task<int> DeleteRangeAsync(List<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            var isDeletedProperty = entity.GetType().GetProperty("IsDeleted");
            if (isDeletedProperty != null)
            {
                // 软删除
                isDeletedProperty.SetValue(entity, true);
                await UpdateAsync(entity);
            }
            else
            {
                // 硬删除
                await _context.DeleteAsync(entity);
            }
        }

        return entities.Count;
    }
}