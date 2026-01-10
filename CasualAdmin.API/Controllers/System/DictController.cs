namespace CasualAdmin.API.Controllers.System;
using AutoMapper;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Models.DTOs.Requests.System;
using CasualAdmin.Application.Models.DTOs.Responses.System;
using CasualAdmin.Domain.Entities.System;
using CasualAdmin.Shared.Common;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// 字典控制器
/// </summary>
[ApiController]
[Route("[controller]")]
public class DictController : ControllerBase
{
    private readonly IDictService _dictService;
    private readonly IMapper _mapper;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dictService">字典服务</param>
    /// <param name="mapper">AutoMapper实例</param>
    public DictController(IDictService dictService, IMapper mapper)
    {
        _dictService = dictService;
        _mapper = mapper;
    }

    /// <summary>
    /// 获取所有字典
    /// </summary>
    /// <returns>字典列表</returns>
    [HttpGet]
    public async Task<ApiResponse<List<SysDictDto>>> GetAllDicts()
    {
        var dicts = await _dictService.GetAllAsync();
        var dictDtos = _mapper.Map<List<SysDictDto>>(dicts);
        return ApiResponse<List<SysDictDto>>.Success(dictDtos, "获取成功");
    }

    /// <summary>
    /// 根据ID获取字典
    /// </summary>
    /// <param name="id">字典ID</param>
    /// <returns>字典信息</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<SysDictDto>> GetDictById(Guid id)
    {
        var dict = await _dictService.GetByIdAsync(id);
        if (dict == null)
        {
            return ApiResponse<SysDictDto>.NotFound("字典不存在");
        }
        var dictDto = _mapper.Map<SysDictDto>(dict);
        return ApiResponse<SysDictDto>.Success(dictDto, "获取成功");
    }

    /// <summary>
    /// 根据字典编码获取字典
    /// </summary>
    /// <param name="dictCode">字典编码</param>
    /// <returns>字典信息</returns>
    [HttpGet("code/{dictCode}")]
    public async Task<ApiResponse<SysDictDto>> GetDictByCode(string dictCode)
    {
        var dict = await _dictService.GetDictByCodeAsync(dictCode);
        if (dict == null)
        {
            return ApiResponse<SysDictDto>.NotFound("字典不存在");
        }
        var dictDto = _mapper.Map<SysDictDto>(dict);
        return ApiResponse<SysDictDto>.Success(dictDto, "获取成功");
    }

    /// <summary>
    /// 创建字典
    /// </summary>
    /// <param name="dictCreateDto">字典创建信息</param>
    /// <returns>创建结果</returns>
    [HttpPost]
    public async Task<ApiResponse<SysDictDto>> CreateDict([FromBody] SysDictCreateDto dictCreateDto)
    {
        var dict = _mapper.Map<SysDict>(dictCreateDto);
        var createdDict = await _dictService.CreateAsync(dict);
        var createdDictDto = _mapper.Map<SysDictDto>(createdDict);
        return ApiResponse<SysDictDto>.Success(createdDictDto, "创建成功");
    }

    /// <summary>
    /// 更新字典
    /// </summary>
    /// <param name="id">字典ID</param>
    /// <param name="dictUpdateDto">字典更新信息</param>
    /// <returns>更新结果</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<SysDictDto>> UpdateDict(Guid id, [FromBody] SysDictUpdateDto dictUpdateDto)
    {
        var dict = await _dictService.GetByIdAsync(id);
        if (dict == null)
        {
            return ApiResponse<SysDictDto>.NotFound("字典不存在");
        }
        _mapper.Map(dictUpdateDto, dict);
        dict.DictId = id;
        var updatedDict = await _dictService.UpdateAsync(dict);
        var updatedDictDto = _mapper.Map<SysDictDto>(updatedDict);
        return ApiResponse<SysDictDto>.Success(updatedDictDto, "更新成功");
    }

    /// <summary>
    /// 删除字典
    /// </summary>
    /// <param name="id">字典ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteDict(Guid id)
    {
        var result = await _dictService.DeleteAsync(id);
        if (!result)
        {
            return ApiResponse<bool>.Failed("删除失败");
        }
        return ApiResponse<bool>.Success(result, "删除成功");
    }
}