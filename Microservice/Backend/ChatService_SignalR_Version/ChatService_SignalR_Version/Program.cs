using Cassandra;
using StackExchange.Redis;
using ChatService_SignalR_Version.Hub;
using ChatService_SignalR_Version.Services;
using ChatService_SignalR_Version.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;

var builder = WebApplication.CreateBuilder(args);

// ScyllaDB Configuration
var scyllaCluster = Cluster.Builder()
    .AddContactPoint("localhost")
    .WithPort(9042)
    .Build();

var scyllaSession = await scyllaCluster.ConnectAsync("chat_service");

// Redis Configuration
var redis = ConnectionMultiplexer.Connect("localhost:6379");
var redisDatabase = redis.GetDatabase();

// Register services
builder.Services.AddSingleton<Cassandra.ISession>(scyllaSession);
builder.Services.AddSingleton<IDatabase>(redisDatabase);

// MVP: Core messaging only 
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IChatService, ChatServiceMVP>();

// Add controllers and SignalR
builder.Services.AddControllers();
builder.Services.AddSignalR();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Configure using AWS Cognito 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? Environment.GetEnvironmentVariable("JWT__Issuer");
    var jwtAudience = builder.Configuration["JWT:Audience"] ?? Environment.GetEnvironmentVariable("JWT__Audience");
    var cognitoAuthority = builder.Configuration["AWS:Cognito:Authority"] ?? Environment.GetEnvironmentVariable("AWS__Cognito__Authority");

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

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

app.Run();
