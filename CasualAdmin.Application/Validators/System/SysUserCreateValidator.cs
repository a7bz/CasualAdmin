namespace CasualAdmin.Application.Validators.System;
using CasualAdmin.Application.Models.DTOs.Requests.System;
using FluentValidation;

/// <summary>
/// 用户创建DTO验证器
/// </summary>
public class SysUserCreateValidator : AbstractValidator<SysUserCreateDto>
{
    /// <summary>
    /// 构造函数，配置验证规则
    /// </summary>
    public SysUserCreateValidator()
    {
        // 用户名验证
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("用户名不能为空")
            .MinimumLength(3).WithMessage("用户名长度不能少于3个字符")
            .MaximumLength(50).WithMessage("用户名长度不能超过50个字符");

        // 密码验证
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(6).WithMessage("密码长度不能少于6个字符")
            .MaximumLength(50).WithMessage("密码长度不能超过50个字符");

        // 真实姓名验证
        RuleFor(x => x.RealName)
            .NotEmpty().WithMessage("真实姓名不能为空")
            .MaximumLength(50).WithMessage("真实姓名长度不能超过50个字符");

        // 邮箱验证
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("邮箱不能为空")
            .EmailAddress().WithMessage("请输入有效的邮箱地址")
            .MaximumLength(200).WithMessage("邮箱长度不能超过200个字符");

        // 手机号验证
        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("手机号长度不能超过20个字符");

        // 状态验证
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("状态值无效");

        // 备注验证
        RuleFor(x => x.Remark)
            .MaximumLength(200).WithMessage("备注长度不能超过200个字符");
    }
}