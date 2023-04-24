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
    public class RefeicaoRecordsController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;

        public RefeicaoRecordsController(ATLManagerAuthContext context, UserManager<ATLManagerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: RefeicaoRecords
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

        // GET: RefeicaoRecords/Details/5
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

        // GET: RefeicaoRecords/Delete/5
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

        // POST: RefeicaoRecords/Delete/5
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

        private bool RefeicaoRecordExists(Guid id)
        {
          return (_context.RefeicaoRecord?.Any(e => e.RefeicaoRecordId == id)).GetValueOrDefault();
        }
    }
}
