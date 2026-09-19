using Microsoft.EntityFrameworkCore;
using Opus127.Dengue.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DengueDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Dengue")));

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
