namespace CasualAdmin.Application.Validators.Auth;

using CasualAdmin.Application.Commands.Auth;
using CasualAdmin.Application.Interfaces.System;
using FluentValidation;
using global::System.Text.RegularExpressions;

/// <summary>
/// 注册命令验证器
/// </summary>
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private readonly IRsaEncryptionService _rsaEncryptionService;

    /// <summary>
    /// 构造函数，配置验证规则
    /// </summary>
    public RegisterCommandValidator(IRsaEncryptionService rsaEncryptionService)
    {
        _rsaEncryptionService = rsaEncryptionService;

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

        // 密码验证（解密后的密码复杂度验证）
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .Must(BeValidBase64).WithMessage("密码格式不正确")
            .Length(1, 500).WithMessage("密码长度超过限制")
            .Must(MeetComplexityRequirements).WithMessage("密码长度不能少于8位，必须包含大小写字母、数字和特殊字符");

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

        // 验证密码复杂度要求
        bool MeetComplexityRequirements(string encryptedPassword)
        {
            try
            {
                var decryptedPassword = _rsaEncryptionService.Decrypt(encryptedPassword);

                // 使用正则表达式验证密码复杂度：至少8位，包含大小写字母、数字和特殊字符
                var pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$";
                return Regex.IsMatch(decryptedPassword, pattern);
            }
            catch
            {
                return false;
            }
        }
    }
}