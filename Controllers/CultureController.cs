using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace RhinoTicketingSystem.Controllers
{
    [Route("Culture/[action]")]
    public partial class CultureController : Controller
    {
        public IActionResult SetCulture(string culture, string redirectUri)
        {
            if (culture != null)
            {
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)));
            }

            // Ensure redirectUri is local
            if (Url.IsLocalUrl(redirectUri))
            {
                return LocalRedirect(redirectUri);
            }

            // If not local, fallback to a default action or return an error
            return LocalRedirect("/");
        }
    }
}
