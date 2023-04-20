using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using ATLManager.Models;
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using MessagePack;
using NuGet.Configuration;

namespace ATLManager.Controllers
{
    public class ReciboRespostasController : Controller
    {
        private readonly ATLManagerAuthContext _context;
		private readonly UserManager<ATLManagerUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly INotificacoesController _notificacoesController;


        public ReciboRespostasController(ATLManagerAuthContext context, 
            IWebHostEnvironment webHostEnvironment, 
			INotificacoesController notificacoesController, 
            UserManager<ATLManagerUser> userManager,
			RoleManager<IdentityRole> roleManager)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
			_notificacoesController = notificacoesController;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		// GET: RecibosRespostas/Details/5
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

		// GET: RecibosRespostas/Edit/5
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

            if (ModelState.IsValid)
			{
				try
				{
                    var resposta = await _context.ReciboResposta.FindAsync(id);

                    if (resposta == null) return NotFound();

                    resposta.Authorized = viewModel.Authorized;
                    resposta.Notes = viewModel.Notes;
                    
                    if (viewModel.Receipt != null)
					    resposta.ReceiptPath = UploadedFile(viewModel.Receipt);

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

		// GET: RecibosRespostas/Edit/5
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

		// POST: RecibosRespostas/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

                    reciboResposta.ComprovativoPath = UploadedFile(viewModel.Comprovativo);
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

		public IActionResult Obrigado()
		{
			return View();
		}

        public IActionResult Download(string fileName)
        {
			string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "files/users/recibos");
			string filePath = Path.Combine(uploadsFolder, fileName);
            string fileCleanName = fileName.Substring(fileName.IndexOf("_id_") + 4);
			return File(System.IO.File.ReadAllBytes(filePath), "application/pdf", fileCleanName);
        }

		private bool ReciboRespostaExists(Guid id)
        {
          return (_context.ReciboResposta?.Any(e => e.ReciboRespostaId == id)).GetValueOrDefault();
        }

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
