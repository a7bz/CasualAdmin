namespace CasualAdmin.Tests.Shared.Helpers;
using CasualAdmin.Shared.Helpers;
using Xunit;

/// <summary>
/// PermissionHelper 测试类
/// </summary>
public class PermissionHelperTests
{
    /// <summary>
    /// 测试精确匹配权限
    /// </summary>
    [Fact]
    public void HasPermission_ShouldReturnTrue_WhenExactMatch()
    {
        // Arrange
        var userPermissions = new List<string> { "system:user:add", "system:dept:list" };

        // Act
        var result = PermissionHelper.HasPermission(userPermissions, "system:user:add");

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 测试不匹配权限
    /// </summary>
    [Fact]
    public void HasPermission_ShouldReturnFalse_WhenNoMatch()
    {
        // Arrange
        var userPermissions = new List<string> { "system:user:add", "system:dept:list" };

        // Act
        var result = PermissionHelper.HasPermission(userPermissions, "system:user:delete");

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试通配符权限匹配（操作级通配符）
    /// </summary>
    [Fact]
    public void HasPermission_ShouldReturnTrue_WhenWildcardAtActionLevel()
    {
        // Arrange
        var userPermissions = new List<string> { "system:user:*", "system:dept:list" };

        // Act
        var result = PermissionHelper.HasPermission(userPermissions, "system:user:add");

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 测试通配符权限匹配（功能级通配符）
    /// </summary>
    [Fact]
    public void HasPermission_ShouldReturnTrue_WhenWildcardAtFeatureLevel()
    {
        // Arrange
        var userPermissions = new List<string> { "system:*:*" };

        // Act
        var result = PermissionHelper.HasPermission(userPermissions, "system:dept:add");

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 测试超级管理员权限
    /// </summary>
    [Fact]
    public void HasPermission_ShouldReturnTrue_WhenSuperAdminPermission()
    {
        // Arrange
        var userPermissions = new List<string> { "*:*:*" };

        // Act
        var result = PermissionHelper.HasPermission(userPermissions, "system:user:add");

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 测试空权限列表
    /// </summary>
    [Fact]
    public void HasPermission_ShouldReturnFalse_WhenPermissionsEmpty()
    {
        // Arrange
        var userPermissions = new List<string>();

        // Act
        var result = PermissionHelper.HasPermission(userPermissions, "system:user:add");

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试空权限列表（null）
    /// </summary>
    [Fact]
    public void HasPermission_ShouldReturnFalse_WhenPermissionsNull()
    {
        // Arrange
        List<string>? userPermissions = null;

        // Act
        var result = PermissionHelper.HasPermission(userPermissions!, "system:user:add");

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试权限模式匹配
    /// </summary>
    [Fact]
    public void MatchPermission_ShouldReturnTrue_WhenExactMatch()
    {
        // Act
        var result = PermissionHelper.MatchPermission("system:user:add", "system:user:add");

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 测试权限模式匹配（通配符）
    /// </summary>
    [Fact]
    public void MatchPermission_ShouldReturnTrue_WhenWildcardMatch()
    {
        // Act
        var result = PermissionHelper.MatchPermission("system:user:*", "system:user:add");

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 测试权限模式不匹配
    /// </summary>
    [Fact]
    public void MatchPermission_ShouldReturnFalse_WhenNoMatch()
    {
        // Act
        var result = PermissionHelper.MatchPermission("system:user:add", "system:dept:add");

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试解析权限编码（完整格式）
    /// </summary>
    [Fact]
    public void ParsePermissionCode_ShouldReturnCorrectParts_WhenFullFormat()
    {
        // Act
        var (module, feature, action) = PermissionHelper.ParsePermissionCode("system:user:add");

        // Assert
        Assert.Equal("system", module);
        Assert.Equal("user", feature);
        Assert.Equal("add", action);
    }

    /// <summary>
    /// 测试解析权限编码（只有模块）
    /// </summary>
    [Fact]
    public void ParsePermissionCode_ShouldReturnCorrectParts_WhenOnlyModule()
    {
        // Act
        var (module, feature, action) = PermissionHelper.ParsePermissionCode("system");

        // Assert
        Assert.Equal("system", module);
        Assert.Equal("", feature);
        Assert.Equal("", action);
    }

    /// <summary>
    /// 测试解析权限编码（模块和功能）
    /// </summary>
    [Fact]
    public void ParsePermissionCode_ShouldReturnCorrectParts_WhenModuleAndFeature()
    {
        // Act
        var (module, feature, action) = PermissionHelper.ParsePermissionCode("system:user");

        // Assert
        Assert.Equal("system", module);
        Assert.Equal("user", feature);
        Assert.Equal("", action);
    }

    /// <summary>
    /// 测试生成权限编码（完整格式）
    /// </summary>
    [Fact]
    public void GeneratePermissionCode_ShouldReturnCorrectCode_WhenAllPartsProvided()
    {
        // Act
        var code = PermissionHelper.GeneratePermissionCode("system", "user", "add");

        // Assert
        Assert.Equal("system:user:add", code);
    }

    /// <summary>
    /// 测试生成权限编码（只有模块）
    /// </summary>
    [Fact]
    public void GeneratePermissionCode_ShouldReturnCorrectCode_WhenOnlyModuleProvided()
    {
        // Act
        var code = PermissionHelper.GeneratePermissionCode("system");

        // Assert
        Assert.Equal("system:*:*", code);
    }

    /// <summary>
    /// 测试生成权限编码（模块和功能）
    /// </summary>
    [Fact]
    public void GeneratePermissionCode_ShouldReturnCorrectCode_WhenModuleAndFeatureProvided()
    {
        // Act
        var code = PermissionHelper.GeneratePermissionCode("system", "user");

        // Assert
        Assert.Equal("system:user:*", code);
    }

    /// <summary>
    /// 测试获取模块权限
    /// </summary>
    [Fact]
    public void GetModulePermissions_ShouldReturnCorrectPermissions()
    {
        // Arrange
        var userPermissions = new List<string>
        {
            "system:user:add",
            "system:user:edit",
            "system:dept:list",
            "system:dept:add",
            "workflow:task:view"
        };

        // Act
        var modulePermissions = PermissionHelper.GetModulePermissions(userPermissions, "system");

        // Assert
        Assert.Equal(4, modulePermissions.Count);
        Assert.Contains("system:user:add", modulePermissions);
        Assert.Contains("system:user:edit", modulePermissions);
        Assert.Contains("system:dept:list", modulePermissions);
        Assert.Contains("system:dept:add", modulePermissions);
        Assert.DoesNotContain("workflow:task:view", modulePermissions);
    }

    /// <summary>
    /// 测试检查模块权限（有权限）
    /// </summary>
    [Fact]
    public void HasAnyModulePermission_ShouldReturnTrue_WhenHasModulePermissions()
    {
        // Arrange
        var userPermissions = new List<string> { "system:user:add", "system:dept:list" };

        // Act
        var result = PermissionHelper.HasAnyModulePermission(userPermissions, "system");

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 测试检查模块权限（通配符）
    /// </summary>
    [Fact]
    public void HasAnyModulePermission_ShouldReturnTrue_WhenHasWildcardPermission()
    {
        // Arrange
        var userPermissions = new List<string> { "system:*:*" };

        // Act
        var result = PermissionHelper.HasAnyModulePermission(userPermissions, "system");

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 测试检查模块权限（无权限）
    /// </summary>
    [Fact]
    public void HasAnyModulePermission_ShouldReturnFalse_WhenNoModulePermissions()
    {
        // Arrange
        var userPermissions = new List<string> { "workflow:task:view", "system:user:add" };

        // Act
        var result = PermissionHelper.HasAnyModulePermission(userPermissions, "dept");

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试通配符不匹配不同层级
    /// </summary>
    [Fact]
    public void HasPermission_ShouldReturnFalse_WhenWildcardNotMatchDifferentLevel()
    {
        // Arrange
        var userPermissions = new List<string> { "system:user:add" };

        // Act
        var result = PermissionHelper.HasPermission(userPermissions, "system:user");

        // Assert
        Assert.False(result);
    }
}