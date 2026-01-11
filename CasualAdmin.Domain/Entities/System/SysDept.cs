namespace CasualAdmin.Domain.Entities.System;
using CasualAdmin.Domain.Common;
using SqlSugar;

/// <summary>
/// 部门实体
/// </summary>
[SugarTable("sys_depts")]
public class SysDept : BaseEntity
{
    /// <summary>
    /// 部门ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
    public Guid DeptId { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// 父部门ID
    /// </summary>
    public Guid? ParentId { get; private set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    [SugarColumn(Length = 100)]
    public string DeptName { get; private set; } = string.Empty;

    /// <summary>
    /// 部门编码
    /// </summary>
    [SugarColumn(Length = 50)]
    public string DeptCode { get; private set; } = string.Empty;

    /// <summary>
    /// 部门负责人
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Leader { get; private set; } = string.Empty;

    /// <summary>
    /// 联系电话
    /// </summary>
    [SugarColumn(Length = 20)]
    public string Phone { get; private set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; private set; } = 0;

    /// <summary>
    /// 状态
    /// </summary>
    public Status Status { get; private set; } = Status.Enabled;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 200)]
    public string Remark { get; private set; } = string.Empty;

    /// <summary>
    /// 子部门列表
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysDept> Children { get; private set; } = [];

    /// <summary>
    /// 父部门
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public SysDept? Parent { get; private set; }

    /// <summary>
    /// 部门用户列表
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysUser> Users { get; private set; } = [];

    /// <summary>
    /// 设置部门名称
    /// </summary>
    /// <param name="deptName">部门名称</param>
    public void SetDeptName(string deptName)
    {
        if (string.IsNullOrWhiteSpace(deptName))
        {
            throw new ArgumentException("部门名称不能为空");
        }
        DeptName = deptName.Trim();
    }

    /// <summary>
    /// 设置部门编码
    /// </summary>
    /// <param name="deptCode">部门编码</param>
    public void SetDeptCode(string deptCode)
    {
        if (string.IsNullOrWhiteSpace(deptCode))
        {
            throw new ArgumentException("部门编码不能为空");
        }
        DeptCode = deptCode.Trim();
    }

    /// <summary>
    /// 设置部门负责人
    /// </summary>
    /// <param name="leader">部门负责人</param>
    public void SetLeader(string leader)
    {
        Leader = leader?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// 设置联系电话
    /// </summary>
    /// <param name="phone">联系电话</param>
    public void SetPhone(string phone)
    {
        Phone = phone?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// 设置父部门
    /// </summary>
    /// <param name="parentId">父部门ID</param>
    public void SetParent(Guid? parentId)
    {
        // 防止循环引用
        if (parentId == DeptId)
        {
            throw new ArgumentException("部门不能设置为自己的父部门");
        }
        ParentId = parentId;
    }

    /// <summary>
    /// 设置排序
    /// </summary>
    /// <param name="sort">排序值</param>
    public void SetSort(int sort)
    {
        Sort = sort;
    }

    /// <summary>
    /// 启用部门
    /// </summary>
    public void Enable()
    {
        Status = Status.Enabled;
    }

    /// <summary>
    /// 禁用部门
    /// </summary>
    public void Disable()
    {
        Status = Status.Disabled;
    }

    /// <summary>
    /// 设置备注
    /// </summary>
    /// <param name="remark">备注信息</param>
    public void SetRemark(string remark)
    {
        Remark = remark?.Trim() ?? string.Empty;
    }
}
