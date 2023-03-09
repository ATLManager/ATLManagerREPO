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
    public class ATLsController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        public List<SelectListItem> AgrupamentoOptions { get; set; }

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

            var atl = await _context.ATL
                .FirstOrDefaultAsync(m => m.AtlId == id);
            if (atl == null)
            {
                return NotFound();
            }

            return View(atl);
        }

        // GET: ATLs/Create
        public IActionResult Create()
        {
            PopulateAgrupamentoOptions();
            return View();
        }

        // POST: ATLs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AtlId,Name,Address,City,PostalCode,AgrupamentoPai,NIPC")] ATL atl)
        {
            if (ModelState.IsValid)
            {
                atl.AtlId = Guid.NewGuid();
                _context.Add(atl);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(atl);
        }

        // GET: ATLs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            PopulateAgrupamentoOptions();

            if (id == null || _context.ATL == null)
            {
                return NotFound();
            }

            var atl = await _context.ATL.FindAsync(id);
            if (atl == null)
            {
                return NotFound();
            }
            return View(atl);
        }

        // POST: ATLs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("AtlId,Name,Address,City,PostalCode,NIPC")] ATL atl)
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
            return View(atl);
        }

        // GET: ATLs/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.ATL == null)
            {
                return NotFound();
            }

            var atl = await _context.ATL
                .FirstOrDefaultAsync(m => m.AtlId == id);
            if (atl == null)
            {
                return NotFound();
            }

            return View(atl);
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

        private void PopulateAgrupamentoOptions () {
            ViewBag.Agrupamentos = _context.Agrupamento.Select(a =>
            new SelectListItem
            {
                Value = a.AgrupamentoID.ToString(),
                Text = a.Name
            }).ToList();
        }
    }
}
