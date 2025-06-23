using System.Text.Json.Serialization;
using Lab8_AngelYaguno.Data;
using Lab8_AngelYaguno.Domain.Interfaces;
using Lab8_AngelYaguno.Services;
using Lab8_AngelYaguno.Models;
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

// Aplicar migraciones automáticamente al iniciar
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        Console.WriteLine("Aplicando migraciones y creando base de datos...");
        context.Database.EnsureCreated();
        
        // Verificar si ya hay datos
        var clientCount = context.Clients.Count();
        Console.WriteLine($"Número de clientes existentes: {clientCount}");
        
        if (clientCount == 0)
        {
            Console.WriteLine("Insertando datos de prueba...");
            SeedData(context);
        }
        else
        {
            Console.WriteLine("Los datos ya existen, omitiendo seed...");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al configurar base de datos: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapControllers();

app.Run();

// Método para insertar datos de prueba
static void SeedData(ApplicationDbContext context)
{
    try
    {
        // Insertar clientes
        var clients = new[]
        {
            new Client { Name = "Juan Pérez", Email = "juan.perez@example.com" },
            new Client { Name = "Ana Gómez", Email = "ana.gomez@example.com" },
            new Client { Name = "Carlos Díaz", Email = "carlos.diaz@example.com" },
            new Client { Name = "Lucía Martínez", Email = "lucia.martinez@example.com" }
        };
        context.Clients.AddRange(clients);
        context.SaveChanges();
        Console.WriteLine("Clientes insertados correctamente");

        // Insertar productos
        var products = new[]
        {
            new Product { Name = "Producto A", Price = 10.50m, Description = "This product is cheap" },
            new Product { Name = "Producto B", Price = 25.00m, Description = "This product is pretty" },
            new Product { Name = "Producto C", Price = 15.75m, Description = "This product is awesome" },
            new Product { Name = "Producto D", Price = 30.20m, Description = null }
        };
        context.Products.AddRange(products);
        context.SaveChanges();
        Console.WriteLine("Productos insertados correctamente");

        // Insertar pedidos
        var orders = new[]
        {
            new Order { ClientId = 1, OrderDate = new DateTime(2025, 5, 1, 10, 0, 0) },
            new Order { ClientId = 1, OrderDate = new DateTime(2025, 5, 2, 11, 0, 0) },
            new Order { ClientId = 2, OrderDate = new DateTime(2025, 5, 3, 12, 0, 0) },
            new Order { ClientId = 2, OrderDate = new DateTime(2025, 5, 4, 13, 0, 0) },
            new Order { ClientId = 3, OrderDate = new DateTime(2025, 5, 5, 14, 0, 0) },
            new Order { ClientId = 3, OrderDate = new DateTime(2025, 5, 6, 15, 0, 0) },
            new Order { ClientId = 4, OrderDate = new DateTime(2025, 5, 7, 16, 0, 0) }
        };
        context.Orders.AddRange(orders);
        context.SaveChanges();
        Console.WriteLine("Pedidos insertados correctamente");

        // Insertar detalles de pedidos
        var orderDetails = new[]
        {
            new Orderdetail { OrderId = 1, ProductId = 2, Quantity = 2 },
            new Orderdetail { OrderId = 1, ProductId = 1, Quantity = 1 },
            new Orderdetail { OrderId = 2, ProductId = 3, Quantity = 1 },
            new Orderdetail { OrderId = 2, ProductId = 4, Quantity = 1 },
            new Orderdetail { OrderId = 3, ProductId = 1, Quantity = 3 },
            new Orderdetail { OrderId = 3, ProductId = 2, Quantity = 2 },
            new Orderdetail { OrderId = 4, ProductId = 3, Quantity = 1 }
        };
        context.Orderdetails.AddRange(orderDetails);
        context.SaveChanges();
        Console.WriteLine("Detalles de pedidos insertados correctamente");

        Console.WriteLine("Seed de datos completado exitosamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error durante el seed de datos: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
}