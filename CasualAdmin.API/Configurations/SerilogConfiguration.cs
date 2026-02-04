namespace CasualAdmin.API.Configurations;
using Serilog;

public static class SerilogConfiguration
{
    public static void ConfigureSerilog(this WebApplicationBuilder builder)
    {
        // 从配置文件加载Serilog配置
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "CasualAdmin")
            .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
        );

        // 确保在程序退出时关闭Serilog
        AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
    }
}
