namespace CasualAdmin.API.Controllers.System;
using AutoMapper;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Models.DTOs.Requests.System;
using CasualAdmin.Application.Models.DTOs.Responses.System;
using CasualAdmin.Domain.Entities.System;
using CasualAdmin.Shared.Common;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// 部门控制器
/// </summary>
[ApiController]
[Route("[controller]")]
public class DeptController : ControllerBase
{
    private readonly IDeptService _deptService;
    private readonly IMapper _mapper;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="deptService">部门服务</param>
    /// <param name="mapper">AutoMapper实例</param>
    public DeptController(IDeptService deptService, IMapper mapper)
    {
        _deptService = deptService;
        _mapper = mapper;
    }

    /// <summary>
    /// 分页查询部门
    /// </summary>
    /// <param name="request">分页请求参数（包含筛选条件）</param>
    /// <returns>部门分页列表</returns>
    [HttpPost("query")]
    public async Task<ApiResponse<PageResponse<SysDeptDto>>> QueryDepts([FromBody] PageRequest<SysDeptDto> request)
    {
        var pageResponse = await _deptService.GetPagedAsync(request.Filter, request);
        var deptDtos = _mapper.Map<List<SysDeptDto>>(pageResponse.Items);
        return ApiResponse<PageResponse<SysDeptDto>>.Success(new PageResponse<SysDeptDto>(deptDtos, pageResponse.Total, pageResponse.PageIndex, pageResponse.PageSize), "查询成功");
    }

    /// <summary>
    /// 获取所有部门
    /// </summary>
    /// <returns>部门列表</returns>
    [HttpGet]
    public async Task<ApiResponse<List<SysDeptDto>>> GetAllDepts()
    {
        var depts = await _deptService.GetAllAsync();
        var deptDtos = _mapper.Map<List<SysDeptDto>>(depts);
        return ApiResponse<List<SysDeptDto>>.Success(deptDtos, "获取成功");
    }

    /// <summary>
    /// 根据ID获取部门
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <returns>部门信息</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<SysDeptDto>> GetDeptById(Guid id)
    {
        var dept = await _deptService.GetByIdAsync(id);
        if (dept == null)
        {
            return ApiResponse<SysDeptDto>.NotFound("部门不存在");
        }
        var deptDto = _mapper.Map<SysDeptDto>(dept);
        return ApiResponse<SysDeptDto>.Success(deptDto, "获取成功");
    }

    /// <summary>
    /// 获取部门树
    /// </summary>
    /// <returns>部门树</returns>
    [HttpGet("tree")]
    public async Task<ApiResponse<List<SysDeptTreeDto>>> GetDeptTree()
    {
        var deptTree = await _deptService.GetDeptTreeAsync();
        var deptTreeDtos = _mapper.Map<List<SysDeptTreeDto>>(deptTree);
        return ApiResponse<List<SysDeptTreeDto>>.Success(deptTreeDtos, "获取成功");
    }

    /// <summary>
    /// 根据父部门ID获取子部门列表
    /// </summary>
    /// <param name="parentId">父部门ID</param>
    /// <returns>子部门列表</returns>
    [HttpGet("children/{parentId}")]
    public async Task<ApiResponse<List<SysDeptDto>>> GetChildrenByParentId(Guid parentId)
    {
        var children = await _deptService.GetChildrenByParentIdAsync(parentId);
        var childDtos = _mapper.Map<List<SysDeptDto>>(children);
        return ApiResponse<List<SysDeptDto>>.Success(childDtos, "获取成功");
    }

    /// <summary>
    /// 创建部门
    /// </summary>
    /// <param name="deptCreateDto">部门创建信息</param>
    /// <returns>创建结果</returns>
    [HttpPost]
    public async Task<ApiResponse<SysDeptDto>> CreateDept([FromBody] SysDeptCreateDto deptCreateDto)
    {
        var dept = _mapper.Map<SysDept>(deptCreateDto);
        var createdDept = await _deptService.CreateAsync(dept);
        var createdDeptDto = _mapper.Map<SysDeptDto>(createdDept);
        return ApiResponse<SysDeptDto>.Success(createdDeptDto, "创建成功");
    }

    /// <summary>
    /// 更新部门
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <param name="deptUpdateDto">部门更新信息</param>
    /// <returns>更新结果</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<SysDeptDto>> UpdateDept(Guid id, [FromBody] SysDeptUpdateDto deptUpdateDto)
    {
        var dept = await _deptService.GetByIdAsync(id);
        if (dept == null)
        {
            return ApiResponse<SysDeptDto>.NotFound("部门不存在");
        }
        _mapper.Map(deptUpdateDto, dept);
        var updatedDept = await _deptService.UpdateAsync(dept);
        var updatedDeptDto = _mapper.Map<SysDeptDto>(updatedDept);
        return ApiResponse<SysDeptDto>.Success(updatedDeptDto, "更新成功");
    }

    /// <summary>
    /// 删除部门
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteDept(Guid id)
    {
        var result = await _deptService.DeleteAsync(id);
        if (!result)
        {
            return ApiResponse<bool>.Failed("删除失败");
        }
        return ApiResponse<bool>.Success(result, "删除成功");
    }
}