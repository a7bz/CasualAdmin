namespace CasualAdmin.Application.Services.System
{
    using CasualAdmin.Application.Interfaces.Events;
    using CasualAdmin.Application.Interfaces.Services;
    using CasualAdmin.Application.Interfaces.System;
    using CasualAdmin.Application.Services;
    using CasualAdmin.Domain.Entities.System;
    using CasualAdmin.Domain.Infrastructure.Data;
    using CasualAdmin.Domain.Infrastructure.Services;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Threading.Tasks;

    /// <summary>
    /// 字典服务实现
    /// </summary>
    public class DictService : BaseService<SysDict>, IDictService
    {
        private readonly IRepository<SysDictItem> _dictItemRepository;
        private readonly ICacheService _cacheService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">字典仓储</param>
        /// <param name="validationService">验证服务</param>
        /// <param name="eventBus">事件总线</param>
        /// <param name="dictItemRepository">字典项仓储</param>
        /// <param name="cacheService">缓存服务</param>
        public DictService(IRepository<SysDict> repository, IValidationService validationService, IEventBus eventBus, IRepository<SysDictItem> dictItemRepository, ICacheService cacheService)
            : base(repository, validationService, eventBus)
        {
            _dictItemRepository = dictItemRepository;
            _cacheService = cacheService;
        }

        /// <summary>
        /// 根据字典编码获取字典
        /// </summary>
        /// <param name="dictCode">字典编码</param>
        /// <returns>字典实体</returns>
        public async Task<SysDict?> GetDictByCodeAsync(string dictCode)
        {
            var cacheKey = $"dict:code:{dictCode}";

            // 尝试从缓存获取
            var cachedDict = _cacheService.Get<SysDict>(cacheKey);
            if (cachedDict != null)
            {
                return cachedDict;
            }

            // 从数据库获取
            var dicts = await _repository.FindAsync(d => d.DictCode == dictCode);
            var dict = dicts.FirstOrDefault();

            // 存入缓存
            if (dict != null)
            {
                _cacheService.Set(cacheKey, dict, TimeSpan.FromHours(1));
            }

            return dict;
        }

        /// <summary>
        /// 根据字典ID获取字典项列表
        /// </summary>
        /// <param name="dictId">字典ID</param>
        /// <returns>字典项列表</returns>
        public async Task<List<SysDictItem>> GetDictItemsByDictIdAsync(Guid dictId)
        {
            var cacheKey = $"dict:items:{dictId}";

            // 尝试从缓存获取
            var cachedItems = _cacheService.Get<List<SysDictItem>>(cacheKey);
            if (cachedItems != null)
            {
                return cachedItems;
            }

            // 从数据库获取
            var items = await _dictItemRepository.FindAsync(item => item.DictId == dictId);

            // 存入缓存
            _cacheService.Set(cacheKey, items, TimeSpan.FromHours(1));

            return items;
        }

        /// <summary>
        /// 清除字典相关缓存
        /// </summary>
        /// <param name="dictId">字典ID</param>
        /// <param name="dictCode">字典编码</param>
        private void ClearDictCache(Guid dictId, string? dictCode = null)
        {
            // 清除字典项缓存
            _cacheService.Remove($"dict:items:{dictId}");

            // 清除字典缓存
            if (!string.IsNullOrEmpty(dictCode))
            {
                _cacheService.Remove($"dict:code:{dictCode}");
            }
        }

        /// <summary>
        /// 创建字典
        /// </summary>
        /// <param name="entity">字典实体</param>
        /// <returns>创建的字典实体</returns>
        public override async Task<SysDict?> CreateAsync(SysDict entity)
        {
            var createdDict = await base.CreateAsync(entity);

            // 清除相关缓存
            if (createdDict != null)
            {
                ClearDictCache(createdDict.DictId, createdDict.DictCode);
            }

            return createdDict;
        }

        /// <summary>
        /// 更新字典
        /// </summary>
        /// <param name="entity">字典实体</param>
        /// <returns>更新的字典实体</returns>
        public override async Task<SysDict?> UpdateAsync(SysDict entity)
        {
            // 获取原字典编码
            var oldDict = await GetByIdAsync(entity.DictId);
            var oldDictCode = oldDict?.DictCode;

            var updatedDict = await base.UpdateAsync(entity);

            // 清除相关缓存
            if (updatedDict != null)
            {
                ClearDictCache(updatedDict.DictId, updatedDict.DictCode);

                // 如果字典编码变更，还需要清除旧编码的缓存
                if (oldDictCode != null && oldDictCode != updatedDict.DictCode)
                {
                    _cacheService.Remove($"dict:code:{oldDictCode}");
                }
            }

            return updatedDict;
        }

        /// <summary>
        /// 删除字典
        /// </summary>
        /// <param name="id">字典ID</param>
        /// <returns>删除结果</returns>
        public override async Task<bool> DeleteAsync(Guid id)
        {
            // 获取字典信息，用于清除缓存
            var dict = await GetByIdAsync(id);
            var dictCode = dict?.DictCode;

            var result = await base.DeleteAsync(id);

            // 清除相关缓存
            if (result && dict != null)
            {
                ClearDictCache(id, dictCode);
            }

            return result;
        }
    }
}