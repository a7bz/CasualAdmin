namespace CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Interfaces;
using CasualAdmin.Application.Interfaces.Base;
using CasualAdmin.Domain.Entities.System;

/// <summary>
/// 角色服务接口
/// </summary>
public interface IRoleService : IBaseService<SysRole>
{
    /// <summary>
    /// 为用户分配角色
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="roleId">角色ID</param>
    /// <returns>分配结果</returns>
    Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId);

    /// <summary>
    /// 移除用户角色
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="roleId">角色ID</param>
    /// <returns>移除结果</returns>
    Task<bool> RemoveRoleFromUserAsync(Guid userId, Guid roleId);

    /// <summary>
    /// 获取用户角色列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>角色列表</returns>
    Task<List<SysRole>> GetRolesByUserIdAsync(Guid userId);

    /// <summary>
    /// 获取角色用户列表
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>用户列表</returns>
    Task<List<SysUser>> GetUsersByRoleIdAsync(Guid roleId);

    // 以下方法是为了兼容现有API控制器而添加的
    /// <summary>
    /// 根据ID获取角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>角色实体，不存在则返回null</returns>
    Task<SysRole?> GetRoleByIdAsync(Guid id);

    /// <summary>
    /// 获取所有角色
    /// </summary>
    /// <returns>角色列表</returns>
    Task<List<SysRole>> GetAllRolesAsync();

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="role">角色实体</param>
    /// <returns>创建的角色</returns>
    Task<SysRole> CreateRoleAsync(SysRole role);

    /// <summary>
    /// 更新角色
    /// </summary>
    /// <param name="role">角色实体</param>
    /// <returns>更新的角色</returns>
    Task<SysRole?> UpdateRoleAsync(SysRole role);

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>删除结果</returns>
    Task<bool> DeleteRoleAsync(Guid id);
}
