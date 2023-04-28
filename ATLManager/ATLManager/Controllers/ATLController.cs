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
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace ATLManager.Controllers
{
    /// <summary>
    /// Controlador para o modelo 'ATL'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class ATLController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly List<string> allowedPrefixesNIPC = new() { "5", "6", "7", "8", "9" };

		public ATLController(ATLManagerAuthContext context,
            UserManager<ATLManagerUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Retorna a view Index com uma lista de todos os ATLs pertencentes à conta administrativa.
        /// </summary>
        /// <returns>View com a lista de ATLs.</returns>

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

            var atls = await (from atl in _context.ATL
                              join atlAdmin in _context.ATLAdmin on atl.AtlId equals atlAdmin.AtlId
                              join admin in _context.ContaAdministrativa on atlAdmin.ContaId equals admin.ContaId
                              where admin.ContaId == userAccount.ContaId
                              select atl).Include(a => a.Agrupamento).ToListAsync();

            return View(atls);
        }

        /// <summary>
        /// Retorna a view Details com os detalhes do ATL especificado pelo parametro id.
        /// </summary>
        /// <param name="id">O id do ATL para o qual se deseja visualizar os detalhes.</param>
        /// <returns>View com os detalhes do ATL.</returns>

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.ATL == null)
            {
                return NotFound();
            }

            var atl = await _context.ATL
                .Include(a => a.Agrupamento)
                .FirstOrDefaultAsync(m => m.AtlId == id);

            if (atl == null)
            {
                return NotFound();
            }

            return View(atl);
        }

        /// <summary>
        /// Retorna a view Create com um formulário para criar um novo ATL.
        /// </summary>
        /// <returns>View com o formulário para criar um novo ATL.</returns>

        public async Task<IActionResult> Create()
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

            ViewData["AgrupamentoId"] = new SelectList(agrupamentos, "AgrupamentoID", "Name");
            return View(new ATLCreateViewModel());
        }

        /// <summary>
        /// Cria um novo ATL com base nos dados fornecidos pelo objeto viewModel.
        /// </summary>
        /// <param name="viewModel">Objeto que contém os dados para criar um novo ATL.</param>
        /// <returns>Redireciona para a view Index após a criação do novo ATL.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ATLCreateViewModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.NIPC) && string.IsNullOrEmpty(viewModel.AgrupamentoId.ToString()))
            {
                var validationMessage = "É necessário introduzir um NIPC ou Agrupamento";
                ModelState.AddModelError("NIPC", validationMessage);
                ModelState.AddModelError("AtlId", validationMessage);
            }

            if (!string.IsNullOrEmpty(viewModel.NIPC) && !string.IsNullOrEmpty(viewModel.AgrupamentoId.ToString()))
            {
                var validationMessage = "Apenas permitido introduzir um NIPC ou um Agrupamento";
                ModelState.AddModelError("NIPC", validationMessage);
                ModelState.AddModelError("AtlId", validationMessage);
            }

            if (!string.IsNullOrEmpty(viewModel.NIPC))
            {
				if (!allowedPrefixesNIPC.Contains(viewModel.NIPC.Trim().Substring(0, 1)))
				{
					var validationMessage = "NIPC requer que o primeiro dígito seja 5, 6, 7, 8 ou 9.";
					ModelState.AddModelError("NIPC", validationMessage);
				}

				if (_context.ATL.Any(a => a.NIPC == viewModel.NIPC)
                    || _context.Agrupamento.Any(a => a.NIPC == viewModel.NIPC))
                {
                    var validationMessage = "Outro ATL/Agrupamento já contém este NIPC";
                    ModelState.AddModelError("NIPC", validationMessage);
                }
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == user.Id);

            if (ModelState.IsValid)
            {
				var atl = new ATL
				{
					AtlId = Guid.NewGuid(),
					Name = viewModel.Name,
					Address = viewModel.Address,
                    City = viewModel.City,
                    PostalCode = viewModel.PostalCode,
					AgrupamentoId = viewModel.AgrupamentoId,
                    NIPC = viewModel.NIPC
				};

                string fileName = UploadedFile(viewModel.LogoPicture);
                if (fileName != null)
                {
                    atl.LogoPicture = fileName;
                }
                else
                {
                    atl.LogoPicture = "logo.png";
                }

                var atlAdmin = new ATLAdmin
                {
                    AtlId = atl.AtlId,
                    ContaId = userAccount.ContaId
                };

                _context.Add(atl);
                _context.Add(atlAdmin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            if (userAccount == null)
            {
                return NotFound();
            }

            var agrupamentos = await _context.Agrupamento
                    .Where(g => g.ContaId == userAccount.ContaId)
                    .ToListAsync();

            ViewData["AgrupamentoId"] = new SelectList(agrupamentos, "AgrupamentoID", "Name");

            return View(viewModel);
        }

        /// <summary>
        /// Edita o ATL com o ID fornecido.
        /// </summary>
        /// <param name="id">O ID do ATL a ser editado.</param>

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.ATL == null)
            {
                return NotFound();
            }

            var atl = await _context.ATL.FindAsync(id);
            if (atl == null)
            {
                return NotFound();
            }

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

            ViewData["AgrupamentoId"] = new SelectList(agrupamentos, "AgrupamentoID", "Name", atl.AgrupamentoId);
            return View(new ATLEditViewModel(atl));
        }

        /// <summary>
        /// Edita o ATL com o ID fornecido.
        /// </summary>
        /// <param name="id">O ID do ATL a ser editado.</param>
        /// <param name="viewModel">O modelo de visualização de edição do ATL.</param>
        /// <returns>O resultado da ação.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ATLEditViewModel viewModel)
        {
            if (id != viewModel.AtlId)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(viewModel.NIPC) && string.IsNullOrEmpty(viewModel.AgrupamentoId.ToString()))
            {
                var validationMessage = "É necessário introduzir um NIPC ou ATLId";
                ModelState.AddModelError("NIPC", validationMessage);             
                ModelState.AddModelError("AtlId", validationMessage);
            }
            
            if (!string.IsNullOrEmpty(viewModel.NIPC) && !string.IsNullOrEmpty(viewModel.AgrupamentoId.ToString()))
            {
                var validationMessage = "Apenas permitido introduzir um NIPC ou um ATLId";
                ModelState.AddModelError("NIPC", validationMessage);
                ModelState.AddModelError("AtlId", validationMessage);
            }

            if (!string.IsNullOrEmpty(viewModel.NIPC))
            {
				if (!allowedPrefixesNIPC.Contains(viewModel.NIPC.Trim().Substring(0, 1)))
				{
					var validationMessage = "NIPC requer que o primeiro dígito seja 5, 6, 7, 8 ou 9.";
					ModelState.AddModelError("NIPC", validationMessage);
				}

				var ATL = _context.ATL.Find(viewModel.AtlId);
                
				if (ATL.NIPC != viewModel.NIPC &&
					_context.ATL.Any(a => a.NIPC == viewModel.NIPC)
                    || _context.Agrupamento.Any(a => a.NIPC == viewModel.NIPC))
                {
                    var validationMessage = "Outro ATL/Agrupamento já contém este NIPC";
                    ModelState.AddModelError("NIPC", validationMessage);
                }
			}

            if (ModelState.IsValid)
            {
                try
                {
                    var atl = await _context.ATL.FindAsync(id);

                    if (atl != null)
                    {
                        atl.Name = viewModel.Name;
                        atl.Address = viewModel.Address;
                        atl.City = viewModel.City;
                        atl.PostalCode = viewModel.PostalCode;
                        atl.AgrupamentoId = viewModel.AgrupamentoId;
                        atl.NIPC = viewModel.NIPC;

                        string fileName = UploadedFile(viewModel.LogoPicture);
                        if (fileName != null)
                        {
                            atl.LogoPicture = fileName;
                        }

                        _context.Update(atl);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ATLExists(viewModel.AtlId))
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

            ViewData["AgrupamentoId"] = new SelectList(agrupamentos, "AgrupamentoID", "Name", viewModel.AgrupamentoId);
            return View(viewModel);
        }

        /// <summary>
        /// Obtém o ATL com o ID fornecido para exclusão.
        /// </summary>
        /// <param name="id">O ID do ATL a ser excluído.</param>
        /// <returns>O resultado da ação.</returns>

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.ATL == null)
            {
                return NotFound();
            }

            var atl = await _context.ATL
                .Include(a => a.Agrupamento)
                .FirstOrDefaultAsync(m => m.AtlId == id);
            if (atl == null)
            {
                return NotFound();
            }

            return View(atl);
        }

        /// <summary>
        /// Confirma a exclusão do ATL com o ID fornecido.
        /// </summary>
        /// <param name="id">O ID do ATL a ser excluído.</param>
        /// <returns>O resultado da ação.</returns>

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.ATL == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.ATL'  is null.");
            }
            var atl = await _context.ATL.FindAsync(id);
            if (atl != null)
            {
                _context.ATL.Remove(atl);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ATLExists(Guid id)
        {
          return _context.ATL.Any(e => e.AtlId == id);
        }

        /// <summary>
        /// Método responsável por fazer o upload do arquivo enviado e retornar o nome único do arquivo gerado.
        /// </summary>
        /// <param name="logoPicture">Objeto IFormFile que contém as informações do arquivo enviado.</param>
        /// <returns>O nome único do arquivo gerado, ou null caso o objeto IFormFile seja nulo.</returns>
        private string UploadedFile(IFormFile logoPicture)
		{
			string uniqueFileName = null;

			if (logoPicture != null)
			{
				string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/uploads/atls");
				uniqueFileName = Guid.NewGuid().ToString() + "_" + logoPicture.FileName;
				string filePath = Path.Combine(uploadsFolder, uniqueFileName);
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					logoPicture.CopyTo(fileStream);
				}
			}
			return uniqueFileName;
		}
	}
}
