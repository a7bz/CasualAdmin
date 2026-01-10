namespace CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Interfaces;
using CasualAdmin.Application.Interfaces.Base;
using CasualAdmin.Domain.Entities.System;

/// <summary>
/// 字典服务接口
/// </summary>
public interface IDictService : IBaseService<SysDict>
{
    /// <summary>
    /// 根据字典编码获取字典
    /// </summary>
    /// <param name="dictCode">字典编码</param>
    /// <returns>字典实体</returns>
    Task<SysDict?> GetDictByCodeAsync(string dictCode);

    /// <summary>
    /// 根据字典ID获取字典项列表
    /// </summary>
    /// <param name="dictId">字典ID</param>
    /// <returns>字典项列表</returns>
    Task<List<SysDictItem>> GetDictItemsByDictIdAsync(Guid dictId);
}