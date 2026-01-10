namespace CasualAdmin.Tests.API.Controllers.System;

using AutoMapper;
using CasualAdmin.API.Controllers.System;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Models.DTOs.Requests.System;
using CasualAdmin.Application.Models.DTOs.Responses.System;
using CasualAdmin.Domain.Entities.System;
using Moq;
using Xunit;

/// <summary>
/// 菜单控制器测试
/// </summary>
public class MenuControllerTests
{
    private readonly Mock<IMenuService> _menuServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly MenuController _menuController;

    /// <summary>
    /// 构造函数，初始化模拟对象和被测控制器
    /// </summary>
    public MenuControllerTests()
    {
        // 初始化模拟对象
        _menuServiceMock = new Mock<IMenuService>();
        _mapperMock = new Mock<IMapper>();

        // 创建被测控制器实例
        _menuController = new MenuController(_menuServiceMock.Object, _mapperMock.Object);
    }

    /// <summary>
    /// 测试获取所有菜单方法，当有菜单时返回成功结果
    /// </summary>
    [Fact]
    public async Task GetAllMenus_ShouldReturnSuccessResult_WhenMenusExist()
    {
        // Arrange
        var menuList = new List<SysMenu> { new SysMenu() };
        var menuDtoList = new List<SysMenuDto> { new SysMenuDto() };

        _menuServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(menuList);
        _mapperMock.Setup(mapper => mapper.Map<List<SysMenuDto>>(menuList)).Returns(menuDtoList);

        // Act
        var result = await _menuController.GetAllMenus();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(menuDtoList, result.Data);
    }

    /// <summary>
    /// 测试根据ID获取菜单方法，当菜单存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task GetMenuById_ShouldReturnSuccessResult_WhenMenuExists()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        var menu = new SysMenu();
        var menuDto = new SysMenuDto();

        _menuServiceMock.Setup(service => service.GetByIdAsync(menuId)).ReturnsAsync(menu);
        _mapperMock.Setup(mapper => mapper.Map<SysMenuDto>(menu)).Returns(menuDto);

        // Act
        var result = await _menuController.GetMenuById(menuId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(menuDto, result.Data);
    }

    /// <summary>
    /// 测试根据ID获取菜单方法，当菜单不存在时返回NotFound结果
    /// </summary>
    [Fact]
    public async Task GetMenuById_ShouldReturnNotFoundResult_WhenMenuDoesNotExist()
    {
        // Arrange
        var menuId = Guid.NewGuid();

        _menuServiceMock.Setup(service => service.GetByIdAsync(menuId)).ReturnsAsync((SysMenu?)null);

        // Act
        var result = await _menuController.GetMenuById(menuId);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(404, result.Code);
    }

    /// <summary>
    /// 测试获取菜单树方法，返回成功结果
    /// </summary>
    [Fact]
    public async Task GetMenuTree_ShouldReturnSuccessResult()
    {
        // Arrange
        var menuTree = new List<SysMenu> { new SysMenu() };
        var menuTreeDto = new List<SysMenuTreeDto> { new SysMenuTreeDto() };

        _menuServiceMock.Setup(service => service.GetAllMenuTreeAsync()).ReturnsAsync(menuTree);
        _mapperMock.Setup(mapper => mapper.Map<List<SysMenuTreeDto>>(menuTree)).Returns(menuTreeDto);

        // Act
        var result = await _menuController.GetMenuTree();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(menuTreeDto, result.Data);
    }

    /// <summary>
    /// 测试根据角色ID获取菜单树方法，返回成功结果
    /// </summary>
    [Fact]
    public async Task GetMenuTreeByRoleId_ShouldReturnSuccessResult()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var menuTree = new List<SysMenu> { new SysMenu() };
        var menuTreeDto = new List<SysMenuTreeDto> { new SysMenuTreeDto() };

        _menuServiceMock.Setup(service => service.GetMenuTreeByRoleIdAsync(roleId)).ReturnsAsync(menuTree);
        _mapperMock.Setup(mapper => mapper.Map<List<SysMenuTreeDto>>(menuTree)).Returns(menuTreeDto);

        // Act
        var result = await _menuController.GetMenuTreeByRoleId(roleId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(menuTreeDto, result.Data);
    }

