using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace ATLManager.Controllers
{
    /// <summary>
    /// Controlador para o modelo 'Resposta de Formulários'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
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

        /// <summary>
        /// Obtém os detalhes de uma resposta de formulário.
        /// </summary>
        /// <param name="id">O ID da resposta de formulário.</param>
        /// <returns>Retorna a visualização com os detalhes da resposta do formulário.</returns>

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

        /// <summary>
        /// Obtém uma visualização para responder um formulário.
        /// </summary>
        /// <param name="id">O ID da resposta de formulário.</param>
        /// <returns>Retorna a visualização para responder o formulário.</returns>

        [Authorize(Roles = "EncarregadoEducacao")]
		public async Task<IActionResult> Responder(Guid? id)
        {
            if (id == null || _context.FormularioResposta == null)
            {
                return NotFound();
            }

			var resposta = await _context.FormularioResposta
				.Include(f => f.Educando)
				.Include(f => f.Formulario)
				.FirstOrDefaultAsync(m => m.FormularioRespostaId == id);
			if (resposta == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formulario.FindAsync(resposta.FormularioId);
            var viewModel = new FormularioResponderViewModel
            {
                FormularioRespostaId = resposta.FormularioRespostaId,
                Name = formulario.Name,
                Educando = resposta.Educando.Name + " " + resposta.Educando.Apelido,
				Description = formulario.Description,
                DateLimit = formulario.DateLimit.ToShortDateString(),
                Authorized = resposta.Authorized
            };

            return View(viewModel);
        }

        /// <summary>
        /// Salva uma resposta de formulário.
        /// </summary>
        /// <param name="id">O ID da resposta de formulário.</param>
        /// <param name="viewModel">A visualização com os dados da resposta do formulário.</param>
        /// <returns>Redireciona para a página "Obrigado" após guardar a resposta do formulário.</returns>

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
			ModelState.Remove("Educando");
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

                    // Get the Formulario Name
                    var formulario = await _context.Formulario.FirstOrDefaultAsync(f => f.FormularioId == formularioResposta.FormularioId);
                    string formularioName = formulario.Name;

                    // Update the notification title and message
                    string notificationTitle = $"Nova Resposta ao Formulário - {formularioName}";
                    string notificationMessage = $"Uma resposta ao formulário foi adicionada ou atualizada. <a href='/FormularioRespostas/Details/{formularioResposta.FormularioRespostaId}'>Clique aqui</a> para ver";


                    var usersToNotify = await (from user in _context.Users
											   join userRole in _context.UserRoles on user.Id equals userRole.UserId
											   join role in _context.Roles on userRole.RoleId equals role.Id
											   join account in _context.ContaAdministrativa on user.Id equals account.UserId
											   where roleNames.Contains(role.Name) && account.AtlId == specificATLId
											   select user).ToListAsync();


					foreach (var user in usersToNotify)
					{
                        await _notificacoesController.CreateNotification(user.Id, notificationTitle, notificationMessage);
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

        /// <summary>
        /// Retorna uma visualização de agradecimento após guardar a resposta do formulário.
        /// </summary>
        /// <returns>Retorna a visualização de agradecimento.</returns>

        public IActionResult Obrigado()
		{
			return View();
		}

        /// <summary>
        /// Verifica se a resposta de formulário existe no contexto.
        /// </summary>
        /// <param name="id">O ID da resposta de formulário.</param>
        /// <returns>Retorna "true" se a resposta de formulário existir no contexto e "false" caso contrário.</returns>

        private bool FormularioRespostaExists(Guid id)
        {
          return (_context.FormularioResposta?.Any(e => e.FormularioRespostaId == id)).GetValueOrDefault();
        }
    }
}
