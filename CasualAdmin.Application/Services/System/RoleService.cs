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
    /// 角色服务实现
    /// </summary>
    public class RoleService : BaseService<SysRole>, IRoleService
    {
        private readonly IRepository<SysUserRole> _userRoleRepository;
        private readonly IRepository<SysUser> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="roleRepository">角色仓储</param>
        /// <param name="validationService">验证服务</param>
        /// <param name="eventBus">事件总线</param>
        /// <param name="userRoleRepository">用户角色关联仓储</param>
        /// <param name="userRepository">用户仓储</param>
        /// <param name="unitOfWork">工作单元</param>
        /// <param name="cacheService">缓存服务</param>
        public RoleService(IRepository<SysRole> roleRepository, IValidationService validationService, IEventBus eventBus, IRepository<SysUserRole> userRoleRepository, IRepository<SysUser> userRepository, IUnitOfWork unitOfWork, ICacheService cacheService)
            : base(roleRepository, validationService, eventBus)
        {
            _userRoleRepository = userRoleRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        /// <summary>
        /// 根据ID获取角色
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns>角色实体，不存在则返回null</returns>
        public async Task<SysRole?> GetRoleByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns>角色列表</returns>
        public async Task<List<SysRole>> GetAllRolesAsync()
        {
            return await _repository.GetAllAsync();
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="role">角色实体</param>
        /// <returns>创建的角色</returns>
        public async Task<SysRole> CreateRoleAsync(SysRole role)
        {
            role.CreatedAt = DateTime.UtcNow;
            role.UpdatedAt = DateTime.UtcNow;
            return await _repository.AddAsync(role);
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="role">角色实体</param>
        /// <returns>更新的角色</returns>
        public async Task<SysRole?> UpdateRoleAsync(SysRole role)
        {
            var existingRole = await GetRoleByIdAsync(role.RoleId);
            if (existingRole == null)
            {
                return null;
            }

            role.UpdatedAt = DateTime.UtcNow;
            return await _repository.UpdateAsync(role);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns>删除结果</returns>
        public async Task<bool> DeleteRoleAsync(Guid id)
        {
            try
            {
                // 开始事务
                _unitOfWork.BeginTransaction();

                // 先删除用户角色关联
                var userRoles = await _userRoleRepository.FindAsync(ur => ur.RoleId == id);
                if (userRoles.Count != 0)
                {
                    await _userRoleRepository.DeleteRangeAsync(userRoles);
                }

                // 删除角色
                var result = await _repository.DeleteAsync(id);

                // 提交事务
                await _unitOfWork.CommitAsync();

                return result;
            }
            catch
            {
                // 回滚事务
                _unitOfWork.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 为用户分配角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleId">角色ID</param>
        /// <returns>分配结果</returns>
        public async Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId)
        {
            // 检查用户和角色是否存在
            var user = await _userRepository.GetByIdAsync(userId);
            var role = await GetRoleByIdAsync(roleId);
            if (user == null || role == null)
            {
                return false;
            }

            // 检查是否已经存在关联
            var existingUserRole = await _userRoleRepository.FindAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
            if (existingUserRole.Count != 0)
            {
                return true; // 已经存在关联，返回成功
            }

            // 创建新的关联
            var userRole = new SysUserRole
            {
                UserId = userId,
                RoleId = roleId,
                CreatedAt = DateTime.UtcNow
            };

            await _userRoleRepository.AddAsync(userRole);

            // 清除用户权限缓存
            var cacheKey = $"user_permissions:{userId}";
            await _cacheService.RemoveAsync(cacheKey);

            return true;
        }

        /// <summary>
        /// 移除用户角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleId">角色ID</param>
        /// <returns>移除结果</returns>
        public async Task<bool> RemoveRoleFromUserAsync(Guid userId, Guid roleId)
        {
            // 查找用户角色关联
            var userRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
            if (userRoles.Count == 0)
            {
                return true; // 不存在关联，返回成功
            }

            // 删除关联
            await _userRoleRepository.DeleteRangeAsync(userRoles);

            // 清除用户权限缓存
            var cacheKey = $"user_permissions:{userId}";
            await _cacheService.RemoveAsync(cacheKey);

            return true;
        }

        /// <summary>
        /// 获取用户角色列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>角色列表</returns>
        public async Task<List<SysRole>> GetRolesByUserIdAsync(Guid userId)
        {
            // 查询用户角色关联
            var userRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == userId);
            if (userRoles.Count == 0)
            {
                return new List<SysRole>();
            }

            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
            return await _repository.FindAsync(r => roleIds.Contains(r.RoleId));
        }

        /// <summary>
        /// 根据角色名称列表获取角色
        /// </summary>
        /// <param name="roleNames">角色名称列表</param>
        /// <returns>角色列表</returns>
        public async Task<List<SysRole>> GetRolesByNamesAsync(List<string> roleNames)
        {
            if (roleNames == null || roleNames.Count == 0)
            {
                return new List<SysRole>();
            }

            return await _repository.FindAsync(r => roleNames.Contains(r.Name));
        }

        /// <summary>
        /// 获取角色用户列表
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>用户列表</returns>
        public async Task<List<SysUser>> GetUsersByRoleIdAsync(Guid roleId)
        {
            // 查询所有关联了该角色的用户
            var userRoles = await _userRoleRepository.FindAsync(ur => ur.RoleId == roleId);
            if (userRoles.Count == 0)
            {
                return [];
            }

            var userIds = userRoles.Select(ur => ur.UserId).ToList();
            return await _userRepository.FindAsync(u => userIds.Contains(u.UserId));
        }
    }
}