    /// <summary>
    /// 测试创建菜单方法，返回成功结果
    /// </summary>
    [Fact]
    public async Task CreateMenu_ShouldReturnSuccessResult()
    {
        // Arrange
        var menuCreateDto = new SysMenuCreateDto();
        var menu = new SysMenu();
        var createdMenu = new SysMenu();
        var createdMenuDto = new SysMenuDto();

        _mapperMock.Setup(mapper => mapper.Map<SysMenu>(menuCreateDto)).Returns(menu);
        _menuServiceMock.Setup(service => service.CreateAsync(menu)).ReturnsAsync(createdMenu);
        _mapperMock.Setup(mapper => mapper.Map<SysMenuDto>(createdMenu)).Returns(createdMenuDto);

        // Act
        var result = await _menuController.CreateMenu(menuCreateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(createdMenuDto, result.Data);
    }

    /// <summary>
    /// 测试更新菜单方法，当菜单存在时返回成功结果
    /// </summary>
    [Fact]
    public async Task UpdateMenu_ShouldReturnSuccessResult_WhenMenuExists()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        var menuUpdateDto = new SysMenuUpdateDto();
        var existingMenu = new SysMenu();
        var updatedMenu = new SysMenu();
        var updatedMenuDto = new SysMenuDto();

        _menuServiceMock.Setup(service => service.GetByIdAsync(menuId)).ReturnsAsync(existingMenu);
        _menuServiceMock.Setup(service => service.UpdateAsync(existingMenu)).ReturnsAsync(updatedMenu);
        _mapperMock.Setup(mapper => mapper.Map(It.IsAny<SysMenuUpdateDto>(), It.IsAny<SysMenu>())).Verifiable();
        _mapperMock.Setup(mapper => mapper.Map<SysMenuDto>(updatedMenu)).Returns(updatedMenuDto);

        // Act
        var result = await _menuController.UpdateMenu(menuId, menuUpdateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal(updatedMenuDto, result.Data);
        _mapperMock.Verify(mapper => mapper.Map(menuUpdateDto, existingMenu), Times.Once);
    }

    /// <summary>
    /// 测试更新菜单方法，当菜单不存在时返回NotFound结果
    /// </summary>
    [Fact]
    public async Task UpdateMenu_ShouldReturnNotFoundResult_WhenMenuDoesNotExist()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        var menuUpdateDto = new SysMenuUpdateDto();

        _menuServiceMock.Setup(service => service.GetByIdAsync(menuId)).ReturnsAsync((SysMenu?)null);

        // Act
        var result = await _menuController.UpdateMenu(menuId, menuUpdateDto);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(404, result.Code);
    }

    /// <summary>
    /// 测试更新菜单方法，当更新失败时返回Failed结果
    /// </summary>
    [Fact]
    public async Task UpdateMenu_ShouldReturnFailedResult_WhenUpdateFails()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        var menuUpdateDto = new SysMenuUpdateDto();
        var existingMenu = new SysMenu();

        _menuServiceMock.Setup(service => service.GetByIdAsync(menuId)).ReturnsAsync(existingMenu);
        _menuServiceMock.Setup(service => service.UpdateAsync(existingMenu)).ReturnsAsync((SysMenu?)null);
        _mapperMock.Setup(mapper => mapper.Map(It.IsAny<SysMenuUpdateDto>(), It.IsAny<SysMenu>())).Verifiable();

        // Act
        var result = await _menuController.UpdateMenu(menuId, menuUpdateDto);

        // Assert
        Assert.NotNull(result);

        _mapperMock.Verify(mapper => mapper.Map(menuUpdateDto, existingMenu), Times.Once);
    }

    /// <summary>
    /// 测试删除菜单方法，当删除成功时返回成功结果
    /// </summary>
    [Fact]
    public async Task DeleteMenu_ShouldReturnSuccessResult_WhenDeleteSucceeds()
    {
        // Arrange
        var menuId = Guid.NewGuid();

        _menuServiceMock.Setup(service => service.DeleteAsync(menuId)).ReturnsAsync(true);

        // Act
        var result = await _menuController.DeleteMenu(menuId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.True(result.Data);
    }

    /// <summary>
    /// 测试删除菜单方法，当删除失败时返回Failed结果
    /// </summary>
    [Fact]
    public async Task DeleteMenu_ShouldReturnFailedResult_WhenDeleteFails()
    {
        // Arrange
        var menuId = Guid.NewGuid();

        _menuServiceMock.Setup(service => service.DeleteAsync(menuId)).ReturnsAsync(false);

        // Act
        var result = await _menuController.DeleteMenu(menuId);

        // Assert
        Assert.NotNull(result);

        Assert.False(result.Data);
    }
}