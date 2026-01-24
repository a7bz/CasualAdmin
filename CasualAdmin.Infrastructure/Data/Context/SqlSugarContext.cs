namespace CasualAdmin.Infrastructure.Data.Context;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using CasualAdmin.Application.Interfaces.Base;
using Microsoft.Extensions.Configuration;
using SqlSugar;

/// <summary>
/// SqlSugar上下文
/// </summary>
public class SqlSugarContext : SqlSugarClient, IDbContext
{
    private readonly IConfiguration _configuration;
    private readonly bool _enableCodeFirst;
    private readonly bool _dropExistingTables;
    private readonly bool _isMultiTenancyEnabled;
    private Guid? _currentTenantId;

    /// <summary>
    /// 当前租户ID
    /// </summary>
    public Guid? CurrentTenantId
    {
        get => _currentTenantId;
        set => _currentTenantId = value;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="connectionString">数据库连接字符串</param>
    /// <param name="dbType">数据库类型字符串</param>
    /// <param name="configuration">配置对象</param>
    public SqlSugarContext(string connectionString, string dbType, IConfiguration configuration)
        : base(new ConnectionConfig
        {
            ConnectionString = connectionString,
            DbType = GetDbType(dbType),
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute,
            ConfigureExternalServices = new ConfigureExternalServices()
            {
                EntityService = (x, p) =>
                {
                    p.DbColumnName = UtilMethods.ToUnderLine(p.DbColumnName);

                    // 自动Nullable配置 - 高版本C#写法
                    if (p.IsPrimarykey == false && new NullabilityInfoContext()
                        .Create(x).WriteState is NullabilityState.Nullable)
                    {
                        p.IsNullable = true;
                    }
                },
                EntityNameService = (x, p) =>
                {
                    p.DbTableName = UtilMethods.ToUnderLine(p.DbTableName);
                }
            }
        })
    {
        _configuration = configuration;

        // 获取当前环境
        var environment = _configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") ?? "Production";

        // 生产环境强制禁用CodeFirst和删除现有表功能
        _enableCodeFirst = environment.Equals("Development", StringComparison.OrdinalIgnoreCase) && _configuration.GetValue("SqlSugar:EnableCodeFirst", false);

        _dropExistingTables = environment.Equals("Development", StringComparison.OrdinalIgnoreCase) && _configuration.GetValue("SqlSugar:DropExistingTables", false);

        _isMultiTenancyEnabled = _configuration.GetValue("MultiTenancy:IsEnabled", false);
        _currentTenantId = _configuration.GetValue<Guid?>("MultiTenancy:DefaultTenantId");

        ConfigureGlobalFilters();
        ConfigureEntityMappings();
    }

    /// <summary>
    /// 配置全局过滤
    /// </summary>
    private void ConfigureGlobalFilters()
    {
        // 软删除和多租户过滤将在Repository层实现
    }

    /// <summary>
    /// 根据字符串获取数据库类型枚举
    /// </summary>
    /// <param name="dbTypeString">数据库类型字符串</param>
    /// <returns>数据库类型枚举</returns>
    private static SqlSugar.DbType GetDbType(string dbTypeString)
    {
        return dbTypeString.ToLower() switch
        {
            "postgresql" => SqlSugar.DbType.PostgreSQL,
            "sqlserver" or "mssql" => SqlSugar.DbType.SqlServer,
            "mysql" => SqlSugar.DbType.MySql,
            "sqlite" => SqlSugar.DbType.Sqlite,
            "oracle" => SqlSugar.DbType.Oracle,
            "dm" => SqlSugar.DbType.Dm,
            "db2" => SqlSugar.DbType.DB2,
            _ => SqlSugar.DbType.PostgreSQL
        };
    }

    /// <summary>
    /// 配置实体映射
    /// </summary>
    private void ConfigureEntityMappings()
    {
        bool showSql = _configuration.GetValue<bool>("SqlSugar:ShowSql", true);

        Aop.OnLogExecuting = (sql, pars) =>
        {
            if (showSql)
            {
                Console.WriteLine($"执行SQL: {sql}");
            }
        };
    }

    /// <summary>
    /// 执行CodeFirst同步数据库结构
    /// </summary>
    public void ExecuteCodeFirst()
    {
        if (!_enableCodeFirst)
        {
            Console.WriteLine("CodeFirst功能未启用，跳过数据库同步");
            return;
        }

        try
        {
            Console.WriteLine("开始执行CodeFirst数据库同步...");

            var entityTypes = GetEntityTypes();

            if (entityTypes.Count == 0)
            {
                Console.WriteLine("未找到需要同步的实体类");
                return;
            }

            Console.WriteLine($"找到 {entityTypes.Count} 个实体类需要同步");

            foreach (var entityType in entityTypes)
            {
                Console.WriteLine($"正在同步表: {entityType.Name}");
            }

            // 如果配置了删除现有表，则执行删除操作
            if (_dropExistingTables)
            {
                // 尝试先删除现有表，然后重新创建，确保表结构与实体类完全匹配
                // 注意：这会删除所有现有数据，仅适用于开发环境
                Console.WriteLine("开始删除现有表...");
                foreach (var entityType in entityTypes)
                {
                    try
                    {
                        Context.DbMaintenance.DropTable(entityType);
                        Console.WriteLine($"已删除表: {entityType.Name}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"删除表 {entityType.Name} 失败: {ex.Message}");
                    }
                }
            }

            // 创建或更新表结构
            Console.WriteLine("开始创建或更新表结构...");
            Context.CodeFirst.SetStringDefaultLength(200).InitTables(entityTypes.ToArray());

            Console.WriteLine("CodeFirst数据库同步完成");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CodeFirst执行失败: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// 获取所有实体类型
    /// </summary>
    /// <returns>实体类型列表</returns>
    private List<Type> GetEntityTypes()
    {
        var entityTypes = new List<Type>();

        var assembly = Assembly.Load("CasualAdmin.Domain");
        var baseNamespace = "CasualAdmin.Domain.Entities";

        foreach (var type in assembly.GetTypes())
        {
            if (type.Namespace?.StartsWith(baseNamespace) == true &&
                type.IsClass &&
                !type.IsAbstract &&
                type.IsPublic)
            {
                entityTypes.Add(type);
            }
        }

        return entityTypes;
    }

    /// <summary>
    /// 获取查询对象
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>查询对象</returns>
    public new dynamic Queryable<TEntity>() where TEntity : class, new()
    {
        return Context.Queryable<TEntity>();
    }

    /// <summary>
    /// 插入实体
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <returns>受影响的行数</returns>
    public async Task<int> InsertAsync<TEntity>(TEntity entity) where TEntity : class, new()
    {
        return await Context.Insertable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <returns>受影响的行数</returns>
    public async Task<int> UpdateAsync<TEntity>(TEntity entity) where TEntity : class, new()
    {
        return await Context.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除实体
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <returns>受影响的行数</returns>
    public async Task<int> DeleteAsync<TEntity>(TEntity entity) where TEntity : class, new()
    {
        return await Context.Deleteable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 批量插入实体
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="entities">实体列表</param>
    /// <returns>受影响的行数</returns>
    public async Task<int> InsertRangeAsync<TEntity>(List<TEntity> entities) where TEntity : class, new()
    {
        return await Context.Insertable(entities).ExecuteCommandAsync();
    }

    /// <summary>
    /// 批量更新实体
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="entities">实体列表</param>
    /// <returns>受影响的行数</returns>
    public async Task<int> UpdateRangeAsync<TEntity>(List<TEntity> entities) where TEntity : class, new()
    {
        return await Context.Updateable(entities).ExecuteCommandAsync();
    }

    /// <summary>
    /// 按条件删除实体
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="predicate">删除条件</param>
    /// <returns>受影响的行数</returns>
    public async Task<int> DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, new()
    {
        return await Context.Deleteable<TEntity>().Where(predicate).ExecuteCommandAsync();
    }
}