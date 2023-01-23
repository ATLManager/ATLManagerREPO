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
    public class ContaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Conta
        public async Task<IActionResult> Index()
        {
              return View(await _context.conta_utilizador.ToListAsync());
        }

        // GET: Conta/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.conta_utilizador == null)
            {
                return NotFound();
            }

            var contaModel = await _context.conta_utilizador
                .FirstOrDefaultAsync(m => m.ContaID == id);
            if (contaModel == null)
            {
                return NotFound();
            }

            return View(contaModel);
        }

        // GET: Conta/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Conta/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContaID,Email,Password,ContaRole,ContaEstadoId,CodigoAtivacao,DataCriacao,DataUltimaAlteracao")] Conta contaModel)
        {
            if (ModelState.IsValid)
            {
                contaModel.ContaID = Guid.NewGuid();
                _context.Add(contaModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contaModel);
        }

        // GET: Conta/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.conta_utilizador == null)
            {
                return NotFound();
            }

            var contaModel = await _context.conta_utilizador.FindAsync(id);
            if (contaModel == null)
            {
                return NotFound();
            }
            return View(contaModel);
        }

        // POST: Conta/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ContaID,Email,Password,ContaRole,ContaEstadoId,CodigoAtivacao,DataCriacao,DataUltimaAlteracao")] Conta contaModel)
        {
            if (id != contaModel.ContaID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contaModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContaModelExists(contaModel.ContaID))
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
            return View(contaModel);
        }

        // GET: Conta/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.conta_utilizador == null)
            {
                return NotFound();
            }

            var contaModel = await _context.conta_utilizador
                .FirstOrDefaultAsync(m => m.ContaID == id);
            if (contaModel == null)
            {
                return NotFound();
            }

            return View(contaModel);
        }

        // POST: Conta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.conta_utilizador == null)
            {
                return Problem("Entity set 'ApplicationDbContext.conta_utilizador'  is null.");
            }
            var contaModel = await _context.conta_utilizador.FindAsync(id);
            if (contaModel != null)
            {
                _context.conta_utilizador.Remove(contaModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContaModelExists(Guid id)
        {
          return _context.conta_utilizador.Any(e => e.ContaID == id);
        }
    }
}
