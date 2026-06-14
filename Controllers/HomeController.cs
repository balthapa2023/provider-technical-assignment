using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProviderAssignmentStarter.Models;

namespace ProviderAssignmentStarter.Controllers
{
    /// <summary>
    /// Home controller - public pages (Index allows anonymous, Privacy/Error require auth)
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Index - public landing page
        /// </summary>
        public IActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page");
                return View();
            }
        }

        /// <summary>
        /// Privacy page
        /// </summary>
        public IActionResult Privacy()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading privacy page");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Error - handles exceptions from other actions
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            try
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in error handler");
                return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
            }
        }
    }
}
