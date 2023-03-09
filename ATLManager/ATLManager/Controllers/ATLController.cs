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
    public class ATLController : Controller
    {
        private readonly ATLManagerAuthContext _context;

        public ATLController(ATLManagerAuthContext context)
        {
            _context = context;
        }

        // GET: ATL
        public async Task<IActionResult> Index()
        {
            var aTLManagerAuthContext = _context.ATL.Include(a => a.Agrupamento);
            return View(await aTLManagerAuthContext.ToListAsync());
        }

        // GET: ATL/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.ATL == null)
            {
                return NotFound();
            }

            var atl = await _context.ATL
                .Include(a => a.Agrupamento)
                .FirstOrDefaultAsync(m => m.AtlId == id);
            if (atl == null)
            {
                return NotFound();
            }

            return View(atl);
        }

        // GET: ATL/Create
        public IActionResult Create()
        {
            ViewData["AgrupamentoId"] = new SelectList(_context.Agrupamento, "AgrupamentoID", "Name");
            return View();
        }

        // POST: ATL/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AtlId,Name,Address,City,PostalCode,AgrupamentoId,NIPC")] ATL atl)
        {
            if (ModelState.IsValid)
            {
                atl.AtlId = Guid.NewGuid();
                _context.Add(atl);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AgrupamentoId"] = new SelectList(_context.Agrupamento, "AgrupamentoID", "Name", atl.AgrupamentoId);
            return View(atl);
        }

        // GET: ATL/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.ATL == null)
            {
                return NotFound();
            }

            var atl = await _context.ATL.FindAsync(id);
            if (atl == null)
            {
                return NotFound();
            }
            ViewData["AgrupamentoId"] = new SelectList(_context.Agrupamento, "AgrupamentoID", "Name", atl.AgrupamentoId);
            return View(atl);
        }

        // POST: ATL/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("AtlId,Name,Address,City,PostalCode,AgrupamentoId,NIPC")] ATL atl)
        {
            if (id != atl.AtlId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(atl);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ATLExists(atl.AtlId))
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
            ViewData["AgrupamentoId"] = new SelectList(_context.Agrupamento, "AgrupamentoID", "Name", atl.AgrupamentoId);
            return View(atl);
        }

        // GET: ATL/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.ATL == null)
            {
                return NotFound();
            }

            var atl = await _context.ATL
                .Include(a => a.Agrupamento)
                .FirstOrDefaultAsync(m => m.AtlId == id);
            if (atl == null)
            {
                return NotFound();
            }

            return View(atl);
        }

        // POST: ATL/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.ATL == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.ATL'  is null.");
            }
            var atl = await _context.ATL.FindAsync(id);
            if (atl != null)
            {
                _context.ATL.Remove(atl);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ATLExists(Guid id)
        {
          return _context.ATL.Any(e => e.AtlId == id);
        }
    }
}
