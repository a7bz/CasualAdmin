namespace CasualAdmin.Domain.Entities.System;
using SqlSugar;

/// <summary>
/// 登录日志实体
/// </summary>
[SugarTable("sys_login_logs")]
[SugarIndex("idx_user_id", nameof(UserId), OrderByType.Asc)]
[SugarIndex("idx_login_time", nameof(LoginTime), OrderByType.Desc)]
[SugarIndex("idx_user_login_time", nameof(UserId), OrderByType.Asc, nameof(LoginTime), OrderByType.Desc)]
public class SysLoginLog : BaseEntity
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
    /// 登录时间
    /// </summary>
    public DateTime LoginTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 登录IP
    /// </summary>
    [SugarColumn(Length = 50)]
    public string LoginIp { get; set; } = string.Empty;

    /// <summary>
    /// 登录结果：0-失败，1-成功
    /// </summary>
    public int LoginResult { get; set; } = 0;

    /// <summary>
    /// 失败原因
    /// </summary>
    [SugarColumn(Length = 200)]
    public string? FailureReason { get; set; }

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

    /// <summary>
    /// 登录类型：1-web，2-app，3-API
    /// </summary>
    public int LoginType { get; set; } = 1;

    /// <summary>
    /// 会话ID
    /// </summary>
    [SugarColumn(Length = 200)]
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// 登出时间
    /// </summary>
    public DateTime? LogoutTime { get; set; }

    /// <summary>
    /// 在线时长（分钟）
    /// </summary>
    public int OnlineDuration { get; set; } = 0;
}