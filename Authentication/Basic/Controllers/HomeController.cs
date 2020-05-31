using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace Basic.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        public IActionResult Authenticate()
        {
            List<Claim> grandmaClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Bob"),
                new Claim(ClaimTypes.Email, "bob@mail.com"),
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
    }
}