using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.Models;
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using ATLManager.ViewModels;
using ATLManager.Models.Historicos;

namespace ATLManager.Controllers
{
    /// <summary>
    /// Controlador para o modelo 'Formulários'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class FormulariosController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly INotificacoesController _notificacoesController;

        public FormulariosController(ATLManagerAuthContext context,
            UserManager<ATLManagerUser> userManager,
            IEmailSender emailSender, 
            INotificacoesController notificacoesController)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _notificacoesController = notificacoesController;
        }

        /// <summary>
        /// Retorna a exibição da página inicial com uma lista de formulários associados à conta administrativa.
        /// </summary>
        /// <returns>Uma instância de IActionResult que representa a exibição da página inicial.</returns>

        [Authorize(Roles = "Coordenador,Funcionario")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            var formularios = await _context.Formulario
                .Include(a => a.Atl)
                .Where(r => r.AtlId == currentUserAccount.AtlId)
                .ToListAsync();

			return View(formularios);
        }

        /// <summary>
        /// Retorna a exibição dos detalhes do formulário especificado pelo ID.
        /// </summary>
        /// <param name="id">O ID do formulário a ser exibido.</param>
        /// <returns>Uma instância de IActionResult que representa a exibição dos detalhes do formulário.</returns>

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Formulario == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formulario
                .Include(f => f.VisitaEstudo)
                .Include(v => v.Atividade)
                .FirstOrDefaultAsync(m => m.FormularioId == id);
            if (formulario == null)
            {
                return NotFound();
            }

            return View(formulario);
        }

        /// <summary>
        /// Retorna a exibição da página inicial para o encarregado de educação atual com uma lista de formulários associados aos seus educandos.
        /// </summary>
        /// <returns>Uma instância de IActionResult que representa a exibição da página inicial do encarregado de educação.</returns>

        [Authorize(Roles = "EncarregadoEducacao")]
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

			var formularios = new List<FormularioRespostasViewModel>();

			foreach (var educando in educandos)
			{
				var respostas = await (from resposta in _context.FormularioResposta
                                       join formulario in _context.Formulario on resposta.FormularioId equals formulario.FormularioId
									   join educandoTable in _context.Educando on resposta.EducandoId equals educandoTable.EducandoId
									   where resposta.EducandoId == educando.EducandoId
									   select new FormularioRespostasViewModel
									   {
										   RespostaId = resposta.FormularioRespostaId,
										   FormularioId = resposta.FormularioId,
                                           FormularioName = formulario.Name,
										   EducandoName = educandoTable.Name + " " + educandoTable.Apelido,
										   Authorized = resposta.Authorized,
										   ResponseDate = ((DateTime)resposta.ResponseDate).ToShortDateString()
									   }).ToListAsync();

				formularios = formularios.Union(respostas).ToList();
			}

			if (formularios == null)
			{
				return NotFound();
			}

			ViewData["EducandoId"] = new SelectList(educandos, "EducandoId", "Name");
			ViewBag.Educandos = educandos;
			return View(formularios);
		}

        /// <summary>
        /// Obtém as respostas de um formulário pelo id.
        /// </summary>
        /// <param name="id">O id do formulário.</param>
        /// <returns>Retorna uma lista de respostas do formulário ou NotFound se não existir.</returns>

        public async Task<IActionResult> Respostas(Guid? id)
        {
            if (id == null || _context.Formulario == null)
            {
                return NotFound();
            }

			var respostas = await (from resposta in _context.FormularioResposta
                                   join educando in _context.Educando on resposta.EducandoId equals educando.EducandoId
                                   where resposta.FormularioId == id
                                   select new FormularioRespostasViewModel
                                   {
                                       RespostaId = resposta.FormularioRespostaId,
                                       FormularioId = resposta.FormularioId,
                                       EducandoName = educando.Name + " " + educando.Apelido,
                                       Authorized = resposta.Authorized,
                                       ResponseDate = ((DateTime)resposta.ResponseDate).ToShortDateString()
                                   }).ToListAsync();

            if (respostas == null)
            {
                return NotFound();
            }

            ViewData["FormularioId"] = id;
            return View(respostas);
        }

        /// <summary>
        /// Obtém as estatísticas de visitas de estudo de um formulário com base em seu ID.
        /// </summary>
        /// <param name="id">O ID do formulário.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona e contém um objeto Dictionary que representa as estatísticas de visitas de estudo.</returns>

        public async Task<IActionResult> Estatisticas(Guid id)
        {
            var estatisticas = await GetVisitasDeEstudoEstatisticas(id);
            ViewData["FormularioId"] = id; // Passa o id do formulário para a view
            return View(estatisticas);
        }

        /// <summary>
        /// Obtém as estatísticas de visitas de estudo de um formulário com base em seu ID.
        /// </summary>
        /// <param name="id">O ID do formulário.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona e contém um objeto Dictionary que representa as estatísticas de visitas de estudo.</returns>

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

            var totalFormularios = respostas.Count;
            var totalAutorizados = respostas.Count(fr => fr.Authorized);
            var totalNaoAutorizados = totalFormularios - totalAutorizados;
            var percentualAutorizados = totalFormularios != 0 ? (decimal)totalAutorizados / totalFormularios * 100 : 0;

            var estatisticas = new Dictionary<string, decimal>
            {
                { "TotalAutorizados", totalAutorizados },
                { "TotalNaoAutorizados", totalNaoAutorizados },
                { "PercentualAutorizados", percentualAutorizados },
                { "PercentualNaoAutorizados", 100 - percentualAutorizados }
            };

            return estatisticas;
        }

        /// <summary>
        /// Obtém as estatísticas de visitas de estudo de um formulário com base em seu ID através de uma solicitação Ajax.
        /// </summary>
        /// <param name="formularioId">O ID do formulário.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona e contém um objeto JsonResult que representa as estatísticas de visitas de estudo.</returns>

        [HttpGet]
        public async Task<JsonResult> GetVisitasDeEstudoEstatisticasAjax(Guid formularioId)
        {
            var estatisticas = await GetVisitasDeEstudoEstatisticas(formularioId);
            return Json(estatisticas);
        }

        /// <summary>
        /// Exibe a página de criação de um novo formulário.
        /// </summary>
        /// <returns>Uma tarefa que representa a operação assíncrona e contém um objeto IActionResult que representa o resultado da ação.</returns>

        [Authorize(Roles = "Coordenador")]
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var userAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            if (userAccount == null)
            {
                return NotFound();
            }

            var visitas = await _context.VisitaEstudo
                .Include(a => a.Atl)
                .Where(r => r.AtlId == userAccount.AtlId)
                .ToListAsync();

            var atividades = await _context.Atividade
                .Include(a => a.Atl)
                .Where(r => r.AtlId == userAccount.AtlId)
                .ToListAsync();

            ViewData["VisitaEstudoId"] = new SelectList(visitas, "VisitaEstudoID", "Name");
            ViewData["AtividadeId"] = new SelectList(atividades, "AtividadeId", "Name");
            return View();
        }

        /// <summary>
        /// Cria um novo formulário com base nos dados fornecidos pelo utilizador.
        /// </summary>
        /// <param name="formulario">O objeto Formulario que contém os dados do formulário a ser criado.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona e retorna uma IActionResult.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FormularioId,Name,Description,VisitaEstudoId,AtividadeId,DateLimit")] Formulario formulario)
        {
			if (formulario.VisitaEstudoId != null && formulario.AtividadeId != null)
			{
				var validationMessage = "Apenas permitido escolher uma Visita ou uma Atividade";
				ModelState.AddModelError("VisitaEdtudoId", validationMessage);
				ModelState.AddModelError("AtividadeId", validationMessage);
			}
            
            if (formulario.DateLimit < DateTime.UtcNow)
            {
                var validationMessage = "Não é possível criarum Formulário com uma data de término anterior à data de incício";
                ModelState.AddModelError("DateLimit", validationMessage);
            }

			var user = await _userManager.GetUserAsync(HttpContext.User);
            var userAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == user.Id);

            ModelState.Remove("StartDate");

            if (ModelState.IsValid)
            {
                formulario.FormularioId = Guid.NewGuid();
                formulario.AtlId = userAccount?.AtlId;
                formulario.StartDate = DateTime.UtcNow.Date;
                _context.Add(formulario);

                var educandos = await _context.Educando
                    .Include(c => c.Atl)
                    .Where(g => g.AtlId == userAccount.AtlId)
                    .ToListAsync(); // Carregue os dados antes de entrar no loop

                foreach (var educando in educandos)
                {
					var resposta = new FormularioResposta(formulario.FormularioId, educando.EducandoId)
					{
						DateLimit = formulario.DateLimit
					};

					// Obter Encarregado do Educando e a sua conta
					var encarregado = await _context.EncarregadoEducacao
                        .FirstOrDefaultAsync(e => e.EncarregadoId == educando.EncarregadoId);
                    var encarregadoAccount = await _context.Users
                        .FirstOrDefaultAsync(e => e.Id == encarregado.UserId);

                    var userEmail = await _userManager.GetEmailAsync(encarregadoAccount);
                    var code = resposta.FormularioRespostaId.ToString();
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Action("Responder", "FormularioRespostas", new { id = resposta.FormularioRespostaId }, Request.Scheme);

                    // Enviar notificação para o Encarregado de Educação
                    var notificationMessage = $"Há um novo formulário disponível para o seu educando {educando.Name} {educando.Apelido}, que pertence ao ATL {educando.Atl.Name}. Por favor, responda o mais rápido possível ao <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicar aqui</a>.";
                    var notificationTitle = $"Novo Formulário - {formulario.Name}";

                    await _emailSender.SendEmailAsync(userEmail, notificationTitle, notificationMessage);

                    await _notificacoesController.CreateNotification(encarregado.UserId, notificationTitle, notificationMessage);

                    _context.Add(resposta);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var visitas = await _context.VisitaEstudo
                .Include(a => a.Atl)
                .Where(r => r.AtlId == userAccount.AtlId)
                .ToListAsync();

            ViewData["VisitaEstudoId"] = new SelectList(visitas, "VisitaEstudoID", "Name", formulario.VisitaEstudoId);
            return View(formulario);
        }

        /// <summary>
        /// Retorna a visualização do formulário para edição.
        /// </summary>
        /// <param name="id">O ID do formulário a ser editado.</param>
        /// <returns>A visualização do formulário para edição.</returns>

        public async Task<IActionResult> Edit(Guid? id)
		{
			if (id == null || _context.Formulario == null)
			{
				return NotFound();
			}

			var formulario = await _context.Formulario.FindAsync(id);
			if (formulario == null)
			{
				return NotFound();
			}

			var viewModel = new FormularioEditViewModel
			{
				FormularioId = formulario.FormularioId,
				Name = formulario.Name,
				Description = formulario.Description,
				DateLimit = formulario.DateLimit.ToShortDateString(),
			};

			return View(viewModel);
		}

        /// <summary>
        /// Método para editar um formulário existente.
        /// </summary>
        /// <param name="id">ID do formulário a ser editado.</param>
        /// <param name="viewModel">ViewModel do formulário editado.</param>
        /// <returns>Uma tarefa assíncrona que retorna uma ação de resultado.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, FormularioEditViewModel viewModel)
        {
            if (id != viewModel.FormularioId)
            {
                return NotFound();
            }

            DateTime dataAtual = DateTime.Now;

            DateTime dataViewModel = DateTime.Parse(viewModel.DateLimit);
            if (dataViewModel.CompareTo(dataAtual) < 0)
            {
                var validationMessage = "Não é possível criar um Formulário com uma data anterior à data atual";
                ModelState.AddModelError("StartDate", validationMessage);
            }


            if (ModelState.IsValid)
            {
                try
                {
					var formulario = await _context.Formulario.FindAsync(viewModel.FormularioId);

					if (formulario != null)
					{
						formulario.Name = viewModel.Name;
						formulario.Description = viewModel.Description;

						if (viewModel.DateLimit != null)
							formulario.DateLimit = DateTime.Parse(viewModel.DateLimit);
                        
                        _context.Update(formulario);
						await _context.SaveChangesAsync();
					}
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormularioExists(viewModel.FormularioId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        /// <summary>
        /// Método para obter o formulário a ser excluído.
        /// </summary>
        /// <param name="id">ID do formulário a ser excluído.</param>
        /// <returns>Uma tarefa assíncrona que retorna uma ação de resultado.</returns>

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Formulario == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formulario
                .Include(f => f.VisitaEstudo)
                .FirstOrDefaultAsync(m => m.FormularioId == id);
            if (formulario == null)
            {
                return NotFound();
            }

            return View(formulario);
        }

        /// <summary>
        /// Método para confirmar a exclusão de um formulário.
        /// </summary>
        /// <param name="id">ID do formulário a ser excluído.</param>
        /// <returns>Uma tarefa assíncrona que retorna uma ação de resultado.</returns>

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Formulario == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.Formulario'  is null.");
            }
            var formulario = await _context.Formulario
                .Include(f => f.VisitaEstudo)
                .Include(f => f.Atividade)
                .FirstOrDefaultAsync(m => m.FormularioId == id);
            if (formulario != null)
            {
                var record = new FormularioRecord()
                {
                    FormularioId = formulario.FormularioId,
                    Name = formulario.Name,
                    Description = formulario.Description,
                    VisitaEstudo = formulario.VisitaEstudo?.Name,
                    Atividade = formulario.Atividade?.Name,
                    StartDate = formulario.StartDate.Date,
                    DateLimit = formulario.DateLimit.Date,
                    AtlId = formulario.AtlId,
                };

                var respostas = await _context.FormularioResposta
                    .Include(r => r.Formulario)
                    .Include(r => r.Educando)
                    .Where(r => r.FormularioId == formulario.FormularioId)
                    .ToListAsync();

                foreach (var resposta in respostas)
                {
                    var respostaRecord = new FormularioRespostaRecord()
                    {
                        FormularioRespostaId = resposta.FormularioRespostaId,
                        FormularioRecordId = record.FormularioRecordId,
                        Educando = resposta.Educando.Name + " " + resposta.Educando.Apelido,
                        Authorized = resposta.Authorized,
                        DateLimit = ((DateTime)resposta.DateLimit).Date,
                        ResponseDate = (resposta.ResponseDate == null) ? null : ((DateTime)resposta.ResponseDate).Date,
                    };
                    
                    _context.Add(respostaRecord);
                };

                _context.Add(record);
                _context.Formulario.Remove(formulario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se um formulário com o ID especificado existe no contexto atual.
        /// </summary>
        /// <param name="id">O ID do formulário a ser verificado.</param>
        /// <returns>Retorna verdadeiro se um formulário com o ID especificado existe no contexto atual; caso contrário, retorna falso.</returns>

        private bool FormularioExists(Guid id)
        {
          return (_context.Formulario?.Any(e => e.FormularioId == id)).GetValueOrDefault();
        }
    }
}
