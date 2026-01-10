namespace CasualAdmin.Application.Validators.File;

using CasualAdmin.Application.Models.File;
using FluentValidation;

/// <summary>
/// 分片上传请求验证器
/// </summary>
public class UploadPartRequestValidator : AbstractValidator<UploadPartRequest>
{
    /// <summary>
    /// 构造函数，配置验证规则
    /// </summary>
    public UploadPartRequestValidator()
    {
        // 文件名验证
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("文件名不能为空")
            .Length(1, 255).WithMessage("文件名长度不能超过255个字符");

        // 分片总数验证
        RuleFor(x => x.TotalParts)
            .GreaterThan(0).WithMessage("分片总数必须大于0");

        // 分片索引验证
        RuleFor(x => x.PartIndex)
            .GreaterThanOrEqualTo(0).WithMessage("分片索引不能小于0")
            .LessThan(x => x.TotalParts).WithMessage("分片索引必须小于分片总数");

        // 分片大小验证
        RuleFor(x => x.PartSize)
            .GreaterThan(0).WithMessage("分片大小必须大于0");

        // 文件总大小验证
        RuleFor(x => x.TotalSize)
            .GreaterThan(0).WithMessage("文件总大小必须大于0");

        // 内容类型验证
        RuleFor(x => x.ContentType)
            .Length(0, 100).WithMessage("内容类型长度不能超过100个字符")
            .When(x => !string.IsNullOrWhiteSpace(x.ContentType));

        // 存储路径验证
        RuleFor(x => x.FolderPath)
            .Length(0, 100).WithMessage("存储路径长度不能超过100个字符")
            .When(x => !string.IsNullOrWhiteSpace(x.FolderPath));
    }
}