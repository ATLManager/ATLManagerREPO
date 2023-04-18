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
        public async Task<IActionResult> Create([Bind("FormularioId,Name,Description,VisitaEstudoId,AtividadeId,StartDate,DateLimit")] Formulario formulario)
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

            if (ModelState.IsValid)
            {
                formulario.FormularioId = Guid.NewGuid();
                formulario.AtlId = userAccount?.AtlId;
                _context.Add(formulario);

                var educandos = await _context.Educando
                    .Include(c => c.Atl)
                    .Where(g => g.AtlId == userAccount.AtlId)
                    .ToListAsync(); // Carregue os dados antes de entrar no loop

                foreach (var educando in educandos)
                {
                    var resposta = new FormularioResposta(formulario.FormularioId, educando.EducandoId);
                    resposta.DateLimit = formulario.DateLimit;

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
				VisitaEstudoId = formulario.VisitaEstudoId,
				Description = formulario.Description,
				StartDate = formulario.StartDate.ToShortDateString(),
				DateLimit = formulario.DateLimit.ToShortDateString(),
			};

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

            ViewData["VisitaEstudoId"] = new SelectList(visitas, "VisitaEstudoID", "Name", formulario.VisitaEstudoId);
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

            if (ModelState.IsValid)
            {
                try
                {
					var formulario = await _context.Formulario.FindAsync(viewModel.FormularioId);

					if (formulario != null)
					{
						formulario.Name = viewModel.Name;
						formulario.Description = viewModel.Description;

						if (viewModel.StartDate != null)
							formulario.StartDate = DateTime.Parse(viewModel.StartDate);
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

            ViewData["VisitaEstudoId"] = new SelectList(visitas, "VisitaEstudoID", "Name", viewModel.VisitaEstudoId);
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
            var formulario = await _context.Formulario.FindAsync(id);
            if (formulario != null)
            {
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
