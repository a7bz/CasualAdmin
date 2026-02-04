namespace CasualAdmin.Infrastructure.Data.Repositories;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using CasualAdmin.Domain.Entities;
using CasualAdmin.Domain.Infrastructure.Data;
using SqlSugar;

/// <summary>
/// 泛型仓储实现
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
{
    private readonly IDbContext _context;
    private ISugarQueryable<TEntity> _dbSet;

    // 反射缓存
    private static readonly ConcurrentDictionary<Type, PropertyInfo?> _primaryKeyCache = new();
    private static readonly ConcurrentDictionary<Type, PropertyInfo?> _createdAtCache = new();
    private static readonly ConcurrentDictionary<Type, PropertyInfo?> _updatedAtCache = new();
    private static readonly ConcurrentDictionary<Type, PropertyInfo?> _isDeletedCache = new();
    private static readonly ConcurrentDictionary<Type, PropertyInfo?> _tenantIdCache = new();
    private static readonly ConcurrentDictionary<Type, Domain.Attributes.EntityConfigAttribute> _entityConfigCache = new();

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
        // 获取实体配置（使用缓存）
        var entityConfig = _entityConfigCache.GetOrAdd(typeof(TEntity), type =>
            type.GetCustomAttributes(typeof(Domain.Attributes.EntityConfigAttribute), true)
                .FirstOrDefault() as Domain.Attributes.EntityConfigAttribute ?? new Domain.Attributes.EntityConfigAttribute()
        );

        // 应用软删除过滤
        if (entityConfig.EnableSoftDelete && _isDeletedCache.GetOrAdd(typeof(TEntity), type => type.GetProperty("IsDeleted")) != null)
        {
            sugarQuery = sugarQuery.Where("is_deleted = @IsDeleted", new { IsDeleted = false });
        }

        // 应用多租户过滤
        if (entityConfig.EnableMultiTenancy && typeof(BaseEntity).IsAssignableFrom(typeof(TEntity)) && _context.CurrentTenantId.HasValue)
        {
            var tenantId = _context.CurrentTenantId.Value;
            sugarQuery = sugarQuery.Where("tenant_id = @TenantId", new { TenantId = tenantId });
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
    private PropertyInfo? GetPrimaryKeyProperty()
    {
        // 使用缓存获取主键属性
        return _primaryKeyCache.GetOrAdd(typeof(TEntity), type =>
        {
            // 尝试查找带有SugarColumn.IsPrimaryKey特性的属性（优先级最高，因为是SqlSugar专用）
            var sugarKeyProperty = type.GetProperties()
                .FirstOrDefault(p =>
                {
                    var sugarColumnAttrs = p.GetCustomAttributes(typeof(SugarColumn), true);
                    foreach (var attr in sugarColumnAttrs)
                    {
                        if (attr is SugarColumn sugarColumn && sugarColumn.IsPrimaryKey)
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
            var keyProperty = type.GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), true).Length != 0);
            if (keyProperty != null)
            {
                return keyProperty;
            }

            // 尝试查找Id属性（最常见的主键名称）
            var idProperty = type.GetProperty("Id");
            if (idProperty != null)
            {
                return idProperty;
            }

            // 尝试查找以Id结尾的属性（比如DictItemId）
            var idEndingProperty = type.GetProperties()
                .FirstOrDefault(p => p.Name.EndsWith("Id") && p.PropertyType == typeof(Guid));
            if (idEndingProperty != null)
            {
                return idEndingProperty;
            }

            // 如果都找不到，返回null
            return null;
        });
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
        // 设置创建时间和更新时间（使用缓存）
        var now = DateTime.Now;
        var createdAtProperty = _createdAtCache.GetOrAdd(typeof(TEntity), type => type.GetProperty("CreatedAt"));
        createdAtProperty?.SetValue(entity, now);
        var updatedAtProperty = _updatedAtCache.GetOrAdd(typeof(TEntity), type => type.GetProperty("UpdatedAt"));
        updatedAtProperty?.SetValue(entity, now);

        // 获取实体配置（使用缓存）
        var entityConfig = _entityConfigCache.GetOrAdd(typeof(TEntity), type =>
            type.GetCustomAttributes(typeof(Domain.Attributes.EntityConfigAttribute), true)
                .FirstOrDefault() as Domain.Attributes.EntityConfigAttribute ?? new Domain.Attributes.EntityConfigAttribute()
        );

        // 设置租户ID（使用缓存）
        if (entityConfig.EnableMultiTenancy && typeof(BaseEntity).IsAssignableFrom(typeof(TEntity)) && _context.CurrentTenantId.HasValue)
        {
            var tenantIdProperty = _tenantIdCache.GetOrAdd(typeof(TEntity), type => type.GetProperty("TenantId"));
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
        // 设置更新时间（使用缓存）
        var updatedAtProperty = _updatedAtCache.GetOrAdd(typeof(TEntity), type => type.GetProperty("UpdatedAt"));
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
        var updatedAtProperty = _updatedAtCache.GetOrAdd(typeof(TEntity), type => type.GetProperty("UpdatedAt"));

        foreach (var entity in entities)
        {
            // 设置更新时间（使用缓存）
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

        var isDeletedProperty = _isDeletedCache.GetOrAdd(typeof(TEntity), type => type.GetProperty("IsDeleted"));
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
        // 获取实体配置（使用缓存）
        var entityConfig = _entityConfigCache.GetOrAdd(typeof(TEntity), type =>
            type.GetCustomAttributes(typeof(Domain.Attributes.EntityConfigAttribute), true)
                .FirstOrDefault() as Domain.Attributes.EntityConfigAttribute ?? new Domain.Attributes.EntityConfigAttribute()
        );

        // 获取属性信息（使用缓存）
        var createdAtProperty = _createdAtCache.GetOrAdd(typeof(TEntity), type => type.GetProperty("CreatedAt"));
        var updatedAtProperty = _updatedAtCache.GetOrAdd(typeof(TEntity), type => type.GetProperty("UpdatedAt"));
        var tenantIdProperty = _tenantIdCache.GetOrAdd(typeof(TEntity), type => type.GetProperty("TenantId"));

        foreach (var entity in entities)
        {
            createdAtProperty?.SetValue(entity, now);
            updatedAtProperty?.SetValue(entity, now);

            // 设置租户ID
            if (entityConfig.EnableMultiTenancy && typeof(BaseEntity).IsAssignableFrom(typeof(TEntity)) && _context.CurrentTenantId.HasValue)
            {
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
        if (entities == null || entities.Count == 0)
        {
            return 0;
        }

        // 获取实体配置（使用缓存）
        var entityConfig = _entityConfigCache.GetOrAdd(typeof(TEntity), type =>
            type.GetCustomAttributes(typeof(Domain.Attributes.EntityConfigAttribute), true)
                .FirstOrDefault() as Domain.Attributes.EntityConfigAttribute ?? new Domain.Attributes.EntityConfigAttribute()
        );

        // 获取 IsDeleted 属性（使用缓存）
        var isDeletedProperty = _isDeletedCache.GetOrAdd(typeof(TEntity), type => type.GetProperty("IsDeleted"));

        if (isDeletedProperty != null && entityConfig.EnableSoftDelete)
        {
            // 批量软删除：使用条件表达式直接批量更新
            var primaryKeyProperty = GetPrimaryKeyProperty();
            if (primaryKeyProperty == null)
            {
                // 如果没有主键，回退到遍历更新
                var currentNow = DateTime.Now;
                var updatedAtProperty = _updatedAtCache.GetOrAdd(typeof(TEntity), type => type.GetProperty("UpdatedAt"));

                foreach (var entity in entities)
                {
                    isDeletedProperty.SetValue(entity, true);
                    updatedAtProperty?.SetValue(entity, currentNow);
                }

                return await _context.UpdateRangeAsync(entities);
            }

            // 提取所有主键值
            var ids = entities.Select(e => (Guid)primaryKeyProperty.GetValue(e)!).ToList();
            var now = DateTime.Now;

            // 构建批量更新条件：WHERE id IN (id1, id2, ...)
            var parameter = Expression.Parameter(typeof(TEntity), "e");
            var propertyAccess = Expression.Property(parameter, primaryKeyProperty);
            var containsMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(Guid));
            var containsCall = Expression.Call(containsMethod, Expression.Constant(ids), propertyAccess);
            var whereLambda = Expression.Lambda<Func<TEntity, bool>>(containsCall, parameter);

            // 由于IDbContext接口没有提供基于条件的批量更新方法，我们使用回退方案
            // 遍历更新每个实体
            var updatedAtProp = _updatedAtCache.GetOrAdd(typeof(TEntity), type => type.GetProperty("UpdatedAt"));
            foreach (var entity in entities)
            {
                isDeletedProperty.SetValue(entity, true);
                updatedAtProp?.SetValue(entity, now);
            }

            return await _context.UpdateRangeAsync(entities);
        }
        else
        {
            // 批量硬删除：使用条件表达式进行批量删除，提高性能
            var primaryKeyProperty = GetPrimaryKeyProperty();
            if (primaryKeyProperty == null)
            {
                // 如果没有主键，回退到循环删除
                int deletedCount = 0;
                foreach (var entity in entities)
                {
                    var result = await _context.DeleteAsync(entity);
                    if (result > 0)
                    {
                        deletedCount++;
                    }
                }
                return deletedCount;
            }

            // 提取所有主键值
            var ids = entities.Select(e => (Guid)primaryKeyProperty.GetValue(e)!).ToList();

            // 构建批量删除条件：WHERE id IN (id1, id2, ...)
            var parameter = Expression.Parameter(typeof(TEntity), "e");
            var propertyAccess = Expression.Property(parameter, primaryKeyProperty);
            var containsMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(Guid));
            var containsCall = Expression.Call(containsMethod, Expression.Constant(ids), propertyAccess);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(containsCall, parameter);

            // 执行批量删除（单条 SQL：DELETE FROM table WHERE id IN (...))
            return await _context.DeleteAsync(lambda);
        }
    }

    /// <summary>
    /// 批量删除实体（根据ID列表）
    /// </summary>
    /// <param name="ids">实体ID列表</param>
    /// <returns>删除的实体数量</returns>
    public async Task<int> DeleteRangeAsync(List<Guid> ids)
    {
        if (ids == null || ids.Count == 0)
        {
            return 0;
        }

        // 获取主键属性
        var primaryKeyProperty = GetPrimaryKeyProperty();
        if (primaryKeyProperty == null)
        {
            // 如果没有主键，回退到循环删除
            int deletedCount = 0;
            foreach (var id in ids)
            {
                var result = await DeleteAsync(id);
                if (result)
                {
                    deletedCount++;
                }
            }
            return deletedCount;
        }

        // 构建批量删除条件：WHERE id IN (id1, id2, ...)
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var propertyAccess = Expression.Property(parameter, primaryKeyProperty);
        var containsMethod = typeof(Enumerable).GetMethods()
            .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(Guid));
        var containsCall = Expression.Call(containsMethod, Expression.Constant(ids), propertyAccess);
        var lambda = Expression.Lambda<Func<TEntity, bool>>(containsCall, parameter);

        // 执行批量删除（单条 SQL：DELETE FROM table WHERE id IN (...))
        return await _context.DeleteAsync(lambda);
    }
}