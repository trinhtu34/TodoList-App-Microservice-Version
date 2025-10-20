using ToDoService.DTOs;

namespace ToDoService.ServiceClients
{
    public interface ITagServiceClient
    {
        Task<List<TagDto>> GetTagsForTodo(int todoId, string token);
        Task RemoveTagsForTodo(int todoId);
        Task AddTagToTodo(int todoId, int tagId, string token);
        Task RemoveTagFromTodo(int todoId, int tagId, string token);
    }

    public class TagServiceClient : ITagServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TagServiceClient> _logger;

        public TagServiceClient(HttpClient httpClient, IConfiguration configuration, ILogger<TagServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // Read from .env or appsettings.json
            var tagServiceUrl = configuration["ServiceEndpoints:TagService"]
                ?? Environment.GetEnvironmentVariable("ServiceEndpoints__TagService");

            if (string.IsNullOrWhiteSpace(tagServiceUrl))
            {
                _logger.LogWarning("TagService URL is not configured (ServiceEndpoint:TagService or ServiceEndpoints__TagService). TagServiceClient will not set HttpClient.BaseAddress and calls will be no-ops or return defaults.");
            }
            else if (Uri.TryCreate(tagServiceUrl, UriKind.Absolute, out var baseUri))
            {
                _httpClient.BaseAddress = baseUri;
            }
            else
            {
                _logger.LogWarning("TagService URL value '{Url}' is not a valid absolute URI.", tagServiceUrl);
            }
        }

        public async Task<List<TagDto>> GetTagsForTodo(int todoId, string token)
        {
            try
            {
                if (_httpClient.BaseAddress == null)
                {
                    _logger.LogWarning("Cannot get tags for todo {TodoId} because TagService BaseAddress is not configured.", todoId);
                    return new List<TagDto>();
                }

                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"/api/tag/todo/{todoId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<TagDto>>() ?? new List<TagDto>();
                }

                return new List<TagDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tags for todo {TodoId}", todoId);
                return new List<TagDto>();
            }
        }

        public async Task RemoveTagsForTodo(int todoId)
        {
            try
            {
                if (_httpClient.BaseAddress == null)
                {
                    _logger.LogWarning("Cannot remove tags for todo {TodoId} because TagService BaseAddress is not configured.", todoId);
                    return;
                }

                await _httpClient.DeleteAsync($"/api/tag/cleanup/todo/{todoId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing tags for todo {TodoId}", todoId);
            }
        }

        public async Task AddTagToTodo(int todoId, int tagId, string token)
        {
            try
            {
                if (_httpClient.BaseAddress == null)
                {
                    _logger.LogWarning("Cannot add tag to todo {TodoId} because TagService BaseAddress is not configured.", todoId);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var content = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(new { tagId }),
                    System.Text.Encoding.UTF8,
                    "application/json");

                await _httpClient.PostAsync($"/api/tag/todo/{todoId}", content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding tag to todo");
            }
        }

        public async Task RemoveTagFromTodo(int todoId, int tagId, string token)
        {
            try
            {
                if (_httpClient.BaseAddress == null)
                {
                    _logger.LogWarning("Cannot remove tag from todo {TodoId} because TagService BaseAddress is not configured.", todoId);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                await _httpClient.DeleteAsync($"/api/tag/todo/{todoId}/tag/{tagId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing tag from todo");
            }
        }
    }
}
