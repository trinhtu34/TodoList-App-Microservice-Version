using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using Domain.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories;

public class UserService : IUserService
{
    private readonly IConfiguration _configuration;

    public UserService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var userPoolId = _configuration["AWS:Cognito:UserPoolId"]
            ?? Environment.GetEnvironmentVariable("AWS__Cognito__UserPoolId");

        var regionName = _configuration["AWS:Region"]
            ?? Environment.GetEnvironmentVariable("AWS__Region");

        if (string.IsNullOrWhiteSpace(userPoolId) || string.IsNullOrWhiteSpace(regionName))
            throw new InvalidOperationException("AWS Cognito configuration (UserPoolId or Region) is missing.");

        var region = RegionEndpoint.GetBySystemName(regionName);

        var accessKey = _configuration["AWS:AccessKey"] ?? Environment.GetEnvironmentVariable("AWS__AccessKey");
        var secretKey = _configuration["AWS:SecretKey"] ?? Environment.GetEnvironmentVariable("AWS__SecretKey");

        AmazonCognitoIdentityProviderClient cognitoClient;
        if (!string.IsNullOrWhiteSpace(accessKey) && !string.IsNullOrWhiteSpace(secretKey))
        {
            var creds = new BasicAWSCredentials(accessKey, secretKey);
            cognitoClient = new AmazonCognitoIdentityProviderClient(creds, region);
        }
        else
        {
            // Use default credential chain (EC2/ECS/EKS/Environment/Shared credentials)
            cognitoClient = new AmazonCognitoIdentityProviderClient(region);
        }

        var listUsersRequest = new ListUsersRequest
        {
            UserPoolId = userPoolId,
            Filter = $"email = \"{email}\"",
            Limit = 1
        };

        var response = await cognitoClient.ListUsersAsync(listUsersRequest, cancellationToken);

        if (response.Users == null || response.Users.Count == 0)
            throw new KeyNotFoundException("User not found");

        var user = response.Users[0];
        var sub = user.Attributes?.FirstOrDefault(a => a.Name == "sub")?.Value;

        if (string.IsNullOrEmpty(sub))
            throw new Exception("User sub not found");

        return sub;
    }
}
