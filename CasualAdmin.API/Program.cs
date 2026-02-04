using CasualAdmin.API.Configurations;

var builder = WebApplication.CreateBuilder(args);

// 配置热重载
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// 验证配置
var validationResult = builder.Configuration.ValidateConfiguration();
if (!validationResult.IsValid)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("========================================");
    Console.WriteLine("配置验证失败，应用程序无法启动：");
    Console.WriteLine("========================================");
    Console.WriteLine(validationResult.GetErrorMessage());
    Console.WriteLine("========================================");
    Console.ResetColor();
    Environment.Exit(1);
    return;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("配置验证通过。");
Console.ResetColor();

// 配置端口
var port = builder.Configuration.GetValue("Port", 5000);
builder.WebHost.UseUrls($"http://*:{port}");

// 配置服务
builder.ConfigureSerilog();
builder.Services.ConfigureDatabase();
builder.Services.RegisterServices(builder.Configuration);
builder.Services.ConfigureAutoMapper();
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.ConfigureAuthorization();
builder.Services.ConfigureFluentValidation();
builder.Services.AddControllers();

var app = builder.Build();

// 配置中间件
app.ConfigureMiddleware();
app.UseSwaggerMiddleware();
app.MapControllers();

// 服务预热
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.WarmupServicesAsync();
}

app.Run();