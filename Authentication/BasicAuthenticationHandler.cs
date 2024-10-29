using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Net.Http.Headers;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;


namespace LogProxyAPI.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        /*This constructor takes several dependencies needed for authentication, including options,
        logging, encoding, and time handling.It passes these parameters to the base AuthenticationHandler class.*/
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Check if the Authorization header is present
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // Setting the WWW-Authenticate header prompts for Basic authentication credentials
                Response.Headers["WWW-Authenticate"] = "Basic realm=\"LogProxyAPI\"";
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
            }
           
            try
            {
                // Parse the Authorization header
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter ?? string.Empty);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

                if (credentials.Length != 2)
                    return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header Format"));

                var username = credentials[0];
                var password = credentials[1];

                // Validate the credentials
                if (username == "admin" && password == "password") // Replace with secure checks in production
                {
                    var claims = new[] { new Claim(ClaimTypes.Name, username) };
                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);
                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }
                else
                {
                    return Task.FromResult(AuthenticateResult.Fail("Invalid Username or Password"));
                }
            }
            catch (FormatException)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }
            catch (Exception ex)
            {
                // Log exception details if needed for debugging
                Logger.LogError(ex, "Authentication failed.");
                return Task.FromResult(AuthenticateResult.Fail("Authentication failed due to an unexpected error."));
            }
        }
    }
}


