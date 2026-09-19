using Microsoft.EntityFrameworkCore;
using Opus127.Dengue.Api.Data;
using Opus127.Dengue.Api.Integrations.AlertaDengue;
using Opus127.Dengue.Api.Repositories;
using Opus127.Dengue.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DengueDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Dengue")));
builder.Services.AddHttpClient<IAlertaDengueClient, AlertaDengueClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["AlertaDengue:BaseUrl"]!));
builder.Services.AddScoped<IDengueAlertRepository, DengueAlertRepository>();
builder.Services.AddScoped<IDengueSyncService, DengueSyncService>();
builder.Services.AddHostedService<DengueSyncHostedService>();

var app = builder.Build();

try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<DengueDbContext>();
    db.Database.Migrate();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Database");
    logger.LogError(
        ex,
        "Falha ao migrar o banco DengueAlerts. Confirme o SQL Server no Docker e a connection string Dengue.");
    throw;
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
