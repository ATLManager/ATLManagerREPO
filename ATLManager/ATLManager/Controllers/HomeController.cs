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
    /// <summary>
    /// Controlador para o modelo 'HomePage'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
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

        /// <summary>
        /// Retorna a View "Index".
        /// </summary>
        /// <returns>A View "Index".</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Retorna a View "IndexEE" com informações do usuário logado e seus educandos.
        /// </summary>
        /// <returns>A View "IndexEE".</returns>
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

        /// <summary>
        /// Retorna as estatísticas de visitas de estudo e atividades por mês de um educando específico em formato JSON.
        /// </summary>
        /// <param name="id">O ID do educando.</param>
        /// <returns>As estatísticas em formato JSON.</returns>

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

        /// <summary>
        /// Retorna a View "AboutUs".
        /// </summary>
        /// <returns>A View "AboutUs".</returns>

        public IActionResult AboutUs()
        {
            return View();
        }

        /// <summary>
        /// Retorna a View "Services".
        /// </summary>
        /// <returns>A View "Services".</returns>
        public IActionResult Services()
        {
            return View();
        }

        /// <summary>
        /// Retorna a View "Contacts".
        /// </summary>
        /// <returns>A View "Contacts".</returns>
        public IActionResult Contacts()
        {
            return View();
        }

        /// <summary>
        /// Retorna a View "Policy".
        /// </summary>
        /// <returns>A View "Policy".</returns>
        public IActionResult Policy()
        {
            return View();
        }

        /// <summary>
        /// Altera o idioma do site e redireciona o usuário de volta para a página anterior.
        /// </summary>
        /// <param name="culture">A cultura desejada.</param>
        /// <returns>O redirecionamento para a página anterior.</returns>

        public IActionResult ChangeLanguage(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return Redirect(Request.Headers["Referer"].ToString());
        }

        /// <summary>
        /// Trata os erros e devolve uma View de erro.
        /// </summary>
        /// <returns>A View de erro.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}