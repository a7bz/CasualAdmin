namespace CasualAdmin.Application.Interfaces.System;

using CasualAdmin.Application.Interfaces.Base;
using CasualAdmin.Domain.Entities.System;

/// <summary>
/// 字典项服务接口
/// </summary>
public interface IDictItemService : IBaseService<SysDictItem>
{
    /// <summary>
    /// 根据字典编码获取字典项列表
    /// </summary>
    /// <param name="dictCode">字典编码</param>
    /// <returns>字典项列表</returns>
    Task<List<SysDictItem>> GetDictItemsByDictCodeAsync(string dictCode);
}