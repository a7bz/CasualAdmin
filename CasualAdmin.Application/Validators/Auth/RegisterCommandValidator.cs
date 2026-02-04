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

        // 密码格式验证（仅验证是否为有效的Base64字符串）
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .Must(BeValidBase64).WithMessage("密码格式不正确，必须为Base64编码的字符串")
            .Length(1, 500).WithMessage("密码长度超过限制");

        // 验证是否为有效的Base64字符串
        bool BeValidBase64(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

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