namespace CasualAdmin.Application.Services.System;
using CasualAdmin.Application.Interfaces.Base;
using CasualAdmin.Application.Interfaces.Events;
using CasualAdmin.Application.Interfaces.Services;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Services;
using CasualAdmin.Domain.Entities.System;

/// <summary>
/// 字典项服务实现
/// </summary>
public class DictItemService : BaseService<SysDictItem>, IDictItemService
{
    private readonly IDictService _dictService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="repository">字典项仓储</param>
    /// <param name="validationService">验证服务</param>
    /// <param name="eventBus">事件总线</param>
    /// <param name="dictService">字典服务</param>
    public DictItemService(IRepository<SysDictItem> repository, IValidationService validationService, IEventBus eventBus, IDictService dictService)
        : base(repository, validationService, eventBus)
    {
        _dictService = dictService;
    }

    /// <summary>
    /// 根据字典编码获取字典项列表
    /// </summary>
    /// <param name="dictCode">字典编码</param>
    /// <returns>字典项列表</returns>
    public async Task<List<SysDictItem>> GetDictItemsByDictCodeAsync(string dictCode)
    {
        var dict = await _dictService.GetDictByCodeAsync(dictCode);
        if (dict == null)
        {
            return new List<SysDictItem>();
        }

        return await _repository.FindAsync(item => item.DictId == dict.DictId);
    }
}