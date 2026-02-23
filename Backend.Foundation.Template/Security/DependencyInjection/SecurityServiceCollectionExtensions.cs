using Backend.Foundation.Template.Abstractions.Security;
using Backend.Foundation.Template.Security.Authorization;
using Backend.Foundation.Template.Security.Configuration;
using Backend.Foundation.Template.Security.CurrentUser;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Foundation.Template.Security.DependencyInjection;

internal static class SecurityServiceCollectionExtensions
{
    public static IServiceCollection AddTemplateSecurity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.Configure<AuthenticationOptions>(configuration.GetSection(AuthenticationOptions.SectionName));
        services.Configure<AuthorizationMappingOptions>(configuration.GetSection(AuthorizationMappingOptions.SectionName));

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, HttpContextCurrentUser>();
        services.AddScoped<IPermissionEvaluator, PermissionEvaluator>();

        var authOptions = configuration
            .GetSection(AuthenticationOptions.SectionName)
            .Get<AuthenticationOptions>() ?? new AuthenticationOptions();

        if (!authOptions.Enabled)
        {
            services.AddAuthentication();
            services.AddAuthorization();
            return services;
        }

        if (string.IsNullOrWhiteSpace(authOptions.Authority))
        {
            throw new InvalidOperationException(
                "Authentication is enabled but Authority is missing. Configure Authentication:Authority.");
        }

        var authenticationBuilder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
        services.AddAuthorization();

        authenticationBuilder
            .AddJwtBearer(options =>
            {
                options.Authority = authOptions.Authority;
                options.RequireHttpsMetadata = authOptions.RequireHttpsMetadata;

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
            });

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return services;
    }
}
