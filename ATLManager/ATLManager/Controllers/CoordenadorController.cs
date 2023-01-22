using Microsoft.AspNetCore.Mvc;

namespace ATLManager.Controllers
{
    public class CoordenadorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
