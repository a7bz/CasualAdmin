namespace CasualAdmin.API.Controllers.System;

using AutoMapper;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Models.DTOs.Requests.System;
using CasualAdmin.Application.Models.DTOs.Responses.System;
using CasualAdmin.Domain.Entities.System;
using CasualAdmin.Shared.Common;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// 权限控制器
/// </summary>
[ApiController]
[Route("[controller]")]
public class PermissionController : ControllerBase
{
    private readonly IPermissionService _permissionService;
    private readonly IMapper _mapper;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="permissionService">权限服务</param>
    /// <param name="mapper">AutoMapper实例</param>
    public PermissionController(IPermissionService permissionService, IMapper mapper)
    {
        _permissionService = permissionService;
        _mapper = mapper;
    }

    /// <summary>
    /// 获取所有权限
    /// </summary>
    /// <returns>权限列表</returns>
    [HttpGet]
    public async Task<ApiResponse<List<SysPermissionDto>>> GetAllPermissions()
    {
        var permissions = await _permissionService.GetAllAsync();
        var permissionDtos = _mapper.Map<List<SysPermissionDto>>(permissions);
        return ApiResponse<List<SysPermissionDto>>.Success(permissionDtos, "获取成功");
    }

    /// <summary>
    /// 根据ID获取权限
    /// </summary>
    /// <param name="id">权限ID</param>
    /// <returns>权限信息</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<SysPermissionDto>> GetPermissionById(Guid id)
    {
        var permission = await _permissionService.GetByIdAsync(id);
        if (permission == null)
        {
            return ApiResponse<SysPermissionDto>.NotFound("权限不存在");
        }
        var permissionDto = _mapper.Map<SysPermissionDto>(permission);
        return ApiResponse<SysPermissionDto>.Success(permissionDto, "获取成功");
    }

    /// <summary>
    /// 根据角色ID获取权限列表
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>权限列表</returns>
    [HttpGet("role/{roleId}")]
    public async Task<ApiResponse<List<SysPermissionDto>>> GetPermissionsByRoleId(Guid roleId)
    {
        var permissions = await _permissionService.GetPermissionsByRoleIdAsync(roleId);
        var permissionDtos = _mapper.Map<List<SysPermissionDto>>(permissions);
        return ApiResponse<List<SysPermissionDto>>.Success(permissionDtos, "获取成功");
    }

    /// <summary>
    /// 根据菜单ID获取权限列表
    /// </summary>
    /// <param name="menuId">菜单ID</param>
    /// <returns>权限列表</returns>
    [HttpGet("menu/{menuId}")]
    public async Task<ApiResponse<List<SysPermissionDto>>> GetPermissionsByMenuId(Guid menuId)
    {
        var permissions = await _permissionService.GetPermissionsByMenuIdAsync(menuId);
        var permissionDtos = _mapper.Map<List<SysPermissionDto>>(permissions);
        return ApiResponse<List<SysPermissionDto>>.Success(permissionDtos, "获取成功");
    }

    /// <summary>
    /// 创建权限
    /// </summary>
    /// <param name="permissionCreateDto">权限创建信息</param>
    /// <returns>创建结果</returns>
    [HttpPost]
    public async Task<ApiResponse<SysPermissionDto>> CreatePermission([FromBody] SysPermissionCreateDto permissionCreateDto)
    {
        var permission = _mapper.Map<SysPermission>(permissionCreateDto);
        var createdPermission = await _permissionService.CreateAsync(permission);
        var createdPermissionDto = _mapper.Map<SysPermissionDto>(createdPermission);
        return ApiResponse<SysPermissionDto>.Success(createdPermissionDto, "创建成功");
    }

    /// <summary>
    /// 更新权限
    /// </summary>
    /// <param name="id">权限ID</param>
    /// <param name="permissionUpdateDto">权限更新信息</param>
    /// <returns>更新结果</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<SysPermissionDto>> UpdatePermission(Guid id, [FromBody] SysPermissionUpdateDto permissionUpdateDto)
    {
        var permission = await _permissionService.GetByIdAsync(id);
        if (permission == null)
        {
            return ApiResponse<SysPermissionDto>.NotFound("权限不存在");
        }
        _mapper.Map(permissionUpdateDto, permission);
        var updatedPermission = await _permissionService.UpdateAsync(permission);
        if (updatedPermission == null)
        {
            return ApiResponse<SysPermissionDto>.Failed("更新失败");
        }
        var updatedPermissionDto = _mapper.Map<SysPermissionDto>(updatedPermission);
        return ApiResponse<SysPermissionDto>.Success(updatedPermissionDto, "更新成功");
    }

    /// <summary>
    /// 删除权限
    /// </summary>
    /// <param name="id">权限ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeletePermission(Guid id)
    {
        var result = await _permissionService.DeleteAsync(id);
        if (!result)
        {
            return ApiResponse<bool>.Failed("删除失败");
        }
        return ApiResponse<bool>.Success(result, "删除成功");
    }
}