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
    public class FormularioRespostaRecordsController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;

        public FormularioRespostaRecordsController(ATLManagerAuthContext context, UserManager<ATLManagerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: FormularioRespostaRecords/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.FormularioRespostaRecord == null)
            {
                return NotFound();
            }

            var formularioRespostaRecord = await _context.FormularioRespostaRecord
                .Include(f => f.FormularioRecord)
                .FirstOrDefaultAsync(m => m.FormularioRespostaRecordId == id);
            if (formularioRespostaRecord == null)
            {
                return NotFound();
            }

            return View(formularioRespostaRecord);
        }

        // GET: FormularioRespostaRecords/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.FormularioRespostaRecord == null)
            {
                return NotFound();
            }

            var formularioRespostaRecord = await _context.FormularioRespostaRecord
                .Include(f => f.FormularioRecord)
                .FirstOrDefaultAsync(m => m.FormularioRespostaRecordId == id);
            if (formularioRespostaRecord == null)
            {
                return NotFound();
            }

            return View(formularioRespostaRecord);
        }

        // POST: FormularioRespostaRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.FormularioRespostaRecord == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.FormularioRespostaRecord'  is null.");
            }
            var formularioRespostaRecord = await _context.FormularioRespostaRecord.FindAsync(id);
            if (formularioRespostaRecord != null)
            {
                _context.FormularioRespostaRecord.Remove(formularioRespostaRecord);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FormularioRespostaRecordExists(Guid id)
        {
          return (_context.FormularioRespostaRecord?.Any(e => e.FormularioRespostaRecordId == id)).GetValueOrDefault();
        }
    }
}
