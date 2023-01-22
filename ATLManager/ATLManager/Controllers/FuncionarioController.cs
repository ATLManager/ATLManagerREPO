using Microsoft.AspNetCore.Mvc;

namespace ATLManager.Controllers
{
    public class FuncionarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
