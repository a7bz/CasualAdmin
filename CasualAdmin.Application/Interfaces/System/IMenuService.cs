namespace CasualAdmin.Application.Interfaces.System;

using CasualAdmin.Application.Interfaces.Base;
using CasualAdmin.Domain.Entities.System;

/// <summary>
/// 菜单服务接口
/// </summary>
public interface IMenuService : IBaseService<SysMenu>
{
    /// <summary>
    /// 根据角色ID获取菜单树
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>菜单树列表</returns>
    Task<List<SysMenu>> GetMenuTreeByRoleIdAsync(Guid roleId);

    /// <summary>
    /// 获取所有菜单树
    /// </summary>
    /// <returns>菜单树列表</returns>
    Task<List<SysMenu>> GetAllMenuTreeAsync();
}