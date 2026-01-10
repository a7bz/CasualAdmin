namespace CasualAdmin.Application.Services.System;
using CasualAdmin.Application.Interfaces.Base;
using CasualAdmin.Application.Interfaces.Events;
using CasualAdmin.Application.Interfaces.Services;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Services;
using CasualAdmin.Domain.Entities.System;

/// <summary>
/// 字典服务实现
/// </summary>
public class DictService : BaseService<SysDict>, IDictService
{
    private readonly IRepository<SysDictItem> _dictItemRepository;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="repository">字典仓储</param>
    /// <param name="validationService">验证服务</param>
    /// <param name="eventBus">事件总线</param>
    /// <param name="dictItemRepository">字典项仓储</param>
    public DictService(IRepository<SysDict> repository, IValidationService validationService, IEventBus eventBus, IRepository<SysDictItem> dictItemRepository)
        : base(repository, validationService, eventBus)
    {
        _dictItemRepository = dictItemRepository;
    }

    /// <summary>
    /// 根据字典编码获取字典
    /// </summary>
    /// <param name="dictCode">字典编码</param>
    /// <returns>字典实体</returns>
    public async Task<SysDict?> GetDictByCodeAsync(string dictCode)
    {
        var dicts = await _repository.FindAsync(d => d.DictCode == dictCode);
        return dicts.FirstOrDefault();
    }

    /// <summary>
    /// 根据字典ID获取字典项列表
    /// </summary>
    /// <param name="dictId">字典ID</param>
    /// <returns>字典项列表</returns>
    public async Task<List<SysDictItem>> GetDictItemsByDictIdAsync(Guid dictId)
    {
        return await _dictItemRepository.FindAsync(item => item.DictId == dictId);
    }
}