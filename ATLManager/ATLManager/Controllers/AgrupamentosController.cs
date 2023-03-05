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
    public class AgrupamentosController : Controller
    {
        private readonly ATLManagerAuthContext _context;

        public AgrupamentosController(ATLManagerAuthContext context)
        {
            _context = context;
        }

        // GET: Agrupamentos
        public async Task<IActionResult> Index()
        {
              return View(await _context.Agrupamento.ToListAsync());
        }

        // GET: Agrupamentos/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Agrupamento == null)
            {
                return NotFound();
            }

            var agrupamento = await _context.Agrupamento
                .FirstOrDefaultAsync(m => m.AgrupamentoID == id);
            if (agrupamento == null)
            {
                return NotFound();
            }

            return View(agrupamento);
        }

        // GET: Agrupamentos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Agrupamentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AgrupamentoID,Name,Location")] Agrupamento agrupamento)
        {
            if (ModelState.IsValid)
            {
                agrupamento.AgrupamentoID = Guid.NewGuid();
                _context.Add(agrupamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(agrupamento);
        }

        // GET: Agrupamentos/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Agrupamento == null)
            {
                return NotFound();
            }

            var agrupamento = await _context.Agrupamento.FindAsync(id);
            if (agrupamento == null)
            {
                return NotFound();
            }
            return View(agrupamento);
        }

        // POST: Agrupamentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("AgrupamentoID,Name,Location")] Agrupamento agrupamento)
        {
            if (id != agrupamento.AgrupamentoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agrupamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgrupamentoExists(agrupamento.AgrupamentoID))
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
            return View(agrupamento);
        }

        // GET: Agrupamentos/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Agrupamento == null)
            {
                return NotFound();
            }

            var agrupamento = await _context.Agrupamento
                .FirstOrDefaultAsync(m => m.AgrupamentoID == id);
            if (agrupamento == null)
            {
                return NotFound();
            }

            return View(agrupamento);
        }

        // POST: Agrupamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Agrupamento == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.Agrupamento'  is null.");
            }
            var agrupamento = await _context.Agrupamento.FindAsync(id);
            if (agrupamento != null)
            {
                _context.Agrupamento.Remove(agrupamento);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgrupamentoExists(Guid id)
        {
          return _context.Agrupamento.Any(e => e.AgrupamentoID == id);
        }
    }
}
