using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.Models;
using ATLManager.ViewModels;

namespace ATLManager.Controllers
{
    public class ATLController : Controller
    {
        private readonly ATLManagerAuthContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ATLController(ATLManagerAuthContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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
            return View(new ATLCreateViewModel());
        }

        // POST: ATL/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ATLCreateViewModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.NIPC) && string.IsNullOrEmpty(viewModel.AgrupamentoId.ToString()))
            {
                var validationMessage = "É necessário introduzir um NIPC ou ATLId";
                ModelState.AddModelError("NIPC", validationMessage);
                ModelState.AddModelError("AtlId", validationMessage);
            }

            if (!string.IsNullOrEmpty(viewModel.NIPC) && !string.IsNullOrEmpty(viewModel.AgrupamentoId.ToString()))
            {
                var validationMessage = "Apenas permitido introduzir um NIPC ou um ATLId";
                ModelState.AddModelError("NIPC", validationMessage);
                ModelState.AddModelError("AtlId", validationMessage);
            }

            if (ModelState.IsValid)
            {
				var atl = new ATL
				{
					AtlId = Guid.NewGuid(),
					Name = viewModel.Name,
					Address = viewModel.Address,
                    City = viewModel.City,
                    PostalCode = viewModel.PostalCode,
					AgrupamentoId = viewModel.AgrupamentoId,
                    NIPC = viewModel.NIPC
				};

                string fileName = UploadedFile(viewModel.LogoPicture);
                if (fileName != null)
                {
                    atl.LogoPicture = fileName;
                }
                else
                {
                    atl.LogoPicture = "logo.png";
                }

                _context.Add(atl);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AgrupamentoId"] = new SelectList(_context.Agrupamento, "AgrupamentoID", "Name", viewModel.AgrupamentoId);
            return View(viewModel);
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
            return View(new ATLEditViewModel(atl));
        }

        // POST: ATL/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ATLEditViewModel viewModel)
        {
            if (id != viewModel.AtlId)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(viewModel.NIPC) && string.IsNullOrEmpty(viewModel.AgrupamentoId.ToString()))
            {
                var validationMessage = "É necessário introduzir um NIPC ou ATLId";
                ModelState.AddModelError("NIPC", validationMessage);             
                ModelState.AddModelError("AtlId", validationMessage);
            }
            
            if (!string.IsNullOrEmpty(viewModel.NIPC) && !string.IsNullOrEmpty(viewModel.AgrupamentoId.ToString()))
            {
                var validationMessage = "Apenas permitido introduzir um NIPC ou um ATLId";
                ModelState.AddModelError("NIPC", validationMessage);
                ModelState.AddModelError("AtlId", validationMessage);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var atl = await _context.ATL.FindAsync(id);

                    if (atl != null)
                    {
                        atl.Name = viewModel.Name;
                        atl.Address = viewModel.Address;
                        atl.City = viewModel.City;
                        atl.PostalCode = viewModel.PostalCode;
                        atl.AgrupamentoId = viewModel.AgrupamentoId;
                        atl.NIPC = viewModel.NIPC;

                        string fileName = UploadedFile(viewModel.LogoPicture);
                        if (fileName != null)
                        {
                            atl.LogoPicture = fileName;
                        }

                        _context.Update(atl);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ATLExists(viewModel.AtlId))
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
            ViewData["AgrupamentoId"] = new SelectList(_context.Agrupamento, "AgrupamentoID", "Name", viewModel.AgrupamentoId);
            return View(viewModel);
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

		private string UploadedFile(IFormFile logoPicture)
		{
			string uniqueFileName = null;

			if (logoPicture != null)
			{
				string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/uploads/atls");
				uniqueFileName = Guid.NewGuid().ToString() + "_" + logoPicture.FileName;
				string filePath = Path.Combine(uploadsFolder, uniqueFileName);
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					logoPicture.CopyTo(fileStream);
				}
			}
			return uniqueFileName;
		}
	}
}
