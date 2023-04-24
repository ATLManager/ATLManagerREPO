using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.Models;
using ATLManager.ViewModels;
using Microsoft.AspNetCore.Identity;
using ATLManager.Areas.Identity.Data;

namespace ATLManager.Controllers
{
    public class FuncionarioRecordsController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;

        public FuncionarioRecordsController(ATLManagerAuthContext context, UserManager<ATLManagerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: FuncionarioRecords
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            var funcionarios = await _context.FuncionarioRecord
                .Include(f => f.Atl)
                .Where(f => f.AtlId == currentUserAccount.AtlId)
                .ToListAsync();

            return View(funcionarios);
        }

        // GET: FuncionarioRecords/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.FuncionarioRecord == null)
            {
                return NotFound();
            }

            var funcionarioRecord = await _context.FuncionarioRecord
                .Include(f => f.Atl)
                .FirstOrDefaultAsync(m => m.FuncionarioRecordId == id);
            if (funcionarioRecord == null)
            {
                return NotFound();
            }

            return View(funcionarioRecord);
        }

        // GET: FuncionarioRecords/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.FuncionarioRecord == null)
            {
                return NotFound();
            }

            var funcionarioRecord = await _context.FuncionarioRecord
                .Include(f => f.Atl)
                .FirstOrDefaultAsync(m => m.FuncionarioRecordId == id);
            if (funcionarioRecord == null)
            {
                return NotFound();
            }

            return View(funcionarioRecord);
        }

        // POST: FuncionarioRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.FuncionarioRecord == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.FuncionarioRecord'  is null.");
            }
            var funcionarioRecord = await _context.FuncionarioRecord.FindAsync(id);
            if (funcionarioRecord != null)
            {
                _context.FuncionarioRecord.Remove(funcionarioRecord);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FuncionarioRecordExists(Guid id)
        {
          return (_context.FuncionarioRecord?.Any(e => e.FuncionarioRecordId == id)).GetValueOrDefault();
        }
    }
}
