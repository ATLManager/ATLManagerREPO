using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.Models;

namespace ATLManager
{
    public class ATLsController : Controller
    {
        private readonly ATLManagerAuthContext _context;

        public ATLsController(ATLManagerAuthContext context)
        {
            _context = context;
        }

        // GET: ATLs
        public async Task<IActionResult> Index()
        {
              return View(await _context.ATL.ToListAsync());
        }

        // GET: ATLs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.ATL == null)
            {
                return NotFound();
            }

            var aTL = await _context.ATL
                .FirstOrDefaultAsync(m => m.AtlId == id);
            if (aTL == null)
            {
                return NotFound();
            }

            return View(aTL);
        }

        // GET: ATLs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ATLs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AtlId,Name,Address,City,PostalCode,NIPC")] ATL aTL)
        {
            if (ModelState.IsValid)
            {
                aTL.AtlId = Guid.NewGuid();
                _context.Add(aTL);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aTL);
        }

        // GET: ATLs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.ATL == null)
            {
                return NotFound();
            }

            var aTL = await _context.ATL.FindAsync(id);
            if (aTL == null)
            {
                return NotFound();
            }
            return View(aTL);
        }

        // POST: ATLs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("AtlId,Name,Address,City,PostalCode,NIPC")] ATL aTL)
        {
            if (id != aTL.AtlId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aTL);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ATLExists(aTL.AtlId))
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
            return View(aTL);
        }

        // GET: ATLs/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.ATL == null)
            {
                return NotFound();
            }

            var aTL = await _context.ATL
                .FirstOrDefaultAsync(m => m.AtlId == id);
            if (aTL == null)
            {
                return NotFound();
            }

            return View(aTL);
        }

        // POST: ATLs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.ATL == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.ATL'  is null.");
            }
            var aTL = await _context.ATL.FindAsync(id);
            if (aTL != null)
            {
                _context.ATL.Remove(aTL);
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
