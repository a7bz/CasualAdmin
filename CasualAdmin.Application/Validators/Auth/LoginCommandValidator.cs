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

        // 密码验证（加密后的密码）
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .Must(BeValidBase64).WithMessage("密码格式不正确")
            .Length(1, 500).WithMessage("密码长度超过限制");

        // 验证是否为有效的Base64字符串
        bool BeValidBase64(string password)
        {
            try
            {
                Convert.FromBase64String(password);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}