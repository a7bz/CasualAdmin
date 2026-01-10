namespace CasualAdmin.Application.Services.System;
using CasualAdmin.Application.Interfaces.Base;
using CasualAdmin.Application.Interfaces.Events;
using CasualAdmin.Application.Interfaces.Services;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Services;
using CasualAdmin.Domain.Entities.System;

/// <summary>
/// 菜单服务实现
/// </summary>
public class MenuService : BaseService<SysMenu>, IMenuService
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="repository">菜单仓储</param>
    /// <param name="validationService">验证服务</param>
    /// <param name="eventBus">事件总线</param>
    public MenuService(IRepository<SysMenu> repository, IValidationService validationService, IEventBus eventBus) : base(repository, validationService, eventBus)
    {
    }

    /// <summary>
    /// 根据角色ID获取菜单树
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>菜单树列表</returns>
    public async Task<List<SysMenu>> GetMenuTreeByRoleIdAsync(Guid roleId)
    {
        // 这里需要根据实际的权限关联查询，暂时返回所有菜单树
        return await GetAllMenuTreeAsync();
    }

    /// <summary>
    /// 获取所有菜单树
    /// </summary>
    /// <returns>菜单树列表</returns>
    public async Task<List<SysMenu>> GetAllMenuTreeAsync()
    {
        var menus = await _repository.GetAllAsync();
        return BuildMenuTree(menus);
    }

    /// <summary>
    /// 构建菜单树
    /// </summary>
    /// <param name="menus">菜单列表</param>
    /// <returns>菜单树</returns>
    private List<SysMenu> BuildMenuTree(List<SysMenu> menus)
    {
        var menuMap = menus.ToDictionary(m => m.MenuId);
        var rootMenus = new List<SysMenu>();

        foreach (var menu in menus)
        {
            if (!menu.ParentId.HasValue)
            {
                rootMenus.Add(menu);
            }
            else if (menuMap.TryGetValue(menu.ParentId.Value, out var parentMenu))
            {
                parentMenu.Children.Add(menu);
            }
        }

        return rootMenus;
    }
}