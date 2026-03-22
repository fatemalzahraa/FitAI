using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AvciFidancilik.WebUI.Areas.admin.Controllers
{
    [Area("admin")]
    public class BaseAdminController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Giriş yapılmamışsa
            if (!User.Identity!.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult(
                    "AdminSignIn",
                    "Account",
                    new { area = "" }
                );
                return;
            }

            // Admin değilse
            if (!User.IsInRole("Admin"))
            {
                context.Result = new RedirectToActionResult(
                    "AccessDenied",
                    "Account",
                    new { area = "" }
                );
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
