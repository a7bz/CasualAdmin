namespace CasualAdmin.Application.Services.System;

using CasualAdmin.Domain.Common;
using CasualAdmin.Domain.Entities.System;
using CasualAdmin.Domain.Infrastructure.Data;
using CasualAdmin.Domain.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

/// <summary>
/// 数据初始化服务实现
/// </summary>
public class SeedService : ISeedService
{
    private readonly IRepository<SysTenant> _tenantRepository;
    private readonly IRepository<SysDept> _deptRepository;
    private readonly IRepository<SysRole> _roleRepository;
    private readonly IRepository<SysUser> _userRepository;
    private readonly IRepository<SysUserRole> _userRoleRepository;
    private readonly IRepository<SysMenu> _menuRepository;
    private readonly IRepository<SysPermission> _permissionRepository;
    private readonly IRepository<SysRolePermission> _rolePermissionRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SeedService> _logger;
    private readonly PasswordHasher<SysUser> _passwordHasher;

    // 固定的种子数据ID，确保幂等性
    private static readonly Guid DefaultTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid RootDeptId = Guid.Parse("00000000-0000-0000-0000-000000000002");
    private static readonly Guid SuperAdminRoleId = Guid.Parse("00000000-0000-0000-0000-000000000003");
    private static readonly Guid SuperAdminUserId = Guid.Parse("00000000-0000-0000-0000-000000000004");

    public SeedService(
        IRepository<SysTenant> tenantRepository,
        IRepository<SysDept> deptRepository,
        IRepository<SysRole> roleRepository,
        IRepository<SysUser> userRepository,
        IRepository<SysUserRole> userRoleRepository,
        IRepository<SysMenu> menuRepository,
        IRepository<SysPermission> permissionRepository,
        IRepository<SysRolePermission> rolePermissionRepository,
        IConfiguration configuration,
        ILogger<SeedService> logger)
    {
        _tenantRepository = tenantRepository;
        _deptRepository = deptRepository;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _menuRepository = menuRepository;
        _permissionRepository = permissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _configuration = configuration;
        _logger = logger;
        _passwordHasher = new PasswordHasher<SysUser>();
    }

    /// <summary>
    /// 检查是否需要初始化
    /// </summary>
    public async Task<bool> NeedsSeedAsync()
    {
        var hasUsers = await _userRepository.AnyAsync(u => !u.IsDeleted);
        return !hasUsers;
    }

