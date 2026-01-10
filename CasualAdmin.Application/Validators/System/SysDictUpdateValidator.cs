namespace CasualAdmin.Application.Validators.System;

using CasualAdmin.Application.Models.DTOs.Requests.System;
using FluentValidation;

/// <summary>
/// 字典更新验证器
/// </summary>
public class SysDictUpdateValidator : AbstractValidator<SysDictUpdateDto>
{
    /// <summary>
    /// 构造函数，配置验证规则
    /// </summary>
    public SysDictUpdateValidator()
    {
        // 字典名称验证
        RuleFor(x => x.DictName)
            .NotEmpty().WithMessage("字典名称不能为空")
            .Length(1, 50).WithMessage("字典名称长度不能超过50个字符");

        // 字典编码验证
        RuleFor(x => x.DictCode)
            .NotEmpty().WithMessage("字典编码不能为空")
            .Length(1, 50).WithMessage("字典编码长度不能超过50个字符")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("字典编码只能包含字母、数字和下划线");

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