namespace CasualAdmin.API.Configurations;
using CasualAdmin.Domain.Infrastructure.Data;
using CasualAdmin.Infrastructure.Data.Context;

public static class DatabaseConfiguration
{
    public static void ConfigureDatabase(this IServiceCollection services)
    {
        // 注册SqlSugar上下文，同时注册接口和实现
        services.AddSingleton<SqlSugarContext>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("数据库连接字符串未配置");

            // 从配置中读取数据库类型
            var dbType = configuration.GetSection("Database:Type").Get<string>() ?? "PostgreSQL";

            return new SqlSugarContext(connectionString, dbType, configuration);
        });

        // 注册IDbContext接口，指向已注册的SqlSugarContext实例
        services.AddSingleton<IDbContext>(provider =>
            provider.GetRequiredService<SqlSugarContext>());
    }
}
