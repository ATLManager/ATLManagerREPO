using Microsoft.AspNetCore.Mvc;

namespace ATLManager.Controllers
{
    public class HistoricoController : Controller
    {

        /// <summary>
        /// Retorna a View "Escolher".
        /// </summary>
        /// <returns>A View "Escolher".</returns>
        public IActionResult Escolher()
        {
            return View();
        }
    }
}
