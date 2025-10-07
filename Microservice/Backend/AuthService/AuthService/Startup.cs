using AuthService.Services;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AuthService;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Env.Load();

        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddScoped<ICognitoService, CognitoService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtIssuer = Environment.GetEnvironmentVariable("JWT__Issuer");
                var jwtAudience = Environment.GetEnvironmentVariable("JWT__Audience");
                var cognitoAuthority = Environment.GetEnvironmentVariable("AWS__Cognito__Authority");

                options.Authority = cognitoAuthority;
                options.Audience = jwtAudience;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,

                    RoleClaimType = "cognito:groups",

                    AudienceValidator = (audiences, securityToken, validationParameters) =>
                    {
                        if (securityToken is JsonWebToken jwt)
                        {
                            var clientId = jwt.Claims?.FirstOrDefault(c => c.Type == "client_id")?.Value;
                            Console.WriteLine($"Validating audience - ClientId: {clientId}, Expected: {jwtAudience}");
                            return clientId == jwtAudience;
                        }
                        return false;
                    }
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine($"Token validated for user: {context.Principal.Identity?.Name}");
                        return Task.CompletedTask;
                    }
                };
            });

        // Remember to spell correctly when defining roles , same as the name � the cognito groups
        services.AddAuthorization(options =>
        {
            options.AddPolicy("PremiumOnly", policy =>
                policy.RequireRole("Premium-user"));
            options.AddPolicy("NormalOnly", policy =>
                policy.RequireRole("Normal-user"));
        });

        // remember to enable CORS , if deploy to Production , change the AllowAll to specific domain . e.g Amplify's domain , API Gateway's domain
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.SetIsOriginAllowed(origin => true)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }
}