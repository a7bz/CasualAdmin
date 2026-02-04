namespace CasualAdmin.Tests.Application.Services.System;

using CasualAdmin.Application.Interfaces.Events;
using CasualAdmin.Application.Interfaces.Services;
using CasualAdmin.Application.Services.System;
using CasualAdmin.Domain.Entities.System;
using CasualAdmin.Domain.Infrastructure.Data;
using CasualAdmin.Domain.Infrastructure.Services;
using global::System.Data;
using global::System.Linq.Expressions;
using Moq;
using Xunit;

/// <summary>
/// 角色服务测试类
/// </summary>
public class RoleServiceTests
{
    private readonly Mock<IRepository<SysRole>> _roleRepositoryMock;
    private readonly Mock<IValidationService> _validationServiceMock;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly Mock<IRepository<SysUserRole>> _userRoleRepositoryMock;
    private readonly Mock<IRepository<SysUser>> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly RoleService _roleService;

    /// <summary>
    /// 测试类构造函数，初始化模拟对象和被测服务
    /// </summary>
    public RoleServiceTests()
    {
        // 初始化模拟仓储和服务
        _roleRepositoryMock = new Mock<IRepository<SysRole>>();
        _validationServiceMock = new Mock<IValidationService>();
        _eventBusMock = new Mock<IEventBus>();
        _userRoleRepositoryMock = new Mock<IRepository<SysUserRole>>();
        _userRepositoryMock = new Mock<IRepository<SysUser>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _cacheServiceMock = new Mock<ICacheService>();

        // 配置模拟验证服务
        _validationServiceMock.Setup(v => v.ValidateAndThrow(It.IsAny<SysRole>())).Verifiable();

        // 创建被测服务实例
        _roleService = new RoleService(
            _roleRepositoryMock.Object,
            _validationServiceMock.Object,
            _eventBusMock.Object,
            _userRoleRepositoryMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _cacheServiceMock.Object
        );
    }

    /// <summary>
    /// 测试根据ID获取角色方法，当角色存在时返回正确的角色
    /// </summary>
    [Fact]
    public async Task GetRoleByIdAsync_ShouldReturnRole_WhenRoleExists()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var expectedRole = new SysRole { RoleId = roleId, Name = "Admin", Description = "管理员角色" };

        _roleRepositoryMock.Setup(r => r.GetByIdAsync(roleId)).ReturnsAsync(expectedRole);

        // Act
        var actualRole = await _roleService.GetRoleByIdAsync(roleId);

