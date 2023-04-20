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

        // GET: EducandoRecords
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

        // GET: EducandoRecords/Details/5
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

        // GET: EducandoRecords/Delete/5
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

        // POST: EducandoRecords/Delete/5
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

        private bool EducandoRecordExists(Guid id)
        {
          return (_context.EducandoRecord?.Any(e => e.EducandoRecordId == id)).GetValueOrDefault();
        }
    }
}
