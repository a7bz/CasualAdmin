namespace CasualAdmin.Application.Validators.System;

using CasualAdmin.Application.Models.DTOs.Requests.System;
using FluentValidation;

/// <summary>
/// 字典项更新验证器
/// </summary>
public class SysDictItemUpdateValidator : AbstractValidator<SysDictItemUpdateDto>
{
    /// <summary>
    /// 构造函数，配置验证规则
    /// </summary>
    public SysDictItemUpdateValidator()
    {
        // 字典ID验证
        RuleFor(x => x.DictId)
            .NotEmpty().WithMessage("字典ID不能为空");

        // 字典项标签验证
        RuleFor(x => x.Label)
            .NotEmpty().WithMessage("字典项标签不能为空")
            .Length(1, 50).WithMessage("字典项标签长度不能超过50个字符");

        // 字典项值验证
        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("字典项值不能为空")
            .Length(1, 50).WithMessage("字典项值长度不能超过50个字符");

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