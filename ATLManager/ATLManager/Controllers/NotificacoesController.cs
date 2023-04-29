using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.Models;
using Microsoft.AspNetCore.Identity;
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;

namespace ATLManager.Controllers
{
    /// <summary>
    /// Controlador para o modelo 'Notificações'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class NotificacoesController : Controller, INotificacoesController
    {
        private readonly ATLManagerAuthContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ATLManagerUser> _userManager;

		public NotificacoesController(ATLManagerAuthContext context, UserManager<ATLManagerUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
			_context = context;
			_userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Apresenta uma lista de notificações para o utilizador actual.
        /// </summary>
        /// <param name="id">ID opcional.</param>
        /// <returns>Devolve uma vista com a lista de notificações para o utilizador actual.</returns>
        [Route("Notificacoes/Index/{id?}")]
        public async Task<IActionResult> Index(Guid? id)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var userId = currentUser.Id;

            var notificacoes = await _context.Notificacoes
                .Where(n => n.UserId == userId)
                .Include(n => n.User)
                .ToListAsync();

            return View(notificacoes);
        }

        /// <summary>
        /// Apresenta os detalhes de uma notificação com o ID especificado.
        /// </summary>
        /// <param name="id">ID da notificação a apresentar.</param>
        /// <returns>Devolve uma vista com os pormenores da notificação com o ID especificado.</returns>

        public async Task<IActionResult> Details(Guid? id)
		{
			if (id == null || _context.Notificacoes == null)
			{
				return NotFound();
			}

			var notificacao = await _context.Notificacoes
				.Include(n => n.User)
				.FirstOrDefaultAsync(m => m.NotificacaoId == id);
			if (notificacao == null)
			{
				return NotFound();
			}

            // Marcar notificação como lida
            try
            {
                notificacao.Lida = true;
                _context.Update(notificacao);
                await _context.SaveChangesAsync();
                return Json(new { success = true, notificacao });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Método para devolver uma matriz JSON com utilizadores cujo primeiro nome começa com o termo de pesquisa especificado.
        /// </summary>
        /// <param name="searchTerm">O termo de pesquisa utilizado para filtrar os utilizadores.</param>
        /// <returns>Devolve uma matriz JSON com utilizadores cujo primeiro nome começa com o termo de pesquisa especificado.</returns>

        [HttpGet]
        public async Task<IActionResult> GetUsers(string searchTerm)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsers = await GetUsersAsync(currentUser);

			var filteredUsers = allUsers
				.Where(u => u.FirstName.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase))
				.Select(u => new { id = u.Id, firstName = u.FirstName, lastName = u.LastName })
				.ToList();


			return Json(filteredUsers);
        }

        /// <summary>
        /// Método que devolve uma lista de utilizadores a que o utilizador actual tem acesso, dependendo da sua função.
        /// </summary>
        /// <param name="currentUser">O utilizador atual.</param>
        /// <returns>Devolve uma lista de utilizadores a que o utilizador actual tem acesso, dependendo da sua função.</returns>

        private async Task<List<ATLManagerUser>> GetUsersAsync(ATLManagerUser currentUser)
		{
			var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
			var userAccount = await _context.ContaAdministrativa.FirstOrDefaultAsync(a => a.UserId == currentUser.Id);

			List<ATLManagerUser> targetUsers;

			if (currentUserRoles.Contains("Coordenador") || currentUserRoles.Contains("Funcionario"))
			{
				// Get Encarregados de Educação users
				var educandos = await _context.Educando
					.Include(c => c.Atl)
					.Where(g => g.AtlId == userAccount.AtlId)
					.ToListAsync();

				var encarregadoUsers = new List<ATLManagerUser>();

				foreach (var educando in educandos)
				{
					var encarregado = await _context.EncarregadoEducacao
						.FirstOrDefaultAsync(e => e.EncarregadoId == educando.EncarregadoId);
					var encarregadoAccount = await _context.Users
						.FirstOrDefaultAsync(e => e.Id == encarregado.UserId);

					if (encarregadoAccount != null && !encarregadoUsers.Any(u => u.Id == encarregadoAccount.Id))
					{
						encarregadoUsers.Add(encarregadoAccount);
					}
				}

				targetUsers = encarregadoUsers;
			}
			else if (currentUserRoles.Contains("EncarregadoEducacao"))
			{
				// Get Coordenador and Funcionario users
				var roleNames = new[] { "Coordenador", "Funcionario" };

				var encarregado = await _context.EncarregadoEducacao.FirstOrDefaultAsync(ee => ee.UserId == currentUser.Id);
				var educando = await _context.Educando.FirstOrDefaultAsync(e => e.EncarregadoId == encarregado.EncarregadoId);

				Guid specificATLId = (Guid)educando.AtlId;

				targetUsers = await (from user in _context.Users
									 join userRole in _context.UserRoles on user.Id equals userRole.UserId
									 join role in _context.Roles on userRole.RoleId equals role.Id
									 join account in _context.ContaAdministrativa on user.Id equals account.UserId
									 where roleNames.Contains(role.Name) && account.AtlId == specificATLId
									 select user).ToListAsync();
			}
			else
			{
				// If the user doesn't have any of the expected roles, return an empty list
				targetUsers = new List<ATLManagerUser>();
			}

			return targetUsers;
		}


        /// <summary>
        /// Mostra a vista para criar uma nova notificação. 
        /// </summary>
        /// <returns>A vista para criar uma nova notificação.</returns>

        [Authorize]
		public async Task<IActionResult> Create()
		{
			var currentUser = await _userManager.GetUserAsync(HttpContext.User);
			var targetUsers = await GetUsersAsync(currentUser);

			ViewData["UserId"] = new SelectList(targetUsers, "Id", "UserName");
			return View();
		}


        /// <summary>
        /// Cria uma nova notificação com base nos dados lançados.
        /// </summary>
        /// <param name="notificacao">Os dados da notificação que foram lançados.</param>
        /// <returns>A vista de índice das notificações se a notificação tiver sido criada com êxito; caso contrário, a vista de criação com mensagens de erro.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NotificacaoId,UserId,Titulo,Mensagem")] Notificacao notificacao)
        {
            var modelStateWithoutUser = new ModelStateDictionary(ModelState);
            modelStateWithoutUser.Remove("User");
            

            if (modelStateWithoutUser.IsValid)
            {
                notificacao.NotificacaoId = Guid.NewGuid();
                _context.Add(notificacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", notificacao.UserId);
            return View(notificacao);
        }

        /// <summary>
        /// Mostra a vista para editar uma notificação existente. 
        /// </summary>
        /// <param name="id">O ID da notificação a ser editada.</param>
        /// <returns>A vista para editar a notificação.</returns>

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Notificacoes == null)
            {
                return NotFound();
            }

            var notificacao = await _context.Notificacoes.FindAsync(id);
            if (notificacao == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", notificacao.UserId);
            return View(notificacao);
        }

        /// <summary>
        /// Actualiza uma notificação existente com base nos dados lançados. 
        /// </summary>
        /// <param name="id">O ID da notificação a actualizar.</param>
        /// <param name="notificacao">Os dados da notificação que foram lançados.</param>
        /// <returns>A vista de índice das notificações se a notificação tiver sido actualizada com êxito; caso contrário, a vista de edição com mensagens de erro.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("NotificacaoId,UserId,Titulo,Mensagem,Lida,DataNotificacao")] Notificacao notificacao)
        {
            if (id != notificacao.NotificacaoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notificacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificacaoExists(notificacao.NotificacaoId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", notificacao.UserId);
            return View(notificacao);
        }

        /// <summary>
        /// Exclui uma notificação com o ID especificado.
        /// </summary>
        /// <param name="id">O ID da notificação a ser excluída.</param>
        /// <returns>Uma instância de IActionResult que representa o resultado da operação.</returns>

        public async Task<IActionResult> Delete(Guid? id)
		{
			if (id == null || _context.Notificacoes == null)
			{
				return NotFound();
			}

			var notificacao = await _context.Notificacoes
				.Include(n => n.User)
				.FirstOrDefaultAsync(m => m.NotificacaoId == id);
			if (notificacao == null)
			{
				return NotFound();
			}

			return View(notificacao);
		}

        /// <summary>
        /// Confirma a exclusão de uma notificação com o ID especificado.
        /// </summary>
        /// <param name="NotificacaoId">O ID da notificação a ser excluída.</param>
        /// <returns>Uma instância de IActionResult que representa o resultado da operação.</returns>

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid NotificacaoId)
        {
            if (_context.Notificacoes == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.Notificacoes'  is null.");
            }
            var notificacao = await _context.Notificacoes.FindAsync(NotificacaoId);
            if (notificacao != null)
            {
                _context.Notificacoes.Remove(notificacao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se uma notificação com o ID especificado existe.
        /// </summary>
        /// <param name="id">O ID da notificação a ser verificada.</param>
        /// <returns>Verdadeiro se a notificação existir; caso contrário, falso.</returns>

        private bool NotificacaoExists(Guid id)
        {
          return (_context.Notificacoes?.Any(e => e.NotificacaoId == id)).GetValueOrDefault();
        }

        /// <summary>
        /// Cria uma nova notificação para um utilizador com o título e a mensagem especificados.
        /// </summary>
        /// <param name="userId">O ID do utilizador para o qual a notificação será criada.</param>
        /// <param name="titulo">O título da notificação.</param>
        /// <param name="mensagem">A mensagem da notificação.</param>

        public async Task CreateNotification(string userId, string titulo, string mensagem)
		{
			var notificacao = new Notificacao(userId, titulo, mensagem);
			_context.Add(notificacao);
			await _context.SaveChangesAsync();
		}

        /// <summary>
        /// Obtém todas as notificações não lidas de um utilizador com o ID especificado.
        /// </summary>
        /// <param name="userId">O ID do utilizador para o qual as notificações serão obtidas.</param>
        /// <returns>Uma lista de objetos Notificacao representando as notificações não lidas do utilizador.</returns>

        public async Task<List<Notificacao>> GetUserNotifications(string userId)
        {
            return await _context.Notificacoes
                .Where(n => n.UserId == userId && !n.Lida)
                .ToListAsync();
        }

        /// <summary>
        /// Marca todas as notificações não lidas do utilizador atual como lidas.
        /// </summary>
        /// <returns>Uma instância de IActionResult que representa o resultado da operação.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var userId = currentUser.Id;

            var unreadNotifications = await _context.Notificacoes
                .Where(n => n.UserId == userId && !n.Lida)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.Lida = true;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// Exclui todas as notificações do utilizador atual.
        /// </summary>
        /// <returns>Uma instância de IActionResult que representa o resultado da operação.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var userId = currentUser.Id;

            var allNotifications = await _context.Notificacoes
                .Where(n => n.UserId == userId)
                .ToListAsync();

            _context.Notificacoes.RemoveRange(allNotifications);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
