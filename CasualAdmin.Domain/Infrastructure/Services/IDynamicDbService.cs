namespace CasualAdmin.Domain.Infrastructure.Services;

/// <summary>
/// 动态数据库服务接口
/// 提供无实体类的数据库操作能力
/// </summary>
public interface IDynamicDbService
{
    /// <summary>
    /// 执行SQL查询，返回动态对象列表
    /// </summary>
    /// <param name="sql">SQL查询语句</param>
    /// <param name="parameters">查询参数</param>
    /// <returns>动态对象列表</returns>
    Task<List<dynamic>> ExecuteQueryAsync(string sql, object? parameters = null);

    /// <summary>
    /// 执行SQL分页查询，返回带分页信息的动态对象列表
    /// </summary>
    /// <param name="sql">SQL查询语句</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <param name="parameters">查询参数</param>
    /// <returns>带分页信息的动态对象列表</returns>
    Task<object> ExecutePagedQueryAsync(string sql, int pageIndex, int pageSize, object? parameters = null);

    /// <summary>
    /// 执行SQL命令，返回受影响的行数
    /// </summary>
    /// <param name="sql">SQL命令</param>
    /// <param name="parameters">命令参数</param>
    /// <returns>受影响的行数</returns>
    Task<int> ExecuteCommandAsync(string sql, object? parameters = null);

    /// <summary>
    /// 执行SQL查询，返回单个动态对象
    /// </summary>
    /// <param name="sql">SQL查询语句</param>
    /// <param name="parameters">查询参数</param>
    /// <returns>查询结果的单个动态对象</returns>
    Task<object> ExecuteScalarAsync(string sql, object? parameters = null);

    /// <summary>
    /// 使用IN条件执行SQL查询，返回动态对象列表
    /// </summary>
    /// <param name="sql">SQL查询语句，包含@ids参数</param>
    /// <param name="ids">ID数组</param>
    /// <returns>动态对象列表</returns>
    Task<List<dynamic>> ExecuteInQueryAsync(string sql, int[] ids);
}