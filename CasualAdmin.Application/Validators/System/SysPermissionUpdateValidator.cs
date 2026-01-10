namespace CasualAdmin.Application.Validators.System;

using CasualAdmin.Application.Models.DTOs.Requests.System;
using FluentValidation;

/// <summary>
/// 权限更新验证器
/// </summary>
public class SysPermissionUpdateValidator : AbstractValidator<SysPermissionUpdateDto>
{
    /// <summary>
    /// 构造函数，配置验证规则
    /// </summary>
    public SysPermissionUpdateValidator()
    {
        // 权限名称验证
        RuleFor(x => x.PermissionName)
            .NotEmpty().WithMessage("权限名称不能为空")
            .Length(1, 50).WithMessage("权限名称长度不能超过50个字符");

        // 权限编码验证
        RuleFor(x => x.PermissionCode)
            .NotEmpty().WithMessage("权限编码不能为空")
            .Length(1, 100).WithMessage("权限编码长度不能超过100个字符")
            .Matches("^[a-zA-Z0-9_:]+$").WithMessage("权限编码只能包含字母、数字、下划线和冒号");

        // 权限类型验证
        RuleFor(x => x.PermissionType)
            .InclusiveBetween(0, 1).WithMessage("权限类型只能是0或1");

        // 所属模块验证
        RuleFor(x => x.Module)
            .Length(0, 50).WithMessage("所属模块长度不能超过50个字符");

        // 状态验证
        RuleFor(x => x.Status)
            .InclusiveBetween(0, 1).WithMessage("状态只能是0或1");

        // 排序验证
        RuleFor(x => x.Sort)
            .GreaterThanOrEqualTo(0).WithMessage("排序值不能小于0")
            .LessThanOrEqualTo(1000).WithMessage("排序值不能超过1000");

        // 备注验证
        RuleFor(x => x.Remark)
            .MaximumLength(200).WithMessage("备注长度不能超过200个字符");
    }
}