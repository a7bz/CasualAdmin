namespace CasualAdmin.Application.Services;
using CasualAdmin.Application.Interfaces.Services;
using FluentValidation;
using global::System.Reflection;

/// <summary>
/// 验证服务实现，使用FluentValidation进行实体验证
/// </summary>
public class ValidationService : IValidationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, IValidator> _validators;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serviceProvider">服务提供器，用于解析验证器</param>
    public ValidationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _validators = new Dictionary<Type, IValidator>();

        // 扫描并注册所有验证器
        RegisterValidators();
    }

    /// <summary>
    /// 扫描并注册所有验证器
    /// </summary>
    private void RegisterValidators()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName != null && a.FullName.Contains("CasualAdmin"));

        foreach (var assembly in assemblies)
        {
            var validatorTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IValidator).IsAssignableFrom(t));

            foreach (var validatorType in validatorTypes)
            {
                // 获取验证器对应的实体类型
                var entityType = validatorType.BaseType?.GetGenericArguments().FirstOrDefault();
                if (entityType != null)
                {
                    var validator = Activator.CreateInstance(validatorType) as IValidator;
                    if (validator != null)
                    {
                        _validators[entityType] = validator;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 验证实体数据
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <returns>验证结果，包含验证是否成功和错误信息</returns>
    public (bool IsValid, IEnumerable<string> Errors) Validate<T>(T entity) where T : class, new()
    {
        if (entity == null)
        {
            return (false, new[] { "实体不能为空" });
        }

        // 真正使用FluentValidation进行验证
        if (_validators.TryGetValue(typeof(T), out var validator))
        {
            var validationResult = validator.Validate(new ValidationContext<T>(entity));
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                return (false, errors);
            }
        }

        return (true, Enumerable.Empty<string>());
    }

    /// <summary>
    /// 验证实体数据，如果验证失败则抛出异常
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    public void ValidateAndThrow<T>(T entity) where T : class, new()
    {
        var (isValid, errors) = Validate(entity);
        if (!isValid)
        {
            throw new ValidationException(string.Join(", ", errors));
        }
    }
}