        // Assert
        Assert.NotNull(actualRole);
        Assert.Equal(expectedRole.RoleId, actualRole.RoleId);
        Assert.Equal(expectedRole.Name, actualRole.Name);
        Assert.Equal(expectedRole.Description, actualRole.Description);
        _roleRepositoryMock.Verify(r => r.GetByIdAsync(roleId), Times.Once);
    }

    /// <summary>
    /// 测试根据ID获取角色方法，当角色不存在时返回null
    /// </summary>
    [Fact]
    public async Task GetRoleByIdAsync_ShouldReturnNull_WhenRoleDoesNotExist()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        _roleRepositoryMock.Setup(r => r.GetByIdAsync(roleId)).ReturnsAsync((SysRole?)null);

        // Act
        var actualRole = await _roleService.GetRoleByIdAsync(roleId);

        // Assert
        Assert.Null(actualRole);
        _roleRepositoryMock.Verify(r => r.GetByIdAsync(roleId), Times.Once);
    }

    /// <summary>
    /// 测试获取所有角色方法，返回所有角色列表
    /// 验证返回的角色列表包含正确的角色信息
    /// </summary>
    [Fact]
    public async Task GetAllRolesAsync_ShouldReturnAllRoles()
    {
        // Arrange
        var roles = new List<SysRole>
        {
            new() { RoleId = Guid.NewGuid(), Name = "Admin", Description = "管理员角色" },
            new() { RoleId = Guid.NewGuid(), Name = "User", Description = "普通用户角色" }
        };

        _roleRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(roles);

        // Act
        var result = await _roleService.GetAllRolesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(roles.Count, result.Count);
        for (int i = 0; i < roles.Count; i++)
        {
            Assert.Equal(roles[i].RoleId, result[i].RoleId);
            Assert.Equal(roles[i].Name, result[i].Name);
            Assert.Equal(roles[i].Description, result[i].Description);
        }
        _roleRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    /// <summary>
    /// 测试创建角色方法，确保能够创建角色并返回创建后的角色对象
    /// 验证角色名称和描述是否正确设置，以及创建时间和更新时间是否非默认值
    /// </summary>
    [Fact]
    public async Task CreateRoleAsync_ShouldCreateRole()
    {
        // Arrange
        var role = new SysRole
        {
            Name = "TestRole",
            Description = "测试角色"
        };

        var createdRole = new SysRole
        {
            RoleId = Guid.NewGuid(),
            Name = "TestRole",
            Description = "测试角色",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _roleRepositoryMock.Setup(r => r.AddAsync(It.IsAny<SysRole>())).ReturnsAsync(createdRole);

        // Act
        var result = await _roleService.CreateRoleAsync(role);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdRole.RoleId, result.RoleId);
        Assert.Equal(createdRole.Name, result.Name);
        Assert.Equal(createdRole.Description, result.Description);
        Assert.NotEqual(default, result.CreatedAt);
        Assert.NotEqual(default, result.UpdatedAt);
        _roleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SysRole>()), Times.Once);
    }

    /// <summary>
    /// 测试更新角色方法，确保能够更新角色信息并返回更新后的角色对象
    /// 验证角色名称和描述是否正确更新，以及更新时间是否非默认值
    /// </summary>
    [Fact]
    public async Task UpdateRoleAsync_ShouldUpdateRole_WhenRoleExists()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var existingRole = new SysRole
        {
            RoleId = roleId,
            Name = "OldRole",
            Description = "旧角色描述"
        };

        var updatedRole = new SysRole
        {
            RoleId = roleId,
            Name = "NewRole",
            Description = "新角色描述"
        };

        var savedRole = new SysRole
        {
            RoleId = roleId,
            Name = "NewRole",
            Description = "新角色描述",
            UpdatedAt = DateTime.UtcNow
        };

        _roleRepositoryMock.Setup(r => r.GetByIdAsync(roleId)).ReturnsAsync(existingRole);
        _roleRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<SysRole>())).ReturnsAsync(savedRole);

        // Act
        var result = await _roleService.UpdateRoleAsync(updatedRole);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(savedRole.RoleId, result.RoleId);
        Assert.Equal(savedRole.Name, result.Name);
        Assert.Equal(savedRole.Description, result.Description);
        Assert.Equal(savedRole.UpdatedAt, result.UpdatedAt);
        Assert.NotEqual(default, result.UpdatedAt);
        _roleRepositoryMock.Verify(r => r.GetByIdAsync(roleId), Times.Once);
        _roleRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SysRole>()), Times.Once);
    }

    /// <summary>
    /// 测试更新角色方法，当角色不存在时返回null
    /// </summary>
    [Fact]
    public async Task UpdateRoleAsync_ShouldReturnNull_WhenRoleDoesNotExist()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var role = new SysRole { RoleId = roleId, Name = "NonExistentRole", Description = "不存在的角色" };

        _roleRepositoryMock.Setup(r => r.GetByIdAsync(roleId)).ReturnsAsync((SysRole?)null);

        // Act
        var result = await _roleService.UpdateRoleAsync(role);

        // Assert
        Assert.Null(result);
        _roleRepositoryMock.Verify(r => r.GetByIdAsync(roleId), Times.Once);
        _roleRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SysRole>()), Times.Never);
    }

    /// <summary>
    /// 测试删除角色方法，确保能够删除角色及其关联的用户角色关联记录
    /// 验证数据库操作是否按预期进行，包括事务提交
    /// </summary>
    [Fact]
    public async Task DeleteRoleAsync_ShouldDeleteRoleAndAssociations_WhenRoleExists()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var userRoles = new List<SysUserRole>
        {
            new() { UserId = Guid.NewGuid(), RoleId = roleId, CreatedAt = DateTime.Now }
        };

        _roleRepositoryMock.Setup(r => r.DeleteAsync(roleId)).ReturnsAsync(true);
        _userRoleRepositoryMock.Setup(r => r.FindAsync(ur => ur.RoleId == roleId)).ReturnsAsync(userRoles);
        _userRoleRepositoryMock.Setup(r => r.DeleteRangeAsync(userRoles)).ReturnsAsync(1);

        // Act
        var result = await _roleService.DeleteRoleAsync(roleId);

        // Assert
        Assert.True(result);
        _unitOfWorkMock.Verify(ts => ts.BeginTransaction(IsolationLevel.ReadCommitted), Times.Once);
        _userRoleRepositoryMock.Verify(r => r.FindAsync(ur => ur.RoleId == roleId), Times.Once);
        _userRoleRepositoryMock.Verify(r => r.DeleteRangeAsync(userRoles), Times.Once);
        _roleRepositoryMock.Verify(r => r.DeleteAsync(roleId), Times.Once);
        _unitOfWorkMock.Verify(ts => ts.CommitAsync(), Times.Once);
    }

    /// <summary>
    /// 测试为用户分配角色方法，当用户和角色都存在时成功分配
    /// </summary>
    [Fact]
    public async Task AssignRoleToUserAsync_ShouldReturnTrue_WhenUserAndRoleExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new SysUser());
        _roleRepositoryMock.Setup(r => r.GetByIdAsync(roleId)).ReturnsAsync(new SysRole());
        _userRoleRepositoryMock.Setup(r => r.FindAsync(ur => ur.UserId == userId && ur.RoleId == roleId)).ReturnsAsync([]);
        _userRoleRepositoryMock.Setup(r => r.AddAsync(It.IsAny<SysUserRole>())).ReturnsAsync(new SysUserRole());

        // Act
        var result = await _roleService.AssignRoleToUserAsync(userId, roleId);

        // Assert
        Assert.True(result);
        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _roleRepositoryMock.Verify(r => r.GetByIdAsync(roleId), Times.Once);
        _userRoleRepositoryMock.Verify(r => r.FindAsync(It.IsAny<Expression<Func<SysUserRole, bool>>>()), Times.Once);
        _userRoleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SysUserRole>()), Times.Once);
    }

    /// <summary>
    /// 测试为用户分配角色方法，当用户不存在时返回false
    /// </summary>
    [Fact]
    public async Task AssignRoleToUserAsync_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((SysUser?)null);
        _roleRepositoryMock.Setup(r => r.GetByIdAsync(roleId)).ReturnsAsync(new SysRole { RoleId = roleId });

        // Act
        var result = await _roleService.AssignRoleToUserAsync(userId, roleId);

        // Assert
        Assert.False(result);
        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _roleRepositoryMock.Verify(r => r.GetByIdAsync(roleId), Times.Once);
        _userRoleRepositoryMock.Verify(r => r.FindAsync(It.IsAny<Expression<Func<SysUserRole, bool>>>()), Times.Never);
    }

    /// <summary>
    /// 测试为用户分配角色方法，当角色不存在时返回false
    /// </summary>
    [Fact]
    public async Task AssignRoleToUserAsync_ShouldReturnFalse_WhenRoleDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new SysUser());
        _roleRepositoryMock.Setup(r => r.GetByIdAsync(roleId)).ReturnsAsync((SysRole?)null);

        // Act
        var result = await _roleService.AssignRoleToUserAsync(userId, roleId);

        // Assert
        Assert.False(result);
        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _roleRepositoryMock.Verify(r => r.GetByIdAsync(roleId), Times.Once);
        _userRoleRepositoryMock.Verify(r => r.FindAsync(It.IsAny<Expression<Func<SysUserRole, bool>>>()), Times.Never);
    }

    /// <summary>
    /// 测试获取用户角色列表方法，返回用户的所有角色
    /// 验证返回的角色列表包含正确的角色信息
    /// </summary>
    [Fact]
    public async Task GetRolesByUserIdAsync_ShouldReturnUserRoles()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var userRoles = roleIds.Select(roleId => new SysUserRole { UserId = userId, RoleId = roleId, CreatedAt = DateTime.Now }).ToList();
        var roles = new List<SysRole>
        {
            new() { RoleId = roleIds[0], Name = "Admin", Description = "管理员角色" },
            new() { RoleId = roleIds[1], Name = "User", Description = "普通用户角色" }
        };

        _userRoleRepositoryMock.Setup(r => r.FindAsync(ur => ur.UserId == userId)).ReturnsAsync(userRoles);
        _roleRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<SysRole, bool>>>())).ReturnsAsync(roles);

        // Act
        var result = await _roleService.GetRolesByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(roles.Count, result.Count);
        for (int i = 0; i < roles.Count; i++)
        {
            Assert.Equal(roles[i].RoleId, result[i].RoleId);
            Assert.Equal(roles[i].Name, result[i].Name);
            Assert.Equal(roles[i].Description, result[i].Description);
        }
        _userRoleRepositoryMock.Verify(r => r.FindAsync(It.IsAny<Expression<Func<SysUserRole, bool>>>()), Times.Once);
        _roleRepositoryMock.Verify(r => r.FindAsync(It.IsAny<Expression<Func<SysRole, bool>>>()), Times.Once);
    }

    /// <summary>
    /// 测试获取角色用户列表方法，返回角色的所有用户
    /// 验证返回的用户列表包含正确的用户信息
    /// </summary>
    [Fact]
    public async Task GetUsersByRoleIdAsync_ShouldReturnRoleUsers()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var userIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var userRoles = userIds.Select(userId => new SysUserRole { UserId = userId, RoleId = roleId, CreatedAt = DateTime.Now }).ToList();
        var user1 = new SysUser();
        var user2 = new SysUser();

        var users = new List<SysUser> { user1, user2 };

        _userRoleRepositoryMock.Setup(r => r.FindAsync(ur => ur.RoleId == roleId)).ReturnsAsync(userRoles);
        _userRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<SysUser, bool>>>())).ReturnsAsync(users);

        // Act
        var result = await _roleService.GetUsersByRoleIdAsync(roleId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(users.Count, result.Count);
        for (int i = 0; i < users.Count; i++)
        {
            Assert.Equal(users[i].UserId, result[i].UserId);
            Assert.Equal(users[i].Username, result[i].Username);
            Assert.Equal(users[i].Email, result[i].Email);
        }
        _userRoleRepositoryMock.Verify(r => r.FindAsync(It.IsAny<Expression<Func<SysUserRole, bool>>>()), Times.Once);
        _userRepositoryMock.Verify(r => r.FindAsync(It.IsAny<Expression<Func<SysUser, bool>>>()), Times.Once);
    }

    /// <summary>
    /// 测试删除角色方法，当删除失败时回滚事务
    /// 验证事务回滚，数据库状态未改变
    /// </summary>
    [Fact]
    public async Task DeleteRoleAsync_ShouldRollbackTransaction_WhenDeleteFails()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var userRoles = new List<SysUserRole> { new() { UserId = Guid.NewGuid(), RoleId = roleId, CreatedAt = DateTime.Now } };

        _userRoleRepositoryMock.Setup(r => r.FindAsync(ur => ur.RoleId == roleId)).ReturnsAsync(userRoles);
        _userRoleRepositoryMock.Setup(r => r.DeleteRangeAsync(userRoles)).ReturnsAsync(1);
        _roleRepositoryMock.Setup(r => r.DeleteAsync(roleId)).ThrowsAsync(new Exception("删除失败"));

        // Act
        Func<Task> act = async () => await _roleService.DeleteRoleAsync(roleId);

        // Assert
        await Assert.ThrowsAsync<Exception>(() => _roleService.DeleteRoleAsync(roleId));
        _unitOfWorkMock.Verify(ts => ts.BeginTransaction(IsolationLevel.ReadCommitted), Times.Once);
        _unitOfWorkMock.Verify(ts => ts.Rollback(), Times.Once);
        _unitOfWorkMock.Verify(ts => ts.CommitAsync(), Times.Never);
    }
}
