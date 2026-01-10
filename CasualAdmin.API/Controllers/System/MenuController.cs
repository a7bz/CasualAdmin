namespace CasualAdmin.API.Controllers.System;
using AutoMapper;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Models.DTOs.Requests.System;
using CasualAdmin.Application.Models.DTOs.Responses.System;
using CasualAdmin.Domain.Entities.System;
using CasualAdmin.Shared.Common;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// 菜单控制器
/// </summary>
[ApiController]
[Route("[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;
    private readonly IMapper _mapper;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="menuService">菜单服务</param>
    /// <param name="mapper">AutoMapper实例</param>
    public MenuController(IMenuService menuService, IMapper mapper)
    {
        _menuService = menuService;
        _mapper = mapper;
    }

    /// <summary>
    /// 获取所有菜单
    /// </summary>
    /// <returns>菜单列表</returns>
    [HttpGet]
    public async Task<ApiResponse<List<SysMenuDto>>> GetAllMenus()
    {
        var menus = await _menuService.GetAllAsync();
        var menuDtos = _mapper.Map<List<SysMenuDto>>(menus);
        return ApiResponse<List<SysMenuDto>>.Success(menuDtos, "获取成功");
    }

    /// <summary>
    /// 根据ID获取菜单
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <returns>菜单信息</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<SysMenuDto>> GetMenuById(Guid id)
    {
        var menu = await _menuService.GetByIdAsync(id);
        if (menu == null)
        {
            return ApiResponse<SysMenuDto>.NotFound("菜单不存在");
        }
        var menuDto = _mapper.Map<SysMenuDto>(menu);
        return ApiResponse<SysMenuDto>.Success(menuDto, "获取成功");
    }

    /// <summary>
    /// 获取菜单树
    /// </summary>
    /// <returns>菜单树</returns>
    [HttpGet("tree")]
    public async Task<ApiResponse<List<SysMenuTreeDto>>> GetMenuTree()
    {
        var menuTree = await _menuService.GetAllMenuTreeAsync();
        var menuTreeDtos = _mapper.Map<List<SysMenuTreeDto>>(menuTree);
        return ApiResponse<List<SysMenuTreeDto>>.Success(menuTreeDtos, "获取成功");
    }

    /// <summary>
    /// 根据角色ID获取菜单树
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>菜单树</returns>
    [HttpGet("tree/{roleId}")]
    public async Task<ApiResponse<List<SysMenuTreeDto>>> GetMenuTreeByRoleId(Guid roleId)
    {
        var menuTree = await _menuService.GetMenuTreeByRoleIdAsync(roleId);
        var menuTreeDtos = _mapper.Map<List<SysMenuTreeDto>>(menuTree);
        return ApiResponse<List<SysMenuTreeDto>>.Success(menuTreeDtos, "获取成功");
    }

    /// <summary>
    /// 创建菜单
    /// </summary>
    /// <param name="menuCreateDto">菜单创建信息</param>
    /// <returns>创建结果</returns>
    [HttpPost]
    public async Task<ApiResponse<SysMenuDto>> CreateMenu([FromBody] SysMenuCreateDto menuCreateDto)
    {
        var menu = _mapper.Map<SysMenu>(menuCreateDto);
        var createdMenu = await _menuService.CreateAsync(menu);
        var createdMenuDto = _mapper.Map<SysMenuDto>(createdMenu);
        return ApiResponse<SysMenuDto>.Success(createdMenuDto, "创建成功");
    }

    /// <summary>
    /// 更新菜单
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <param name="menuUpdateDto">菜单更新信息</param>
    /// <returns>更新结果</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<SysMenuDto>> UpdateMenu(Guid id, [FromBody] SysMenuUpdateDto menuUpdateDto)
    {
        var menu = await _menuService.GetByIdAsync(id);
        if (menu == null)
        {
            return ApiResponse<SysMenuDto>.NotFound("菜单不存在");
        }
        _mapper.Map(menuUpdateDto, menu);
        var updatedMenu = await _menuService.UpdateAsync(menu);
        if (updatedMenu == null)
        {
            return ApiResponse<SysMenuDto>.Failed("更新失败");
        }
        var updatedMenuDto = _mapper.Map<SysMenuDto>(updatedMenu);
        return ApiResponse<SysMenuDto>.Success(updatedMenuDto, "更新成功");
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteMenu(Guid id)
    {
        var result = await _menuService.DeleteAsync(id);
        if (!result)
        {
            return ApiResponse<bool>.Failed("删除失败");
        }
        return ApiResponse<bool>.Success(result, "删除成功");
    }
}