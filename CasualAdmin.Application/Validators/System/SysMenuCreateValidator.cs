namespace CasualAdmin.Application.Validators.System;

using CasualAdmin.Application.Models.DTOs.Requests.System;
using FluentValidation;

/// <summary>
/// 菜单创建验证器
/// </summary>
public class SysMenuCreateValidator : AbstractValidator<SysMenuCreateDto>
{
    /// <summary>
    /// 构造函数，配置验证规则
    /// </summary>
    public SysMenuCreateValidator()
    {
        // 菜单名称验证
        RuleFor(x => x.MenuName)
            .NotEmpty().WithMessage("菜单名称不能为空")
            .Length(1, 50).WithMessage("菜单名称长度不能超过50个字符");

        // 菜单类型验证
        RuleFor(x => x.MenuType)
            .InclusiveBetween(0, 2).WithMessage("菜单类型只能是0-2之间的整数");

        // 路由路径验证
        RuleFor(x => x.Path)
            .Length(0, 100).WithMessage("路由路径长度不能超过100个字符");

        // 组件路径验证
        RuleFor(x => x.Component)
            .Length(0, 100).WithMessage("组件路径长度不能超过100个字符");

        // 权限标识验证
        RuleFor(x => x.Permission)
            .Length(0, 100).WithMessage("权限标识长度不能超过100个字符");

        // 图标验证
        RuleFor(x => x.Icon)
            .Length(0, 50).WithMessage("图标长度不能超过50个字符");

        // 排序验证
        RuleFor(x => x.Sort)
            .GreaterThanOrEqualTo(0).WithMessage("排序值不能小于0")
            .LessThanOrEqualTo(1000).WithMessage("排序值不能超过1000");

        // 状态验证
        RuleFor(x => x.Status)
            .InclusiveBetween(0, 1).WithMessage("状态只能是0或1");

        // 是否显示验证
        RuleFor(x => x.IsVisible)
            .InclusiveBetween(0, 1).WithMessage("是否显示只能是0或1");

        // 是否缓存验证
        RuleFor(x => x.IsCache)
            .InclusiveBetween(0, 1).WithMessage("是否缓存只能是0或1");

        // 备注验证
        RuleFor(x => x.Remark)
            .MaximumLength(200).WithMessage("备注长度不能超过200个字符");
    }
}