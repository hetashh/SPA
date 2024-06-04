using Microsoft.AspNetCore.Mvc;

namespace SPA.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}