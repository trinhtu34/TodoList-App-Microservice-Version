using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace ToDoService.Extensions;

public static class CacheExtensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis");
        
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            // Use Redis for distributed caching
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "TodoService";
            });
        }
        else
        {
            // Fallback to in-memory cache for development
            services.AddMemoryCache();
            services.AddSingleton<IDistributedCache, MemoryDistributedCache>();
        }

        return services;
    }
}

// Usage in Startup.cs or Program.cs
public void ConfigureServices(IServiceCollection services)
{
    // Add caching
    services.AddCaching(Configuration);
    
    // Register services
    services.AddScoped<ITodoService, TodoService>();
    services.Decorate<ITodoService, CachedTodoService>();
}