using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.Models;

namespace ATLManager.Controllers
{
    public class EducandoSaudeController : Controller
    {
        private readonly ATLManagerAuthContext _context;

        public EducandoSaudeController(ATLManagerAuthContext context)
        {
            _context = context;
        }

        // GET: EducandoSaude
        public async Task<IActionResult> Index()
        {
            var aTLManagerAuthContext = _context.EducandoSaude.Include(e => e.Educando);
            return View(await aTLManagerAuthContext.ToListAsync());
        }

        // GET: EducandoSaude/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.EducandoSaude == null)
            {
                return NotFound();
            }

            var educandoSaude = await _context.EducandoSaude
                .Include(e => e.Educando)
                .FirstOrDefaultAsync(m => m.EducandoId == id);
            if (educandoSaude == null)
            {
                return NotFound();
            }

            return View(educandoSaude);
        }

        // GET: EducandoSaude/Create
        public IActionResult Create()
        {
            ViewData["EducandoId"] = new SelectList(_context.Educando, "EducandoId", "Apelido");
            return View();
        }

        // POST: EducandoSaude/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EducandoSaudeId,EducandoId,BloodType,EmergencyContact,InsuranceName,InsuranceNumber,Allergies,Diseases,Medication,MedicalHistory")] EducandoSaude educandoSaude)
        {
            if (ModelState.IsValid)
            {
                educandoSaude.EducandoSaudeId = Guid.NewGuid();
                _context.Add(educandoSaude);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EducandoId"] = new SelectList(_context.Educando, "EducandoId", "Apelido", educandoSaude.EducandoId);
            return View(educandoSaude);
        }

        // GET: EducandoSaude/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.EducandoSaude == null)
            {
                return NotFound();
            }

            var educandoSaude = await _context.EducandoSaude.FindAsync(id);
            if (educandoSaude == null)
            {
                return NotFound();
            }
            ViewData["EducandoId"] = new SelectList(_context.Educando, "EducandoId", "Apelido", educandoSaude.EducandoId);
            return View(educandoSaude);
        }

        // POST: EducandoSaude/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("EducandoSaudeId,EducandoId,BloodType,EmergencyContact,InsuranceName,InsuranceNumber,Allergies,Diseases,Medication,MedicalHistory")] EducandoSaude educandoSaude)
        {
            if (id != educandoSaude.EducandoSaudeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(educandoSaude);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EducandoSaudeExists(educandoSaude.EducandoSaudeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EducandoId"] = new SelectList(_context.Educando, "EducandoId", "Apelido", educandoSaude.EducandoId);
            return View(educandoSaude);
        }

        // GET: EducandoSaude/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.EducandoSaude == null)
            {
                return NotFound();
            }

            var educandoSaude = await _context.EducandoSaude
                .Include(e => e.Educando)
                .FirstOrDefaultAsync(m => m.EducandoSaudeId == id);
            if (educandoSaude == null)
            {
                return NotFound();
            }

            return View(educandoSaude);
        }

        // POST: EducandoSaude/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.EducandoSaude == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.EducandoSaude'  is null.");
            }
            var educandoSaude = await _context.EducandoSaude.FindAsync(id);
            if (educandoSaude != null)
            {
                _context.EducandoSaude.Remove(educandoSaude);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EducandoSaudeExists(Guid id)
        {
          return (_context.EducandoSaude?.Any(e => e.EducandoSaudeId == id)).GetValueOrDefault();
        }
    }
}
