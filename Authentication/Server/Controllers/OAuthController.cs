using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controllers
{
    public class OAuthController : Controller
    {
        [HttpGet]
        public IActionResult Authorize(
            string response_type, // Authorization flow type
            string client_id, // Client ID
            string redirect_uri,
            string scope, // Scope of info for Auth (e.g. tel#, email, cookie)
            string state) // Random string to confirm client
        {
            // ?a=foo&b=bar
            var query = new QueryBuilder();
            query.Add("redirectUri", redirect_uri);
            query.Add("state", state);
            return View(model: query.ToString());
        }

        [HttpPost]
        public IActionResult Authorize(
            string username,
            string redirectUri,
            string state)
        {
            const string code = "BABABABSDIOAW";
            var query = new QueryBuilder();
            query.Add("code", code);
            query.Add("state", state);
            return Redirect($"{redirectUri}{query.ToString()}");
        }

        public async Task<IActionResult> Token(
            string grant_type, // flow of access token request
            string code, // confirmation of auth process (code dealt out by Authorize)
            string redirect_uri,
            string client_id)
        {
            // Validate code (against db)



            // Create Token
            var claims = new Claim[]
            {
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub, "some_id"),
                new Claim("Custom Claim", "cookie")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.Secret));
            var algorithm = SecurityAlgorithms.HmacSha256;
            SigningCredentials creds = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audience,
                claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(3),
                creds);

            var access_token = new JwtSecurityTokenHandler().WriteToken(token);

            var responseObject = new
            {
                access_token,
                token_type = "Bearer",
                raw_claim = "oauthTutorial"
            };

            return Ok(responseObject);
        }

        [Authorize]
        public IActionResult Validate()
        {
            // TODO: Get from header instead of URL!!!
            if (HttpContext.Request.Query.TryGetValue("access_token", out var accessToken))
            {
                // Check Token 
                return Ok();
            }
            return BadRequest();
        }
    }
}