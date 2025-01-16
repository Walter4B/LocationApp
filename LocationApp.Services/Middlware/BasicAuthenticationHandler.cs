using LocationApp.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace LocationApp.Services.Extensions
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IRepositoryManager _repositoryManager;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IRepositoryManager repositoryManager)
            : base(options, logger, encoder)
        {
            _repositoryManager = repositoryManager;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter));
                var parts = credentials.Split(':', 2);
                if (parts.Length != 2)
                    return AuthenticateResult.Fail("Invalid Authorization Header");

                var apiKey = parts[0];
                var user = await _repositoryManager.User.GetUser(apiKey);

                if (user == null)
                    return AuthenticateResult.Fail("Invalid API Key");

                // Create authenticated user
                var claims = new[] { new Claim(ClaimTypes.Name, user.Username) };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }
        }
    }
}
