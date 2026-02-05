namespace CasualAdmin.API.Controllers.System;
using AutoMapper;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Models.DTOs.Requests.System;
using CasualAdmin.Application.Models.DTOs.Responses.System;
using CasualAdmin.Domain.Entities.System;
using CasualAdmin.Shared.Common;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// 角色控制器
/// </summary>
[ApiController]
[Route("[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly IMapper _mapper;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="roleService">角色服务</param>
    /// <param name="mapper">AutoMapper实例</param>
    public RoleController(IRoleService roleService, IMapper mapper)
    {
        _roleService = roleService;
        _mapper = mapper;
    }

    /// <summary>
    /// 分页查询角色
    /// </summary>
    /// <param name="request">分页请求参数（包含筛选条件）</param>
    /// <returns>角色分页列表</returns>
    [HttpPost("query")]
    public async Task<ApiResponse<PageResponse<SysRoleDto>>> QueryRoles([FromBody] PageRequest<SysRoleDto> request)
    {
        var pageResponse = await _roleService.GetPagedAsync(request.Filter, request);
        var roleDtos = _mapper.Map<List<SysRoleDto>>(pageResponse.Items);
        return ApiResponse<PageResponse<SysRoleDto>>.Success(new PageResponse<SysRoleDto>(roleDtos, pageResponse.Total, pageResponse.PageIndex, pageResponse.PageSize), "查询成功");
    }

    /// <summary>
    /// 获取所有角色
    /// </summary>
    /// <returns>角色列表</returns>
    [HttpGet]
    public async Task<ApiResponse<List<SysRoleDto>>> GetAllRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();
        var roleDtos = _mapper.Map<List<SysRoleDto>>(roles);
        return ApiResponse<List<SysRoleDto>>.Success(roleDtos, "获取成功");
    }

    /// <summary>
    /// 根据ID获取角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>角色信息</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<SysRoleDto>> GetRoleById(Guid id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
        {
            return ApiResponse<SysRoleDto>.NotFound("角色不存在");
        }
        var roleDto = _mapper.Map<SysRoleDto>(role);
        return ApiResponse<SysRoleDto>.Success(roleDto, "获取成功");
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="roleCreateDto">角色创建信息</param>
    /// <returns>创建结果</returns>
    [HttpPost]
    public async Task<ApiResponse<SysRoleDto>> CreateRole([FromBody] SysRoleCreateDto roleCreateDto)
    {
        var role = _mapper.Map<SysRole>(roleCreateDto);
        var createdRole = await _roleService.CreateRoleAsync(role);
        if (createdRole == null)
        {
            return ApiResponse<SysRoleDto>.Failed("创建失败");
        }
        var createdRoleDto = _mapper.Map<SysRoleDto>(createdRole);
        return ApiResponse<SysRoleDto>.Success(createdRoleDto, "创建成功");
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <param name="roleUpdateDto">角色更新信息</param>
    /// <returns>更新结果</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<SysRoleDto>> UpdateRole(Guid id, [FromBody] SysRoleUpdateDto roleUpdateDto)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
        {
            return ApiResponse<SysRoleDto>.NotFound("角色不存在");
        }
        _mapper.Map(roleUpdateDto, role);
        var updatedRole = await _roleService.UpdateRoleAsync(role);
        if (updatedRole == null)
        {
            return ApiResponse<SysRoleDto>.Failed("更新失败");
        }
        var updatedRoleDto = _mapper.Map<SysRoleDto>(updatedRole);
        return ApiResponse<SysRoleDto>.Success(updatedRoleDto, "更新成功");
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteRole(Guid id)
    {
        var result = await _roleService.DeleteRoleAsync(id);
        if (!result)
        {
            return ApiResponse<bool>.Failed("删除失败");
        }
        return ApiResponse<bool>.Success(result, "删除成功");
    }

    /// <summary>
    /// 为用户分配角色
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="roleId">角色ID</param>
    /// <returns>分配结果</returns>
    [HttpPost("{roleId}/users/{userId}")]
    public async Task<ApiResponse<bool>> AssignRoleToUser(Guid roleId, Guid userId)
    {
        var result = await _roleService.AssignRoleToUserAsync(userId, roleId);
        if (!result)
        {
            return ApiResponse<bool>.Failed("分配失败");
        }
        return ApiResponse<bool>.Success(result, "分配成功");
    }

    /// <summary>
    /// 移除用户角色
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="roleId">角色ID</param>
    /// <returns>移除结果</returns>
    [HttpDelete("{roleId}/users/{userId}")]
    public async Task<ApiResponse<bool>> RemoveRoleFromUser(Guid roleId, Guid userId)
    {
        var result = await _roleService.RemoveRoleFromUserAsync(userId, roleId);
        if (!result)
        {
            return ApiResponse<bool>.Failed("移除失败");
        }
        return ApiResponse<bool>.Success(result, "移除成功");
    }

    /// <summary>
    /// 获取用户角色列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>角色列表</returns>
    [HttpGet("users/{userId}")]
    public async Task<ApiResponse<List<SysRoleDto>>> GetRolesByUserId(Guid userId)
    {
        var roles = await _roleService.GetRolesByUserIdAsync(userId);
        var roleDtos = _mapper.Map<List<SysRoleDto>>(roles);
        return ApiResponse<List<SysRoleDto>>.Success(roleDtos, "获取成功");
    }

    /// <summary>
    /// 获取角色用户列表
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>用户列表</returns>
    [HttpGet("{roleId}/users")]
    public async Task<ApiResponse<List<SysUserDto>>> GetUsersByRoleId(Guid roleId)
    {
        var users = await _roleService.GetUsersByRoleIdAsync(roleId);
        var userDtos = _mapper.Map<List<SysUserDto>>(users);
        return ApiResponse<List<SysUserDto>>.Success(userDtos, "获取成功");
    }
}