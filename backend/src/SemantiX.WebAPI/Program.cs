using SemantiX.Infrastructure;
using SemantiX.WebAPI.Hubs;
using SemantiX.WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// === Services ===

// Onion Architecture DI (Infrastructure layer qeydiyyatı)
builder.Services.AddInfrastructure(builder.Configuration);

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        opts.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        opts.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
});

// CORS — Mobile app üçün
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });

    options.AddPolicy("SignalR", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "SemantiX API",
        Version = "v1",
        Description = "AI-powered semantik söz oyunu API — Azərbaycan dilində"
    });
});

var app = builder.Build();

// === Middleware Pipeline ===

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SemantiX API v1"));
}

app.UseCors();

app.MapControllers();
app.MapHub<GameHub>("/hubs/game", options =>
{
    options.AllowStatefulReconnects = true;
}).RequireCors("SignalR");

// Health check
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "SemantiX API" }));

// Auto-migrate database in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<SemantiX.Infrastructure.Data.AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.Run();
