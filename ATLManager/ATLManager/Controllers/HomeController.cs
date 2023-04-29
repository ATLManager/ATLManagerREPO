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
using System.Linq;
using System.Globalization;

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
        private readonly RefeicoesController _refeicoesController;


        public HomeController(ILogger<HomeController> logger, ATLManagerAuthContext context, UserManager<ATLManagerUser> userManager, EstatisticasController estatisticasController, RefeicoesController refeicoesController)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _estatisticasController = estatisticasController;
            _refeicoesController = refeicoesController;
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
            ViewBag.EncarregadoId = currentUserAccount.EncarregadoId;

            return View();
        }

        public async Task<IActionResult> IndexADM()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var currentUserAccount = await _context.EncarregadoEducacao
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            // Busca os ATLs pertencentes ao administrador atual
            var atls = await (from atl in _context.ATL
                              join atlAdmin in _context.ATLAdmin on atl.AtlId equals atlAdmin.AtlId
                              join admin in _context.ContaAdministrativa on atlAdmin.ContaId equals admin.ContaId
                              where admin.UserId == currentUser.Id
                              select atl).Include(a => a.Agrupamento).ToListAsync();



            // Preenche a ViewBag com os ATLs
            ViewBag.ATLs = atls;

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


			var ATLbyEducando = await _context.Educando
					.Include(f => f.Atl)
					.Include(f => f.Encarregado)
					.FirstOrDefaultAsync(m => m.EducandoId == id);
            
                visitasEstudoPorMesEstatisticas = await _estatisticasController.GetVisitasEstudoPorMesEstatisticasEnc(ATLbyEducando.AtlId);
                atividadesPorMesEstatisticas = await _estatisticasController.GetAtividadesPorMesEstatisticasEnc(ATLbyEducando.AtlId);
           

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
        [HttpGet]
        public async Task<IActionResult> GetRefeicoesByEducandoId(Guid educandoId)
        {
            var ATLbyEducando = await _context.Educando
            .Include(f => f.Atl)
            .Include(f => f.Encarregado)
            .FirstOrDefaultAsync(m => m.EducandoId == educandoId);

            var result = await _refeicoesController.GetRefeicoesByATLId(ATLbyEducando.AtlId) as JsonResult;
            var refeicoes = result?.Value as List<Refeicao>;

            return Json(refeicoes);
        }

        [HttpGet]
        public async Task<IActionResult> GetFormulariosByEncarregadoId(Guid encarregadoId)
        {
            var educandos = await _context.Educando
                .Include(e => e.Atl)
                .Include(e => e.Encarregado)
                .Where(e => e.EncarregadoId == encarregadoId)
                .ToListAsync();

            var formularios = new List<dynamic>();

            foreach (var educando in educandos)
            {


                var respostas = await (from resposta in _context.FormularioResposta
                                       join educandoTable in _context.Educando on resposta.EducandoId equals educandoTable.EducandoId
                                       join formularioTable in _context.Formulario on resposta.FormularioId equals formularioTable.FormularioId
                                       where resposta.EducandoId == educando.EducandoId && resposta.Authorized == false
                                       select new
                                       {
                                           RespostaId = resposta.FormularioRespostaId,
                                           FormularioId = resposta.FormularioId,
                                           FormularioName = formularioTable.Name,
                                           FormularioDescription = formularioTable.Description,
                                           FormularioEndDate = formularioTable.DateLimit,
                                           EducandoName = educandoTable.Name + " " + educandoTable.Apelido,
                                           Authorized = resposta.Authorized,
                                           ResponseDate = ((DateTime)resposta.ResponseDate).ToShortDateString()
                                       }).ToListAsync();

                formularios = formularios.Concat(respostas).ToList(); // usando 'Concat' em vez de 'Union'
            }

            return Json(formularios);
        }

        [HttpGet]
        public async Task<IActionResult> GetRecibosByEncarregadoId(Guid encarregadoId)
        {
            // Obtenha a lista de Educandos associados ao Encarregado de Educação
            var educandos = await _context.Educando
                .Include(e => e.Atl)
                .Include(e => e.Encarregado)
                .Where(e => e.EncarregadoId == encarregadoId)
                .ToListAsync();

            var recibos = new List<dynamic>();

            foreach (var educando in educandos)
            {
                // Busque os recibos associados a cada educando
                var recibosEducando = await (from recibo in _context.ReciboResposta
                                             join educandoTable in _context.Educando on recibo.EducandoId equals educandoTable.EducandoId
                                             where recibo.EducandoId == educando.EducandoId && recibo.Authorized == false
                                             select new
                                             {
                                                 RespostaId = recibo.ReciboRespostaId,
                                                 ReciboId = recibo.ReciboId,
                                                 ReciboName = recibo.Name,
                                                 EducandoName = educandoTable.Name + " " + educandoTable.Apelido,
                                                 DateLimit = recibo.DateLimit,
                                                 Valor = recibo.Price
                                             }).ToListAsync();
                
                recibos = recibos.Concat(recibosEducando).ToList();
            }

            return Json(recibos);
        }

        [HttpGet]
        public async Task<IActionResult> GetEducandoAtividadesByEncarregadoId(Guid encarregadoId)
        {
            // Obtenha a lista de Educandos associados ao Encarregado de Educação
            var educandos = await _context.Educando
                .Include(e => e.Atl)
                .Include(e => e.Encarregado)
                .Where(e => e.EncarregadoId == encarregadoId)
                .ToListAsync();
            
            var educandoAtividades = new List<dynamic>();

            foreach (var educando in educandos)
            {
                // Armazene o ID do educando atual em uma variável
                var currentEducandoId = educando.EducandoId;

                var atividadesEducando = await (from Educando in _context.Educando
                                                join atividade in _context.Atividade on educando.AtlId equals atividade.AtlId
                                                where Educando.EducandoId == currentEducandoId // Use a variável para fazer a comparação correta
                                                select new
                                                {
                                                    AtividadeId = atividade.AtividadeId,
                                                    EducandoName = educando.Name + " " + educando.Apelido,
                                                    AtividadeName = atividade.Name,
                                                    AtividadeDescription = atividade.Description,
                                                    AtividadeStartDate = atividade.StartDate,
                                                    AtividadeEndDate = atividade.EndDate,
                                                    AtividadePhoto = atividade.Picture
                                                }).ToListAsync();

                educandoAtividades = educandoAtividades.Concat(atividadesEducando).ToList();
            }

            return Json(educandoAtividades);
        }

        //Index -----------------------------------------------------------
		[HttpGet]
		public async Task<IActionResult> GetRefeicoesByATLId()
		{

			// Obtenha o usuário atual
			var currentUser = await _userManager.GetUserAsync(HttpContext.User);

			// Obtenha a conta administrativa do usuário atual
			var currentUserAccount = await _context.ContaAdministrativa
				.Include(f => f.User)
				.FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

			// Obtenha as atividades gerenciadas pelo usuário atual
			var refeicoes = await _context.Refeicao
				.Where(a => a.AtlId == currentUserAccount.AtlId)
				.ToListAsync();

			return Json(refeicoes);
		}
        [HttpGet]
        public async Task<IActionResult> GetFormulariosByATLId()
        {
            // Obtenha o usuário atual
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // Obtenha a conta administrativa do usuário atual
            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            // Obtenha os educandos associados ao ATL do usuário atual
            var educandos = await _context.Educando
                .Include(e => e.Atl)
                .Where(e => e.AtlId == currentUserAccount.AtlId)
                .ToListAsync();

            var formularios = new List<dynamic>();

            foreach (var educando in educandos)
            {
                var respostas = await (from resposta in _context.FormularioResposta
                                       join educandoTable in _context.Educando on resposta.EducandoId equals educandoTable.EducandoId
                                       join formularioTable in _context.Formulario on resposta.FormularioId equals formularioTable.FormularioId
                                       where resposta.EducandoId == educando.EducandoId && resposta.Authorized == false
                                       select new
                                       {
                                           RespostaId = resposta.FormularioRespostaId,
                                           FormularioId = resposta.FormularioId,
                                           FormularioName = formularioTable.Name,
                                           FormularioDescription = formularioTable.Description,
                                           FormularioEndDate = formularioTable.DateLimit,
                                           EducandoName = educandoTable.Name + " " + educandoTable.Apelido,
                                           Authorized = resposta.Authorized,
                                           ResponseDate = ((DateTime)resposta.ResponseDate).ToShortDateString()
                                       }).ToListAsync();

                formularios = formularios.Concat(respostas).ToList(); // usando 'Concat' em vez de 'Union'
            }

            return Json(formularios);
        }

        [HttpGet]
        public async Task<IActionResult> GetAtividadesByATL()
        {
            // Obtenha o usuário atual
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // Obtenha a conta administrativa do usuário atual
            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            // Obtenha as atividades relacionadas ao ATL do usuário atual
            var atividades = await _context.Atividade
                .Include(a => a.Atl)
                .Where(a => a.AtlId == currentUserAccount.AtlId)
                .Select(a => new
                {
                    AtividadeId = a.AtividadeId,
                    AtividadeName = a.Name,
                    AtividadeDescription = a.Description,
                    AtividadeStartDate = a.StartDate,
                    AtividadeEndDate = a.EndDate,
                    AtividadePhoto = a.Picture
                })
                .ToListAsync();

            return Json(atividades);
        }

        [HttpGet]
        public async Task<IActionResult> GetRecibosByATLId()
        {
            // Obtenha o usuário atual
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // Obtenha a conta administrativa do usuário atual
            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            // Obtenha a lista de Educandos associados ao ATL do usuário atual
            var educandos = await _context.Educando
                .Include(e => e.Atl)
                .Where(e => e.AtlId == currentUserAccount.AtlId)
                .ToListAsync();

            var recibos = new List<dynamic>();

            foreach (var educando in educandos)
            {
                // Busque os recibos associados a cada educando
                var recibosEducando = await (from recibo in _context.ReciboResposta
                                             join educandoTable in _context.Educando on recibo.EducandoId equals educandoTable.EducandoId
                                             where recibo.EducandoId == educando.EducandoId && recibo.Authorized == false
                                             select new
                                             {
                                                 RespostaId = recibo.ReciboRespostaId,
                                                 ReciboId = recibo.ReciboId,
                                                 ReciboName = recibo.Name,
                                                 EducandoName = educandoTable.Name + " " + educandoTable.Apelido,
                                                 DateLimit = recibo.DateLimit,
                                                 Valor = recibo.Price
                                             }).ToListAsync();

                recibos = recibos.Concat(recibosEducando).ToList();
            }

            return Json(recibos);
        }
        
        [HttpGet]
        public async Task<Dictionary<string, int>> GetVisitasEstudoPorMesEstatisticas()
        {
            // Obtenha o usuário atual
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // Obtenha a conta administrativa do usuário atual
            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);
            

            // Obtenha as visitas de estudo gerenciadas pelo educando atual
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

        [HttpGet]
        public async Task<Dictionary<string, int>> GetAtividadesPorMesEstatisticas()
        {
            // Obtenha o usuário atual
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // Obtenha a conta administrativa do usuário atual
            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            // Obtenha as atividades gerenciadas pelo educando atual
            var atividades = await _context.Atividade
                .Where(a => a.AtlId == currentUserAccount.AtlId)
                .ToListAsync();

            var anoAtual = DateTime.Now.Year;
            var estatisticas = new Dictionary<string, int>();

            for (int mes = 1; mes <= 12; mes++)
            {
                var atividadesNoMes = atividades.Count(a => a.StartDate.Year == anoAtual && a.StartDate.Month == mes && a.EndDate.Year == anoAtual && a.EndDate.Month == mes);
                estatisticas.Add($"AtividadeMes{mes}", atividadesNoMes);
            }

            return estatisticas;
        }

        /// <summary>
        /// Obtém o número de educandos para um determinado ID de ATL.
        /// </summary>
        /// <param name="id">O ID do ATL</param>
        /// <returns>O número de educandos</returns>
        [HttpGet]
        public async Task<int> GetNumeroDeEducandosADM()
        {
            // Obtenha o usuário atual
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // Obtenha a conta administrativa do usuário atual
            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);
            
            var educandos = await _context.Educando
                .Where(c => c.AtlId == currentUserAccount.AtlId)
                .ToListAsync();

            return educandos.Count;
        }

        /// <summary>
        /// Obtém o número de educandos novos (inscritos no último mês) para um determinado ID de ATL.
        /// </summary>
        /// <param name="id">O ID do ATL</param>
        /// <returns>O número de educandos novos</returns>
        [HttpGet]
        public async Task<int> GetNumeroDeEducandosNovos()
        {
            // Obtenha o usuário atual
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // Obtenha a conta administrativa do usuário atual
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
        /// Retorna um dicionário contendo o número de educandos inscritos por mês.
        /// </summary>
        /// <returns>Dicionário com o número de educandos inscritos em cada mês.</returns>
        [HttpGet]
        public async Task<Dictionary<string, int>> GetEducandosPorMesEstatisticas()
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
        [HttpGet]
        public async Task<int> GetNumeroDeRapazes()
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
        [HttpGet]
        public async Task<int> GetNumeroDeRaparigas()
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
        [HttpGet]
        public async Task<int> GetFaturasEmAtraso()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            var faturasEmAtraso = await _context.ReciboResposta
                .Where(r => r.Recibo.AtlId == currentUserAccount.AtlId && !r.Authorized)
                .ToListAsync();

            return faturasEmAtraso.Count;
        }

        /// <summary>
        /// Retorna o número e o total pago de faturas autorizadas de um ATL.
        /// </summary>
        /// <returns>Tupla contendo o número de faturas e o valor total pago.</returns>
        [HttpGet]
        public async Task<decimal> GetFaturacaoMesAtual()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            var faturasPagasMesAtual = await _context.ReciboResposta
                .Where(r => r.Recibo.AtlId == currentUserAccount.AtlId && r.Authorized && r.DateLimit.Month == DateTime.Now.Month)
                .ToListAsync();

            decimal totalValorPagoMesAtual = faturasPagasMesAtual.Sum(f => decimal.Parse(f.Price.Replace(',', '.'), CultureInfo.InvariantCulture));
            return totalValorPagoMesAtual;
        }

        //IndexADM----------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetRefeicoesByATLIdADM(Guid atlId)
        {

            // Obtenha as atividades gerenciadas pelo usuário atual
            var refeicoes = await _context.Refeicao
                .Where(a => a.AtlId == atlId)
                .ToListAsync();

            return Json(refeicoes);
        }
        
        
        [HttpGet]
        public async Task<IActionResult> GetFormulariosByATLIdADM(Guid atlId)
        {
 
            // Obtenha os educandos associados ao ATL do usuário atual
            var educandos = await _context.Educando
                .Include(e => e.Atl)
                .Where(e => e.AtlId == atlId)
                .ToListAsync();

            var formularios = new List<dynamic>();

            foreach (var educando in educandos)
            {
                var respostas = await (from resposta in _context.FormularioResposta
                                       join educandoTable in _context.Educando on resposta.EducandoId equals educandoTable.EducandoId
                                       join formularioTable in _context.Formulario on resposta.FormularioId equals formularioTable.FormularioId
                                       where resposta.EducandoId == educando.EducandoId && resposta.Authorized == false
                                       select new
                                       {
                                           RespostaId = resposta.FormularioRespostaId,
                                           FormularioId = resposta.FormularioId,
                                           FormularioName = formularioTable.Name,
                                           FormularioDescription = formularioTable.Description,
                                           FormularioEndDate = formularioTable.DateLimit,
                                           EducandoName = educandoTable.Name + " " + educandoTable.Apelido,
                                           Authorized = resposta.Authorized,
                                           ResponseDate = ((DateTime)resposta.ResponseDate).ToShortDateString()
                                       }).ToListAsync();

                formularios = formularios.Concat(respostas).ToList(); // usando 'Concat' em vez de 'Union'
            }

            return Json(formularios);
        }

        [HttpGet]
        public async Task<IActionResult> GetAtividadesByATLADM(Guid atlId)
        {

            // Obtenha as atividades relacionadas ao ATL do usuário atual
            var atividades = await _context.Atividade
                .Include(a => a.Atl)
                .Where(a => a.AtlId == atlId)
                .Select(a => new
                {
                    AtividadeId = a.AtividadeId,
                    AtividadeName = a.Name,
                    AtividadeDescription = a.Description,
                    AtividadeStartDate = a.StartDate,
                    AtividadeEndDate = a.EndDate,
                    AtividadePhoto = a.Picture
                })
                .ToListAsync();

            return Json(atividades);
        }

        [HttpGet]
        public async Task<IActionResult> GetRecibosByATLIdADM(Guid atlId)
        {
            // Obtenha a lista de Educandos associados ao ATL do usuário atual
            var educandos = await _context.Educando
                .Include(e => e.Atl)
                .Where(e => e.AtlId == atlId)
                .ToListAsync();

            var recibos = new List<dynamic>();

            foreach (var educando in educandos)
            {
                // Busque os recibos associados a cada educando
                var recibosEducando = await (from recibo in _context.ReciboResposta
                                             join educandoTable in _context.Educando on recibo.EducandoId equals educandoTable.EducandoId
                                             where recibo.EducandoId == educando.EducandoId && recibo.Authorized == false
                                             select new
                                             {
                                                 RespostaId = recibo.ReciboRespostaId,
                                                 ReciboId = recibo.ReciboId,
                                                 ReciboName = recibo.Name,
                                                 EducandoName = educandoTable.Name + " " + educandoTable.Apelido,
                                                 DateLimit = recibo.DateLimit,
                                                 Valor = recibo.Price
                                             }).ToListAsync();

                recibos = recibos.Concat(recibosEducando).ToList();
            }

            return Json(recibos);
        }

        [HttpGet]
        public async Task<Dictionary<string, int>> GetVisitasEstudoPorMesEstatisticasADM(Guid atlId)
        {
            // Obtenha as visitas de estudo gerenciadas pelo educando atual
            var visitasEstudo = await _context.VisitaEstudo
                .Where(a => a.AtlId == atlId)
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

        [HttpGet]
        public async Task<Dictionary<string, int>> GetAtividadesPorMesEstatisticasADM(Guid atlId)
        {
            // Obtenha as atividades gerenciadas pelo educando atual
            var atividades = await _context.Atividade
                .Where(a => a.AtlId == atlId)
                .ToListAsync();

            var anoAtual = DateTime.Now.Year;
            var estatisticas = new Dictionary<string, int>();

            for (int mes = 1; mes <= 12; mes++)
            {
                var atividadesNoMes = atividades.Count(a => a.StartDate.Year == anoAtual && a.StartDate.Month == mes && a.EndDate.Year == anoAtual && a.EndDate.Month == mes);
                estatisticas.Add($"AtividadeMes{mes}", atividadesNoMes);
            }

            return estatisticas;
        }

        /// <summary>
        /// Obtém o número de educandos para um determinado ID de ATL.
        /// </summary>
        /// <param name="id">O ID do ATL</param>
        /// <returns>O número de educandos</returns>
        [HttpGet]
        public async Task<int> GetNumeroDeEducandosADM(Guid atlId)
        {
            var educandos = await _context.Educando
                .Where(c => c.AtlId == atlId)
                .ToListAsync();

            return educandos.Count;
        }

        /// <summary>
        /// Obtém o número de educandos novos (inscritos no último mês) para um determinado ID de ATL.
        /// </summary>
        /// <param name="id">O ID do ATL</param>
        /// <returns>O número de educandos novos</returns>
        [HttpGet]
        public async Task<int> GetNumeroDeEducandosNovosADM(Guid atlId)
        {
  
            DateTime umMesAtras = DateTime.Now.AddMonths(-1);

            var educandos = await _context.Educando
                .Where(c => c.AtlId == atlId && c.DataDeInscricao >= umMesAtras)
                .ToListAsync();

            return educandos.Count;
        }

        /// <summary>
        /// Retorna um dicionário contendo o número de educandos inscritos por mês.
        /// </summary>
        /// <returns>Dicionário com o número de educandos inscritos em cada mês.</returns>
        [HttpGet]
        public async Task<Dictionary<string, int>> GetEducandosPorMesEstatisticasADM(Guid atlID)
        {
            var educandos = await _context.Educando
                .Where(c => c.AtlId == atlID)
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
        [HttpGet]
        public async Task<int> GetNumeroDeRapazesADM(Guid atlId)
        {
            var educandos = await _context.Educando
                .Where(c => c.AtlId == atlId && c.Genero == "Masculino")
                .ToListAsync();

            return educandos.Count;
        }

        /// <summary>
        /// Retorna o número de raparigas inscritas no ATL.
        /// </summary>
        /// <returns>Número de raparigas inscritas.</returns>
        [HttpGet]
        public async Task<int> GetNumeroDeRaparigasADM(Guid atlId)
        {
            var educandos = await _context.Educando
                .Where(c => c.AtlId == atlId && c.Genero == "Feminino")
                .ToListAsync();

            return educandos.Count;
        }

        /// <summary>
        /// Retorna o número e o total em atraso de faturas não autorizadas de um ATL.
        /// </summary>
        /// <returns>Tupla contendo o número de faturas e o valor total em atraso.</returns>
        [HttpGet]
        public async Task<int> GetFaturasEmAtrasoADM(Guid atlID)
        {
            var faturasEmAtraso = await _context.ReciboResposta
                .Where(r => r.Recibo.AtlId == atlID && !r.Authorized)
                .ToListAsync();

            return faturasEmAtraso.Count;
        }

        /// <summary>
        /// Retorna o número e o total pago de faturas autorizadas de um ATL.
        /// </summary>
        /// <returns>Tupla contendo o número de faturas e o valor total pago.</returns>
        [HttpGet]
        public async Task<decimal> GetFaturacaoMesAtualADM(Guid atlId)
        {
            var faturasPagasMesAtual = await _context.ReciboResposta
                .Where(r => r.Recibo.AtlId == atlId && r.Authorized && r.DateLimit.Month == DateTime.Now.Month)
                .ToListAsync();

            decimal totalValorPagoMesAtual = faturasPagasMesAtual.Sum(f => decimal.Parse(f.Price.Replace(',', '.'), CultureInfo.InvariantCulture));
            return totalValorPagoMesAtual;
        }


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