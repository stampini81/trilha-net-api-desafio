using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TrilhaApiDesafio.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var conn = builder.Configuration.GetConnectionString("ConexaoPadrao");
builder.Services.AddDbContext<OrganizadorContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(conn) && conn.Contains("Data Source=", StringComparison.OrdinalIgnoreCase))
    {
        // SQLite connection string example: Data Source=organizador.db
        options.UseSqlite(conn);
    }
    else if (!string.IsNullOrWhiteSpace(conn))
    {
        options.UseSqlServer(conn);
    }
    else
    {
        // Fallback to SQLite local file if no connection string
        options.UseSqlite("Data Source=organizador.db");
    }
});

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Apply database initialization automatically at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrganizadorContext>();
    try
    {
        var hasMigrations = db.Database.GetMigrations().Any();
        if (hasMigrations)
            db.Database.Migrate();
        else
            db.Database.EnsureCreated();
    }
    catch (Exception)
    {
        // Intencionalmente suprimido para evitar falha de boot em ambientes sem banco configurado
    }
}

app.Run();
