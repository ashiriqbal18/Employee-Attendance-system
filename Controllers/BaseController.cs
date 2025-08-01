using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace final.Controllers
{
    public class BaseController : Controller
    {
        protected int? LoggedInUserId => HttpContext.Session.GetInt32("UserId");
        protected string LoggedInUserRole => HttpContext.Session.GetString("Role");

        protected bool IsLoggedIn => LoggedInUserId.HasValue;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!IsLoggedIn)
            {
                context.Result = RedirectToAction("Login", "Login");
            }

            base.OnActionExecuting(context);
        }
    }

}
