using Microsoft.AspNetCore.Mvc;

namespace ATLManager.Controllers
{
    public class AdministradorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
