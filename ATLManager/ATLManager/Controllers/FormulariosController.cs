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
    public class FormulariosController : Controller
    {
        private readonly ATLManagerAuthContext _context;

        public FormulariosController(ATLManagerAuthContext context)
        {
            _context = context;
        }

        // GET: Formularios
        public async Task<IActionResult> Index()
        {
            var aTLManagerAuthContext = _context.Formulario.Include(f => f.VisitaEstudo);
            return View(await aTLManagerAuthContext.ToListAsync());
        }

        // GET: Formularios/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Formulario == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formulario
                .Include(f => f.VisitaEstudo)
                .FirstOrDefaultAsync(m => m.FormularioId == id);
            if (formulario == null)
            {
                return NotFound();
            }

            return View(formulario);
        }

        // GET: Formularios/Create
        public IActionResult Create()
        {
            ViewData["VisitaEstudoId"] = new SelectList(_context.VisitaEstudo, "VisitaEstudoID", "Descripton");
            return View();
        }

        // POST: Formularios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FormularioId,Name,Description,VisitaEstudoId,StartDate,DateLimit")] Formulario formulario)
        {
            if (ModelState.IsValid)
            {
                formulario.FormularioId = Guid.NewGuid();
                _context.Add(formulario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VisitaEstudoId"] = new SelectList(_context.VisitaEstudo, "VisitaEstudoID", "Descripton", formulario.VisitaEstudoId);
            return View(formulario);
        }

        // GET: Formularios/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Formulario == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formulario.FindAsync(id);
            if (formulario == null)
            {
                return NotFound();
            }
            ViewData["VisitaEstudoId"] = new SelectList(_context.VisitaEstudo, "VisitaEstudoID", "Descripton", formulario.VisitaEstudoId);
            return View(formulario);
        }

        // POST: Formularios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("FormularioId,Name,Description,VisitaEstudoId,StartDate,DateLimit")] Formulario formulario)
        {
            if (id != formulario.FormularioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(formulario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormularioExists(formulario.FormularioId))
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
            ViewData["VisitaEstudoId"] = new SelectList(_context.VisitaEstudo, "VisitaEstudoID", "Descripton", formulario.VisitaEstudoId);
            return View(formulario);
        }

        // GET: Formularios/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Formulario == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formulario
                .Include(f => f.VisitaEstudo)
                .FirstOrDefaultAsync(m => m.FormularioId == id);
            if (formulario == null)
            {
                return NotFound();
            }

            return View(formulario);
        }

        // POST: Formularios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Formulario == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.Formulario'  is null.");
            }
            var formulario = await _context.Formulario.FindAsync(id);
            if (formulario != null)
            {
                _context.Formulario.Remove(formulario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FormularioExists(Guid id)
        {
          return (_context.Formulario?.Any(e => e.FormularioId == id)).GetValueOrDefault();
        }
    }
}
