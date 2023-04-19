using ATLManager.Areas.Identity.Data;
using ATLManager.Data;
using ATLManager.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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
			var faturasEmAtraso = await GetFaturasEmAtraso();
			var faturasPagas = await GetFaturasPagas();

			var estatisticasViewModel = new EstatisticasViewModel
			{
				VisitasEstudoPorMesEstatisticas = await GetVisitasEstudoPorMesEstatisticas(),
				AtividadesPorMesEstatisticas = await GetAtividadesPorMesEstatisticas(),
				NumeroDeEducandosNovos = await GetNumeroDeEducandosNovos(),
				EducandosPorMes = await GetEducandosPorMesEstatisticas(),
				NumeroDeEducandos = await GetNumeroDeEducandos(),
				NumeroDeRapazes = await GetNumeroDeRapazes(),
				NumeroDeRaparigas = await GetNumeroDeRaparigas(),
				FaturasEmAtraso = faturasEmAtraso.Count,
				FaturasPagas = faturasPagas.Count,
				TotalValorEmAtraso = faturasEmAtraso.Total,
				TotalValorPago = faturasPagas.Total
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

        private async Task<Dictionary<string, int>> GetVisitasEstudoPorMesEstatisticas()
        {
            // Obtenha o usuário atual
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // Obtenha a conta administrativa do usuário atual
            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            // Obtenha as atividades gerenciadas pelo usuário atual
            var visitasEstudo = await _context.VisitaEstudo
                .Where(a => a.AtlId == currentUserAccount.AtlId)
                .ToListAsync();


            var anoAtual = DateTime.Now.Year;
            var estatisticas = new Dictionary<string, int>();

            for (int mes = 1; mes <= 12; mes++)
            {
                var visitasEstudoNoMes = visitasEstudo.Count(a => a.Date.Year == anoAtual && a.Date.Month == mes);
                estatisticas.Add($"VisitaEstudoMes{mes}", visitasEstudoNoMes);
            }

            return estatisticas;
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


            var anoAtual = DateTime.Now.Year;
            var estatisticas = new Dictionary<string, int>();

            for (int mes = 1; mes <= 12; mes++)
            {
                var atividadesNoMes = atividades.Count(a => a.StartDate.Year == anoAtual && a.StartDate.Month == mes);
                estatisticas.Add($"AtividadesMes{mes}", atividadesNoMes);
            }

            return estatisticas;
        }

		private async Task<int> GetNumeroDeEducandosNovos()
		{
			var currentUser = await _userManager.GetUserAsync(HttpContext.User);
			var currentUserAccount = await _context.ContaAdministrativa
				.Include(f => f.User)
				.FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

			DateTime umMesAtras = DateTime.Now.AddMonths(-1);

			var educandos = await _context.Educando
				.Where(c => c.AtlId == currentUserAccount.AtlId && c.DataDeInscricao >= umMesAtras)
				.ToListAsync();

			return educandos.Count;
		}

		private async Task<int> GetNumeroDeEducandos()
		{
			var currentUser = await _userManager.GetUserAsync(HttpContext.User);
			var currentUserAccount = await _context.ContaAdministrativa
				.Include(f => f.User)
				.FirstOrDefaultAsync(m => m.UserId == currentUser.Id);


			var educandos = await _context.Educando
				.Where(c => c.AtlId == currentUserAccount.AtlId)
				.ToListAsync();

			return educandos.Count;
		}

		private async Task<Dictionary<string, int>> GetEducandosPorMesEstatisticas()
		{
			var currentUser = await _userManager.GetUserAsync(HttpContext.User);
			var currentUserAccount = await _context.ContaAdministrativa
				.Include(f => f.User)
				.FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

			var educandos = await _context.Educando
				.Where(c => c.AtlId == currentUserAccount.AtlId)
				.ToListAsync();

			var anoAtual = DateTime.Now.Year;
			var estatisticas = new Dictionary<string, int>();

			for (int mes = 1; mes <= 12; mes++)
			{
				var educandosNoMes = educandos.Count(a => a.DataDeInscricao.Year == anoAtual && a.DataDeInscricao.Month == mes);
				estatisticas.Add($"EducandosMes{mes}", educandosNoMes);
			}

			return estatisticas;
		}


		private async Task<int> GetNumeroDeRapazes()
		{
			var currentUser = await _userManager.GetUserAsync(HttpContext.User);
			var currentUserAccount = await _context.ContaAdministrativa
				.Include(f => f.User)
				.FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

			var educandos = await _context.Educando
				.Where(c => c.AtlId == currentUserAccount.AtlId && c.Genero == "Masculino")
				.ToListAsync();

			return educandos.Count;
		}

		private async Task<int> GetNumeroDeRaparigas()
		{
			var currentUser = await _userManager.GetUserAsync(HttpContext.User);
			var currentUserAccount = await _context.ContaAdministrativa
				.Include(f => f.User)
				.FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

			var educandos = await _context.Educando
				.Where(c => c.AtlId == currentUserAccount.AtlId && c.Genero == "Feminino")
				.ToListAsync();

			return educandos.Count;
		}

		private async Task<(int Count, decimal Total)> GetFaturasEmAtraso()
		{
			var currentUser = await _userManager.GetUserAsync(HttpContext.User);
			var currentUserAccount = await _context.ContaAdministrativa
				.Include(f => f.User)
				.FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

			var faturasEmAtraso = await _context.ReciboResposta
				.Where(r => r.Recibo.AtlId == currentUserAccount.AtlId && !r.Authorized)
				.ToListAsync();

			decimal totalValorEmAtraso = faturasEmAtraso.Sum(f => decimal.Parse(f.Price.Replace(',', '.'), CultureInfo.InvariantCulture));
			return (faturasEmAtraso.Count, totalValorEmAtraso);
		}

		private async Task<(int Count, decimal Total)> GetFaturasPagas()
		{
			var currentUser = await _userManager.GetUserAsync(HttpContext.User);
			var currentUserAccount = await _context.ContaAdministrativa
				.Include(f => f.User)
				.FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

			var faturasPagas = await _context.ReciboResposta
				.Where(r => r.Recibo.AtlId == currentUserAccount.AtlId && r.Authorized)
				.ToListAsync();

			decimal totalValorPago = faturasPagas.Sum(f => decimal.Parse(f.Price.Replace(',', '.'), CultureInfo.InvariantCulture));
			return (faturasPagas.Count, totalValorPago);
		}

	}

}
