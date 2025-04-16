using Microsoft.Graph;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using Azure.Core;

namespace RemoteMcpMsGraph.Tools
{
    [McpServerToolType]
    public class ShowUserProfileTool
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShowUserProfileTool(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [McpServerTool, Description("Shows current user profile.")]
        public async Task<string> ShowUserProfile()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            // Get the token from the header
            if (!httpContext.Request.Headers.TryGetValue("X-Resource-Access-Token", out var tokenValue))
            {
                return "Error: Microsoft Graph token not found in request headers.";
            }

            string accessToken = tokenValue.ToString();

            try
            {
                // Create a Graph client with the provided token
                var graphClient = CreateGraphClient(accessToken);

                // Get the user profile
                var user = await graphClient.Me.GetAsync();

                if (user == null)
                {
                    return "Error: Unable to retrieve user profile from Microsoft Graph API.";
                }

                // Format the user profile as JSON
                var userProfile = new
                {
                    DisplayName = user.DisplayName,
                    Email = user.Mail ?? user.UserPrincipalName,
                    Id = user.Id,
                    JobTitle = user.JobTitle,
                    Department = user.Department,
                    OfficeLocation = user.OfficeLocation
                };

                return JsonSerializer.Serialize(userProfile, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private GraphServiceClient CreateGraphClient(string accessToken)
        {
            var tokenCredential = new AccessTokenCredential(accessToken);
            return new GraphServiceClient(tokenCredential);
        }
    }

    class AccessTokenCredential : TokenCredential
    {
        private readonly string _accessToken;

        public AccessTokenCredential(string accessToken)
        {
            _accessToken = accessToken;
        }

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return new AccessToken(_accessToken, DateTimeOffset.UtcNow.AddHours(1)); // For demo purpose only, assuming the token is valid for 1 hour
        }

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return new ValueTask<AccessToken>(new AccessToken(_accessToken, DateTimeOffset.UtcNow.AddHours(1))); // For demo purpose only, assuming the token is valid for 1 hour
        }
    }
}
