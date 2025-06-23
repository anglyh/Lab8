using System.Text.Json.Serialization;
using Lab8_AngelYaguno.Data;
using Lab8_AngelYaguno.Domain.Interfaces;
using Lab8_AngelYaguno.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar cadena de conexión basada en el entorno
string connectionString;

if (builder.Environment.IsDevelopment())
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}
else
{
    // En producción, usar DATABASE_URL de Render
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    
    if (!string.IsNullOrEmpty(databaseUrl))
    {
        // Parsear la URL de Render: postgresql://user:password@host:port/database
        var uri = new Uri(databaseUrl);
        connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.LocalPath.TrimStart('/')};Username={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]};SSL Mode=Require;Trust Server Certificate=true";
    }
    else
    {
        // Fallback a variables individuales si DATABASE_URL no existe
        connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};Port={Environment.GetEnvironmentVariable("DB_PORT")};Database={Environment.GetEnvironmentVariable("DB_NAME")};Username={Environment.GetEnvironmentVariable("DB_USER")};Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};SSL Mode=Require;Trust Server Certificate=true";
    }
}

// Debug: Log de la cadena de conexión (solo para depuración)
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"DATABASE_URL exists: {Environment.GetEnvironmentVariable("DATABASE_URL") != null}");
Console.WriteLine($"Connection string length: {connectionString?.Length ?? 0}");
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("ERROR: Connection string is null or empty!");
}
else
{
    // Log solo la parte del host para debugging (sin exponer credenciales)
    var hostPart = connectionString.Split(';')[0];
    Console.WriteLine($"Host part: {hostPart}");
}

// Configurar PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ILinqService, LinqService>();

builder.Services
    .AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "My API", Version = "v1" });
});

var app = builder.Build();

// Configurar para producción y desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    // Habilitar Swagger también en producción para Render
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // Swagger UI en la raíz
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapControllers();

app.Run();