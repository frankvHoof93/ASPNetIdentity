using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.AuthRequirements
{
    public class JwtRequirementHandler : AuthorizationHandler<JwtRequirement>
    {
        private readonly HttpClient _client;
        private readonly HttpContext _httpContext;

        public JwtRequirementHandler(
            IHttpClientFactory clientFactory, 
            IHttpContextAccessor contextAccessor)
        {
            _client = clientFactory.CreateClient();
            _httpContext = contextAccessor.HttpContext;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            JwtRequirement requirement)
        {
            if (_httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                string accessToken = authHeader.ToString().Split(' ')[1]; // Bearer {token}

                // VALIDATE TOKEN ON AUTH-SERVER
                var apiResponse = await _client
                    .GetAsync($"https://localhost:5001/oauth/validate?access_token={accessToken}"); 

                if (apiResponse.StatusCode == HttpStatusCode.OK)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
