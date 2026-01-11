namespace CasualAdmin.Application.Validators.System;

using CasualAdmin.Application.Models.DTOs.Requests.System;
using FluentValidation;

/// <summary>
/// 角色创建验证器
/// </summary>
public class SysRoleCreateValidator : AbstractValidator<SysRoleCreateDto>
{
    /// <summary>
    /// 构造函数，配置验证规则
    /// </summary>
    public SysRoleCreateValidator()
    {
        // 角色名称验证
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("角色名称不能为空")
            .Length(1, 50).WithMessage("角色名称长度不能超过50个字符");

        // 角色代码验证
        RuleFor(x => x.RoleCode)
            .NotEmpty().WithMessage("角色代码不能为空")
            .Length(1, 50).WithMessage("角色代码长度不能超过50个字符")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("角色代码只能包含字母、数字和下划线");

        // 角色类型验证
        RuleFor(x => x.RoleType)
            .InclusiveBetween(0, 1).WithMessage("角色类型只能是0或1");

        // 状态验证
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("状态值无效");

        // 排序验证
        RuleFor(x => x.Sort)
            .GreaterThanOrEqualTo(0).WithMessage("排序值不能小于0")
            .LessThanOrEqualTo(1000).WithMessage("排序值不能超过1000");

        // 角色描述验证
        RuleFor(x => x.Description)
            .MaximumLength(200).WithMessage("角色描述长度不能超过200个字符");
    }
}