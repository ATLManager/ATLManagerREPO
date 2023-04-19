using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.Models.Historicos;

namespace ATLManager.Controllers
{
    public class VisitaEstudoRecordsController : Controller
    {
        private readonly ATLManagerAuthContext _context;

        public VisitaEstudoRecordsController(ATLManagerAuthContext context)
        {
            _context = context;
        }

        // GET: VisitaEstudoRecords
        public async Task<IActionResult> Index()
        {
            var aTLManagerAuthContext = _context.VisitaEstudoRecord.Include(v => v.Atl);
            return View(await aTLManagerAuthContext.ToListAsync());
        }

        // GET: VisitaEstudoRecords/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.VisitaEstudoRecord == null)
            {
                return NotFound();
            }

            var visitaEstudoRecord = await _context.VisitaEstudoRecord
                .Include(v => v.Atl)
                .FirstOrDefaultAsync(m => m.VisitaEstudoRecordID == id);
            if (visitaEstudoRecord == null)
            {
                return NotFound();
            }

            return View(visitaEstudoRecord);
        }

        // GET: VisitaEstudoRecords/Create
        public IActionResult Create()
        {
            ViewData["AtlId"] = new SelectList(_context.ATL, "AtlId", "Address");
            return View();
        }

        // POST: VisitaEstudoRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VisitaEstudoRecordID,VisitaEstudoID,Name,Location,Description,Date,Picture,AtlId")] VisitaEstudoRecord visitaEstudoRecord)
        {
            if (ModelState.IsValid)
            {
                visitaEstudoRecord.VisitaEstudoRecordID = Guid.NewGuid();
                _context.Add(visitaEstudoRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AtlId"] = new SelectList(_context.ATL, "AtlId", "Address", visitaEstudoRecord.AtlId);
            return View(visitaEstudoRecord);
        }

        // GET: VisitaEstudoRecords/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.VisitaEstudoRecord == null)
            {
                return NotFound();
            }

            var visitaEstudoRecord = await _context.VisitaEstudoRecord.FindAsync(id);
            if (visitaEstudoRecord == null)
            {
                return NotFound();
            }
            ViewData["AtlId"] = new SelectList(_context.ATL, "AtlId", "Address", visitaEstudoRecord.AtlId);
            return View(visitaEstudoRecord);
        }

        // POST: VisitaEstudoRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("VisitaEstudoRecordID,VisitaEstudoID,Name,Location,Description,Date,Picture,AtlId")] VisitaEstudoRecord visitaEstudoRecord)
        {
            if (id != visitaEstudoRecord.VisitaEstudoRecordID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(visitaEstudoRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitaEstudoRecordExists(visitaEstudoRecord.VisitaEstudoRecordID))
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
            ViewData["AtlId"] = new SelectList(_context.ATL, "AtlId", "Address", visitaEstudoRecord.AtlId);
            return View(visitaEstudoRecord);
        }

        // GET: VisitaEstudoRecords/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.VisitaEstudoRecord == null)
            {
                return NotFound();
            }

            var visitaEstudoRecord = await _context.VisitaEstudoRecord
                .Include(v => v.Atl)
                .FirstOrDefaultAsync(m => m.VisitaEstudoRecordID == id);
            if (visitaEstudoRecord == null)
            {
                return NotFound();
            }

            return View(visitaEstudoRecord);
        }

        // POST: VisitaEstudoRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.VisitaEstudoRecord == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.VisitaEstudoRecord'  is null.");
            }
            var visitaEstudoRecord = await _context.VisitaEstudoRecord.FindAsync(id);
            if (visitaEstudoRecord != null)
            {
                _context.VisitaEstudoRecord.Remove(visitaEstudoRecord);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VisitaEstudoRecordExists(Guid id)
        {
          return (_context.VisitaEstudoRecord?.Any(e => e.VisitaEstudoRecordID == id)).GetValueOrDefault();
        }
    }
}
