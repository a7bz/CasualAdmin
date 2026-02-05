namespace CasualAdmin.API.Controllers.System;
using AutoMapper;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Models.DTOs.Requests.System;
using CasualAdmin.Application.Models.DTOs.Responses.System;
using CasualAdmin.Domain.Entities.System;
using CasualAdmin.Shared.Common;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// 字典项控制器
/// </summary>
[ApiController]
[Route("[controller]")]
public class DictItemController : ControllerBase
{
    private readonly IDictItemService _dictItemService;
    private readonly IMapper _mapper;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dictItemService">字典项服务</param>
    /// <param name="mapper">AutoMapper实例</param>
    public DictItemController(IDictItemService dictItemService, IMapper mapper)
    {
        _dictItemService = dictItemService;
        _mapper = mapper;
    }

    /// <summary>
    /// 分页查询字典项
    /// </summary>
    /// <param name="request">分页请求参数（包含筛选条件）</param>
    /// <returns>字典项分页列表</returns>
    [HttpPost("query")]
    public async Task<ApiResponse<PageResponse<SysDictItemDto>>> QueryDictItems([FromBody] PageRequest<SysDictItemDto> request)
    {
        var pageResponse = await _dictItemService.GetPagedAsync(request.Filter, request);
        var dictItemDtos = _mapper.Map<List<SysDictItemDto>>(pageResponse.Items);
        return ApiResponse<PageResponse<SysDictItemDto>>.Success(new PageResponse<SysDictItemDto>(dictItemDtos, pageResponse.Total, pageResponse.PageIndex, pageResponse.PageSize), "查询成功");
    }

    /// <summary>
    /// 获取所有字典项
    /// </summary>
    /// <returns>字典项列表</returns>
    [HttpGet]
    public async Task<ApiResponse<List<SysDictItemDto>>> GetAllDictItems()
    {
        var dictItems = await _dictItemService.GetAllAsync();
        var dictItemDtos = _mapper.Map<List<SysDictItemDto>>(dictItems);
        return ApiResponse<List<SysDictItemDto>>.Success(dictItemDtos, "获取成功");
    }

    /// <summary>
    /// 根据ID获取字典项
    /// </summary>
    /// <param name="id">字典项ID</param>
    /// <returns>字典项信息</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<SysDictItemDto>> GetDictItemById(Guid id)
    {
        var dictItem = await _dictItemService.GetByIdAsync(id);
        if (dictItem == null)
        {
            return ApiResponse<SysDictItemDto>.NotFound("字典项不存在");
        }
        var dictItemDto = _mapper.Map<SysDictItemDto>(dictItem);
        return ApiResponse<SysDictItemDto>.Success(dictItemDto, "获取成功");
    }

    /// <summary>
    /// 根据字典编码获取字典项列表
    /// </summary>
    /// <param name="dictCode">字典编码</param>
    /// <returns>字典项列表</returns>
    [HttpGet("dict/{dictCode}")]
    public async Task<ApiResponse<List<SysDictItemDto>>> GetDictItemsByDictCode(string dictCode)
    {
        var dictItems = await _dictItemService.GetDictItemsByDictCodeAsync(dictCode);
        var dictItemDtos = _mapper.Map<List<SysDictItemDto>>(dictItems);
        return ApiResponse<List<SysDictItemDto>>.Success(dictItemDtos, "获取成功");
    }

    /// <summary>
    /// 创建字典项
    /// </summary>
    /// <param name="dictItemCreateDto">字典项创建信息</param>
    /// <returns>创建结果</returns>
    [HttpPost]
    public async Task<ApiResponse<SysDictItemDto>> CreateDictItem([FromBody] SysDictItemCreateDto dictItemCreateDto)
    {
        var dictItem = _mapper.Map<SysDictItem>(dictItemCreateDto);
        var createdDictItem = await _dictItemService.CreateAsync(dictItem);
        var createdDictItemDto = _mapper.Map<SysDictItemDto>(createdDictItem);
        return ApiResponse<SysDictItemDto>.Success(createdDictItemDto, "创建成功");
    }

    /// <summary>
    /// 更新字典项
    /// </summary>
    /// <param name="id">字典项ID</param>
    /// <param name="dictItemUpdateDto">字典项更新信息</param>
    /// <returns>更新结果</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<SysDictItemDto>> UpdateDictItem(Guid id, [FromBody] SysDictItemUpdateDto dictItemUpdateDto)
    {
        var dictItem = await _dictItemService.GetByIdAsync(id);
        if (dictItem == null)
        {
            return ApiResponse<SysDictItemDto>.NotFound("字典项不存在");
        }
        _mapper.Map(dictItemUpdateDto, dictItem);
        dictItem.DictItemId = id;
        var updatedDictItem = await _dictItemService.UpdateAsync(dictItem);
        var updatedDictItemDto = _mapper.Map<SysDictItemDto>(updatedDictItem);
        return ApiResponse<SysDictItemDto>.Success(updatedDictItemDto, "更新成功");
    }

    /// <summary>
    /// 删除字典项
    /// </summary>
    /// <param name="id">字典项ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteDictItem(Guid id)
    {
        var result = await _dictItemService.DeleteAsync(id);
        if (!result)
        {
            return ApiResponse<bool>.Failed("删除失败");
        }
        return ApiResponse<bool>.Success(result, "删除成功");
    }
}