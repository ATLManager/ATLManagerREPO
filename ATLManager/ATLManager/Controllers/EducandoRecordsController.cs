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
    /// Controlador para o modelo 'Histórico de Educandos'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class EducandoRecordsController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;

        public EducandoRecordsController(ATLManagerAuthContext context, 
            UserManager<ATLManagerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Retorna a página inicial da aplicação, mostrando os educandos associados à conta do utilizador atual.
        /// </summary>
        /// <returns>Uma exibição da lista de educandos associados à conta do utilizador atual.</returns>

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            var educandos = await _context.EducandoRecord
                .Include(e => e.Atl)
                .Where(e => e.AtlId == currentUserAccount.AtlId)
                .ToListAsync();

            return View(educandos);
            
        }

        /// <summary>
        /// Retorna a página de detalhes do registo do educando com o ID especificado.
        /// </summary>
        /// <param name="id">O ID do registo do educando.</param>
        /// <returns>Uma exibição dos detalhes do registo do educando com o ID especificado.</returns>

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.EducandoRecord == null)
            {
                return NotFound();
            }

            var educandoRecord = await _context.EducandoRecord
                .Include(e => e.Atl)
                .FirstOrDefaultAsync(m => m.EducandoRecordId == id);
            if (educandoRecord == null)
            {
                return NotFound();
            }

            return View(educandoRecord);
        }

        /// <summary>
        /// Retorna a página de exclusão do registo do educando com o ID especificado.
        /// </summary>
        /// <param name="id">O ID do registo do educando.</param>
        /// <returns>Uma exibição da página de exclusão do registo do educando com o ID especificado.</returns>

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.EducandoRecord == null)
            {
                return NotFound();
            }

            var educandoRecord = await _context.EducandoRecord
                .Include(e => e.Atl)
                .FirstOrDefaultAsync(m => m.EducandoRecordId == id);
            if (educandoRecord == null)
            {
                return NotFound();
            }

            return View(educandoRecord);
        }

        /// <summary>
        /// Confirma a exclusão do registo do educando com o ID especificado.
        /// </summary>
        /// <param name="id">O ID do registo do educando a ser excluído.</param>
        /// <returns>Redireciona para a página inicial após a exclusão do registo.</returns>

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.EducandoRecord == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.EducandoRecord'  is null.");
            }
            var educandoRecord = await _context.EducandoRecord.FindAsync(id);
            if (educandoRecord != null)
            {
                _context.EducandoRecord.Remove(educandoRecord);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se existe um registo de educando com o ID especificado.
        /// </summary>
        /// <param name="id">O ID do registo do educando a ser verificado.</param>
        /// <returns>True se o registo do educando com o ID especificado existir, False caso contrário.</returns>

        private bool EducandoRecordExists(Guid id)
        {
          return (_context.EducandoRecord?.Any(e => e.EducandoRecordId == id)).GetValueOrDefault();
        }
    }
}
