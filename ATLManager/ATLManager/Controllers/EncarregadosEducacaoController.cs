using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.Models;
using Microsoft.AspNetCore.Identity;
using ATLManager.Areas.Identity.Data;

namespace ATLManager.Controllers
{
    public class EncarregadosEducacaoController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;

        public EncarregadosEducacaoController(ATLManagerAuthContext context, UserManager<ATLManagerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EncarregadosEducacao
        public async Task<IActionResult> Index()
        {
            return View(await _context.EncarregadoEducacao.ToListAsync());
        }

        // GET: EncarregadosEducacao/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.EncarregadoEducacao == null)
            {
                return NotFound();
            }

            var encarregadoEducacao = await _context.EncarregadoEducacao
                .FirstOrDefaultAsync(m => m.EncarregadoId == id);
            if (encarregadoEducacao == null)
            {
                return NotFound();
            }

            return View(encarregadoEducacao);
        }

        // GET: EncarregadosEducacao/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EncarregadosEducacao/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EncarregadoId,Phone,Address,PostalCode,City,NIF")] EncarregadoEducacao encarregadoEducacao)
        {
            if (ModelState.IsValid)
            {
                encarregadoEducacao.EncarregadoId = Guid.NewGuid();
                _context.Add(encarregadoEducacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(encarregadoEducacao);
        }

        // GET: EncarregadosEducacao/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.EncarregadoEducacao == null)
            {
                return NotFound();
            }

            var encarregadoEducacao = await _context.EncarregadoEducacao.FindAsync(id);
            if (encarregadoEducacao == null)
            {
                return NotFound();
            }
            return View(encarregadoEducacao);
        }

        // POST: EncarregadosEducacao/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("EncarregadoId,Phone,Address,PostalCode,City,NIF")] EncarregadoEducacao encarregadoEducacao)
        {
            if (id != encarregadoEducacao.EncarregadoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(encarregadoEducacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EncarregadoEducacaoExists(encarregadoEducacao.EncarregadoId))
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
            return View(encarregadoEducacao);
        }

        // GET: EncarregadosEducacao/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.EncarregadoEducacao == null)
            {
                return NotFound();
            }

            var encarregadoEducacao = await _context.EncarregadoEducacao
                .FirstOrDefaultAsync(m => m.EncarregadoId == id);
            if (encarregadoEducacao == null)
            {
                return NotFound();
            }

            return View(encarregadoEducacao);
        }

        // POST: EncarregadosEducacao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.EncarregadoEducacao == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.EncarregadoEducacao'  is null.");
            }
            var encarregadoEducacao = await _context.EncarregadoEducacao.FindAsync(id);
            if (encarregadoEducacao != null)
            {
				var result = await _userManager.DeleteAsync(encarregadoEducacao.User);

				if (!result.Succeeded)
				{
					throw new InvalidOperationException($"Unexpected error occurred deleting user.");
				}

                _context.EncarregadoEducacao.Remove(encarregadoEducacao);
			}

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EncarregadoEducacaoExists(Guid id)
        {
            return _context.EncarregadoEducacao.Any(e => e.EncarregadoId == id);
        }
    }
}
