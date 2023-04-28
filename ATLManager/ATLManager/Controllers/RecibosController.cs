using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.Models;
using ATLManager.ViewModels;
using Microsoft.AspNetCore.Identity;
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using ATLManager.Models.Historicos;

namespace ATLManager.Controllers
{
    /// <summary>
    /// Controlador para o modelo 'Recibos'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class RecibosController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly INotificacoesController _notificacoesController;

        public RecibosController(ATLManagerAuthContext context,
            UserManager<ATLManagerUser> userManager,
            IEmailSender emailSender, INotificacoesController notificacoesController)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _notificacoesController = notificacoesController;
        }

        /// <summary>
        /// Retorna uma lista de recibos com base no utilizador atual.
        /// </summary>
        /// <returns>Uma instância de IActionResult que contém uma exibição dos recibos do utilizador.</returns>

        [Authorize(Roles = "Coordenador,Funcionario")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            var recibos = await _context.Recibo
                .Include(e => e.Atl)
                .Where(e => e.AtlId == currentUserAccount.AtlId)
                .ToListAsync();

            return View(recibos);
        }

        /// <summary>
        /// Retorna uma lista de recibos para os educandos do encarregado atual.
        /// </summary>
        /// <returns>Uma instância de IActionResult que contém uma exibição dos recibos do encarregado.</returns>

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

            var recibos = new List<ReciboRespostasViewModel>();

            foreach (var educando in educandos)
            {
                var respostas = await (from resposta in _context.ReciboResposta
                                       join educandoTable in _context.Educando on resposta.EducandoId equals educandoTable.EducandoId
                                       where resposta.EducandoId == educando.EducandoId
                                       select new ReciboRespostasViewModel
                                       {
                                           RespostaId = resposta.ReciboRespostaId,
                                           ReciboId = resposta.ReciboId,
                                           EducandoName = educandoTable.Name + " " + educandoTable.Apelido,
                                           Authorized = resposta.Authorized,
                                           ResponseDate = ((DateTime)resposta.ResponseDate).ToShortDateString(),
                                           ComprovativoPath = resposta.ComprovativoPath
                                       }).ToListAsync();

                recibos = recibos.Union(respostas).ToList();
            }

            ViewData["EducandoId"] = new SelectList(educandos, "EducandoId", "Name");
			ViewBag.Educandos = educandos;
			return View(recibos);
        }
        

        /// <summary>
        /// Retorna os detalhes de um recibo com base no ID fornecido.
        /// </summary>
        /// <param name="id">O ID do recibo.</param>
        /// <returns>Uma instância de IActionResult que contém uma exibição dos detalhes do recibo.</returns>

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Recibo == null)
            {
                return NotFound();
            }

            var recibo = await _context.Recibo
                .FirstOrDefaultAsync(m => m.ReciboId == id);
            if (recibo == null)
            {
                return NotFound();
            }

            return View(recibo);
        }

        /// <summary>
        /// Retorna uma lista de respostas de recibo com base no ID do recibo.
        /// </summary>
        /// <param name="id">O ID do recibo.</param>
        /// <returns>Uma lista de respostas de recibo.</returns>

        public async Task<IActionResult> Respostas(Guid? id)
        {
            if (id == null || _context.Recibo == null)
            {
                return NotFound();
            }

            var respostas = await (from resposta in _context.ReciboResposta
                                   join educando in _context.Educando on resposta.EducandoId equals educando.EducandoId
                                   where resposta.ReciboId == id
                                   select new ReciboRespostasViewModel
                                   {
                                       RespostaId = resposta.ReciboRespostaId,
                                       ReciboId = resposta.ReciboId,
                                       EducandoName = educando.Name + " " + educando.Apelido,
                                       Authorized = resposta.Authorized,
                                       ResponseDate = ((DateTime)resposta.ResponseDate).ToShortDateString(),
                                       ComprovativoPath = resposta.ComprovativoPath
                                   }).ToListAsync();

            if (respostas == null)
            {
                return NotFound();
            }

            ViewData["ReciboId"] = id;
            return View(respostas);
        }

        /// <summary>
        /// Retorna a view para criar um novo recibo.
        /// </summary>
        /// <returns>A view para criar um novo recibo.</returns>

        public async Task<IActionResult> Create()
        {
			var currentUser = await _userManager.GetUserAsync(HttpContext.User);

			var currentUserAccount = await _context.ContaAdministrativa
				.Include(f => f.User)
				.FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

			var educandos = await _context.Educando
                .Include(e => e.Atl)
                .Where(e => e.AtlId == currentUserAccount.AtlId)
                .Select(e => new { Id = e.EducandoId, Name = e.Name + " " + e.Apelido + " (CC: " + e.CC + ")"})
                .ToListAsync();

            ViewData["Educandos"] = new SelectList(educandos, "Id", "Name");
			return View();
        }

        /// <summary>
        /// Cria um novo recibo com base nos dados fornecidos e notifica os encarregados de educação relevantes.
        /// </summary>
        /// <param name="viewModel">Os dados do recibo a serem criados.</param>
        /// <returns>O resultado da criação do recibo.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,NIB,Description,DateLimit,Educando")] ReciboCreateViewModel viewModel)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == user.Id);

			if (viewModel.DateLimit.CompareTo(DateTime.UtcNow) < 0)
			{
				var validationMessage = "Não é possível criar uma fatura com uma data limite anterior à data atual";
				ModelState.AddModelError("DateLimit", validationMessage);
			}

			if (ModelState.IsValid)
            {
				var recibo = new Recibo()
				{
                    Name = viewModel.Name,
                    Price = viewModel.Price,
                    NIB = viewModel.NIB,
                    Description = viewModel.Description,
                    DateLimit = viewModel.DateLimit,
					EmissionDate = DateTime.UtcNow.Date,
					AtlId = userAccount.AtlId
				};
				_context.Add(recibo);

                if (viewModel.Educando == null)
                {
                    var educandos = await _context.Educando
                        .Include(c => c.Atl)
                        .Where(g => g.AtlId == userAccount.AtlId)
                        .ToListAsync();

                    foreach (var educando in educandos)
                    {
                        var resposta = new ReciboResposta(recibo.ReciboId, educando.EducandoId)
                        {
                            Name = recibo.Name,
                            Price = recibo.Price,
                            NIB = recibo.NIB,
                            Description = recibo.Description,
                            DateLimit = recibo.DateLimit
                        };

                        // Obter Encarregado do Educando e a sua conta
                        var encarregado = await _context.EncarregadoEducacao
                            .FirstOrDefaultAsync(e => e.EncarregadoId == educando.EncarregadoId);
                        var encarregadoAccount = await _context.Users
                            .FirstOrDefaultAsync(e => e.Id == encarregado.UserId);

                        var userEmail = await _userManager.GetEmailAsync(encarregadoAccount);
                        var code = resposta.ReciboRespostaId.ToString();
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Action("Responder", "ReciboRespostas", new { id = resposta.ReciboRespostaId }, Request.Scheme);
                    
                        // Enviar notificação para o Encarregado de Educação
                        var notificationMessage = $"Há um novo recibo disponível para o seu educando {educando.Name} {educando.Apelido}, que pertence ao ATL {educando.Atl.Name}. Por favor, responda o mais rápido possível ao <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicar aqui</a>.";
                        var notificationTitle = $"Novo Recibo - {recibo.Name}";

                        await _emailSender.SendEmailAsync(userEmail, notificationTitle, notificationMessage);

                        await _notificacoesController.CreateNotification(encarregado.UserId, notificationTitle, notificationMessage);

                        _context.Add(resposta);
                    }
                }
                else
                {
                    var educando = await _context.Educando.
                        Include(e => e.Atl).
                        FirstOrDefaultAsync(e => e.EducandoId == (Guid)viewModel.Educando);

                    if (educando == null) return NotFound();

					var resposta = new ReciboResposta(recibo.ReciboId, educando.EducandoId)
					{
						Name = recibo.Name,
						Price = recibo.Price,
						NIB = recibo.NIB,
						Description = recibo.Description,
						DateLimit = recibo.DateLimit
					};

					// Obter Encarregado do Educando e a sua conta
					var encarregado = await _context.EncarregadoEducacao
						.FirstOrDefaultAsync(e => e.EncarregadoId == educando.EncarregadoId);
					var encarregadoAccount = await _context.Users
						.FirstOrDefaultAsync(e => e.Id == encarregado.UserId);

					var userEmail = await _userManager.GetEmailAsync(encarregadoAccount);
					var code = resposta.ReciboRespostaId.ToString();
					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
					var callbackUrl = Url.Action("Responder", "ReciboRespostas", new { id = resposta.ReciboRespostaId }, Request.Scheme);

					// Enviar notificação para o Encarregado de Educação
					var notificationMessage = $"Há um novo recibo disponível para o seu educando {educando.Name} {educando.Apelido}, que pertence ao ATL {educando.Atl.Name}. Por favor, responda o mais rápido possível ao <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicar aqui</a>.";
					var notificationTitle = $"Novo Recibo - {recibo.Name}";

					await _emailSender.SendEmailAsync(userEmail, notificationTitle, notificationMessage);

					await _notificacoesController.CreateNotification(encarregado.UserId, notificationTitle, notificationMessage);

					_context.Add(resposta);
				}

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        /// <summary>
        /// Método responsável por retornar a View de edição de um recibo.
        /// </summary>
        /// <param name="id">Id do recibo a ser editado.</param>
        /// <returns>Retorna a View de edição do recibo.</returns>
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Recibo == null)
            {
                return NotFound();
            }

            var recibo = await _context.Recibo.FindAsync(id);
            if (recibo == null)
            {
                return NotFound();
            }

            var viewModel = new ReciboEditViewModel
            {
                ReciboId = recibo.ReciboId,
                Name = recibo.Name,
                Price = recibo.Price,
                NIB = recibo.NIB,
                Description = recibo.Description,
                DateLimit = recibo.DateLimit,
            };

            return View(viewModel);
        }

        /// <summary>
        /// Método responsável por atualizar um recibo.
        /// </summary>
        /// <param name="id">Id do recibo a ser atualizado.</param>
        /// <param name="viewModel">ViewModel com os dados atualizados do recibo.</param>
        /// <returns>Retorna a View de edição do recibo caso haja algum erro de validação ou a View Index caso a atualização tenha sido bem sucedida.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ReciboId,Name,Price,NIB,Description,DateLimit")] ReciboEditViewModel viewModel)
        {
            if (id != viewModel.ReciboId)
            {
                return NotFound();
            }

            if (viewModel.DateLimit != null)
            {
                if (((DateTime)viewModel.DateLimit).CompareTo(DateTime.UtcNow) < 0)
                {
                    var validationMessage = "Não é possível criar uma fatura com uma data limite anterior à data atual";
                    ModelState.AddModelError("DateLimit", validationMessage);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var recibo = await _context.Recibo.FindAsync(id);

                    if (recibo == null) return NotFound();

                    if (viewModel.Name != recibo.Name) 
                        recibo.Name = viewModel.Name;

                    if (viewModel.Price != recibo.Price) 
                        recibo.Price = viewModel.Price;

                    if (viewModel.NIB != recibo.NIB) 
                        recibo.NIB = viewModel.NIB;

                    if (viewModel.Description != recibo.Description) 
                        recibo.Description = viewModel.Description;
                    
                    if (viewModel.DateLimit != null && viewModel.DateLimit != recibo.DateLimit) 
                        recibo.DateLimit = (DateTime)viewModel.DateLimit;

					_context.Update(recibo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReciboExists(viewModel.ReciboId))
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
        /// Método de ação que retorna a view para excluir um recibo com o ID especificado.
        /// </summary>
        /// <param name="id">O ID do recibo a ser excluído.</param>
        /// <returns>A view para excluir o recibo com o ID especificado.</returns>


        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Recibo == null)
            {
                return NotFound();
            }

            var recibo = await _context.Recibo
                .FirstOrDefaultAsync(m => m.ReciboId == id);
            if (recibo == null)
            {
                return NotFound();
            }

            return View(recibo);
        }

        /// <summary>
        /// Método de ação que exclui permanentemente um recibo com o ID especificado.
        /// </summary>
        /// <param name="id">O ID do recibo a ser excluído.</param>
        /// <returns>Uma ação redirecionando para a view Index.</returns>

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Recibo == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.Recibo'  is null.");
            }

            var recibo = await _context.Recibo
                .Include(f => f.Atl)
                .FirstOrDefaultAsync(m => m.ReciboId == id);
            if (recibo != null)
            {
                var record = new ReciboRecord()
                {
                    Name = recibo.Name,
                    Price = recibo.Price,
                    NIB = recibo.NIB,
                    Description = recibo.Description,
                    EmissionDate = recibo.EmissionDate.Date,
                    DateLimit = recibo.DateLimit,
                    AtlId = recibo.AtlId,
                };

                var respostas = await _context.ReciboResposta
                    .Include(r => r.Recibo)
                    .Include(r => r.Educando)
                    .Where(r => r.ReciboId == recibo.ReciboId)
                    .ToListAsync();

                foreach (var resposta in respostas)
                {
                    var respostaRecord = new ReciboRespostaRecord()
                    {
                        ReciboRecordId = record.ReciboRecordId,
                        Educando = resposta.Educando.Name + " " + resposta.Educando.Apelido,
                        Name = resposta.Name, 
                        Price = resposta.Price,
                        NIB = resposta.NIB,
                        Description = resposta.Description,
                        DateLimit = ((DateTime)resposta.DateLimit).Date,
                        Authorized = resposta.Authorized,
                        ResponseDate = (resposta.ResponseDate == null) ? null : ((DateTime)resposta.ResponseDate).Date,
                        ComprovativoPath = resposta.ComprovativoPath,
                        ReceiptPath = resposta.ReceiptPath,
                        Notes = resposta.Notes,
                    };

                    _context.Add(respostaRecord);
                };

                _context.Add(record);
                _context.Recibo.Remove(recibo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se um recibo com o ID especificado existe.
        /// </summary>
        /// <param name="id">O ID do recibo a ser verificado.</param>
        /// <returns>Verdadeiro se o recibo existir, falso caso contrário.</returns>

        private bool ReciboExists(Guid id)
        {
          return (_context.Recibo?.Any(e => e.ReciboId == id)).GetValueOrDefault();
        }
    }
}
