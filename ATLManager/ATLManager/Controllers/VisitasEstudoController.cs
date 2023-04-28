using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.Models;
using Microsoft.AspNetCore.Hosting;
using ATLManager.ViewModels;
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using ATLManager.Models.Historicos;
using ATLManager.Services;

namespace ATLManager.Controllers
{
    /// <summary>
    /// Controlador para o modelo 'Visitas de Estudo'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class VisitasEstudoController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly IFileManager _fileManager;

        private readonly string FolderName = "visitas";

        public VisitasEstudoController(ATLManagerAuthContext context,
            UserManager<ATLManagerUser> userManager,
            IFileManager fileManager
            )
        {
            _context = context;
            _userManager = userManager;
            _fileManager = fileManager;
        }

        /// <summary>
        /// Retorna uma lista de visitas de estudo do ATL correspondente ao utilizador atual.
        /// </summary>
        /// <returns>Uma vista com a lista de visitas de estudo.</returns>
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            
            if (HttpContext.User.IsInRole("Coordenador") || HttpContext.User.IsInRole("Funcionario"))
            { 
                var currentUserAccount = await _context.ContaAdministrativa
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

                var visitas = await _context.VisitaEstudo
                    .Include(a => a.Atl)
                    .Where(r => r.AtlId == currentUserAccount.AtlId)
                    .ToListAsync();

                return View(visitas);
            }
            else
            {
                var currentUserAccount = await _context.EncarregadoEducacao
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

                var educandos = await _context.Educando
                    .Include(e => e.Atl)
                    .Include(e => e.Encarregado)
                    .Where(e => e.EncarregadoId == currentUserAccount.EncarregadoId)
                    .ToListAsync();

                var visitas = new List<VisitaEstudo>();

                foreach (var educando in educandos)
                {
				    var tempVisitas = await _context.VisitaEstudo
					    .Include(a => a.Atl)
					    .Where(r => r.AtlId == educando.AtlId)
					    .ToListAsync();

                    visitas = visitas.Union(tempVisitas).ToList();
                }

				ViewData["EducandoId"] = new SelectList(educandos, "EducandoId", "Name");
				return View(visitas);
            }
        }

        /// <summary>
        /// Retorna os detalhes de uma visita de estudo com base no ID especificado.
        /// </summary>
        /// <param name="id">O ID da visita de estudo.</param>
        /// <returns>Uma vista com os detalhes da visita de estudo.</returns>

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.VisitaEstudo == null)
            {
                return NotFound();
            }

            var visitaEstudo = await _context.VisitaEstudo
                .FirstOrDefaultAsync(m => m.VisitaEstudoID == id);
            if (visitaEstudo == null)
            {
                return NotFound();
            }

            return View(visitaEstudo);
        }

        /// <summary>
        /// Exibe o formulário de criação de uma nova visita de estudo.
        /// </summary>
        /// <returns>Uma vista com o formulário de criação de visita de estudo.</returns>

        public IActionResult Create()
        {
            return View(new VisitaEstudoCreateViewModel());
        }

        /// <summary>
        /// Cria uma nova visita de estudo.
        /// </summary>
        /// <param name="viewModel">O modelo de exibição que contém as informações da nova visita de estudo.</param>
        /// <returns>Redireciona para a lista de visitas de estudo.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VisitaEstudoCreateViewModel viewModel)
        {
			if (DateTime.Compare(viewModel.Date, DateTime.UtcNow) < 0)
			{
				var validationMessage = "Não é possível criar uma Visita de Estudo com uma data anterior à data atual";
				ModelState.AddModelError("Date", validationMessage);
			}

			if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                var currentUserAccount = await _context.ContaAdministrativa
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

                var visitaEstudo = new VisitaEstudo
                {
                    VisitaEstudoID = Guid.NewGuid(),
                    Name = viewModel.Name,
                    Date = viewModel.Date,
                    Description = viewModel.Descripton,
                    Location = viewModel.Location,
                    AtlId = currentUserAccount.AtlId
                };

                string fileName = _fileManager.UploadFile(viewModel.Picture, FolderName);

                if (fileName != null)
                {
                    visitaEstudo.Picture = fileName;
                }
                else
                {
                    visitaEstudo.Picture = "logo.png";
                }


                _context.Add(visitaEstudo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        /// <summary>
        /// Exibe o formulário de edição de uma visita de estudo com base no ID especificado.
        /// </summary>
        /// <param name="id">O ID da visita de estudo.</param>
        /// <returns>Uma vista com o formulário de edição da visita de estudo.</returns>

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.VisitaEstudo == null)
            {
                return NotFound();
            }

            var visitaEstudo = await _context.VisitaEstudo.FindAsync(id);
            if (visitaEstudo == null)
            {
                return NotFound();
            }
            return View(new VisitaEstudoEditViewModel(visitaEstudo));
        }

        /// <summary>
        /// Edita uma visita de estudo existente.
        /// </summary>
        /// <param name="id">O ID da visita de estudo a ser editada.</param>
        /// <param name="viewModel">O modelo de exibição que contém as informações atualizadas da visita de estudo.</param>
        /// <returns>Redireciona para a lista de visitas de estudo.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, VisitaEstudoEditViewModel viewModel)
        {
            if (id != viewModel.VisitaEstudoID)
            {
                return NotFound();
            }

            if (viewModel.Date != null)
            {
                if (DateTime.Compare((DateTime)viewModel.Date, DateTime.UtcNow) < 0)
                {
                    var validationMessage = "Não é possível criar uma Visita de Estudo com uma data anterior à data atual";
                    ModelState.AddModelError("Date", validationMessage);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var visitaEstudo = await _context.VisitaEstudo.FindAsync(id);

                    if (visitaEstudo != null)
                    {
                        visitaEstudo.Name = viewModel.Name;
                        visitaEstudo.Location = viewModel.Location;
                        visitaEstudo.Description = viewModel.Descripton;


                        if (viewModel.Date != null)
                        {
                            visitaEstudo.Date = (DateTime)viewModel.Date;
                        }

                        string fileName = _fileManager.UploadFile(viewModel.Picture, FolderName);
						if (fileName != null)
                        {
                            visitaEstudo.Picture = fileName;
                        }

                        _context.Update(visitaEstudo);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitaEstudoExists(viewModel.VisitaEstudoID))
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
        /// Remove uma VisitaEstudo .
        /// </summary>
        /// <param name="id"> ID de uma VisitaEstudo para remover</param>
        /// <returns>A ViewResult object.</returns>

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.VisitaEstudo == null)
            {
                return NotFound();
            }

            var visitaEstudo = await _context.VisitaEstudo
                .FirstOrDefaultAsync(m => m.VisitaEstudoID == id);
            if (visitaEstudo == null)
            {
                return NotFound();
            }

            return View(visitaEstudo);
        }

        /// <summary>
        /// Confirma a remoção uma visita de estudo
        /// </summary>
        /// <param name="id">O ID da visita de estudo</param>
        /// <returns>Uma ação assíncrona do tipo IActionResult</returns>

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.VisitaEstudo == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.VisitaEstudo'  is null.");
            }
            var visitaEstudo = await _context.VisitaEstudo.FindAsync(id);
            if (visitaEstudo != null)
            {
                var record = new VisitaEstudoRecord()
                {
                    VisitaEstudoID = id,
                    Name = visitaEstudo.Name,
                    Location = visitaEstudo.Location,
                    Description = visitaEstudo.Description,
                    Date = visitaEstudo.Date.Date,
                    Picture = visitaEstudo.Picture,
                    AtlId = visitaEstudo.AtlId
                };

                _context.Add(record);
                _context.VisitaEstudo.Remove(visitaEstudo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se uma visita de estudo existe
        /// </summary>
        /// <param name="id">O ID da visita de estudo</param>
        /// <returns>True se a visita de estudo existir, False caso contrário</returns>

        private bool VisitaEstudoExists(Guid id)
        {
          return (_context.VisitaEstudo?.Any(e => e.VisitaEstudoID == id)).GetValueOrDefault();
        }
    }
}
