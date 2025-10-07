namespace ToDoService.ServiceClients
{
    public interface IGroupServiceClient
    {
        Task<bool> VerifyMembership(int groupId, string userId);
    }

    public class GroupServiceClient : IGroupServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GroupServiceClient> _logger;

        public GroupServiceClient(HttpClient httpClient, IConfiguration configuration, ILogger<GroupServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // Read from .env or appsettings.json
            var groupServiceUrl = configuration["ServiceEndpoint:GroupService"]
                ?? Environment.GetEnvironmentVariable("ServiceEndpoint__GroupService");

            _httpClient.BaseAddress = new Uri(groupServiceUrl);
        }

        public async Task<bool> VerifyMembership(int groupId, string userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/group/{groupId}/verify-member/{userId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<bool>();
                    return result;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying group membership");
                return false;
            }
        }
    }
}
