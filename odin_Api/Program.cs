using Microsoft.EntityFrameworkCore;
using odin_Application.Interfaces;
using odin_Infrastructure.Data;
using odin_Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. DATABASE CONFIGURATION (Postgres + PostGIS)
// ==========================================
builder.Services.AddDbContext<OsintDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseNetTopologySuite() // CRITICAL: Enables spatial translation
    ));

// ==========================================
// 2. DEPENDENCY INJECTION (Clean Architecture)
// ==========================================
// We use AddScoped so a new repository instance is created per HTTP request
builder.Services.AddScoped<IMeshtasticRepository, MeshtasticRepository>();
builder.Services.AddScoped<IOsintEventRepository, OsintEventRepository>();
builder.Services.AddScoped<ISigintRepository, SigintRepository>();

// ==========================================
// 3. API & FRONTEND CONFIGURATION
// ==========================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS Policy so your future React/Vue dashboard can actually talk to this API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDashboard", policy =>
    {
        policy.AllowAnyOrigin() // We will lock this down to your specific IPs later
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVite",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Your Vite dev server
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// ==========================================
// SEED DATABASE (Development Only)
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<OsintDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

// ==========================================
// 4. HTTP REQUEST PIPELINE
// ==========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowDashboard");
app.UseCors("AllowVite");

app.UseAuthorization();

app.MapControllers();

app.Run();
