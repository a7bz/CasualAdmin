namespace CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Interfaces;
using CasualAdmin.Application.Interfaces.Base;
using CasualAdmin.Domain.Entities.System;

/// <summary>
/// 部门服务接口
/// </summary>
public interface IDeptService : IBaseService<SysDept>
{
    /// <summary>
    /// 获取部门树
    /// </summary>
    /// <returns>部门树列表</returns>
    Task<List<SysDept>> GetDeptTreeAsync();

    /// <summary>
    /// 根据部门ID获取子部门列表
    /// </summary>
    /// <param name="parentId">父部门ID</param>
    /// <returns>子部门列表</returns>
    Task<List<SysDept>> GetChildrenByParentIdAsync(Guid parentId);
}