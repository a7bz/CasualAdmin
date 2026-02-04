using CasualAdmin.API.Configurations;

var builder = WebApplication.CreateBuilder(args);

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