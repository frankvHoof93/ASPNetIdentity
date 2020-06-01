using Basic.CustomPolicyProvider;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Basic.Controllers
{
    public class HomeController : Controller
    {
        //private readonly IAuthorizationService _authorizationService;

        //public HomeController(IAuthorizationService authorizationService)
        //{
        //    _authorizationService = authorizationService;
        //}

        public IActionResult Index()
        {
            return View();
        }

        // Require Authorization
        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        // Require specific Policy
        [Authorize(Policy = "Claim.DoB")]
        [Authorize(Policy = "SecurityLevel.5")] // Type.Value
        public IActionResult SecretPolicy()
        {
            return View("Secret");
        }

        // Require specific Role
        [Authorize(Roles = "Admin")]
        public IActionResult SecretRole()
        {
            return View("Secret");
        }

        // Custom AuthorizationProvider
        [SecurityLevel(3)]
        public IActionResult SecretLevel()
        {
            return View("Secret");
        }

        // Custom AuthorizationProvider
        [SecurityLevel(5)]
        public IActionResult SecretHigherLevel()
        {
            return View("Secret");
        }

        // Don't require authorization
        [AllowAnonymous]
        public IActionResult Authenticate()
        {
            List<Claim> grandmaClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Bob"),
                new Claim(ClaimTypes.Email, "bob@mail.com"),
                new Claim(ClaimTypes.DateOfBirth, "11/11/2000"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(DynamicPolicies.SecurityLevel, "4"),
                new Claim("Grandma.Says", "Very nice boy")
            };

            List<Claim> licenseClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Bob the Builder"),
                new Claim("Driving License", "A+")
            };

            ClaimsIdentity grandmaIdentity = new ClaimsIdentity(grandmaClaims, "Grandma Identity");
            ClaimsIdentity licenseIdentity = new ClaimsIdentity(licenseClaims, "Government");

            ClaimsPrincipal userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity, licenseIdentity });

            HttpContext.SignInAsync(userPrincipal);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DoStuff([FromServices] IAuthorizationService authorizationService)
        {
            var builder = new AuthorizationPolicyBuilder("Schema");
            var customPolicy = builder.RequireClaim("Hello").Build(); // Non-existing claim

            var authResult = await authorizationService.AuthorizeAsync(HttpContext.User, customPolicy);

            if (authResult.Succeeded)
            {
                // Authorization succeeded
                return View("Index");
            }
            Console.WriteLine("Auth Failed");
            return View("Index");
        }
    }
}