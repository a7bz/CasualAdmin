namespace CasualAdmin.Application.Services.System;
using CasualAdmin.Application.Interfaces.Base;
using CasualAdmin.Application.Interfaces.Events;
using CasualAdmin.Application.Interfaces.Services;
using CasualAdmin.Application.Interfaces.System;
using CasualAdmin.Application.Services;
using CasualAdmin.Domain.Entities.System;

/// <summary>
/// 部门服务实现
/// </summary>
public class DeptService : BaseService<SysDept>, IDeptService
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="repository">部门仓储</param>
    /// <param name="validationService">验证服务</param>
    /// <param name="eventBus">事件总线</param>
    public DeptService(IRepository<SysDept> repository, IValidationService validationService, IEventBus eventBus) : base(repository, validationService, eventBus)
    {
    }

    /// <summary>
    /// 获取部门树
    /// </summary>
    /// <returns>部门树列表</returns>
    public async Task<List<SysDept>> GetDeptTreeAsync()
    {
        var depts = await _repository.GetAllAsync();
        return BuildDeptTree(depts);
    }

    /// <summary>
    /// 根据部门ID获取子部门列表
    /// </summary>
    /// <param name="parentId">父部门ID</param>
    /// <returns>子部门列表</returns>
    public async Task<List<SysDept>> GetChildrenByParentIdAsync(Guid parentId)
    {
        return await _repository.FindAsync(d => d.ParentId == parentId);
    }

    /// <summary>
    /// 构建部门树
    /// </summary>
    /// <param name="depts">部门列表</param>
    /// <returns>部门树</returns>
    private List<SysDept> BuildDeptTree(List<SysDept> depts)
    {
        var deptMap = depts.ToDictionary(d => d.DeptId);
        var rootDepts = new List<SysDept>();

        foreach (var dept in depts)
        {
            if (!dept.ParentId.HasValue)
            {
                rootDepts.Add(dept);
            }
            else if (deptMap.TryGetValue(dept.ParentId.Value, out var parentDept))
            {
                parentDept.Children.Add(dept);
            }
        }

        return rootDepts;
    }

    /// <summary>
    /// 验证上级部门是否存在
    /// </summary>
    /// <param name="entity">部门实体</param>
    /// <returns>是否验证通过</returns>
    private async Task ValidateParentDeptAsync(SysDept entity)
    {
        // 验证上级部门是否存在
        if (entity.ParentId.HasValue)
        {
            var parentDept = await _repository.GetByIdAsync(entity.ParentId.Value);
            if (parentDept == null)
            {
                throw new ArgumentException("上级部门不存在");
            }
        }
    }

    /// <summary>
    /// 创建前业务逻辑处理，验证上级部门是否存在
    /// </summary>
    /// <param name="entity">部门实体</param>
    /// <returns>处理后的部门实体</returns>
    protected override async Task<SysDept> OnBeforeCreateAsync(SysDept entity)
    {
        await ValidateParentDeptAsync(entity);
        return await base.OnBeforeCreateAsync(entity);
    }

    /// <summary>
    /// 更新前业务逻辑处理，验证上级部门是否存在
    /// </summary>
    /// <param name="entity">部门实体</param>
    /// <returns>处理后的部门实体</returns>
    protected override async Task<SysDept> OnBeforeUpdateAsync(SysDept entity)
    {
        await ValidateParentDeptAsync(entity);
        return await base.OnBeforeUpdateAsync(entity);
    }
}