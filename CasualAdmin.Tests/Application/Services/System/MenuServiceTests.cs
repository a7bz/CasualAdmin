namespace CasualAdmin.Tests.Application.Services.System;

using CasualAdmin.Application.Interfaces.Events;
using CasualAdmin.Application.Interfaces.Services;
using CasualAdmin.Application.Services.System;
using CasualAdmin.Domain.Entities.System;
using CasualAdmin.Domain.Infrastructure.Data;
using CasualAdmin.Domain.Infrastructure.Services;
using Moq;
using Xunit;

/// <summary>
/// 菜单服务测试
/// </summary>
public class MenuServiceTests
{
    private readonly Mock<IRepository<SysMenu>> _menuRepositoryMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<IValidationService> _validationServiceMock;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly MenuService _menuService;

    /// <summary>
    /// 测试类构造函数，初始化模拟对象和被测服务
    /// </summary>
    public MenuServiceTests()
    {
        // 初始化模拟对象
        _menuRepositoryMock = new Mock<IRepository<SysMenu>>();
        _cacheServiceMock = new Mock<ICacheService>();
        _validationServiceMock = new Mock<IValidationService>();
        _eventBusMock = new Mock<IEventBus>();

        // 配置模拟验证服务
        _validationServiceMock.Setup(v => v.ValidateAndThrow(It.IsAny<SysMenu>())).Verifiable();

        // 创建被测服务实例
        _menuService = new MenuService(_menuRepositoryMock.Object, _cacheServiceMock.Object, _validationServiceMock.Object, _eventBusMock.Object);
    }

    /// <summary>
    /// 测试根据ID获取菜单方法，当菜单存在时返回正确的菜单
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnMenu_WhenMenuExists()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        var expectedMenu = new SysMenu
        {
            MenuName = "Test Menu",
            Path = "/test",
            Component = "TestComponent",
            Icon = "test-icon"
        };

        _menuRepositoryMock.Setup(r => r.GetByIdAsync(menuId)).ReturnsAsync(expectedMenu);

        // Act
        var actualMenu = await _menuService.GetByIdAsync(menuId);

        // Assert
        Assert.NotNull(actualMenu);
        Assert.Equal(expectedMenu.MenuId, actualMenu.MenuId);
        Assert.Equal("Test Menu", actualMenu.MenuName);
        Assert.Equal("/test", actualMenu.Path);
        Assert.Equal("TestComponent", actualMenu.Component);
        Assert.Equal("test-icon", actualMenu.Icon);
        _menuRepositoryMock.Verify(r => r.GetByIdAsync(menuId), Times.Once);
    }

    /// <summary>
    /// 测试获取所有菜单方法，返回所有菜单列表
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllMenus()
    {
        // Arrange
        var menu1 = new SysMenu
        {
            MenuName = "Menu 1",
            Path = "/menu1",
            Component = "Menu1Component"
        };

        var menu2 = new SysMenu
        {
            MenuName = "Menu 2",
            Path = "/menu2",
            Component = "Menu2Component"
        };

        var menus = new List<SysMenu> { menu1, menu2 };

        _menuRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(menus);

        // Act
        var result = await _menuService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(menus.Count, result.Count);
        Assert.Equal("Menu 1", result[0].MenuName);
        Assert.Equal("Menu 2", result[1].MenuName);
        _menuRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    /// <summary>
    /// 测试创建菜单方法，确保能够创建菜单并返回正确的菜单对象
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldCreateMenu()
    {
        // Arrange
        var menu = new SysMenu
        {
            MenuName = "New Menu",
            Path = "/new-menu",
            Component = "NewMenuComponent",
            Icon = "new-icon"
        };

        var createdMenu = new SysMenu
        {
            MenuName = "New Menu",
            Path = "/new-menu",
            Component = "NewMenuComponent",
            Icon = "new-icon"
        };

        _menuRepositoryMock.Setup(r => r.AddAsync(It.IsAny<SysMenu>())).ReturnsAsync(createdMenu);

        // Act
        var result = await _menuService.CreateAsync(menu);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdMenu.MenuId, result.MenuId);
        Assert.Equal(createdMenu.MenuName, result.MenuName);
        Assert.Equal(createdMenu.Path, result.Path);
        Assert.Equal(createdMenu.Component, result.Component);
        Assert.NotEqual(default, result.CreatedAt);
        Assert.NotEqual(default, result.UpdatedAt);
        _menuRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SysMenu>()), Times.Once);
    }

    /// <summary>
    /// 测试更新菜单方法，当菜单存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ShouldUpdateMenu_WhenMenuExists()
    {
        // Arrange
        var existingMenu = new SysMenu
        {
            MenuName = "Old Menu",
            Path = "/old-menu",
            Component = "OldMenuComponent"
        };

        var updatedMenu = new SysMenu
        {
            MenuId = existingMenu.MenuId,
            MenuName = "Updated Menu",
            Path = "/updated-menu",
            Component = "UpdatedMenuComponent"
        };

        var savedMenu = new SysMenu
        {
            MenuId = existingMenu.MenuId,
            MenuName = "Updated Menu",
            Path = "/updated-menu",
            Component = "UpdatedMenuComponent"
        };

        _menuRepositoryMock.Setup(r => r.GetByIdAsync(existingMenu.MenuId)).ReturnsAsync(existingMenu);
        _menuRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<SysMenu>())).ReturnsAsync(savedMenu);

        // Act
        var result = await _menuService.UpdateAsync(updatedMenu);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(savedMenu.MenuId, result.MenuId);
        Assert.Equal("Updated Menu", result.MenuName);
        Assert.Equal("/updated-menu", result.Path);
        Assert.Equal("UpdatedMenuComponent", result.Component);
        _menuRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SysMenu>()), Times.Once);
    }

    /// <summary>
    /// 测试删除菜单方法，当菜单存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenMenuExists()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        _menuRepositoryMock.Setup(r => r.DeleteAsync(menuId)).ReturnsAsync(true);

        // Act
        var result = await _menuService.DeleteAsync(menuId);

        // Assert
        Assert.True(result);
        _menuRepositoryMock.Verify(r => r.DeleteAsync(menuId), Times.Once);
    }

    /// <summary>
    /// 测试删除菜单方法，当菜单不存在时返回失败结果
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenMenuDoesNotExist()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        _menuRepositoryMock.Setup(r => r.DeleteAsync(menuId)).ReturnsAsync(false);

        // Act
        var result = await _menuService.DeleteAsync(menuId);

        // Assert
        Assert.False(result);
        _menuRepositoryMock.Verify(r => r.DeleteAsync(menuId), Times.Once);
    }
}
