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
    public class EducandosController : Controller
    {
        private readonly ATLManagerAuthContext _context;

        public EducandosController(ATLManagerAuthContext context)
        {
            _context = context;
        }

        // GET: Educandos
        public async Task<IActionResult> Index()
        {
            var aTLManagerAuthContext = _context.Educando.Include(e => e.Atl).Include(e => e.Encarregado);
            return View(await aTLManagerAuthContext.ToListAsync());
        }

        // GET: Educandos/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Educando == null)
            {
                return NotFound();
            }

            var educando = await _context.Educando
                .Include(e => e.Atl)
                .Include(e => e.Encarregado)
                .FirstOrDefaultAsync(m => m.EducandoId == id);
            if (educando == null)
            {
                return NotFound();
            }

            return View(educando);
        }

        // GET: Educandos/Create
        public IActionResult Create()
        {
            ViewData["AtlId"] = new SelectList(_context.ATL, "AtlId", "Address");
            ViewData["EncarregadoId"] = new SelectList(_context.EncarregadoEducacao, "EncarregadoId", "Address");
            return View();
        }

        // POST: Educandos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EducandoId,Name,Apelido,NIF,Genero,AtlId,EncarregadoId")] Educando educando)
        {
            if (ModelState.IsValid)
            {
                educando.EducandoId = Guid.NewGuid();
                _context.Add(educando);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AtlId"] = new SelectList(_context.ATL, "AtlId", "Address", educando.AtlId);
            ViewData["EncarregadoId"] = new SelectList(_context.EncarregadoEducacao, "EncarregadoId", "Address", educando.EncarregadoId);
            return View(educando);
        }

        // GET: Educandos/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Educando == null)
            {
                return NotFound();
            }

            var educando = await _context.Educando.FindAsync(id);
            if (educando == null)
            {
                return NotFound();
            }
            ViewData["AtlId"] = new SelectList(_context.ATL, "AtlId", "Address", educando.AtlId);
            ViewData["EncarregadoId"] = new SelectList(_context.EncarregadoEducacao, "EncarregadoId", "Address", educando.EncarregadoId);
            return View(educando);
        }

        // POST: Educandos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("EducandoId,Name,Apelido,NIF,Genero,AtlId,EncarregadoId")] Educando educando)
        {
            if (id != educando.EducandoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(educando);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EducandoExists(educando.EducandoId))
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
            ViewData["AtlId"] = new SelectList(_context.ATL, "AtlId", "Address", educando.AtlId);
            ViewData["EncarregadoId"] = new SelectList(_context.EncarregadoEducacao, "EncarregadoId", "Address", educando.EncarregadoId);
            return View(educando);
        }

        // GET: Educandos/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Educando == null)
            {
                return NotFound();
            }

            var educando = await _context.Educando
                .Include(e => e.Atl)
                .Include(e => e.Encarregado)
                .FirstOrDefaultAsync(m => m.EducandoId == id);
            if (educando == null)
            {
                return NotFound();
            }

            return View(educando);
        }

        // POST: Educandos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Educando == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.Educando'  is null.");
            }
            var educando = await _context.Educando.FindAsync(id);
            if (educando != null)
            {
                _context.Educando.Remove(educando);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EducandoExists(Guid id)
        {
          return _context.Educando.Any(e => e.EducandoId == id);
        }
    }
}
