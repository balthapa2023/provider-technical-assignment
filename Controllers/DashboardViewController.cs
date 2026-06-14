using Microsoft.AspNetCore.Mvc;

namespace ProviderAssignmentStarter.Controllers
{
    public class DashboardViewController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Dashboard/Index.cshtml");
        }
    }
}