    /// <summary>
    /// 执行数据初始化
    /// </summary>
    public async Task SeedAsync()
    {
        if (!await NeedsSeedAsync())
        {
            _logger.LogInformation("数据库已有数据，跳过初始化");
            return;
        }

        _logger.LogInformation("开始执行数据初始化...");

        try
        {
            // 1. 初始化默认租户
            await SeedTenantAsync();

            // 2. 初始化根部门
            await SeedDeptAsync();

            // 3. 初始化超级管理员角色
            await SeedRoleAsync();

            // 4. 初始化超级管理员用户
            await SeedUserAsync();

            // 5. 初始化菜单
            await SeedMenusAsync();

            // 6. 初始化权限
            await SeedPermissionsAsync();

            // 7. 关联角色权限
            await SeedRolePermissionsAsync();

            _logger.LogInformation("数据初始化完成");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "数据初始化失败");
            throw;
        }
    }

    /// <summary>
    /// 初始化默认租户
    /// </summary>
    private async Task SeedTenantAsync()
    {
        var exists = await _tenantRepository.AnyAsync(t => t.TenantId == DefaultTenantId);
        if (exists)
        {
            _logger.LogDebug("默认租户已存在，跳过");
            return;
        }

        var tenant = new SysTenant
        {
            TenantId = DefaultTenantId,
            TenantName = "默认租户",
            TenantCode = "DEFAULT",
            Contact = "系统管理员",
            Phone = "00000000000",
            Email = "admin@casualadmin.local",
            Status = Status.Enabled,
            ExpireTime = null, // 永不过期
            Remark = "系统默认租户",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _tenantRepository.AddAsync(tenant);
        _logger.LogInformation("已创建默认租户: {TenantName}", tenant.TenantName);
    }

    /// <summary>
    /// 初始化根部门
    /// </summary>
    private async Task SeedDeptAsync()
    {
        var exists = await _deptRepository.AnyAsync(d => d.DeptId == RootDeptId);
        if (exists)
        {
            _logger.LogDebug("根部门已存在，跳过");
            return;
        }

        var dept = new SysDept();
        // 使用反射设置私有属性
        typeof(SysDept).GetProperty("DeptId")?.SetValue(dept, RootDeptId);
        typeof(SysDept).GetProperty("DeptName")?.SetValue(dept, "总公司");
        typeof(SysDept).GetProperty("DeptCode")?.SetValue(dept, "ROOT");
        typeof(SysDept).GetProperty("Leader")?.SetValue(dept, "超级管理员");
        typeof(SysDept).GetProperty("Phone")?.SetValue(dept, "");
        typeof(SysDept).GetProperty("Sort")?.SetValue(dept, 0);
        typeof(SysDept).GetProperty("Status")?.SetValue(dept, Status.Enabled);
        typeof(SysDept).GetProperty("Remark")?.SetValue(dept, "根部门");
        dept.CreatedAt = DateTime.Now;
        dept.UpdatedAt = DateTime.Now;

        await _deptRepository.AddAsync(dept);
        _logger.LogInformation("已创建根部门: {DeptName}", "总公司");
    }

    /// <summary>
    /// 初始化超级管理员角色
    /// </summary>
    private async Task SeedRoleAsync()
    {
        var exists = await _roleRepository.AnyAsync(r => r.RoleId == SuperAdminRoleId);
        if (exists)
        {
            _logger.LogDebug("超级管理员角色已存在，跳过");
            return;
        }

        var role = new SysRole
        {
            RoleId = SuperAdminRoleId,
            Name = "超级管理员",
            RoleCode = "SUPER_ADMIN",
            RoleType = 0, // 系统角色
            Description = "系统超级管理员，拥有所有权限",
            Status = Status.Enabled,
            Sort = 0,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _roleRepository.AddAsync(role);
        _logger.LogInformation("已创建超级管理员角色: {Name}", role.Name);
    }

    /// <summary>
    /// 初始化超级管理员用户
    /// </summary>
    private async Task SeedUserAsync()
    {
        var exists = await _userRepository.AnyAsync(u => u.UserId == SuperAdminUserId);
        if (exists)
        {
            _logger.LogDebug("超级管理员用户已存在，跳过");
            return;
        }

        var user = new SysUser();
        typeof(SysUser).GetProperty("UserId")?.SetValue(user, SuperAdminUserId);
        typeof(SysUser).GetProperty("Username")?.SetValue(user, "casual_admin");
        typeof(SysUser).GetProperty("RealName")?.SetValue(user, "超级管理员");
        typeof(SysUser).GetProperty("Email")?.SetValue(user, "admin@casualadmin.local");
        typeof(SysUser).GetProperty("Phone")?.SetValue(user, "");
        typeof(SysUser).GetProperty("Avatar")?.SetValue(user, "");
        typeof(SysUser).GetProperty("Gender")?.SetValue(user, Gender.Unknown);
        typeof(SysUser).GetProperty("DeptId")?.SetValue(user, RootDeptId);
        typeof(SysUser).GetProperty("Position")?.SetValue(user, "系统管理员");
        typeof(SysUser).GetProperty("Status")?.SetValue(user, Status.Enabled);

        // 密码处理
        var password = "admin1223";
        var hashedPassword = _passwordHasher.HashPassword(user, password);
        typeof(SysUser).GetProperty("Password")?.SetValue(user, hashedPassword);
        typeof(SysUser).GetProperty("Salt")?.SetValue(user, ""); // PasswordHasher 已包含盐值

        user.CreatedAt = DateTime.Now;
        user.UpdatedAt = DateTime.Now;

        await _userRepository.AddAsync(user);

        // 创建用户角色关联
        var userRole = new SysUserRole
        {
            UserId = SuperAdminUserId,
            RoleId = SuperAdminRoleId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _userRoleRepository.AddAsync(userRole);
        _logger.LogInformation("已创建超级管理员用户: casual_admin / admin1223");
    }

    /// <summary>
    /// 初始化菜单
    /// </summary>
    private async Task SeedMenusAsync()
    {
        var exists = await _menuRepository.AnyAsync(m => !m.IsDeleted);
        if (exists)
        {
            _logger.LogDebug("菜单数据已存在，跳过");
            return;
        }

        var menus = new List<SysMenu>
        {
            new()
            {
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                ParentId = null,
                MenuName = "系统管理",
                MenuType = 0, // 目录
                Path = "/system",
                Component = "Layout",
                Permission = "",
                Icon = "setting",
                Sort = 1,
                Status = Status.Enabled,
                IsVisible = 1,
                IsCache = 0,
                Remark = "系统管理目录"
            },
            new()
            {
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                ParentId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                MenuName = "用户管理",
                MenuType = 1, // 菜单
                Path = "/system/user",
                Component = "system/user/index",
                Permission = "system:user:list",
                Icon = "user",
                Sort = 1,
                Status = Status.Enabled,
                IsVisible = 1,
                IsCache = 0,
                Remark = "用户管理菜单"
            },
            new()
            {
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                ParentId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                MenuName = "角色管理",
                MenuType = 1,
                Path = "/system/role",
                Component = "system/role/index",
                Permission = "system:role:list",
                Icon = "peoples",
                Sort = 2,
                Status = Status.Enabled,
                IsVisible = 1,
                IsCache = 0,
                Remark = "角色管理菜单"
            },
            new()
            {
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                ParentId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                MenuName = "部门管理",
                MenuType = 1,
                Path = "/system/dept",
                Component = "system/dept/index",
                Permission = "system:dept:list",
                Icon = "tree",
                Sort = 3,
                Status = Status.Enabled,
                IsVisible = 1,
                IsCache = 0,
                Remark = "部门管理菜单"
            },
            new()
            {
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                ParentId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                MenuName = "菜单管理",
                MenuType = 1,
                Path = "/system/menu",
                Component = "system/menu/index",
                Permission = "system:menu:list",
                Icon = "tree-table",
                Sort = 4,
                Status = Status.Enabled,
                IsVisible = 1,
                IsCache = 0,
                Remark = "菜单管理菜单"
            },
            new()
            {
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                ParentId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                MenuName = "权限管理",
                MenuType = 1,
                Path = "/system/permission",
                Component = "system/permission/index",
                Permission = "system:permission:list",
                Icon = "lock",
                Sort = 5,
                Status = Status.Enabled,
                IsVisible = 1,
                IsCache = 0,
                Remark = "权限管理菜单"
            }
        };

        foreach (var menu in menus)
        {
            menu.CreatedAt = DateTime.Now;
            menu.UpdatedAt = DateTime.Now;
            await _menuRepository.AddAsync(menu);
        }

        _logger.LogInformation("已创建 {Count} 个系统菜单", menus.Count);
    }

    /// <summary>
    /// 初始化权限
    /// </summary>
    private async Task SeedPermissionsAsync()
    {
        var exists = await _permissionRepository.AnyAsync(p => !p.IsDeleted);
        if (exists)
        {
            _logger.LogDebug("权限数据已存在，跳过");
            return;
        }

        var permissions = new List<SysPermission>
        {
            // 用户管理权限
            new()
            {
                PermissionId = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                PermissionName = "用户查询",
                PermissionCode = "system:user:query",
                PermissionType = PermissionType.Function,
                Module = "system",
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Status = Status.Enabled,
                Sort = 1,
                Remark = "查询用户列表"
            },
            new()
            {
                PermissionId = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                PermissionName = "用户新增",
                PermissionCode = "system:user:add",
                PermissionType = PermissionType.Function,
                Module = "system",
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Status = Status.Enabled,
                Sort = 2,
                Remark = "新增用户"
            },
            new()
            {
                PermissionId = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                PermissionName = "用户编辑",
                PermissionCode = "system:user:edit",
                PermissionType = PermissionType.Function,
                Module = "system",
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Status = Status.Enabled,
                Sort = 3,
                Remark = "编辑用户"
            },
            new()
            {
                PermissionId = Guid.Parse("20000000-0000-0000-0000-000000000004"),
                PermissionName = "用户删除",
                PermissionCode = "system:user:delete",
                PermissionType = PermissionType.Function,
                Module = "system",
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Status = Status.Enabled,
                Sort = 4,
                Remark = "删除用户"
            },
            // 角色管理权限
            new()
            {
                PermissionId = Guid.Parse("20000000-0000-0000-0000-000000000005"),
                PermissionName = "角色查询",
                PermissionCode = "system:role:query",
                PermissionType = PermissionType.Function,
                Module = "system",
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Status = Status.Enabled,
                Sort = 1,
                Remark = "查询角色列表"
            },
            new()
            {
                PermissionId = Guid.Parse("20000000-0000-0000-0000-000000000006"),
                PermissionName = "角色新增",
                PermissionCode = "system:role:add",
                PermissionType = PermissionType.Function,
                Module = "system",
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Status = Status.Enabled,
                Sort = 2,
                Remark = "新增角色"
            },
            new()
            {
                PermissionId = Guid.Parse("20000000-0000-0000-0000-000000000007"),
                PermissionName = "角色编辑",
                PermissionCode = "system:role:edit",
                PermissionType = PermissionType.Function,
                Module = "system",
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Status = Status.Enabled,
                Sort = 3,
                Remark = "编辑角色"
            },
            new()
            {
                PermissionId = Guid.Parse("20000000-0000-0000-0000-000000000008"),
                PermissionName = "角色删除",
                PermissionCode = "system:role:delete",
                PermissionType = PermissionType.Function,
                Module = "system",
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Status = Status.Enabled,
                Sort = 4,
                Remark = "删除角色"
            },
            // 部门管理权限
            new()
            {
                PermissionId = Guid.Parse("20000000-0000-0000-0000-000000000009"),
                PermissionName = "部门查询",
                PermissionCode = "system:dept:query",
                PermissionType = PermissionType.Function,
                Module = "system",
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                Status = Status.Enabled,
                Sort = 1,
                Remark = "查询部门列表"
            },
            new()
            {
                PermissionId = Guid.Parse("20000000-0000-0000-0000-000000000010"),
                PermissionName = "部门新增",
                PermissionCode = "system:dept:add",
                PermissionType = PermissionType.Function,
                Module = "system",
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                Status = Status.Enabled,
                Sort = 2,
                Remark = "新增部门"
            },
            new()
            {
                PermissionId = Guid.Parse("20000000-0000-0000-0000-000000000011"),
                PermissionName = "部门编辑",
                PermissionCode = "system:dept:edit",
                PermissionType = PermissionType.Function,
                Module = "system",
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                Status = Status.Enabled,
                Sort = 3,
                Remark = "编辑部门"
            },
            new()
            {
                PermissionId = Guid.Parse("20000000-0000-0000-0000-000000000012"),
                PermissionName = "部门删除",
                PermissionCode = "system:dept:delete",
                PermissionType = PermissionType.Function,
                Module = "system",
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                Status = Status.Enabled,
                Sort = 4,
                Remark = "删除部门"
            },
            // 菜单管理权限
            new()
            {
                PermissionId = Guid.Parse("20000000-0000-0000-0000-000000000013"),
                PermissionName = "菜单查询",
                PermissionCode = "system:menu:query",
                PermissionType = PermissionType.Function,
                Module = "system",
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                Status = Status.Enabled,
                Sort = 1,
                Remark = "查询菜单列表"
            },
            new()
            {
                PermissionId = Guid.Parse("20000000-0000-0000-0000-000000000014"),
                PermissionName = "菜单新增",
                PermissionCode = "system:menu:add",
                PermissionType = PermissionType.Function,
                Module = "system",
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                Status = Status.Enabled,
                Sort = 2,
                Remark = "新增菜单"
            },
            new()
            {
                PermissionId = Guid.Parse("20000000-0000-0000-0000-000000000015"),
                PermissionName = "菜单编辑",
                PermissionCode = "system:menu:edit",
                PermissionType = PermissionType.Function,
                Module = "system",
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                Status = Status.Enabled,
                Sort = 3,
                Remark = "编辑菜单"
            },
            new()
            {
                PermissionId = Guid.Parse("20000000-0000-0000-0000-000000000016"),
                PermissionName = "菜单删除",
                PermissionCode = "system:menu:delete",
                PermissionType = PermissionType.Function,
                Module = "system",
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                Status = Status.Enabled,
                Sort = 4,
                Remark = "删除菜单"
            },
            // 权限管理权限
            new()
            {
                PermissionId = Guid.Parse("20000000-0000-0000-0000-000000000017"),
                PermissionName = "权限查询",
                PermissionCode = "system:permission:query",
                PermissionType = PermissionType.Function,
                Module = "system",
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                Status = Status.Enabled,
                Sort = 1,
                Remark = "查询权限列表"
            },
            new()
            {
                PermissionId = Guid.Parse("20000000-0000-0000-0000-000000000018"),
                PermissionName = "权限新增",
                PermissionCode = "system:permission:add",
                PermissionType = PermissionType.Function,
                Module = "system",
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                Status = Status.Enabled,
                Sort = 2,
                Remark = "新增权限"
            },
            new()
            {
                PermissionId = Guid.Parse("20000000-0000-0000-0000-000000000019"),
                PermissionName = "权限编辑",
                PermissionCode = "system:permission:edit",
                PermissionType = PermissionType.Function,
                Module = "system",
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                Status = Status.Enabled,
                Sort = 3,
                Remark = "编辑权限"
            },
            new()
            {
                PermissionId = Guid.Parse("20000000-0000-0000-0000-000000000020"),
                PermissionName = "权限删除",
                PermissionCode = "system:permission:delete",
                PermissionType = PermissionType.Function,
                Module = "system",
                MenuId = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                Status = Status.Enabled,
                Sort = 4,
                Remark = "删除权限"
            }
        };

        foreach (var permission in permissions)
        {
            permission.CreatedAt = DateTime.Now;
            permission.UpdatedAt = DateTime.Now;
            await _permissionRepository.AddAsync(permission);
        }

        _logger.LogInformation("已创建 {Count} 个系统权限", permissions.Count);
    }

    /// <summary>
    /// 初始化角色权限关联
    /// </summary>
    private async Task SeedRolePermissionsAsync()
    {
        var exists = await _rolePermissionRepository.AnyAsync(rp => rp.RoleId == SuperAdminRoleId);
        if (exists)
        {
            _logger.LogDebug("角色权限关联已存在，跳过");
            return;
        }

        // 获取所有权限
        var permissions = await _permissionRepository.GetAllAsync();

        var rolePermissions = permissions.Select(p => new SysRolePermission
        {
            RoleId = SuperAdminRoleId,
            PermissionId = p.PermissionId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        }).ToList();

        foreach (var rp in rolePermissions)
        {
            await _rolePermissionRepository.AddAsync(rp);
        }

        _logger.LogInformation("已为超级管理员角色分配 {Count} 个权限", rolePermissions.Count);
    }
}
