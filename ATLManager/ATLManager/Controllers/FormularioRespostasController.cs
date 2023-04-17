using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace ATLManager.Controllers
{
    public class FormularioRespostasController : Controller
    {
		private readonly ATLManagerAuthContext _context;
		private readonly UserManager<ATLManagerUser> _userManager;
		private readonly INotificacoesController _notificacoesController;


		public FormularioRespostasController(ATLManagerAuthContext context,
			UserManager<ATLManagerUser> userManager,
            INotificacoesController notificacoesController)
		{
			_context = context;
			_userManager = userManager;
			_notificacoesController = notificacoesController;
		}

		// GET: FormularioRespostas/Details/5
		public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.FormularioResposta == null)
            {
                return NotFound();
            }

            var formularioResposta = await _context.FormularioResposta
                .Include(f => f.Educando)
                .Include(f => f.Formulario)
                .FirstOrDefaultAsync(m => m.FormularioRespostaId == id);
            if (formularioResposta == null)
            {
                return NotFound();
            }

            return View(formularioResposta);
		}

		// GET: FormularioRespostas/Edit/5
		[Authorize(Roles = "EncarregadoEducacao")]
		public async Task<IActionResult> Responder(Guid? id)
        {
            if (id == null || _context.FormularioResposta == null)
            {
                return NotFound();
            }

            var formularioResposta = await _context.FormularioResposta.FindAsync(id);
            if (formularioResposta == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formulario.FindAsync(formularioResposta.FormularioId);
            var viewModel = new FormularioResponderViewModel
            {
                FormularioRespostaId = formularioResposta.FormularioRespostaId,
                Name = formulario.Name,
                Description = formulario.Description,
                DateLimit = formulario.DateLimit.ToShortDateString(),
                Authorized = formularioResposta.Authorized
            };

            return View(viewModel);
        }

        // POST: FormularioRespostas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Responder(Guid id, FormularioResponderViewModel viewModel)
        {
            if (id != viewModel.FormularioRespostaId)
            {
                return NotFound();
            }

            ModelState.Remove("Name");
            ModelState.Remove("Description");
            ModelState.Remove("DateLimit");

            if (ModelState.IsValid)
            {
                try
                {
                    var formularioResposta = await _context.FormularioResposta.FindAsync(id);

                    formularioResposta.Authorized = viewModel.Authorized;
                    formularioResposta.ResponseDate = DateTime.UtcNow.Date;

					_context.Update(formularioResposta);
                    await _context.SaveChangesAsync();

					// Enviar notificações para os usuários relevantes (Coordenadores, Funcionários) do ATL específico
					var roleNames = new[] { "Coordenador", "Funcionario" };

                    // Enviar notificação aos coordenadores e funcionários
                    var educando = await _context.Educando.FirstOrDefaultAsync(e => e.EducandoId == formularioResposta.EducandoId);
                    
                    var currentUser = await _userManager.GetUserAsync(HttpContext.User);

					Guid specificATLId = (Guid)educando.AtlId;

                    

					var usersToNotify = await (from user in _context.Users
											   join userRole in _context.UserRoles on user.Id equals userRole.UserId
											   join role in _context.Roles on userRole.RoleId equals role.Id
											   join account in _context.ContaAdministrativa on user.Id equals account.UserId
											   where roleNames.Contains(role.Name) && account.AtlId == specificATLId
											   select user).ToListAsync();


					foreach (var user in usersToNotify)
					{
						await _notificacoesController.CreateNotification(user.Id, "Nova Resposta ao Formulário", "Uma resposta ao formulário foi adicionada ou atualizada.");
					}

				}
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormularioRespostaExists(viewModel.FormularioRespostaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Obrigado));
            }
            return View(viewModel);
        }

		public IActionResult Obrigado()
		{
			return View();
		}

		private bool FormularioRespostaExists(Guid id)
        {
          return (_context.FormularioResposta?.Any(e => e.FormularioRespostaId == id)).GetValueOrDefault();
        }
    }
}
