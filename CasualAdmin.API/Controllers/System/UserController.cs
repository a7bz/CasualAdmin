namespace CasualAdmin.API.Controllers.System;
using AutoMapper;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Models.DTOs.Requests.System;
using CasualAdmin.Application.Models.DTOs.Responses.System;
using CasualAdmin.Domain.Entities.System;
using CasualAdmin.Shared.Common;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// 用户控制器
/// </summary>
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userService">用户服务</param>
    /// <param name="mapper">AutoMapper实例</param>
    public UserController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    /// <summary>
    /// 获取所有用户
    /// </summary>
    /// <returns>用户列表</returns>
    [HttpGet]
    public async Task<ApiResponse<List<SysUserDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        var userDtos = _mapper.Map<List<SysUserDto>>(users);
        return ApiResponse<List<SysUserDto>>.Success(userDtos, "获取成功");
    }

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户信息</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<SysUserDto>> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return ApiResponse<SysUserDto>.NotFound("用户不存在");
        }
        var userDto = _mapper.Map<SysUserDto>(user);
        return ApiResponse<SysUserDto>.Success(userDto, "获取成功");
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="userCreateDto">用户创建信息</param>
    /// <returns>创建结果</returns>
    [HttpPost]
    public async Task<ApiResponse<SysUserDto>> CreateUser([FromBody] SysUserCreateDto userCreateDto)
    {
        var user = _mapper.Map<SysUser>(userCreateDto);
        var createdUser = await _userService.CreateUserAsync(user);
        if (createdUser == null)
        {
            return ApiResponse<SysUserDto>.Failed("创建失败");
        }
        var createdUserDto = _mapper.Map<SysUserDto>(createdUser);
        return ApiResponse<SysUserDto>.Success(createdUserDto, "创建成功");
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="userUpdateDto">用户更新信息</param>
    /// <returns>更新结果</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<SysUserDto>> UpdateUser(Guid id, [FromBody] SysUserUpdateDto userUpdateDto)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return ApiResponse<SysUserDto>.NotFound("用户不存在");
        }
        _mapper.Map(userUpdateDto, user);
        var updatedUser = await _userService.UpdateUserAsync(user);
        var updatedUserDto = _mapper.Map<SysUserDto>(updatedUser);
        return ApiResponse<SysUserDto>.Success(updatedUserDto, "更新成功");
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteUser(Guid id)
    {
        var result = await _userService.DeleteUserAsync(id);
        if (!result)
        {
            return ApiResponse<bool>.Failed("删除失败");
        }
        return ApiResponse<bool>.Success(result, "删除成功");
    }
}