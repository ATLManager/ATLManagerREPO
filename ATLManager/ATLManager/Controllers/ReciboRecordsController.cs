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
using Microsoft.AspNetCore.Authorization;

namespace ATLManager.Controllers
{
    public class ReciboRecordsController : Controller
    {
        private readonly ATLManagerAuthContext _context;
		private readonly UserManager<ATLManagerUser> _userManager;

		public ReciboRecordsController(ATLManagerAuthContext context, 
            UserManager<ATLManagerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ReciboRecords
        [Authorize(Roles ="Coordenador")]
        public async Task<IActionResult> Index()
        {
			var currentUser = await _userManager.GetUserAsync(HttpContext.User);

			var currentUserAccount = await _context.ContaAdministrativa
				.Include(f => f.User)
				.FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

			var recibos = await _context.ReciboRecord
				.Include(e => e.Atl)
				.Where(e => e.AtlId == currentUserAccount.AtlId)
				.ToListAsync();

			return View(recibos);
		}

        // GET: ReciboRecords/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.ReciboRecord == null)
            {
                return NotFound();
            }

            var reciboRecord = await _context.ReciboRecord
                .Include(r => r.Atl)
                .FirstOrDefaultAsync(m => m.ReciboRecordId == id);
            if (reciboRecord == null)
            {
                return NotFound();
            }

            return View(reciboRecord);
        }

        // GET: ReciboRecords/Respostas/5
		public async Task<IActionResult> Respostas(Guid? id)
        {
            if (id == null || _context.ReciboRecord == null)
            {
                return NotFound();
            }

            var respostas = await _context.ReciboRespostaRecord
                .Where(f => f.ReciboRecordId == id)
                .ToListAsync();

            if (respostas == null)
            {
                return NotFound();
            }

            return View(respostas);
        }

        // GET: ReciboRecords/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.ReciboRecord == null)
            {
                return NotFound();
            }

            var reciboRecord = await _context.ReciboRecord
                .Include(r => r.Atl)
                .FirstOrDefaultAsync(m => m.ReciboRecordId == id);
            if (reciboRecord == null)
            {
                return NotFound();
            }

            return View(reciboRecord);
        }

        // POST: ReciboRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.ReciboRecord == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.ReciboRecord'  is null.");
            }
            var reciboRecord = await _context.ReciboRecord.FindAsync(id);
            if (reciboRecord != null)
            {
                _context.ReciboRecord.Remove(reciboRecord);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReciboRecordExists(Guid id)
        {
          return (_context.ReciboRecord?.Any(e => e.ReciboRecordId == id)).GetValueOrDefault();
        }
    }
}
