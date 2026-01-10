using CasualAdmin.API.Configurations;
using CasualAdmin.Infrastructure.Data.Context;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var port = builder.Configuration.GetValue("Port", 5000);
builder.WebHost.UseUrls($"http://*:{port}");

builder.ConfigureSerilog();

builder.Services.ConfigureDatabase();

builder.Services.RegisterServices(builder.Configuration);

builder.Services.ConfigureAutoMapper();

builder.Services.ConfigureJwt(builder.Configuration);

builder.Services.ConfigureSwagger();

builder.Services.AddControllers();

// 添加FluentValidation自动验证
builder.Services.AddFluentValidationAutoValidation()
    .AddValidatorsFromAssembly(typeof(CasualAdmin.Application.Validators.System.SysDeptCreateValidator).Assembly);

var app = builder.Build();

app.ConfigureMiddleware();

app.UseSwaggerMiddleware();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var sqlSugarContext = scope.ServiceProvider.GetRequiredService<SqlSugarContext>();
    sqlSugarContext.ExecuteCodeFirst();
}

app.Run();