namespace CasualAdmin.Application.Services.System
{
    using CasualAdmin.Application.Interfaces.Base;
    using CasualAdmin.Application.Interfaces.Events;
    using CasualAdmin.Application.Interfaces.Services;
    using CasualAdmin.Application.Interfaces.System;
    using CasualAdmin.Application.Services;
    using CasualAdmin.Domain.Entities.System;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Threading.Tasks;

    /// <summary>
    /// 字典项服务实现
    /// </summary>
    public class DictItemService : BaseService<SysDictItem>, IDictItemService
    {
        private readonly IDictService _dictService;
        private readonly ICacheService _cacheService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">字典项仓储</param>
        /// <param name="validationService">验证服务</param>
        /// <param name="eventBus">事件总线</param>
        /// <param name="dictService">字典服务</param>
        /// <param name="cacheService">缓存服务</param>
        public DictItemService(IRepository<SysDictItem> repository, IValidationService validationService, IEventBus eventBus, IDictService dictService, ICacheService cacheService)
            : base(repository, validationService, eventBus)
        {
            _dictService = dictService;
            _cacheService = cacheService;
        }

        /// <summary>
        /// 根据字典编码获取字典项列表
        /// </summary>
        /// <param name="dictCode">字典编码</param>
        /// <returns>字典项列表</returns>
        public async Task<List<SysDictItem>> GetDictItemsByDictCodeAsync(string dictCode)
        {
            var cacheKey = $"dict:items:code:{dictCode}";

            // 尝试从缓存获取
            var cachedItems = _cacheService.Get<List<SysDictItem>>(cacheKey);
            if (cachedItems != null)
            {
                return cachedItems;
            }

            // 从数据库获取
            var dict = await _dictService.GetDictByCodeAsync(dictCode);
            if (dict == null)
            {
                return new List<SysDictItem>();
            }

            var items = await _repository.FindAsync(item => item.DictId == dict.DictId);

            // 存入缓存
            _cacheService.Set(cacheKey, items, TimeSpan.FromHours(1));

            return items;
        }

        /// <summary>
        /// 创建字典项
        /// </summary>
        /// <param name="entity">字典项实体</param>
        /// <returns>创建的字典项实体</returns>
        public override async Task<SysDictItem?> CreateAsync(SysDictItem entity)
        {
            var createdItem = await base.CreateAsync(entity);

            // 清除相关缓存
            if (createdItem != null)
            {
                ClearDictItemCache(createdItem.DictId);
            }

            return createdItem;
        }

        /// <summary>
        /// 更新字典项
        /// </summary>
        /// <param name="entity">字典项实体</param>
        /// <returns>更新的字典项实体</returns>
        public override async Task<SysDictItem?> UpdateAsync(SysDictItem entity)
        {
            var updatedItem = await base.UpdateAsync(entity);

            // 清除相关缓存
            if (updatedItem != null)
            {
                ClearDictItemCache(updatedItem.DictId);
            }

            return updatedItem;
        }

        /// <summary>
        /// 删除字典项
        /// </summary>
        /// <param name="id">字典项ID</param>
        /// <returns>删除结果</returns>
        public override async Task<bool> DeleteAsync(Guid id)
        {
            // 获取字典项信息，用于清除缓存
            var item = await GetByIdAsync(id);
            var dictId = item?.DictId;

            var result = await base.DeleteAsync(id);

            // 清除相关缓存
            if (result && dictId.HasValue)
            {
                ClearDictItemCache(dictId.Value);
            }

            return result;
        }

        /// <summary>
        /// 清除字典项相关缓存
        /// </summary>
        /// <param name="dictId">字典ID</param>
        private void ClearDictItemCache(Guid dictId)
        {
            // 清除字典ID对应的字典项缓存
            _cacheService.Remove($"dict:items:{dictId}");

            // 注意：由于我们不知道字典编码，所以无法直接清除 dict:items:code:{dictCode} 格式的缓存
            // 在实际应用中，可以考虑在字典项中存储字典编码，或者通过字典服务获取字典编码后再清除缓存
        }
    }
}