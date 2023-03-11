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
    public class RefeicoesController : Controller
    {
        private readonly ATLManagerAuthContext _context;

        public RefeicoesController(ATLManagerAuthContext context)
        {
            _context = context;
        }

        // GET: Refeicoes
        public async Task<IActionResult> Index()
        {
              return View(await _context.Refeicao.ToListAsync());
        }

        // GET: Refeicoes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Refeicao == null)
            {
                return NotFound();
            }

            var refeicao = await _context.Refeicao
                .FirstOrDefaultAsync(m => m.RefeicaoId == id);
            if (refeicao == null)
            {
                return NotFound();
            }

            return View(refeicao);
        }

        // GET: Refeicoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Refeicoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RefeicaoId,Name,Categoria,Data,Descricao,Proteina,HidratosCarbono,VR,Acucar,Lipidos,ValorEnergetico,AGSat,Sal")] Refeicao refeicao)
        {
            if (ModelState.IsValid)
            {
                refeicao.RefeicaoId = Guid.NewGuid();
                _context.Add(refeicao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(refeicao);
        }

        // GET: Refeicoes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Refeicao == null)
            {
                return NotFound();
            }

            var refeicao = await _context.Refeicao.FindAsync(id);
            if (refeicao == null)
            {
                return NotFound();
            }
            return View(refeicao);
        }

        // POST: Refeicoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("RefeicaoId,Name,Categoria,Data,Descricao,Proteina,HidratosCarbono,VR,Acucar,Lipidos,ValorEnergetico,AGSat,Sal")] Refeicao refeicao)
        {
            if (id != refeicao.RefeicaoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(refeicao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RefeicaoExists(refeicao.RefeicaoId))
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
            return View(refeicao);
        }

        // GET: Refeicoes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Refeicao == null)
            {
                return NotFound();
            }

            var refeicao = await _context.Refeicao
                .FirstOrDefaultAsync(m => m.RefeicaoId == id);
            if (refeicao == null)
            {
                return NotFound();
            }

            return View(refeicao);
        }

        // POST: Refeicoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Refeicao == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.Refeicao'  is null.");
            }
            var refeicao = await _context.Refeicao.FindAsync(id);
            if (refeicao != null)
            {
                _context.Refeicao.Remove(refeicao);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RefeicaoExists(Guid id)
        {
          return _context.Refeicao.Any(e => e.RefeicaoId == id);
        }
    }
}
