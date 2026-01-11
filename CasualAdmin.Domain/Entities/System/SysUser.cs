namespace CasualAdmin.Domain.Entities.System;
using CasualAdmin.Domain.Common;
using global::System.Text.Json.Serialization;
using SqlSugar;

/// <summary>
/// 用户实体
/// </summary>
[SugarTable("sys_users")]
public class SysUser : BaseEntity
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
    public Guid UserId { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// 用户名
    /// </summary>
    [SugarColumn(Length = 100)]
    public string Username { get; private set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    [SugarColumn(Length = 50)]
    public string RealName { get; private set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(Length = 200)]
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// 手机号
    /// </summary>
    [SugarColumn(Length = 20)]
    public string Phone { get; private set; } = string.Empty;

    /// <summary>
    /// 头像
    /// </summary>
    [SugarColumn(Length = 500)]
    public string Avatar { get; private set; } = string.Empty;

    /// <summary>
    /// 性别
    /// </summary>
    public Gender Gender { get; private set; } = Gender.Unknown;

    /// <summary>
    /// 生日
    /// </summary>
    public DateTime? Birthday { get; private set; }

    /// <summary>
    /// 部门ID
    /// </summary>
    public Guid? DeptId { get; private set; }

    /// <summary>
    /// 职位
    /// </summary>
    [SugarColumn(Length = 100)]
    public string Position { get; private set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    [JsonIgnore]
    [SugarColumn(Length = 500)]
    public string Password { get; private set; } = string.Empty;

    /// <summary>
    /// 盐值
    /// </summary>
    [JsonIgnore]
    [SugarColumn(Length = 100)]
    public string Salt { get; private set; } = string.Empty;

    /// <summary>
    /// 状态
    /// </summary>
    public Status Status { get; private set; } = Status.Enabled;

    /// <summary>
    /// 最后登录时间
    /// </summary>
    public DateTime? LastLoginTime { get; private set; }

    /// <summary>
    /// 最后登录IP
    /// </summary>
    [SugarColumn(Length = 50)]
    public string LastLoginIp { get; private set; } = string.Empty;

    /// <summary>
    /// 锁定结束时间
    /// </summary>
    public DateTime? LockEndTime { get; private set; }

    /// <summary>
    /// 登录失败次数
    /// </summary>
    public int LoginFailCount { get; private set; } = 0;

    /// <summary>
    /// 用户角色关联列表
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysUserRole> UserRoles { get; private set; } = [];

    /// <summary>
    /// 角色列表
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysRole> Roles { get; private set; } = [];

    /// <summary>
    /// 所属部门
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public SysDept? Dept { get; private set; }

    /// <summary>
    /// 所属租户
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public SysTenant? Tenant { get; private set; }

    /// <summary>
    /// 设置用户名
    /// </summary>
    /// <param name="username">用户名</param>
    public void SetUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("用户名不能为空");
        }
        Username = username.Trim();
    }

    /// <summary>
    /// 设置真实姓名
    /// </summary>
    /// <param name="realName">真实姓名</param>
    public void SetRealName(string realName)
    {
        RealName = realName?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// 设置邮箱
    /// </summary>
    /// <param name="email">邮箱地址</param>
    public void SetEmail(string email)
    {
        Email = email?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// 设置手机号
    /// </summary>
    /// <param name="phone">手机号</param>
    public void SetPhone(string phone)
    {
        Phone = phone?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// 设置密码
    /// </summary>
    /// <param name="password">原始密码</param>
    /// <param name="salt">盐值</param>
    public void SetPassword(string password, string salt)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("密码不能为空");
        }
        if (string.IsNullOrWhiteSpace(salt))
        {
            throw new ArgumentException("盐值不能为空");
        }
        Password = password;
        Salt = salt;
    }

    /// <summary>
    /// 重置登录失败次数
    /// </summary>
    public void ResetLoginFailCount()
    {
        LoginFailCount = 0;
        LockEndTime = null;
    }

    /// <summary>
    /// 增加登录失败次数
    /// </summary>
    public void IncrementLoginFailCount()
    {
        LoginFailCount++;
    }

    /// <summary>
    /// 记录登录信息
    /// </summary>
    /// <param name="ipAddress">登录IP</param>
    public void RecordLogin(string ipAddress)
    {
        LastLoginTime = DateTime.Now;
        LastLoginIp = ipAddress;
        ResetLoginFailCount();
    }

    /// <summary>
    /// 启用用户
    /// </summary>
    public void Enable()
    {
        Status = Status.Enabled;
    }

    /// <summary>
    /// 禁用用户
    /// </summary>
    public void Disable()
    {
        Status = Status.Disabled;
    }

    /// <summary>
    /// 设置部门
    /// </summary>
    /// <param name="deptId">部门ID</param>
    public void SetDept(Guid? deptId)
    {
        DeptId = deptId;
    }
}

