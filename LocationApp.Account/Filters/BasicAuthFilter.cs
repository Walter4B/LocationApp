using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http.Headers;
using System.Text;

namespace LocationApp.Account.Filters
{
    public class BasicAuthFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            try
            {
                var authHeaderValue = AuthenticationHeaderValue.Parse(authHeader);
                var credentialBytes = Convert.FromBase64String(authHeaderValue.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                var username = credentials[0];
                var password = credentials[1];

                // Replace this with your user validation logic
                if (username != "admin" || password != "password")
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
            }
            catch
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
