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
    public class ContasAdministrativasController : Controller
    {
        private readonly ATLManagerAuthContext _context;
		private readonly UserManager<ATLManagerUser> _userManager;

		public ContasAdministrativasController(ATLManagerAuthContext context, UserManager<ATLManagerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ContasAdministrativas
        public async Task<IActionResult> Index()
        {
            return View(await _context.ContaAdministrativa.ToListAsync());
        }

        // GET: ContasAdministrativas/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.ContaAdministrativa == null)
            {
                return NotFound();
            }

            var contaAdministrativa = await _context.ContaAdministrativa
                .FirstOrDefaultAsync(m => m.ContaId == id);
            if (contaAdministrativa == null)
            {
                return NotFound();
            }

            return View(contaAdministrativa);
        }

        // GET: ContasAdministrativas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ContasAdministrativas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContaId,DateOfBirth,CC")] ContaAdministrativa contaAdministrativa)
        {
            if (ModelState.IsValid)
            {
                contaAdministrativa.ContaId = Guid.NewGuid();
                _context.Add(contaAdministrativa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contaAdministrativa);
        }

        // GET: ContasAdministrativas/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.ContaAdministrativa == null)
            {
                return NotFound();
            }

            var contaAdministrativa = await _context.ContaAdministrativa.FindAsync(id);
            if (contaAdministrativa == null)
            {
                return NotFound();
            }
            return View(contaAdministrativa);
        }

        // POST: ContasAdministrativas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ContaId,DateOfBirth,CC")] ContaAdministrativa contaAdministrativa)
        {
            if (id != contaAdministrativa.ContaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contaAdministrativa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContaAdministrativaExists(contaAdministrativa.ContaId))
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
            return View(contaAdministrativa);
        }

        // GET: ContasAdministrativas/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.ContaAdministrativa == null)
            {
                return NotFound();
            }

            var contaAdministrativa = await _context.ContaAdministrativa
                .FirstOrDefaultAsync(m => m.ContaId == id);
            if (contaAdministrativa == null)
            {
                return NotFound();
            }

            return View(contaAdministrativa);
        }

        // POST: ContasAdministrativas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.ContaAdministrativa == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.ContaAdministrativa'  is null.");
            }
            var contaAdministrativa = await _context.ContaAdministrativa.FindAsync(id);
            if (contaAdministrativa != null)
            {
				var result = await _userManager.DeleteAsync(contaAdministrativa.User);

				if (!result.Succeeded)
				{
					throw new InvalidOperationException($"Unexpected error occurred deleting user.");
				}

				_context.ContaAdministrativa.Remove(contaAdministrativa);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContaAdministrativaExists(Guid id)
        {
            return _context.ContaAdministrativa.Any(e => e.ContaId == id);
        }
    }
}
