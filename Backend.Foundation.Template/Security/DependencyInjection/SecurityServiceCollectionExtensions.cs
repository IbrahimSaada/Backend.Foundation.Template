using Backend.Foundation.Template.Abstractions.Security;
using Backend.Foundation.Template.Security.Authentication;
using Backend.Foundation.Template.Security.Authorization;
using Backend.Foundation.Template.Security.Configuration;
using Backend.Foundation.Template.Security.CurrentUser;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using TemplateAuthenticationOptions = Backend.Foundation.Template.Security.Configuration.AuthenticationOptions;

namespace Backend.Foundation.Template.Security.DependencyInjection;

internal static class SecurityServiceCollectionExtensions
{
    public static IServiceCollection AddTemplateSecurity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.Configure<TemplateAuthenticationOptions>(configuration.GetSection(TemplateAuthenticationOptions.SectionName));
        services.Configure<AuthorizationMappingOptions>(configuration.GetSection(AuthorizationMappingOptions.SectionName));

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, HttpContextCurrentUser>();
        services.AddScoped<IPermissionEvaluator, PermissionEvaluator>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        var authOptions = configuration
            .GetSection(TemplateAuthenticationOptions.SectionName)
            .Get<TemplateAuthenticationOptions>() ?? new TemplateAuthenticationOptions();

        if (!authOptions.Enabled)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = NoAuthAuthenticationHandler.SchemeName;
                    options.DefaultChallengeScheme = NoAuthAuthenticationHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, NoAuthAuthenticationHandler>(
                    NoAuthAuthenticationHandler.SchemeName,
                    _ => { });

            services.AddAuthorization();
            return services;
        }

        ValidateAuthenticationOptions(authOptions);

        var authenticationBuilder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
        services.AddAuthorization();

        authenticationBuilder
            .AddJwtBearer(options =>
            {
                options.Authority = authOptions.Authority;
                options.RequireHttpsMetadata = authOptions.RequireHttpsMetadata;
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = authOptions.ValidateIssuer,
                    ValidateAudience = authOptions.ValidateAudience,
                    ValidateLifetime = authOptions.ValidateLifetime,
                    ValidateIssuerSigningKey = true,
                    NameClaimType = authOptions.NameClaimType,
                    RoleClaimType = authOptions.RoleClaimType,
                    ClockSkew = TimeSpan.FromSeconds(Math.Max(0, authOptions.ClockSkewSeconds))
                };

                var audiences = authOptions.Audiences
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                if (audiences.Length > 0)
                {
                    options.TokenValidationParameters.ValidAudiences = audiences;
                }

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger("JwtBearer");
                        logger.LogWarning(
                            context.Exception,
                            "JWT authentication failed.");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger("JwtBearer");
                        logger.LogDebug(
                            "JWT token validated for subject {Subject}.",
                            context.Principal?.FindFirst("sub")?.Value ?? "(unknown)");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger("JwtBearer");
                        logger.LogDebug("JWT challenge executed. Error: {Error}", context.Error);
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }

    private static void ValidateAuthenticationOptions(TemplateAuthenticationOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Authority))
        {
            throw new InvalidOperationException(
                "Authentication is enabled but Authority is missing. Configure Authentication:Authority.");
        }

        if (options.ValidateAudience &&
            (options.Audiences is null || options.Audiences.Count == 0))
        {
            throw new InvalidOperationException(
                "Authentication audience validation is enabled, but no audiences are configured. Set Authentication:Audiences.");
        }

        if (options.ClockSkewSeconds < 0)
        {
            throw new InvalidOperationException("Authentication:ClockSkewSeconds must be >= 0.");
        }
    }
}
