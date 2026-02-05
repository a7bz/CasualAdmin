namespace CasualAdmin.Shared.Helpers;

using System.Linq.Expressions;
using System.Reflection;

/// <summary>
/// 表达式构建器，用于动态构建查询表达式
/// </summary>
public static class ExpressionBuilder
{
    /// <summary>
    /// 根据筛选器对象构建查询表达式
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TFilter">筛选器类型</typeparam>
    /// <param name="filter">筛选器对象</param>
    /// <returns>查询表达式</returns>
    public static Expression<Func<TEntity, bool>> BuildPredicate<TEntity, TFilter>(TFilter filter)
        where TEntity : class
        where TFilter : class
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        Expression? combinedExpression = null;

        if (filter == null)
        {
            return Expression.Lambda<Func<TEntity, bool>>(Expression.Constant(true), parameter);
        }

        var filterProperties = typeof(TFilter).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var filterProperty in filterProperties)
        {
            var filterValue = filterProperty.GetValue(filter);

            // 跳过 null 或空值
            if (filterValue == null)
            {
                continue;
            }

            // 对于字符串，跳过空字符串
            if (filterValue is string str && string.IsNullOrWhiteSpace(str))
            {
                continue;
            }

            // 对于 Guid，跳过 Guid.Empty
            if (filterValue is Guid guid && guid == Guid.Empty)
            {
                continue;
            }

            // 对于 DateTime，跳过默认值
            if (filterValue is DateTime dateTime && dateTime == default)
            {
                continue;
            }

            // 对于 int，跳过默认值 0
            if (filterValue is int intValue && intValue == 0)
            {
                continue;
            }
            var nullableInt = filterValue as int?;
            if (nullableInt != null && (!nullableInt.HasValue || nullableInt.Value == 0))
            {
                continue;
            }
            // 对于枚举，跳过默认值（枚举的第一个值，通常是0）
            if (filterValue.GetType().IsEnum)
            {
                var defaultValue = Enum.ToObject(filterValue.GetType(), 0);
                if (filterValue.Equals(defaultValue))
                {
                    continue;
                }
            }

            // 查找实体中对应的属性
            var entityProperty = typeof(TEntity).GetProperty(filterProperty.Name, BindingFlags.Public | BindingFlags.Instance);
            if (entityProperty == null)
            {
                continue;
            }

            // 构建属性访问表达式
            var entityPropertyAccess = Expression.Property(parameter, entityProperty);

            // 根据属性类型构建条件表达式
            Expression? conditionExpression = null;

            if (entityProperty.PropertyType == typeof(string))
            {
                // 字符串类型：使用 Contains 模糊匹配
                var stringConstant = Expression.Constant(filterValue.ToString(), typeof(string));
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
                conditionExpression = Expression.Call(entityPropertyAccess, containsMethod, stringConstant);
            }
            else if (entityProperty.PropertyType == typeof(bool?) || entityProperty.PropertyType == typeof(bool))
            {
                // 布尔类型：精确匹配
                var boolValue = Convert.ToBoolean(filterValue);
                var boolConstant = Expression.Constant(boolValue);
                conditionExpression = Expression.Equal(entityPropertyAccess, boolConstant);
            }
            else if (entityProperty.PropertyType == typeof(int?) || entityProperty.PropertyType == typeof(int))
            {
                // 整数类型：精确匹配
                var intValue2 = Convert.ToInt32(filterValue);
                var intConstant = Expression.Constant(intValue2);
                conditionExpression = Expression.Equal(entityPropertyAccess, intConstant);
            }
            else if (entityProperty.PropertyType == typeof(Guid?) || entityProperty.PropertyType == typeof(Guid))
            {
                // Guid 类型：精确匹配
                var guidValue = (Guid)filterValue;
                var guidConstant = Expression.Constant(guidValue);
                conditionExpression = Expression.Equal(entityPropertyAccess, guidConstant);
            }
            else if (entityProperty.PropertyType == typeof(DateTime?) || entityProperty.PropertyType == typeof(DateTime))
            {
                // DateTime 类型：精确匹配
                var dateTimeValue = (DateTime)filterValue;
                var dateTimeConstant = Expression.Constant(dateTimeValue);
                conditionExpression = Expression.Equal(entityPropertyAccess, dateTimeConstant);
            }
            else if (entityProperty.PropertyType.IsEnum)
            {
                // 枚举类型：精确匹配
                var enumValue = Enum.ToObject(entityProperty.PropertyType, filterValue);
                var enumConstant = Expression.Constant(enumValue, entityProperty.PropertyType);
                conditionExpression = Expression.Equal(entityPropertyAccess, enumConstant);
            }
            else
            {
                // 其他类型：尝试使用 Equals 方法
                var valueConstant = Expression.Constant(filterValue);
                var equalsMethod = entityProperty.PropertyType.GetMethod("Equals", new[] { filterValue.GetType() });
                if (equalsMethod != null)
                {
                    conditionExpression = Expression.Call(entityPropertyAccess, equalsMethod, valueConstant);
                }
                else
                {
                    // 使用 Equal 表达式
                    conditionExpression = Expression.Equal(entityPropertyAccess, valueConstant);
                }
            }

            // 组合所有条件
            if (conditionExpression != null)
            {
                if (combinedExpression == null)
                {
                    combinedExpression = conditionExpression;
                }
                else
                {
                    combinedExpression = Expression.AndAlso(combinedExpression, conditionExpression);
                }
            }
        }

        // 如果没有任何条件，返回 true 表达式
        if (combinedExpression == null)
        {
            return Expression.Lambda<Func<TEntity, bool>>(Expression.Constant(true), parameter);
        }

        return Expression.Lambda<Func<TEntity, bool>>(combinedExpression, parameter);
    }

    /// <summary>
    /// 构建排序表达式
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortDirection">排序方向</param>
    /// <returns>排序表达式（属性信息）</returns>
    public static PropertyInfo? BuildSortProperty<TEntity>(string? sortField)
    {
        if (string.IsNullOrWhiteSpace(sortField))
        {
            return null;
        }

        var property = typeof(TEntity).GetProperty(sortField, BindingFlags.Public | BindingFlags.Instance);
        return property;
    }
}