using ATLManager.Areas.Identity.Data;
using ATLManager.Data;
using ATLManager.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ATLManager.Controllers
{
    public class EstatisticasController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;

        public EstatisticasController(ATLManagerAuthContext context,
            UserManager<ATLManagerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index(Guid? id)
        {
            var estatisticasViewModel = new EstatisticasViewModel
            {
                VisitasDeEstudoEstatisticas = await GetVisitasDeEstudoEstatisticas(id),
                AtividadesPorMesEstatisticas = await GetAtividadesPorMesEstatisticas()
            };

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var currentUserAccount = await _context.ContaAdministrativa
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);
            var formularios = await _context.Formulario
                    .Where(r => r.AtlId == currentUserAccount.AtlId)
                    .ToListAsync();
            ViewBag.Formularios = formularios;

            return View(estatisticasViewModel);
        }

        private async Task<Dictionary<string, decimal>> GetVisitasDeEstudoEstatisticas(Guid? id)
        {
            // Obtenha o usuário atual
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // Obtenha a conta administrativa do usuário atual
            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            // Obtenha os formulários gerenciados pelo usuário atual
            var formularios = await _context.Formulario
                .Where(r => r.AtlId == currentUserAccount.AtlId)
                .ToListAsync();

            // Obtenha as respostas dos formulários gerenciados pelo usuário atual
            var respostas = await _context.FormularioResposta
                .Include(fr => fr.Formulario)
                .Where(fr => fr.Formulario.AtlId == currentUserAccount.AtlId && (!id.HasValue || fr.Formulario.FormularioId == id))
                .ToListAsync();
            
            // Implemente a lógica para calcular as estatísticas de visitas de estudo aqui.
            // Por exemplo, você pode calcular a porcentagem de autorizações concedidas e negadas.

            // Exemplo de implementação:
            var totalFormularios = respostas.Count;
            var totalAutorizados = respostas.Count(fr => fr.Authorized);
            var percentualAutorizados = totalFormularios != 0 ? (decimal)totalAutorizados / totalFormularios * 100 : 0;

            var estatisticas = new Dictionary<string, decimal>
    {
        { "PercentualAutorizados", percentualAutorizados },
        { "PercentualNaoAutorizados", 100 - percentualAutorizados }
    };

            return estatisticas;
        }

        [HttpGet]
        public async Task<JsonResult> GetVisitasDeEstudoEstatisticasAjax(Guid formularioId)
        {
            var estatisticas = await GetVisitasDeEstudoEstatisticas(formularioId);
            return Json(estatisticas);
        }



        private async Task<Dictionary<string, int>> GetAtividadesPorMesEstatisticas()
        {
            // Obtenha o usuário atual
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // Obtenha a conta administrativa do usuário atual
            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            // Obtenha as atividades gerenciadas pelo usuário atual
            var atividades = await _context.Atividade
                .Where(a => a.AtlId == currentUserAccount.AtlId)
                .ToListAsync();

            // Implemente a lógica para calcular as estatísticas de atividades por mês aqui.
            // Por exemplo, você pode contar o número de atividades para cada mês do ano atual.

            // Exemplo de implementação:
            var anoAtual = DateTime.Now.Year;
            var estatisticas = new Dictionary<string, int>();

            for (int mes = 1; mes <= 12; mes++)
            {
                var atividadesNoMes = atividades.Count(a => a.StartDate.Year == anoAtual && a.StartDate.Month == mes);
                estatisticas.Add($"AtividadesMes{mes}", atividadesNoMes);
            }

            return estatisticas;
        }

    }
}
