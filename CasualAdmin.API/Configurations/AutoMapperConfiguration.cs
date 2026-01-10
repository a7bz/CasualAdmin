namespace CasualAdmin.API.Configurations;
using System.Reflection;

public static class AutoMapperConfiguration
{
    public static void ConfigureAutoMapper(this IServiceCollection services)
    {
        // 配置AutoMapper
        var config = new AutoMapper.MapperConfiguration(cfg =>
        {
            cfg.AddMaps(
            [
                Assembly.Load("CasualAdmin.Application"),
                Assembly.Load("CasualAdmin.API")
            ]);
        });

        var mapper = config.CreateMapper();
        services.AddSingleton(mapper);
    }
}
