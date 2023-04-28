using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using ATLManager.Services;

namespace ATLManager.Controllers
{
    /// <summary>
    /// Controlador para o modelo 'Resposta de Recibos'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class ReciboRespostasController : Controller
    {
        private readonly ATLManagerAuthContext _context;
		private readonly UserManager<ATLManagerUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly INotificacoesController _notificacoesController;
        private readonly IFileManager _fileManager;

        private readonly string FolderName = "recibos";

        public ReciboRespostasController(ATLManagerAuthContext context, 
            IWebHostEnvironment webHostEnvironment, 
			INotificacoesController notificacoesController, 
            UserManager<ATLManagerUser> userManager,
			RoleManager<IdentityRole> roleManager,
			IFileManager fileManager)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
			_notificacoesController = notificacoesController;
			_userManager = userManager;
			_roleManager = roleManager;
			_fileManager = fileManager;
		}

        /// <summary>
        /// Obtém os detalhes de uma resposta a um recibo específico.
        /// </summary>
        /// <param name="id">O identificador da resposta a ser exibida.</param>
        /// <returns>Retorna uma exibição de detalhes da resposta.</returns>

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.ReciboResposta == null)
            {
                return NotFound();
            }

            var resposta = await _context.ReciboResposta
                .Include(f => f.Educando)
                .Include(f => f.Recibo)
                .FirstOrDefaultAsync(m => m.ReciboRespostaId == id);
            if (resposta == null)
            {
                return NotFound();
            }

            return View(resposta);
		}

        /// <summary>
        /// Obtém a resposta a um recibo para edição.
        /// </summary>
        /// <param name="id">O identificador da resposta a ser editada.</param>
        /// <returns>Retorna uma exibição da resposta com dados editáveis.</returns>

        public async Task<IActionResult> Edit(Guid? id)
		{
			if (id == null || _context.ReciboResposta == null)
			{
				return NotFound();
			}

			var resposta = await _context.ReciboResposta
				.Include(f => f.Educando)
				.Include(f => f.Recibo)
				.FirstOrDefaultAsync(m => m.ReciboRespostaId == id);
			if (resposta == null)
			{
				return NotFound();
			}

            var viewModel = new ReciboRespostaEditViewModel()
            {
				RespostaId = resposta.ReciboRespostaId,
			    ReciboId = resposta.ReciboId,
			    Educando = resposta.Educando.Name + " " + resposta.Educando.Apelido,
			    Name = resposta.Name,
			    Price = resposta.Price,
			    NIB = resposta.NIB,
			    Description = resposta.Description,
			    ResponseDate = (resposta.ResponseDate != null) ? resposta.ResponseDate.ToString() : "-",
			    DateLimit = resposta.DateLimit.ToShortDateString(),
			    ComprovativoPath = resposta.ComprovativoPath,
			    Authorized = resposta.Authorized,
			    Notes = resposta.Notes
		    };

			return View(viewModel);
		}

        /// <summary>
        /// Salva as alterações feitas em uma resposta a um recibo.
        /// </summary>
        /// <param name="id">O identificador da resposta a ser atualizada.</param>
        /// <param name="viewModel">O modelo de exibição da resposta editada.</param>
        /// <returns>Retorna uma exibição da lista de respostas ao recibo.</returns>

        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid id, ReciboRespostaEditViewModel viewModel)
		{
			if (id != viewModel.RespostaId)
			{
				return NotFound();
			}

            ModelState.Remove("Recibo");
            ModelState.Remove("Educando");
            ModelState.Remove("Name");
            ModelState.Remove("Price");
            ModelState.Remove("NIB");
            ModelState.Remove("DateLimit");
            ModelState.Remove("Description");
            ModelState.Remove("ComprovativoPath");
            ModelState.Remove("ResponseDate");
			ModelState.Remove("DateLimit");

			if (ModelState.IsValid)
			{
				try
				{
                    var resposta = await _context.ReciboResposta.FindAsync(id);

                    if (resposta == null) return NotFound();

                    resposta.Authorized = viewModel.Authorized;
                    resposta.Notes = viewModel.Notes;
                    
                    if (viewModel.Receipt != null)
					    resposta.ReceiptPath = _fileManager.UploadFile(viewModel.Receipt, FolderName);

					_context.Update(resposta);
					await _context.SaveChangesAsync();

					// Enviar notificações para os usuários relevantes (Coordenadores, Funcionários) do ATL específico
					var roleNames = new[] { "Coordenador", "Funcionario" };

					var currentUser = await _userManager.GetUserAsync(HttpContext.User);
					var currentUserAccount = await _context.ContaAdministrativa
						.Include(f => f.User)
						.FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

					Guid specificATLId = (Guid)currentUserAccount.AtlId;

					var usersToNotify = await (from user in _context.Users
											   join userRole in _context.UserRoles on user.Id equals userRole.UserId
											   join role in _context.Roles on userRole.RoleId equals role.Id
											   join account in _context.ContaAdministrativa on user.Id equals account.UserId
											   where roleNames.Contains(role.Name) && account.AtlId == specificATLId
											   select user).ToListAsync();

					foreach (var user in usersToNotify)
					{
						await _notificacoesController.CreateNotification(user.Id, "Novo Recibo", "Uma resposta ao recibo foi adicionada ou atualizada.");
					}
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ReciboRespostaExists(viewModel.RespostaId))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction("Respostas", "Recibos", new { id = viewModel.ReciboId });
			}
			return View(viewModel);
		}

        /// <summary>
        /// Obtém a resposta a um recibo para ser respondida por um Encarregado de Educação.
        /// </summary>
        /// <param name="id">O identificador da resposta a ser respondida.</param>
        /// <returns>Retorna uma exibição da resposta com formulário para ser preenchido pelo Encarregado de Educação.</returns>

        [Authorize(Roles = "EncarregadoEducacao")]
		public async Task<IActionResult> Responder(Guid? id)
        {
            if (id == null || _context.ReciboResposta == null)
            {
                return NotFound();
            }

			var resposta = await _context.ReciboResposta
				.Include(f => f.Educando)
				.Include(f => f.Recibo)
				.FirstOrDefaultAsync(m => m.ReciboRespostaId == id);
			if (resposta == null)
            {
                return NotFound();
            }

            var recibo = await _context.Recibo.FindAsync(resposta.ReciboId);
            var viewModel = new ReciboResponderViewModel
            {
                ReciboRespostaId = resposta.ReciboRespostaId,
                Name = recibo.Name,
                Educando = resposta.Educando.Name + " " + resposta.Educando.Apelido,
				NIB = recibo.NIB,
                Price = recibo.Price,
                Description = recibo.Description,
                DateLimit = recibo.DateLimit.ToShortDateString()
            };

            return View(viewModel);
        }

        /// <summary>
        /// Método para responder um recibo.
        /// </summary>
        /// <param name="id">o ID do recibo a ser respondido.</param>
        /// <param name="viewModel">o ViewModel que contém as informações da resposta.</param>
        /// <returns>um objeto Task<IActionResult>.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Responder(Guid id, ReciboResponderViewModel viewModel)
        {
            if (id != viewModel.ReciboRespostaId)
            {
                return NotFound();
            }

            ModelState.Remove("Name");
            ModelState.Remove("Price");
            ModelState.Remove("NIB");
			ModelState.Remove("Educando");
			ModelState.Remove("Description");
            ModelState.Remove("DateLimit");

            if (ModelState.IsValid)
            {
                try
                {
                    var reciboResposta = await _context.ReciboResposta.FindAsync(id);

                    if (reciboResposta == null) 
                        return NotFound();

                    reciboResposta.ComprovativoPath = _fileManager.UploadFile(viewModel.Comprovativo, FolderName);
                    reciboResposta.ResponseDate = DateTime.UtcNow.Date;

					_context.Update(reciboResposta);
                    await _context.SaveChangesAsync();

					// Enviar notificações para os usuários relevantes (Coordenadores, Funcionários) do ATL específico
					var roleNames = new[] { "Coordenador", "Funcionario" };

                    // Enviar notificação aos coordenadores e funcionários
                    var educando = await _context.Educando.FirstOrDefaultAsync(e => e.EducandoId == reciboResposta.EducandoId);


                    var currentUser = await _userManager.GetUserAsync(HttpContext.User);

					Guid specificATLId = (Guid)educando.AtlId;

                    // Get the Recibo Name
                    var recibo = await _context.Recibo.FirstOrDefaultAsync(r => r.ReciboId == reciboResposta.ReciboId);
                    string reciboName = recibo.Name;

                    // Update the notification title and message
                    string notificationTitle = $"Nova Resposta ao Recibo - {reciboName}";
                    string notificationMessage = $"Uma resposta ao recibo foi adicionada ou atualizada. <a href='/ReciboRespostas/Details/{reciboResposta.ReciboRespostaId}'>Clique aqui</a> para ver";


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
                    if (!ReciboRespostaExists(viewModel.ReciboRespostaId))
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
        /// Retorna a visualização da página de agradecimento.
        /// </summary>
        /// <returns>Visualização da página de agradecimento.</returns>

        public IActionResult Obrigado()
		{
			return View();
		}

        /// <summary>
        /// Faz o download do arquivo especificado e envia ao navegador.
        /// </summary>
        /// <param name="fileName">Nome do arquivo a ser baixado.</param>
        /// <returns>Arquivo para download.</returns>

        public IActionResult Download(string fileName)
        {
			string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "files/users/recibos");
			string filePath = Path.Combine(uploadsFolder, fileName);
            string fileCleanName = fileName.Substring(fileName.IndexOf("_id_") + 4);
			return File(System.IO.File.ReadAllBytes(filePath), "application/pdf", fileCleanName);
        }

        /// <summary>
        /// Verifica se existe um objeto ReciboResposta com o ID especificado.
        /// </summary>
        /// <param name="id">ID do objeto ReciboResposta a ser procurado.</param>
        /// <returns>True se o objeto existir, caso contrário False.</returns>

        private bool ReciboRespostaExists(Guid id)
        {
          return (_context.ReciboResposta?.Any(e => e.ReciboRespostaId == id)).GetValueOrDefault();
        }

        /// <summary>
        /// Salva o arquivo enviado pelo usuário no servidor e retorna o nome do arquivo gerado.
        /// </summary>
        /// <param name="comprovativo">Arquivo enviado pelo utilizador.</param>
        /// <returns>Nome do arquivo gerado.</returns>

        private string UploadedFile(IFormFile comprovativo)
        {
            string uniqueFileName = null;

            if (comprovativo != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "files/users/recibos");
                uniqueFileName = Guid.NewGuid().ToString() + "_id_" + comprovativo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
					comprovativo.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
