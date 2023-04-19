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
    public class VisitaEstudoRecordsController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;

        public VisitaEstudoRecordsController(ATLManagerAuthContext context, UserManager<ATLManagerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: VisitaEstudoRecords
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

        // GET: VisitaEstudoRecords/Details/5
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

        // GET: VisitaEstudoRecords/Delete/5
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

        // POST: VisitaEstudoRecords/Delete/5
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

        private bool VisitaEstudoRecordExists(Guid id)
        {
          return (_context.VisitaEstudoRecord?.Any(e => e.VisitaEstudoRecordID == id)).GetValueOrDefault();
        }
    }
}
