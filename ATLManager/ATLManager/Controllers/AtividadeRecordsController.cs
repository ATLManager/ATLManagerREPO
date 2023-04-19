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
    public class AtividadeRecordsController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;

        public AtividadeRecordsController(ATLManagerAuthContext context, UserManager<ATLManagerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: AtividadeRecords
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

        // GET: AtividadeRecords/Details/5
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

        // GET: AtividadeRecords/Delete/5
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

        // POST: AtividadeRecords/Delete/5
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

        private bool AtividadeRecordExists(Guid id)
        {
          return (_context.AtividadeRecord?.Any(e => e.AtividadeRecordId == id)).GetValueOrDefault();
        }
    }
}
