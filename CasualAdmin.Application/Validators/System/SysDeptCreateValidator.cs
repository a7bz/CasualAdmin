namespace CasualAdmin.Application.Validators.System;
using CasualAdmin.Application.Models.DTOs.Requests.System;
using FluentValidation;

/// <summary>
/// 部门创建DTO验证器
/// </summary>
public class SysDeptCreateValidator : AbstractValidator<SysDeptCreateDto>
{
    /// <summary>
    /// 构造函数，配置验证规则
    /// </summary>
    public SysDeptCreateValidator()
    {
        // 部门名称验证
        RuleFor(x => x.DeptName)
            .NotEmpty().WithMessage("部门名称不能为空")
            .MaximumLength(100).WithMessage("部门名称长度不能超过100个字符");

        // 部门编码验证
        RuleFor(x => x.DeptCode)
            .NotEmpty().WithMessage("部门编码不能为空")
            .MaximumLength(50).WithMessage("部门编码长度不能超过50个字符");

        // 部门负责人验证
        RuleFor(x => x.Leader)
            .MaximumLength(50).WithMessage("部门负责人长度不能超过50个字符");

        // 联系电话验证
        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("联系电话长度不能超过20个字符");

        // 状态验证
        RuleFor(x => x.Status)
            .InclusiveBetween(0, 1).WithMessage("状态必须为0或1");

        // 备注验证
        RuleFor(x => x.Remark)
            .MaximumLength(200).WithMessage("备注长度不能超过200个字符");
    }
}