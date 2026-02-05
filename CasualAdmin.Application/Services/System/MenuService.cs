namespace CasualAdmin.Application.Services.System
{
    using CasualAdmin.Application.Interfaces.Events;
    using CasualAdmin.Application.Interfaces.Services;
    using CasualAdmin.Application.Interfaces.System;
    using CasualAdmin.Application.Services;
    using CasualAdmin.Domain.Entities.System;
    using CasualAdmin.Domain.Infrastructure.Data;
    using CasualAdmin.Domain.Infrastructure.Services;
    using global::System.Linq;

    /// <summary>
    /// 菜单服务实现
    /// </summary>
    public class MenuService : BaseService<SysMenu>, IMenuService
    {
        private readonly ICacheService _cacheService;
        private const string MENU_TREE_CACHE_KEY = "menu:tree:all";
        private readonly TimeSpan _menuCacheExpiration = TimeSpan.FromHours(1);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">菜单仓储</param>
        /// <param name="cacheService">缓存服务</param>
        /// <param name="validationService">验证服务</param>
        /// <param name="eventBus">事件总线</param>
        public MenuService(IRepository<SysMenu> repository, ICacheService cacheService, IValidationService validationService, IEventBus eventBus) : base(repository, validationService, eventBus)
        {
            _cacheService = cacheService;
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
            // 尝试从缓存获取
            var cachedMenus = await _cacheService.GetAsync<List<SysMenu>>(MENU_TREE_CACHE_KEY);
            if (cachedMenus != null)
            {
                return cachedMenus;
            }

            // 缓存不存在，从数据库获取
            var menus = await _repository.GetAllAsync();
            var menuTree = BuildMenuTree(menus);

            // 存入缓存
            await _cacheService.SetAsync(MENU_TREE_CACHE_KEY, menuTree, _menuCacheExpiration);

            return menuTree;
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

        /// <summary>
        /// 重写创建方法，清除菜单树缓存
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>创建的实体</returns>
        public override async Task<SysMenu?> CreateAsync(SysMenu entity)
        {
            var result = await base.CreateAsync(entity);
            await ClearMenuTreeCache();
            return result;
        }

        /// <summary>
        /// 重写更新方法，清除菜单树缓存
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>更新的实体</returns>
        public override async Task<SysMenu?> UpdateAsync(SysMenu entity)
        {
            var result = await base.UpdateAsync(entity);
            await ClearMenuTreeCache();
            return result;
        }

        /// <summary>
        /// 重写删除方法，清除菜单树缓存
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>删除结果</returns>
        public override async Task<bool> DeleteAsync(Guid id)
        {
            var result = await base.DeleteAsync(id);
            await ClearMenuTreeCache();
            return result;
        }

        /// <summary>
        /// 清除菜单树缓存
        /// </summary>
        private async Task ClearMenuTreeCache()
        {
            await _cacheService.RemoveAsync(MENU_TREE_CACHE_KEY);
        }
    }
}