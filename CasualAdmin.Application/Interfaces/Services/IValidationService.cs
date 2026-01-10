namespace CasualAdmin.Application.Interfaces.Services;
using FluentValidation;

/// <summary>
/// 验证服务接口，用于验证实体数据
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// 验证实体数据
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <returns>验证结果，包含验证是否成功和错误信息</returns>
    (bool IsValid, IEnumerable<string> Errors) Validate<T>(T entity) where T : class, new();

    /// <summary>
    /// 验证实体数据，如果验证失败则抛出异常
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    void ValidateAndThrow<T>(T entity) where T : class, new();
}