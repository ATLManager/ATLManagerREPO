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

			var faturasEmAtrasoADM = await GetFaturasEmAtrasoADM(id);
			var faturasPagasADM = await GetFaturasPagasADM(id);


			var estatisticasViewModel = new EstatisticasViewModel
			{
				VisitasEstudoPorMesEstatisticasCoordenadores = await GetVisitasEstudoPorMesEstatisticasCoord(),
				AtividadesPorMesEstatisticasCoordenadores = await GetAtividadesPorMesEstatisticasCoord(),
				GetVisitasEstudoPorMesEstatisticasEnc = await GetVisitasEstudoPorMesEstatisticasEnc(id),
				GetAtividadesPorMesEstatisticasEnc = await GetAtividadesPorMesEstatisticasEnc(id),
				GetNumeroDeEducandosNovosADM = await GetNumeroDeEducandosNovosADM(id),
				NumeroDeEducandosADM = await GetNumeroDeEducandosADM(id),
				EducandosPorMesADM = await GetEducandosPorMesEstatisticasADM(id),
				NumeroDeRapazesADM = await GetNumeroDeRapazesADM(id),
				NumeroDeRaparigasADM = await GetNumeroDeRaparigasADM(id),
				FaturasEmAtrasoADM = faturasEmAtrasoADM.Count,
				FaturasPagasADM = faturasPagasADM.Count,
				TotalValorEmAtrasoADM = faturasEmAtrasoADM.Total,
				TotalValorPagoADM = faturasPagasADM.Total,
				
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

			if (User.IsInRole("Administrador")) {

				// Busca os ATLs pertencentes ao administrador atual
				var atls = await (from atl in _context.ATL
								  join atlAdmin in _context.ATLAdmin on atl.AtlId equals atlAdmin.AtlId
								  join admin in _context.ContaAdministrativa on atlAdmin.ContaId equals admin.ContaId
								  where admin.UserId == currentUser.Id
								  select atl).Include(a => a.Agrupamento).ToListAsync();



				// Preenche a ViewBag com os ATLs
				ViewBag.ATLs = atls;
			}
			
            return View(estatisticasViewModel);
        }

		[HttpGet]
		public async Task<IActionResult> GetAgrupamentoIdFromAtl(Guid atlId)
		{
			var atl = await _context.ATL.FindAsync(atlId);
			if (atl == null)
			{
				return NotFound(); // ou você pode retornar um valor padrão
			}

			Guid? agrupamentoId = atl.AgrupamentoId;
			return Json(new { agrupamentoId = agrupamentoId });
		}



		public async Task<Dictionary<string, int>> GetVisitasEstudoPorMesEstatisticasCoord()
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
        
        public async Task<Dictionary<string, int>> GetAtividadesPorMesEstatisticasCoord()
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
		
		public async Task<Dictionary<string, int>> GetVisitasEstudoPorMesEstatisticasEnc(Guid? id)
		{
            if (id == Guid.Empty) return new Dictionary<string, int>();

			// Obtenha as visitas de estudo gerenciadas pelo educando atual
			var visitasEstudo = await _context.VisitaEstudo
				.Where(a => a.AtlId == id)
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

		public async Task<Dictionary<string, int>> GetAtividadesPorMesEstatisticasEnc(Guid? id)
		{
            if (id == Guid.Empty) return new Dictionary<string, int>();


			// Obtenha as atividades gerenciadas pelo educando atual
			var atividades = await _context.Atividade
				.Where(a => a.AtlId == id)
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

        [HttpGet]
        public async Task<IActionResult> GetEstatisticasPorATL(Guid id)
        {
            var visitasEstudoPorMes = await GetVisitasEstudoPorMesEstatisticasEnc(id);
            var atividadesPorMes = await GetAtividadesPorMesEstatisticasEnc(id);
			var numEducandosPorMes = await GetNumeroDeEducandosNovosADM(id);
			var numEducandos = await GetNumeroDeEducandosADM(id);
			var educandoPorMes = await GetEducandosPorMesEstatisticasADM(id);
			var numRapazes = await GetNumeroDeRapazesADM(id);
			var numRaparigas = await GetNumeroDeRaparigasADM(id);
			var faturasAtraso = await GetFaturasEmAtrasoADM(id);
			var faturasPagas = await GetFaturasPagasADM(id);

			var FaturasEmAtrasoADM = faturasAtraso.Count;
			var FaturasPagasADM = faturasPagas.Count;
			var TotalValorEmAtrasoADM = faturasAtraso.Total;
			var TotalValorPagoADM = faturasPagas.Total;


			return Json(new { visitasEstudoPorMes, atividadesPorMes, numEducandosPorMes,
								numEducandos, educandoPorMes, numRapazes, numRaparigas,
								FaturasEmAtrasoADM,	FaturasPagasADM, TotalValorEmAtrasoADM, TotalValorPagoADM});
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

		//Faturas Pagas de um ATL em especifico
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

		//Faturas Pagas por ATL
		private async Task<(int Count, decimal Total)> GetFaturasPagasADM(Guid? id)
		{
			var faturasPagas = await _context.ReciboResposta
							.Where(r => r.Recibo.AtlId == id && r.Authorized)
							.ToListAsync();

			decimal totalValorPago = faturasPagas.Sum(f => decimal.Parse(f.Price.Replace(',', '.'), CultureInfo.InvariantCulture));
			return (faturasPagas.Count, totalValorPago);
		}

		//Faturas em Atraso de um ATL
		private async Task<(int Count, decimal Total)> GetFaturasEmAtrasoADM(Guid? id)
		{
			var faturasEmAtraso = await _context.ReciboResposta
				.Where(r => r.Recibo.AtlId == id && !r.Authorized)
				.ToListAsync();

			decimal totalValorEmAtraso = faturasEmAtraso.Sum(f => decimal.Parse(f.Price.Replace(',', '.'), CultureInfo.InvariantCulture));
			return (faturasEmAtraso.Count, totalValorEmAtraso);
		}

		private async Task<int> GetNumeroDeRaparigasADM(Guid? id)
		{
			var educandos = await _context.Educando
				.Where(c => c.AtlId == id && c.Genero == "Feminino")
				.ToListAsync();

			return educandos.Count;
		}

		private async Task<int> GetNumeroDeRapazesADM(Guid? id)
		{
			var educandos = await _context.Educando
							.Where(c => c.AtlId == id && c.Genero == "Masculino")
							.ToListAsync();

			return educandos.Count;
		}

		private async Task<Dictionary<string, int>> GetEducandosPorMesEstatisticasADM(Guid? id)
		{
			var educandos = await _context.Educando
							.Where(c => c.AtlId == id)
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

		private async Task<int> GetNumeroDeEducandosADM(Guid? id)
		{
			var educandos = await _context.Educando
				.Where(c => c.AtlId == id)
				.ToListAsync();

			return educandos.Count;
		}

		private async Task<int> GetNumeroDeEducandosNovosADM(Guid? id)
		{

			DateTime umMesAtras = DateTime.Now.AddMonths(-1);

			var educandos = await _context.Educando
				.Where(c => c.AtlId == id && c.DataDeInscricao >= umMesAtras)
				.ToListAsync();

			return educandos.Count;
		}


	}

}
