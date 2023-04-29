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
    /// Controlador para o modelo 'Histórico de Atividades'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    
    public class AtividadeRecordsController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;

        public AtividadeRecordsController(ATLManagerAuthContext context, UserManager<ATLManagerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Obtém a página de índice de atividades para o utilizador atual.
        /// </summary>
        /// <returns>A página de índice de atividades com as atividades do utilizador atual.</returns>

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var currentUserAccount = await _context.ContaAdministrativa
            .Include(f => f.User)
            .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            var atividades = await _context.AtividadeRecord
                .Where(a => a.AtlId == currentUserAccount.AtlId)
                .ToListAsync();

            return View(atividades);
        }

        /// <summary>
        /// Obtém a página de detalhes da atividade com o ID fornecido.
        /// </summary>
        /// <param name="id">O ID da atividade.</param>
        /// <returns>A página de detalhes da atividade com o ID fornecido.</returns>

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.AtividadeRecord == null)
            {
                return NotFound();
            }

            var atividadeRecord = await _context.AtividadeRecord
                .Include(a => a.Atl)
                .FirstOrDefaultAsync(m => m.AtividadeRecordId == id);
            if (atividadeRecord == null)
            {
                return NotFound();
            }

            return View(atividadeRecord);
        }

        /// <summary>
        /// Obtém a página de exclusão da atividade com o ID fornecido.
        /// </summary>
        /// <param name="id">O ID da atividade.</param>
        /// <returns>A página de exclusão da atividade com o ID fornecido.</returns>

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.AtividadeRecord == null)
            {
                return NotFound();
            }

            var atividadeRecord = await _context.AtividadeRecord
                .Include(a => a.Atl)
                .FirstOrDefaultAsync(m => m.AtividadeRecordId == id);
            if (atividadeRecord == null)
            {
                return NotFound();
            }

            return View(atividadeRecord);
        }

        /// <summary>
        /// Exclui a atividade com o ID fornecido.
        /// </summary>
        /// <param name="id">O ID da atividade.</param>
        /// <returns>Redireciona para a página de índice de atividades após a exclusão da atividade com o ID fornecido.</returns>

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.AtividadeRecord == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.AtividadeRecord'  is null.");
            }
            var atividadeRecord = await _context.AtividadeRecord.FindAsync(id);
            if (atividadeRecord != null)
            {
                _context.AtividadeRecord.Remove(atividadeRecord);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se existe um registo de atividade com o ID especificado.
        /// </summary>
        /// <param name="id">O ID do registo de atividade a ser verificado.</param>
        /// <returns>True se o registo de atividade existir, False caso contrário.</returns>

        private bool AtividadeRecordExists(Guid id)
        {
          return (_context.AtividadeRecord?.Any(e => e.AtividadeRecordId == id)).GetValueOrDefault();
        }
    }
}
