using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.Models;
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace ATLManager.Controllers
{
    /// <summary>
    /// Controlador para o modelo 'Histórico de formulários'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class FormularioRecordsController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;

        public FormularioRecordsController(ATLManagerAuthContext context, UserManager<ATLManagerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Retorna a visualização de índice da conta administrativa.
        /// </summary>
        /// <returns>Uma instância de IActionResult que representa a visualização de índice da conta administrativa.</returns>

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            if (currentUserAccount == null) return NotFound();

            var formularios = await _context.FormularioRecord
                .Include(a => a.Atl)
                .Where(r => r.AtlId == currentUserAccount.AtlId)
                .ToListAsync();

            return View(formularios);
        }

        /// <summary>
        /// Retorna a visualização de respostas para o formulário com o ID especificado.
        /// </summary>
        /// <param name="id">O ID do formulário.</param>
        /// <returns>Uma instância de IActionResult que representa a visualização de respostas para o formulário.</returns>

        public async Task<IActionResult> Respostas(Guid? id)
        {
            if (id == null || _context.Formulario == null)
            {
                return NotFound();
            }

            var respostas = await _context.FormularioRespostaRecord
                .Where(f => f.FormularioRecordId == id)
                .ToListAsync();

            if (respostas == null)
            {
                return NotFound();
            }

            return View(respostas);
        }

        /// <summary>
        /// Retorna a visualização de detalhes para o formulário com o ID especificado.
        /// </summary>
        /// <param name="id">O ID do formulário.</param>
        /// <returns>Uma instância de IActionResult que representa a visualização de detalhes para o formulário.</returns>

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.FormularioRecord == null)
            {
                return NotFound();
            }

            var formularioRecord = await _context.FormularioRecord
                .Include(f => f.Atl)
                .FirstOrDefaultAsync(m => m.FormularioRecordId == id);
            if (formularioRecord == null)
            {
                return NotFound();
            }

            return View(formularioRecord);
        }

        /// <summary>
        /// Retorna a visualização de exclusão para o formulário com o ID especificado.
        /// </summary>
        /// <param name="id">O ID do formulário.</param>
        /// <returns>Uma instância de IActionResult que representa a visualização de exclusão para o formulário.</returns>

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.FormularioRecord == null)
            {
                return NotFound();
            }

            var formularioRecord = await _context.FormularioRecord
                .Include(f => f.Atl)
                .FirstOrDefaultAsync(m => m.FormularioRecordId == id);
            if (formularioRecord == null)
            {
                return NotFound();
            }

            return View(formularioRecord);
        }

        /// <summary>
        /// Confirma a exclusão do formulário com o ID especificado.
        /// </summary>
        /// <param name="id">O ID do formulário.</param>
        /// <returns>Uma instância de IActionResult que redireciona para a visualização de índice após a exclusão do formulário.</returns>

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.FormularioRecord == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.FormularioRecord'  is null.");
            }
            var formularioRecord = await _context.FormularioRecord.FindAsync(id);
            if (formularioRecord != null)
            {
                _context.FormularioRecord.Remove(formularioRecord);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se o formulário com o ID especificado existe.
        /// </summary>
        /// <param name="id">O ID do formulário.</param>
        /// <returns>Verdadeiro se o formulário existir, caso contrário, falso.</returns>

        private bool FormularioRecordExists(Guid id)
        {
          return (_context.FormularioRecord?.Any(e => e.FormularioRecordId == id)).GetValueOrDefault();
        }
    }
}
