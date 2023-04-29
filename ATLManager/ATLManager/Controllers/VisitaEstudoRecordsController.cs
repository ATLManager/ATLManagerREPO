using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.Models.Historicos;
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace ATLManager.Controllers
{
    /// <summary>
    /// Controlador para o modelo 'Históricos de Visitas de Estudo'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class VisitaEstudoRecordsController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;

        public VisitaEstudoRecordsController(ATLManagerAuthContext context, UserManager<ATLManagerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Retorna a exibição da lista de registos de visita de estudo do utilizador atual.
        /// </summary>
        /// <returns>A exibição da lista de registos de visita de estudo do utilizador atual.</returns>

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            var visitasRecords = await _context.VisitaEstudoRecord
                .Include(a => a.Atl)
                .Where(r => r.AtlId == currentUserAccount.AtlId)
                .ToListAsync();

            return View(visitasRecords);
        }

        /// <summary>
        /// Retorna a exibição dos detalhes do registo de visita de estudo com o ID especificado.
        /// </summary>
        /// <param name="id">O ID do registo de visita de estudo.</param>
        /// <returns>A exibição dos detalhes do registo de visita de estudo.</returns>

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.VisitaEstudoRecord == null)
            {
                return NotFound();
            }

            var visitaEstudoRecord = await _context.VisitaEstudoRecord
                .Include(v => v.Atl)
                .FirstOrDefaultAsync(m => m.VisitaEstudoRecordID == id);
            if (visitaEstudoRecord == null)
            {
                return NotFound();
            }

            return View(visitaEstudoRecord);
        }

        /// <summary>
        /// Retorna a exibição para confirmar a exclusão do registo de visita de estudo com o ID especificado.
        /// </summary>
        /// <param name="id">O ID do registo de visita de estudo.</param>
        /// <returns>A exibição para confirmar a exclusão do registo de visita de estudo.</returns>

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.VisitaEstudoRecord == null)
            {
                return NotFound();
            }

            var visitaEstudoRecord = await _context.VisitaEstudoRecord
                .Include(v => v.Atl)
                .FirstOrDefaultAsync(m => m.VisitaEstudoRecordID == id);
            if (visitaEstudoRecord == null)
            {
                return NotFound();
            }

            return View(visitaEstudoRecord);
        }

        /// <summary>
        /// Confirma a exclusão de um registo de visita de estudo.
        /// </summary>
        /// <param name="id">O ID do regisro a ser excluído.</param>
        /// <returns>O resultado da ação de exclusão.</returns>

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.VisitaEstudoRecord == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.VisitaEstudoRecord'  is null.");
            }
            var visitaEstudoRecord = await _context.VisitaEstudoRecord.FindAsync(id);
            if (visitaEstudoRecord != null)
            {
                _context.VisitaEstudoRecord.Remove(visitaEstudoRecord);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// Verifica se um registo de visita de estudo com o ID especificado existe no contexto de dados.
        /// </summary>
        /// <param name="id">O ID do registo a ser verificado.</param>
        /// <returns>Verdadeiro se o registo existir, falso caso contrário.</returns>

        private bool VisitaEstudoRecordExists(Guid id)
        {
          return (_context.VisitaEstudoRecord?.Any(e => e.VisitaEstudoRecordID == id)).GetValueOrDefault();
        }
    }
}
