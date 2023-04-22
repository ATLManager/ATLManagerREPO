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
    public class FormulariosController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly INotificacoesController _notificacoesController;

        public FormulariosController(ATLManagerAuthContext context,
            UserManager<ATLManagerUser> userManager,
            IEmailSender emailSender, INotificacoesController notificacoesController)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _notificacoesController = notificacoesController;
        }

        // GET: Formularios
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

        // GET: Formularios/Details/5
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
									   join educandoTable in _context.Educando on resposta.EducandoId equals educandoTable.EducandoId
									   where resposta.EducandoId == educando.EducandoId
									   select new FormularioRespostasViewModel
									   {
										   RespostaId = resposta.FormularioRespostaId,
										   FormularioId = resposta.FormularioId,
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

		// GET: Formularios/Respostas/5
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

            return View(respostas);
        }

        public async Task<IActionResult> Estatisticas(Guid id)
        {
            var estatisticas = await GetVisitasDeEstudoEstatisticas(id);
            ViewData["formularioId"] = id; // Passa o id do formulário para a view
            return View(estatisticas);
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

        [HttpGet]
        public async Task<JsonResult> GetVisitasDeEstudoEstatisticasAjax(Guid formularioId)
        {
            var estatisticas = await GetVisitasDeEstudoEstatisticas(formularioId);
            return Json(estatisticas);
        }

        // GET: Formularios/Create
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

        // POST: Formularios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

			var user = await _userManager.GetUserAsync(HttpContext.User);
            var userAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == user.Id);

            DateTime dataAtual = DateTime.Now;

            DateTime dataViewModel = formulario.StartDate;
            if (dataViewModel.CompareTo(dataAtual) < 0)
            {
                var validationMessage = "Não é possível criar um Formulário com uma data anterior à data atual";
                ModelState.AddModelError("StartDate", validationMessage);
            }

            if (formulario.DateLimit < formulario.StartDate)
            {
                var validationMessage = "Não é possível criarum Formulário com uma data de término anterior à data de incício";
                ModelState.AddModelError("DateLimit", validationMessage);
            }


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

		// GET: Formularios/Edit/5
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

		// POST: Formularios/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Formularios/Delete/5
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

        // POST: Formularios/Delete/5
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

        private bool FormularioExists(Guid id)
        {
          return (_context.Formulario?.Any(e => e.FormularioId == id)).GetValueOrDefault();
        }
    }
}
