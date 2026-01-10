namespace CasualAdmin.Domain.Entities.System;
using SqlSugar;

/// <summary>
/// 操作日志实体
/// </summary>
[SugarTable("sys_operation_logs")]
public class SysOperationLog : BaseEntity
{
    /// <summary>
    /// 日志ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
    public Guid LogId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    [SugarColumn(Length = 100)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 操作模块
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// 操作类型：1-新增，2-修改，3-删除，4-查询，5-登录，6-登出
    /// </summary>
    public int OperationType { get; set; } = 0;

    /// <summary>
    /// 操作内容
    /// </summary>
    [SugarColumn(Length = 500)]
    public string OperationContent { get; set; } = string.Empty;

    /// <summary>
    /// 请求URL
    /// </summary>
    [SugarColumn(Length = 500)]
    public string RequestUrl { get; set; } = string.Empty;

    /// <summary>
    /// 请求方法
    /// </summary>
    [SugarColumn(Length = 10)]
    public string RequestMethod { get; set; } = string.Empty;

    /// <summary>
    /// 请求参数
    /// </summary>
    [SugarColumn(Length = 2000)]
    public string RequestParams { get; set; } = string.Empty;

    /// <summary>
    /// 响应结果
    /// </summary>
    [SugarColumn(Length = 2000)]
    public string ResponseResult { get; set; } = string.Empty;

    /// <summary>
    /// 操作IP
    /// </summary>
    [SugarColumn(Length = 50)]
    public string OperationIp { get; set; } = string.Empty;

    /// <summary>
    /// 执行时长（毫秒）
    /// </summary>
    public long ExecutionTime { get; set; } = 0;

    /// <summary>
    /// 状态：0-失败，1-成功
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 错误信息
    /// </summary>
    [SugarColumn(Length = 2000)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 浏览器信息
    /// </summary>
    [SugarColumn(Length = 500)]
    public string Browser { get; set; } = string.Empty;

    /// <summary>
    /// 操作系统
    /// </summary>
    [SugarColumn(Length = 100)]
    public string Os { get; set; } = string.Empty;
}