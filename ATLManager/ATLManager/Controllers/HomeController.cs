using ATLManager.Models;
using ATLManager.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ATLManager.ViewModels;
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ATLManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly EstatisticasController _estatisticasController;

        public HomeController(ILogger<HomeController> logger, ATLManagerAuthContext context, UserManager<ATLManagerUser> userManager, EstatisticasController estatisticasController)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _estatisticasController = estatisticasController;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<IActionResult> IndexEE()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            
            var currentUserAccount = await _context.EncarregadoEducacao
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);
      
       
                var educandos = await _context.Educando
                    .Include(e => e.Atl)
                    .Include(e => e.Encarregado)
                    .Where(e => e.EncarregadoId == currentUserAccount.EncarregadoId)
                    .ToListAsync();

                ViewData["EducandoId"] = new SelectList(educandos, "EducandoId", "Name");
                ViewBag.Educandos = educandos;
            

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEstatisticasPorEducando(Guid? id)
        {
            var visitasEstudoPorMesEstatisticas = new Dictionary<string, int>();
            var atividadesPorMesEstatisticas = new Dictionary<string, int>();

            if (id.HasValue)
            {
                visitasEstudoPorMesEstatisticas = await _estatisticasController.GetVisitasEstudoPorMesEstatisticasEnc(id.Value);
                atividadesPorMesEstatisticas = await _estatisticasController.GetAtividadesPorMesEstatisticasEnc(id.Value);
            }

            var result = new
            {
                visitasEstudoPorMes = visitasEstudoPorMesEstatisticas,
                atividadesPorMes = atividadesPorMesEstatisticas
            };

            return Json(result);
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }

        public IActionResult Contacts()
        {
            return View();
        }
        public IActionResult Policy()
        {
            return View();
        }

        public IActionResult ChangeLanguage(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}