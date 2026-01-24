namespace CasualAdmin.Application.Services.System;
using CasualAdmin.Application.Interfaces.Base;
using CasualAdmin.Application.Interfaces.Events;
using CasualAdmin.Application.Interfaces.Services;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Services;
using CasualAdmin.Domain.Entities.System;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// 用户服务实现
/// </summary>
public class UserService : BaseService<SysUser>, IUserService
{
    private readonly PasswordHasher<SysUser> _passwordHasher;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userRepository">用户仓储</param>
    /// <param name="validationService">验证服务</param>
    /// <param name="eventBus">事件总线</param>
    public UserService(IRepository<SysUser> userRepository, IValidationService validationService, IEventBus eventBus)
        : base(userRepository, validationService, eventBus)
    {
        _passwordHasher = new PasswordHasher<SysUser>();
    }

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户实体，不存在则返回null</returns>
    public async Task<SysUser?> GetUserByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    /// <summary>
    /// 获取所有用户
    /// </summary>
    /// <returns>用户列表</returns>
    public async Task<List<SysUser>> GetAllUsersAsync()
    {
        return await _repository.GetAllAsync();
    }

    /// <summary>
    /// 根据邮箱获取用户
    /// </summary>
    /// <param name="email">用户邮箱</param>
    /// <returns>用户实体</returns>
    public async Task<SysUser?> GetUserByEmailAsync(string email)
    {
        return await _repository.FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <returns>创建的用户实体</returns>
    public async Task<SysUser> CreateUserAsync(SysUser user)
    {
        // 密码已经通过实体的业务方法设置，这里只需要设置创建时间
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        return await _repository.AddAsync(user);
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <returns>更新的用户实体</returns>
    public async Task<SysUser?> UpdateUserAsync(SysUser user)
    {
        var existingUser = await GetUserByIdAsync(user.UserId);
        if (existingUser == null)
        {
            return null;
        }

        // 更新时间由仓储层统一处理，这里不需要手动设置
        return await _repository.UpdateAsync(user);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>删除结果</returns>
    public async Task<bool> DeleteUserAsync(Guid id)
    {
        return await base.DeleteAsync(id);
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="password">密码</param>
    /// <returns>验证结果</returns>
    public bool VerifyPassword(SysUser user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
        return result == PasswordVerificationResult.Success;
    }

    /// <summary>
    /// 异步验证密码
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="password">密码</param>
    /// <returns>验证结果</returns>
    public Task<bool> VerifyPasswordAsync(SysUser user, string password)
    {
        // 密码验证是CPU密集型但耗时很短的操作，不需要使用Task.Run
        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
        return Task.FromResult(result == PasswordVerificationResult.Success);
    }
}
