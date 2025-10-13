using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ToDoService.Models;

namespace ToDoService;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Env.Load();

        var builder = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .AddEnvironmentVariables();
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddDbContext<TodoServiceDbContext>(options =>
            options.UseMySql(
                Configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("DefaultConnection"))
            ));

        // Service Clients
        services.AddHttpClient<ServiceClients.ITagServiceClient, ServiceClients.TagServiceClient>(client =>
        {
            client.BaseAddress = new Uri(Configuration["ServiceEndpoints:TagService"]);
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtSettings = Configuration.GetSection("JWT");
                var awsSettings = Configuration.GetSection("AWS:Cognito");

                options.Authority = awsSettings["Authority"];
                options.Audience = jwtSettings["Audience"];

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
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
                            return clientId == jwtSettings["Audience"];
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

        services.AddAuthorization(options =>
        {
            options.AddPolicy("PremiumOnly", policy =>
                policy.RequireRole("Premium-user"));
            options.AddPolicy("NormalOnly", policy =>
                policy.RequireRole("Normal-user"));
        });

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

        services.AddLogging();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
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