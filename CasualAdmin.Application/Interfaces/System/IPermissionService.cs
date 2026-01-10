namespace CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Interfaces;
using CasualAdmin.Application.Interfaces.Base;
using CasualAdmin.Domain.Entities.System;

/// <summary>
/// 权限服务接口
/// </summary>
public interface IPermissionService : IBaseService<SysPermission>
{
    /// <summary>
    /// 根据角色ID获取权限列表
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>权限列表</returns>
    Task<List<SysPermission>> GetPermissionsByRoleIdAsync(Guid roleId);

    /// <summary>
    /// 根据菜单ID获取权限列表
    /// </summary>
    /// <param name="menuId">菜单ID</param>
    /// <returns>权限列表</returns>
    Task<List<SysPermission>> GetPermissionsByMenuIdAsync(Guid menuId);
}