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
    /// <summary>
    /// Controlador para o modelo 'Estatísticas'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
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

        /// <summary>
        /// Método que retorna uma View com a ViewModel de estatísticas
        /// </summary>
        /// <param name="id">O id de um objeto do tipo Guid</param>
        /// <returns>Uma View contendo uma ViewModel de estatísticas</returns>


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

        /// <summary>
        /// Método que retorna o ID do Agrupamento a partir do ID do ATL
        /// </summary>
        /// <param name="atlId">O ID de um ATL</param>
        /// <returns>Um JSON contendo o ID do Agrupamento</returns>

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


        /// <summary>
        /// Método que retorna um dicionário contendo as estatísticas de visitas de estudo por mês
        /// </summary>
        /// <returns>Um dicionário contendo as estatísticas de visitas de estudo por mês</returns>

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

        /// <summary>
        /// Obtém as estatísticas de atividades por mês.
        /// </summary>
        /// <returns>Um dicionário com as estatísticas das atividades por mês.</returns>

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

        /// <summary>
        /// Obtém as estatísticas de visitas de estudo por mês.
        /// </summary>
        /// <param name="id">O id do educando atual.</param>
        /// <returns>Um dicionário com as estatísticas das visitas de estudo por mês.</returns>

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

        /// <summary>
        /// Obtém as estatísticas de atividades por mês.
        /// </summary>
        /// <param name="id">O id do educando atual.</param>
        /// <returns>Um dicionário com as estatísticas das atividades por mês.</returns>

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

        /// <summary>
        /// Obtém as estatísticas de visitas de estudo, atividades, educandos novos e educandos de um ATL.
        /// </summary>
        /// <param name="id">O id da ATL.</param>
        /// <returns>Um objeto JSON com as estatísticas de visitas de estudo, atividades, educandos novos e educandos.</returns>

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

        /// <summary>
        /// Obtém o número de educandos novos no ATL nos últimos 30 dias.
        /// </summary>
        /// <returns>O número de educandos novos.</returns>

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

        /// <summary>
        /// Obtém o número de educandos no ATL.
        /// </summary>
        /// <returns>O número de educandos.</returns>

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

        /// <summary>
        /// Retorna um dicionário contendo o número de educandos inscritos por mês.
        /// </summary>
        /// <returns>Dicionário com o número de educandos inscritos em cada mês.</returns>

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

        /// <summary>
        /// Retorna o número de rapazes inscritos no ATL.
        /// </summary>
        /// <returns>Número de rapazes inscritos.</returns>

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

        /// <summary>
        /// Retorna o número de raparigas inscritas no ATL.
        /// </summary>
        /// <returns>Número de raparigas inscritas.</returns>

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

        /// <summary>
        /// Retorna o número e o total em atraso de faturas não autorizadas de um ATL.
        /// </summary>
        /// <returns>Tupla contendo o número de faturas e o valor total em atraso.</returns>

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

        /// <summary>
        /// Retorna o número e o total pago de faturas autorizadas de um ATL.
        /// </summary>
        /// <returns>Tupla contendo o número de faturas e o valor total pago.</returns>

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

        /// <summary>
        /// Retorna o número e o total pago de faturas autorizadas de um ATL específico.
        /// </summary>
        /// <param name="id">ID do ATL.</param>
        /// <returns>Tupla contendo o número de faturas e o valor total pago.</returns>

        private async Task<(int Count, decimal Total)> GetFaturasPagasADM(Guid? id)
		{
			var faturasPagas = await _context.ReciboResposta
							.Where(r => r.Recibo.AtlId == id && r.Authorized)
							.ToListAsync();

			decimal totalValorPago = faturasPagas.Sum(f => decimal.Parse(f.Price.Replace(',', '.'), CultureInfo.InvariantCulture));
			return (faturasPagas.Count, totalValorPago);
		}

        /// <summary>
        /// Retorna o número e o total em atraso de faturas não autorizadas de um ATL específico.
        /// </summary>
        /// <param name="id">ID do ATL.</param>
        /// <returns>Tupla contendo o número de faturas e o valor total em atraso.</returns>

        private async Task<(int Count, decimal Total)> GetFaturasEmAtrasoADM(Guid? id)
		{
			var faturasEmAtraso = await _context.ReciboResposta
				.Where(r => r.Recibo.AtlId == id && !r.Authorized)
				.ToListAsync();

			decimal totalValorEmAtraso = faturasEmAtraso.Sum(f => decimal.Parse(f.Price.Replace(',', '.'), CultureInfo.InvariantCulture));
			return (faturasEmAtraso.Count, totalValorEmAtraso);
		}

        /// <summary>
        /// Retorna o número de raparigas inscritas em um ATL específico.
        /// </summary>
        /// <param name="id">ID do ATL.</param>
        /// <returns>Número de raparigas inscritas.</returns>

        private async Task<int> GetNumeroDeRaparigasADM(Guid? id)
		{
			var educandos = await _context.Educando
				.Where(c => c.AtlId == id && c.Genero == "Feminino")
				.ToListAsync();

			return educandos.Count;
		}

        /// <summary>
        /// Retorna o número de rapazes inscritos em um ATL específico.
        /// </summary>
        /// <param name="id">ID do ATL.</param>
        /// <returns>Número de rapazes inscritos.</returns>

        private async Task<int> GetNumeroDeRapazesADM(Guid? id)
		{
			var educandos = await _context.Educando
							.Where(c => c.AtlId == id && c.Genero == "Masculino")
							.ToListAsync();

			return educandos.Count;
		}

        /// <summary>
        /// Obtém estatísticas de educandos por mês para um determinado ID de ATL.
        /// </summary>
        /// <param name="id">O ID do ATL</param>
        /// <returns>Um dicionário com as estatísticas de educandos por mês</returns>
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

        /// <summary>
        /// Obtém o número de educandos para um determinado ID de ATL.
        /// </summary>
        /// <param name="id">O ID do ATL</param>
        /// <returns>O número de educandos</returns>
        private async Task<int> GetNumeroDeEducandosADM(Guid? id)
		{
			var educandos = await _context.Educando
				.Where(c => c.AtlId == id)
				.ToListAsync();

			return educandos.Count;
		}

        /// <summary>
        /// Obtém o número de educandos novos (inscritos no último mês) para um determinado ID de ATL.
        /// </summary>
        /// <param name="id">O ID do ATL</param>
        /// <returns>O número de educandos novos</returns>
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
