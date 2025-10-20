using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace ToDoService.Services;

public class CachedTodoService : ITodoService
{
    private readonly ITodoService _todoService;
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachedTodoService> _logger;
    private readonly TimeSpan _defaultCacheDuration = TimeSpan.FromMinutes(10);

    public CachedTodoService(
        ITodoService todoService,
        IDistributedCache cache,
        ILogger<CachedTodoService> logger)
    {
        _todoService = todoService;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<Todo>> GetTodosAsync(string userId, int? groupId = null)
    {
        var cacheKey = $"todos:user:{userId}:group:{groupId ?? 0}";
        
        try
        {
            // Try to get from cache first
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                return JsonSerializer.Deserialize<List<Todo>>(cachedData) ?? new List<Todo>();
            }

            _logger.LogInformation("Cache miss for key: {CacheKey}", cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache read failed for key: {CacheKey}", cacheKey);
        }

        // Get from database
        var todos = await _todoService.GetTodosAsync(userId, groupId);

        // Cache the result
        try
        {
            var serializedData = JsonSerializer.Serialize(todos);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _defaultCacheDuration
            };
            
            await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions);
            _logger.LogInformation("Cached data for key: {CacheKey}", cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache write failed for key: {CacheKey}", cacheKey);
        }

        return todos;
    }

    public async Task<Todo> CreateTodoAsync(CreateTodoRequest request, string userId)
    {
        var todo = await _todoService.CreateTodoAsync(request, userId);
        
        // Invalidate related caches
        await InvalidateUserCaches(userId, request.GroupId);
        
        return todo;
    }

    public async Task<Todo> UpdateTodoAsync(int todoId, UpdateTodoRequest request, string userId)
    {
        var todo = await _todoService.UpdateTodoAsync(todoId, request, userId);
        
        // Invalidate related caches
        await InvalidateUserCaches(userId, todo.GroupId);
        
        return todo;
    }

    public async Task DeleteTodoAsync(int todoId, string userId)
    {
        // Get todo first to know which caches to invalidate
        var todo = await _todoService.GetTodoByIdAsync(todoId, userId);
        
        await _todoService.DeleteTodoAsync(todoId, userId);
        
        // Invalidate related caches
        await InvalidateUserCaches(userId, todo.GroupId);
    }

    private async Task InvalidateUserCaches(string userId, int? groupId)
    {
        var cacheKeys = new List<string>
        {
            $"todos:user:{userId}:group:0", // Personal todos
            $"todos:user:{userId}:group:{groupId ?? 0}" // Group todos
        };

        foreach (var key in cacheKeys)
        {
            try
            {
                await _cache.RemoveAsync(key);
                _logger.LogInformation("Invalidated cache key: {CacheKey}", key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to invalidate cache key: {CacheKey}", key);
            }
        }
    }
}