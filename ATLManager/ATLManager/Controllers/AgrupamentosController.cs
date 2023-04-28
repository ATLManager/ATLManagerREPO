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
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using ATLManager.Services;

namespace ATLManager.Controllers
{
    /// <summary>
    /// Controlador para o modelo 'Agrupamentos'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class AgrupamentosController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly IFileManager _fileManager;

        private readonly string FolderName = "agrupamentos";
        private readonly List<string> allowedPrefixesNIPC = new() { "5", "6", "7", "8", "9" };

		public AgrupamentosController(ATLManagerAuthContext context,
            UserManager<ATLManagerUser> userManager,
            IFileManager fileManager)
        {
            _context = context;
            _userManager = userManager;
            _fileManager = fileManager;
        }

        /// <summary>
        /// Exibe a página inicial dos agrupamentos para um utilizador autenticado.
        /// </summary>
        /// <returns>Uma ActionResult que retorna a página Index com uma lista de agrupamentos.</returns>

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == user.Id);

            if (userAccount == null)
            {
                return NotFound();
            }

            var agrupamentos = await _context.Agrupamento
                    .Where(g => g.ContaId == userAccount.ContaId)
                    .ToListAsync();
            
            return View(agrupamentos);
        }

        /// <summary>
        /// Exibe os detalhes de um agrupamento específico.
        /// </summary>
        /// <param name="id">O ID do agrupamento.</param>
        /// <returns>Uma ActionResult que retorna a página Details com os detalhes do agrupamento.</returns>

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Agrupamento == null)
            {
                return NotFound();
            }

            var agrupamento = await _context.Agrupamento
                .FirstOrDefaultAsync(m => m.AgrupamentoID == id);
            if (agrupamento == null)
            {
                return NotFound();
            }

            return View(agrupamento);
        }

        /// <summary>
        /// Exibe a página de criação de um novo agrupamento.
        /// </summary>
        /// <returns>Uma ActionResult que retorna a página Create com um novo AgrupamentoCreateViewModel.</returns>

        public IActionResult Create()
        {
            return View(new AgrupamentoCreateViewModel());
        }

        /// <summary>
        /// Cria um novo agrupamento com base nos dados fornecidos pelo utilizador.
        /// </summary>
        /// <param name="viewModel">Um AgrupamentoCreateViewModel que contém os dados do novo agrupamento.</param>
        /// <returns>Uma ActionResult que redireciona para a página Index se o novo agrupamento for criado com sucesso.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AgrupamentoCreateViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.NIPC))
            {
                if (!allowedPrefixesNIPC.Contains(viewModel.NIPC.Trim().Substring(0, 1)))
                {
                    var validationMessage = "NIPC requer que o primeiro dígito seja 5, 6, 7, 8 ou 9.";
                    ModelState.AddModelError("NIPC", validationMessage);
                }

				if (_context.Agrupamento.Any(a => a.NIPC == viewModel.NIPC))
				{
					var validationMessage = "Outro Agrupamento já contém este NIPC";
					ModelState.AddModelError("NIPC", validationMessage);
				}
			}

			if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var userAccount = await _context.ContaAdministrativa
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(m => m.UserId == user.Id);

                var agrupamento = new Agrupamento
                {
                    AgrupamentoID = Guid.NewGuid(),
                    Name = viewModel.Name,
                    Location = viewModel.Location,
                    NIPC = viewModel.NIPC,
                    ContaId = userAccount?.ContaId
                };

                string fileName = _fileManager.UploadFile(viewModel.LogoPicture, FolderName);

                if (fileName != null)
                {
                    agrupamento.LogoPicture = fileName;
                } 
                else
                {
                    agrupamento.LogoPicture = "logo.png";
                }

                _context.Add(agrupamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        /// <summary>
        /// Retorna a view de edição de um agrupamento com o ID especificado.
        /// </summary>
        /// <param name="id">O ID do agrupamento a ser editado.</param>
        /// <returns>A view de edição de um agrupamento.</returns>

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Agrupamento == null)
            {
                return NotFound();
            }

            var agrupamento = await _context.Agrupamento.FindAsync(id);
            if (agrupamento == null)
            {
                return NotFound();
            }
            return View(new AgrupamentoEditViewModel(agrupamento));
        }

        /// <summary>
        /// Atualiza um agrupamento com as informações fornecidas pelo view model.
        /// </summary>
        /// <param name="id">O ID do agrupamento a ser atualizado.</param>
        /// <param name="viewModel">O view model com as informações a serem atualizadas.</param>
        /// <returns>A view de edição de um agrupamento se o modelo for inválido; caso contrário, redireciona para a página inicial de agrupamentos.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AgrupamentoEditViewModel viewModel)
        {
            if (id != viewModel.AgrupamentoId)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(viewModel.NIPC))
            {
				if (!allowedPrefixesNIPC.Contains(viewModel.NIPC.Trim().Substring(0, 1)))
				{
					var validationMessage = "NIPC requer que o primeiro dígito seja 5, 6, 7, 8 ou 9.";
					ModelState.AddModelError("NIPC", validationMessage);
				}

				var agrupamento = _context.Agrupamento.Find(viewModel.AgrupamentoId);
				if (agrupamento.NIPC != viewModel.NIPC &&
					_context.Agrupamento.Any(a => a.NIPC == viewModel.NIPC))
				{
					var validationMessage = "Outro Agrupamento já contém este NIPC";
					ModelState.AddModelError("NIPC", validationMessage);
				}
			}

            if (ModelState.IsValid)
            {
                try
                {
                    var agrupamento = await _context.Agrupamento.FindAsync(id);

                    if (agrupamento != null)
                    {
                        agrupamento.Name = viewModel.Name;
                        agrupamento.Location = viewModel.Location;
                        agrupamento.NIPC = viewModel.NIPC;

					    string fileName = _fileManager.UploadFile(viewModel.LogoPicture, FolderName);
                        if (fileName != null)
                        {
                            agrupamento.LogoPicture = fileName;
                        }

					    _context.Update(agrupamento);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgrupamentoExists(viewModel.AgrupamentoId))
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
        /// Remove o agrupamento com o id fornecido.
        /// </summary>
        /// <param name="id">O id do agrupamento a ser removido.</param>
        /// <returns>Uma instância de IActionResult.</returns>

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Agrupamento == null)
            {
                return NotFound();
            }

            var agrupamento = await _context.Agrupamento
                .FirstOrDefaultAsync(m => m.AgrupamentoID == id);
            if (agrupamento == null)
            {
                return NotFound();
            }

            return View(agrupamento);
        }

        /// <summary>
        /// Confirma a remoção do agrupamento com o id fornecido.
        /// </summary>
        /// <param name="id">O id do agrupamento a ser removido.</param>
        /// <returns>Uma instância de IActionResult.</returns>

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Agrupamento == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.Agrupamento'  is null.");
            }
            var agrupamento = await _context.Agrupamento.FindAsync(id);
            if (agrupamento != null)
            {
                _context.Agrupamento.Remove(agrupamento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se o agrupamento com o id fornecido existe.
        /// </summary>
        /// <param name="id">O id do agrupamento a ser verificado.</param>
        /// <returns>Um valor booleano que indica se o agrupamento existe.</returns>

        private bool AgrupamentoExists(Guid id)
        {
            return _context.Agrupamento.Any(e => e.AgrupamentoID == id);
        }


        /// <summary>
        /// Faz upload do arquivo especificado para a pasta de uploads do agrupamento.
        /// </summary>
        /// <param name="logoPicture">O arquivo a ser enviado.</param>
        /// <returns>O nome único do arquivo enviado.</returns>

        private string UploadedFile(IFormFile logoPicture)
		{
			string uniqueFileName = null;

			if (logoPicture != null)
			{
				string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/uploads/agrupamentos");
				uniqueFileName = Guid.NewGuid().ToString() + "_" + logoPicture.FileName;
				string filePath = Path.Combine(uploadsFolder, uniqueFileName);
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					logoPicture.CopyTo(fileStream);
				}
			}
			return uniqueFileName;
		}

        /// <summary>
        /// Obtém todas as ATLs associadas ao agrupamento especificado.
        /// </summary>
        /// <param name="agrupamentoId">O id do agrupamento.</param>
        /// <returns>Uma instância de IActionResult que representa as ATLs.</returns>

        [HttpGet]
		public async Task<IActionResult> GetATLsByAgrupamento(Guid agrupamentoId)
		{
			var atls = await _context.ATL
				.Where(a => a.AgrupamentoId == agrupamentoId)
				.Select(a => new
				{
					a.AtlId,
					a.Name,
					a.Address,
					a.City,
					a.PostalCode
				})
				.ToListAsync();

			return Json(atls);
		}

        /// <summary>
        /// Obtém todos os coordenadores associados à ATL especificada.
        /// </summary>
        /// <param name="atlId">O id da ATL.</param>
        /// <returns>Uma instância de IActionResult que representa os coordenadores.</returns>

        [HttpGet]
		public async Task<IActionResult> GetCoordenadoresByATL(Guid atlId)
		{
			var coordenadores = await _context.ContaAdministrativa
				.Include(ca => ca.User)
				.Where(ca => ca.AtlId == atlId)
				.Select(ca => new {
                    ca.User.FirstName,
					ca.User.LastName
				})
				.ToListAsync();

			return Json(new { Coordenadores = coordenadores });
		}
	}
}
