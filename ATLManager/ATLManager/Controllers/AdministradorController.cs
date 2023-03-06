using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ATLManager.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministradorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
