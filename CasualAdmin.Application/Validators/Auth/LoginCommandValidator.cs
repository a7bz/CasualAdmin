namespace CasualAdmin.Application.Validators.Auth;

using CasualAdmin.Application.Commands.Auth;
using FluentValidation;

/// <summary>
/// 登录命令验证器
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    /// <summary>
    /// 构造函数，配置验证规则
    /// </summary>
    public LoginCommandValidator()
    {
        // 账号验证（支持用户名或邮箱）
        RuleFor(x => x.Account)
            .NotEmpty().WithMessage("账号不能为空")
            .Length(1, 100).WithMessage("账号长度不能超过100个字符");

        // 密码验证
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .Length(1, 255).WithMessage("密码长度不能超过255个字符");
    }
}