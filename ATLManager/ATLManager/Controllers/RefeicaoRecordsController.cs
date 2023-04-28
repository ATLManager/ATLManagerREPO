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
    /// Controlador para o modelo 'Históricos de Refeições'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class RefeicaoRecordsController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;

        public RefeicaoRecordsController(ATLManagerAuthContext context, UserManager<ATLManagerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Método que retorna a view Index com as refeições.
        /// </summary>
        /// <returns>View com as refeições.</returns>

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var currentUserAccount = await _context.ContaAdministrativa
            .Include(f => f.User)
            .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            var refeicoes = await _context.RefeicaoRecord
                .Include(a => a.Atl)
                .Where(r => r.AtlId == currentUserAccount.AtlId)
                .ToListAsync();

            return View(refeicoes);
        }

        /// <summary>
        /// Método que retorna a view Details para uma determinada RefeicaoRecord.
        /// </summary>
        /// <param name="id">Id da RefeicaoRecord a ser visualizada.</param>
        /// <returns>View com os detalhes da RefeicaoRecord.</returns>

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.RefeicaoRecord == null)
            {
                return NotFound();
            }

            var refeicaoRecord = await _context.RefeicaoRecord
                .Include(r => r.Atl)
                .FirstOrDefaultAsync(m => m.RefeicaoRecordId == id);
            if (refeicaoRecord == null)
            {
                return NotFound();
            }

            return View(refeicaoRecord);
        }

        /// <summary>
        /// Método que retorna a view Delete para uma determinada RefeicaoRecord.
        /// </summary>
        /// <param name="id">Id da RefeicaoRecord a ser deletada.</param>
        /// <returns>View com a confirmação de exclusão da RefeicaoRecord.</returns>

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.RefeicaoRecord == null)
            {
                return NotFound();
            }

            var refeicaoRecord = await _context.RefeicaoRecord
                .Include(r => r.Atl)
                .FirstOrDefaultAsync(m => m.RefeicaoRecordId == id);
            if (refeicaoRecord == null)
            {
                return NotFound();
            }

            return View(refeicaoRecord);
        }

        /// <summary>
        /// Método que exclui uma RefeicaoRecord.
        /// </summary>
        /// <param name="id">Id da RefeicaoRecord a ser deletada.</param>
        /// <returns>Redireciona para a Index após a exclusão.</returns>

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.RefeicaoRecord == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.RefeicaoRecord'  is null.");
            }
            var refeicaoRecord = await _context.RefeicaoRecord.FindAsync(id);
            if (refeicaoRecord != null)
            {
                _context.RefeicaoRecord.Remove(refeicaoRecord);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Método que verifica se uma RefeicaoRecord existe.
        /// </summary>
        /// <param name="id">Id da RefeicaoRecord a ser verificada.</param>
        /// <returns>Verdadeiro se a RefeicaoRecord existe, falso caso contrário.</returns>

        private bool RefeicaoRecordExists(Guid id)
        {
          return (_context.RefeicaoRecord?.Any(e => e.RefeicaoRecordId == id)).GetValueOrDefault();
        }
    }
}
