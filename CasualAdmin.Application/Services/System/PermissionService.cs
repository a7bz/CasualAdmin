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
    /// 权限服务实现
    /// </summary>
    public class PermissionService : BaseService<SysPermission>, IPermissionService
    {
        private readonly IRepository<SysRolePermission> _rolePermissionRepository;
        private readonly ICacheService _cacheService;
        private readonly TimeSpan _permissionCacheExpiration = TimeSpan.FromHours(1);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">权限仓储</param>
        /// <param name="rolePermissionRepository">角色权限关联仓储</param>
        /// <param name="cacheService">缓存服务</param>
        /// <param name="validationService">验证服务</param>
        /// <param name="eventBus">事件总线</param>
        public PermissionService(
            IRepository<SysPermission> repository,
            IRepository<SysRolePermission> rolePermissionRepository,
            ICacheService cacheService,
            IValidationService validationService,
            IEventBus eventBus) : base(repository, validationService, eventBus)
        {
            _rolePermissionRepository = rolePermissionRepository;
            _cacheService = cacheService;
        }

        /// <summary>
        /// 根据角色ID获取权限列表
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>权限列表</returns>
        public async Task<List<SysPermission>> GetPermissionsByRoleIdAsync(Guid roleId)
        {
            var cacheKey = $"permission:role:{roleId}";
            // 尝试从缓存获取
            var cachedPermissions = await _cacheService.GetAsync<List<SysPermission>>(cacheKey);
            if (cachedPermissions != null)
            {
                return cachedPermissions;
            }

            // 从数据库获取
            // 1. 获取该角色的所有角色权限关联记录
            var rolePermissions = await _rolePermissionRepository.FindAsync(rp => rp.RoleId == roleId);
            if (rolePermissions.Count == 0)
            {
                return [];
            }

            // 2. 获取权限ID列表
            var permissionIds = rolePermissions.Select(rp => rp.PermissionId).ToList();

            // 3. 获取对应的权限记录
            var permissions = await _repository.FindAsync(p => permissionIds.Contains(p.PermissionId));

            // 缓存结果
            await _cacheService.SetAsync(cacheKey, permissions, _permissionCacheExpiration);

            return permissions;
        }

        /// <summary>
        /// 批量根据角色ID获取权限列表
        /// </summary>
        /// <param name="roleIds">角色ID列表</param>
        /// <returns>权限列表</returns>
        public async Task<List<SysPermission>> GetPermissionsByRoleIdsAsync(List<Guid> roleIds)
        {
            if (roleIds == null || roleIds.Count == 0)
            {
                return [];
            }

            var cacheKey = $"permission:roles:{string.Join(",", roleIds.OrderBy(id => id))}";
            // 尝试从缓存获取
            var cachedPermissions = await _cacheService.GetAsync<List<SysPermission>>(cacheKey);
            if (cachedPermissions != null)
            {
                return cachedPermissions;
            }

            // 从数据库获取
            // 1. 获取这些角色的所有角色权限关联记录
            var rolePermissions = await _rolePermissionRepository.FindAsync(rp => roleIds.Contains(rp.RoleId));
            if (rolePermissions.Count == 0)
            {
                return [];
            }

            // 2. 获取权限ID列表（去重）
            var permissionIds = rolePermissions.Select(rp => rp.PermissionId).Distinct().ToList();

            // 3. 获取对应的权限记录
            var permissions = await _repository.FindAsync(p => permissionIds.Contains(p.PermissionId));

            // 缓存结果
            await _cacheService.SetAsync(cacheKey, permissions, _permissionCacheExpiration);

            return permissions;
        }

        /// <summary>
        /// 根据菜单ID获取权限列表
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <returns>权限列表</returns>
        public async Task<List<SysPermission>> GetPermissionsByMenuIdAsync(Guid menuId)
        {
            var cacheKey = $"permission:menu:{menuId}";
            // 尝试从缓存获取
            var cachedPermissions = await _cacheService.GetAsync<List<SysPermission>>(cacheKey);
            if (cachedPermissions != null)
            {
                return cachedPermissions;
            }

            // 从数据库获取
            var permissions = await _repository.FindAsync(p => p.MenuId == menuId);

            // 缓存结果
            await _cacheService.SetAsync(cacheKey, permissions, _permissionCacheExpiration);

            return permissions;
        }
    }
}