using GroupService.Application.Common;
using MediatR;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;

namespace GroupService.Application.Users.Queries;

public record GetUserByEmailQuery(string Email) : IQuery<string>;

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, string>
{
    private readonly IConfiguration _configuration;

    public GetUserByEmailQueryHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var userPoolId = _configuration["AWS:Cognito:UserPoolId"] 
            ?? Environment.GetEnvironmentVariable("AWS__Cognito__UserPoolId");
        
        var regionName = _configuration["AWS:Region"] 
            ?? Environment.GetEnvironmentVariable("AWS__Region");
        var region = Amazon.RegionEndpoint.GetBySystemName(regionName);
        
        var accessKey = _configuration["AWS:AccessKey"] 
            ?? Environment.GetEnvironmentVariable("AWS__AccessKey");
        var secretKey = _configuration["AWS:SecretKey"] 
            ?? Environment.GetEnvironmentVariable("AWS__SecretKey");
        
        var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
        using var cognitoClient = new AmazonCognitoIdentityProviderClient(credentials, region);
        
        var listUsersRequest = new ListUsersRequest
        {
            UserPoolId = userPoolId,
            Filter = $"email = \"{request.Email}\""
        };

        var response = await cognitoClient.ListUsersAsync(listUsersRequest, cancellationToken);

        if (response.Users.Count == 0)
            throw new Exception("User not found");

        var user = response.Users[0];
        var sub = user.Attributes.FirstOrDefault(a => a.Name == "sub")?.Value;

        if (string.IsNullOrEmpty(sub))
            throw new Exception("User sub not found");

        return sub;
    }
}
