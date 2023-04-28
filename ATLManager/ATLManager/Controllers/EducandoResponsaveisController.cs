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
using ATLManager.Services;

namespace ATLManager.Controllers
{
    /// <summary>
    /// Controlador para o modelo 'Responsáveis Educandos'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class EducandoResponsaveisController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly IFileManager _fileManager;

        private readonly string FolderName = "responsaveis";

        public EducandoResponsaveisController(ATLManagerAuthContext context,
            IFileManager fileManager)
        {
			_context = context;
            _fileManager = fileManager;
        }

        /// <summary>
        /// Mostra os detalhes do responsável pelo educando com o ID especificado.
        /// </summary>
        /// <param name="id">O ID do responsável pelo educando.</param>
        /// <returns>Retorna uma visualização dos detalhes do responsável pelo educando com o ID especificado.</returns>

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.EducandoResponsavel == null)
            {
                return NotFound();
            }

            var educandoResponsavel = await _context.EducandoResponsavel
                .Include(e => e.Educando)
                .FirstOrDefaultAsync(m => m.EducandoResponsavelId == id);
            if (educandoResponsavel == null)
            {
                return NotFound();
            }

            return View(educandoResponsavel);
        }

        /// <summary>
        /// Mostra a página para criar um novo responsável pelo educando.
        /// </summary>
        /// <param name="id">O ID do educando relacionado.</param>
        /// <returns>Retorna uma visualização da página de criação de um novo responsável pelo educando.</returns>

        public IActionResult Create(Guid id)
        {
            return View(new ResponsavelCreateViewModel(id));
        }

        /// <summary>
        /// Cria um novo responsável pelo educando.
        /// </summary>
        /// <param name="viewModel">O modelo de visualização do responsável pelo educando a ser criado.</param>
        /// <returns>Retorna uma visualização dos detalhes do educando relacionado.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ResponsavelCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var responsavel = new EducandoResponsavel() 
                { 
                    EducandoId = viewModel.EducandoId,
                    Name = viewModel.Name,
                    Apelido = viewModel.Apelido,
                    CC = viewModel.CC,
                    Phone = viewModel.Phone,
                    Parentesco = viewModel.Parentesco,
                };

				string photoFileName = _fileManager.UploadFile(viewModel.ProfilePicture, FolderName);

				if (photoFileName != null)
				{
					responsavel.ProfilePicture = photoFileName;
				}
				else
				{
					responsavel.ProfilePicture = "logo.png";
				}

				_context.Add(responsavel);
                await _context.SaveChangesAsync();
                return RedirectToAction("DetailsResponsaveis", "Educandos", new { id = responsavel.EducandoId });
            }
            return View(viewModel);
        }

        /// <summary>
        /// Edita um responsável pelo educando.
        /// </summary>
        /// <param name="id">O ID do responsável.</param>
        /// <returns>Uma instância de IActionResult.</returns>

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.EducandoResponsavel == null)
            {
                return NotFound();
            }

            var responsavel = await _context.EducandoResponsavel.FindAsync(id);

            if (responsavel == null)
            {
                return NotFound();
            }

            var viewModel = new ResponsavelEditViewModel()
            {
                EducandoResponsavelId = responsavel.EducandoResponsavelId,
                EducandoId = responsavel.EducandoId,
                Name = responsavel.Name,
                Apelido = responsavel.Apelido,
                CC = responsavel.CC,
                Phone = responsavel.Phone,
                Parentesco = responsavel.Parentesco
            };

            return View(viewModel);
        }

        /// <summary>
        /// Edita um responsável pelo educando.
        /// </summary>
        /// <param name="id">O ID do responsável.</param>
        /// <param name="viewModel">O ViewModel com as informações a serem atualizadas.</param>
        /// <returns>Uma instância de IActionResult.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ResponsavelEditViewModel viewModel)
        {
            if (id != viewModel.EducandoResponsavelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var responsavel = await _context.EducandoResponsavel.FindAsync(id);

                    responsavel.Name = viewModel.Name;
                    responsavel.Apelido = viewModel.Apelido;
                    responsavel.CC = viewModel.CC;
                    responsavel.Phone = viewModel.Phone;
                    responsavel.Parentesco = viewModel.Parentesco;

                    string photoFileName = _fileManager.UploadFile(viewModel.ProfilePicture, FolderName);

                    if (photoFileName != null)
                    {
                        responsavel.ProfilePicture = photoFileName;
                    }

                    _context.Update(responsavel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EducandoResponsavelExists(viewModel.EducandoResponsavelId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("DetailsResponsaveis", "Educandos", new { id = viewModel.EducandoId });
            }
            
            return View(viewModel);
        }

        /// <summary>
        /// Remove um registo EducandoResponsavel com base em um ID fornecido.
        /// </summary>
        /// <param name="id">O ID do registo EducandoResponsavel a ser removido.</param>
        /// <returns>Uma instância de IActionResult que representa o resultado da ação.</returns>

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.EducandoResponsavel == null)
            {
                return NotFound();
            }

            var educandoResponsavel = await _context.EducandoResponsavel
                .Include(e => e.Educando)
                .FirstOrDefaultAsync(m => m.EducandoResponsavelId == id);
            if (educandoResponsavel == null)
            {
                return NotFound();
            }

            return View(educandoResponsavel);
        }

        /// <summary>
        /// Confirma a remoção de um registo EducandoResponsavel com base em um ID fornecido.
        /// </summary>
        /// <param name="id">O ID do registo EducandoResponsavel a ser removido.</param>
        /// <returns>Uma instância de IActionResult que representa o resultado da ação.</returns>

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.EducandoResponsavel == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.EducandoResponsavel'  is null.");
            }
            var educandoResponsavel = await _context.EducandoResponsavel.FindAsync(id);
            var educandoId = educandoResponsavel.EducandoId;
            if (educandoResponsavel != null)
            {
                _context.EducandoResponsavel.Remove(educandoResponsavel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("DetailsResponsaveis", "Educandos", new { id = educandoId });
        }

        /// <summary>
        /// Verifica se um registo EducandoResponsavel com o ID fornecido existe no contexto atual.
        /// </summary>
        /// <param name="id">O ID do registo EducandoResponsavel a ser verificado.</param>
        /// <returns>Um valor booleano que indica se o registo EducandoResponsavel existe ou não.</returns>

        private bool EducandoResponsavelExists(Guid id)
        {
          return (_context.EducandoResponsavel?.Any(e => e.EducandoResponsavelId == id)).GetValueOrDefault();
        }
	}
}
