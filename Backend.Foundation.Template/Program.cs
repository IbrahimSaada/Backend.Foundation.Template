using Backend.Foundation.Template.Abstractions.Clock;
using Backend.Foundation.Template.Abstractions.Persistence;
using Backend.Foundation.Template.Application;
using Backend.Foundation.Template.Infrastructure;
using Backend.Foundation.Template.Infrastructure.Caching;
using Backend.Foundation.Template.Persistence;
using Backend.Foundation.Template.Security.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Backend Foundation Template API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddApplication();
builder.Services.AddTemplateSecurity(builder.Configuration);
builder.Services.AddTemplateCaching(builder.Configuration);

// Fallback infrastructure so template runs even when persistence provider is not configured yet.
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddScoped<IUnitOfWork, NoOpUnitOfWork>();

ConfigurePersistence(builder);

var app = builder.Build();

app.UseGlobalExceptionHandling(app.Environment.IsDevelopment());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend Foundation Template API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");
app.MapControllers();

app.Run();

static void ConfigurePersistence(WebApplicationBuilder builder)
{
    var provider = builder.Configuration["Persistence:Provider"] ?? "None";

    if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
    {
        builder.Services.AddSqlServerPersistence(builder.Configuration);
        return;
    }

    if (provider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
    {
        builder.Services.AddPostgresPersistence(builder.Configuration);
        return;
    }

    if (provider.Equals("Mongo", StringComparison.OrdinalIgnoreCase))
    {
        builder.Services.AddMongoPersistence(builder.Configuration);
    }
}

public partial class Program;
