namespace CasualAdmin.Application.Validators.Auth;
using CasualAdmin.Application.Commands.Auth;
using FluentValidation;

/// <summary>
/// 注册命令验证器
/// </summary>
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    /// <summary>
    /// 构造函数，配置验证规则
    /// </summary>
    public RegisterCommandValidator()
    {
        // 用户名验证
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("用户名不能为空")
            .MinimumLength(3).WithMessage("用户名长度不能少于3个字符")
            .MaximumLength(50).WithMessage("用户名长度不能超过50个字符");

        // 邮箱验证
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("邮箱不能为空")
            .EmailAddress().WithMessage("请输入有效的邮箱地址")
            .MaximumLength(100).WithMessage("邮箱长度不能超过100个字符");

        // 密码验证
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(6).WithMessage("密码长度不能少于6个字符")
            .MaximumLength(20).WithMessage("密码长度不能超过20个字符")
            .Matches(@"[A-Za-z]").WithMessage("密码必须包含至少一个字母")
            .Matches(@"[0-9]").WithMessage("密码必须包含至少一个数字");
    }
}