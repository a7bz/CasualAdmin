namespace CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Interfaces;
using CasualAdmin.Application.Interfaces.Base;
using CasualAdmin.Domain.Entities.System;

/// <summary>
/// 用户服务接口
/// </summary>
public interface IUserService : IBaseService<SysUser>
{
    /// <summary>
    /// 根据邮箱获取用户
    /// </summary>
    /// <param name="email">用户邮箱</param>
    /// <returns>用户实体，可能为null</returns>
    Task<SysUser?> GetUserByEmailAsync(string email);

    /// <summary>
    /// 验证密码
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="password">密码</param>
    /// <returns>验证结果</returns>
    bool VerifyPassword(SysUser user, string password);

    /// <summary>
    /// 异步验证密码
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="password">密码</param>
    /// <returns>验证结果</returns>
    Task<bool> VerifyPasswordAsync(SysUser user, string password);

    // 以下方法是为了兼容现有API控制器而添加的
    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户实体，不存在则返回null</returns>
    Task<SysUser?> GetUserByIdAsync(Guid id);

    /// <summary>
    /// 获取所有用户
    /// </summary>
    /// <returns>用户列表</returns>
    Task<List<SysUser>> GetAllUsersAsync();

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <returns>创建的用户实体</returns>
    Task<SysUser> CreateUserAsync(SysUser user);

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <returns>更新的用户实体，可能为null</returns>
    Task<SysUser?> UpdateUserAsync(SysUser user);

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>删除结果</returns>
    Task<bool> DeleteUserAsync(Guid id);
}
