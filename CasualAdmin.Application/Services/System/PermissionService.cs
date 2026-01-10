namespace CasualAdmin.Application.Services.System;
using CasualAdmin.Application.Interfaces.Base;
using CasualAdmin.Application.Interfaces.Events;
using CasualAdmin.Application.Interfaces.Services;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Services;
using CasualAdmin.Domain.Entities.System;

/// <summary>
/// 权限服务实现
/// </summary>
public class PermissionService : BaseService<SysPermission>, IPermissionService
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="repository">权限仓储</param>
    /// <param name="validationService">验证服务</param>
    /// <param name="eventBus">事件总线</param>
    public PermissionService(IRepository<SysPermission> repository, IValidationService validationService, IEventBus eventBus) : base(repository, validationService, eventBus)
    {
    }

    /// <summary>
    /// 根据角色ID获取权限列表
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>权限列表</returns>
    public async Task<List<SysPermission>> GetPermissionsByRoleIdAsync(Guid roleId)
    {
        // 这里需要根据实际的角色权限关联查询，暂时返回所有权限
        return await _repository.GetAllAsync();
    }

    /// <summary>
    /// 根据菜单ID获取权限列表
    /// </summary>
    /// <param name="menuId">菜单ID</param>
    /// <returns>权限列表</returns>
    public async Task<List<SysPermission>> GetPermissionsByMenuIdAsync(Guid menuId)
    {
        return await _repository.FindAsync(p => p.MenuId == menuId);
    }
}