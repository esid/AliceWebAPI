using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using System.Security.Claims;
using AspNet.Security.OpenIdConnect.Extensions;
using AliceWebApp.Lib;

namespace AliceWebApp.Controllers
{
    public class AuthController : Controller 
    {
        public object Authrization { get; private set; }

        [HttpPost("/auth/token"), Produces("application/json")]
        public IActionResult IssueToken()
        {

            string basicAuthValue = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (basicAuthValue == null || basicAuthValue.Equals(string.Empty))
            {
                return Unauthorized();
            }

            var creds = GetCreds(basicAuthValue);
            if (creds.Count == 0 || !Authorization.IsAuthed(creds))
                return Unauthorized();
            var token = Authorization.CreateToken();
            return Ok(  token.ToString());
        }

        private static List<string> GetCreds(Microsoft.Extensions.Primitives.StringValues authHeader)
        {
            try
            {
                var usr = Convert.FromBase64String(authHeader[0].Split(' ').Last());
                string str = System.Text.Encoding.UTF8.GetString(usr);
                return str.Split(':').ToList<string>();
            } 
            catch(Exception e)
            {
                return new List<string>();
            }

        }

        [HttpPost("/connect/token"), Produces("application/json")]
        public IActionResult Exchange(OpenIdConnectRequest request)
        {
            if (request.IsPasswordGrantType())
            {
                // Validate the user credentials.
                // Note: to mitigate brute force attacks, you SHOULD strongly consider
                // applying a key derivation function like PBKDF2 to slow down
                // the password validation process. You SHOULD also consider
                // using a time-constant comparer to prevent timing attacks.
                if (request.Username != "alice@wonderland.com" ||
                    request.Password != "P@ssw0rd")
                {
                    return Forbid(OpenIdConnectServerDefaults.AuthenticationScheme);
                }
                // Create a new ClaimsIdentity holding the user identity.
                var identity = new ClaimsIdentity(
                    OpenIdConnectServerDefaults.AuthenticationScheme,
                    OpenIdConnectConstants.Claims.Name,
                    OpenIdConnectConstants.Claims.Role);

                // Add a "sub" claim containing the user identifier, and attach
                // the "access_token" destination to allow OpenIddict to store it
                // in the access token, so it can be retrieved from your controllers.
                identity.AddClaim(OpenIdConnectConstants.Claims.Subject,
                    "71346D62-9BA5-4B6D-9ECA-755574D628D8",
                    OpenIdConnectConstants.Destinations.AccessToken);
                identity.AddClaim(OpenIdConnectConstants.Claims.Name, "Alice",
                    OpenIdConnectConstants.Destinations.AccessToken);
                // ... add other claims, if necessary.
                var principal = new ClaimsPrincipal(identity);
                // Ask OpenIddict to generate a new token and return an OAuth2 token response.
                return SignIn(principal, OpenIdConnectServerDefaults.AuthenticationScheme);
            }
            throw new InvalidOperationException("The specified grant type is not supported.");
        }
    }
}
