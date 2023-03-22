using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.Models;
using Microsoft.AspNetCore.Hosting;
using ATLManager.ViewModels;

namespace ATLManager.Controllers
{
    public class VisitasEstudoController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VisitasEstudoController(ATLManagerAuthContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: VisitasEstudo
        public async Task<IActionResult> Index()
        {
              return _context.VisitaEstudo != null ? 
                          View(await _context.VisitaEstudo.ToListAsync()) :
                          Problem("Entity set 'ATLManagerAuthContext.VisitaEstudo'  is null.");
        }

        // GET: VisitasEstudo/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.VisitaEstudo == null)
            {
                return NotFound();
            }

            var visitaEstudo = await _context.VisitaEstudo
                .FirstOrDefaultAsync(m => m.VisitaEstudoID == id);
            if (visitaEstudo == null)
            {
                return NotFound();
            }

            return View(visitaEstudo);
        }

        // GET: VisitasEstudo/Create
        public IActionResult Create()
        {
            return View(new VisitaEstudoCreateViewModel());
        }

        // POST: VisitasEstudo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VisitaEstudoCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var visitaEstudo = new VisitaEstudo
                {
                    VisitaEstudoID = Guid.NewGuid(),
                    Name = viewModel.Name,
                    Date = viewModel.Date,
                    Descripton = viewModel.Descripton,
                    Location = viewModel.Location
                };

                string fileName = UploadedFile(viewModel.Picture);

                if (fileName != null)
                {
                    visitaEstudo.Picture = fileName;
                }
                else
                {
                    visitaEstudo.Picture = "logo.png";
                }


                _context.Add(visitaEstudo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

            // GET: VisitasEstudo/Edit/5
            public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.VisitaEstudo == null)
            {
                return NotFound();
            }

            var visitaEstudo = await _context.VisitaEstudo.FindAsync(id);
            if (visitaEstudo == null)
            {
                return NotFound();
            }
            return View(new VisitaEstudoEditViewModel(visitaEstudo));
        }

        // POST: VisitasEstudo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, VisitaEstudoEditViewModel viewModel)
        {
            if (id != viewModel.VisitaEstudoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var visitaEstudo = await _context.VisitaEstudo.FindAsync(id);

                    if (visitaEstudo != null)
                    {
                        visitaEstudo.Name = viewModel.Name;
                        visitaEstudo.Location = viewModel.Location;
                        visitaEstudo.Descripton = viewModel.Descripton;


                        if (viewModel.Date != null)
                        {
                            visitaEstudo.Date = DateTime.Parse(viewModel.Date);
                        }

                        string fileName = UploadedFile(viewModel.Picture);

                        if (fileName != null)
                        {
                            visitaEstudo.Picture = fileName;
                        }

                        _context.Update(visitaEstudo);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitaEstudoExists(viewModel.VisitaEstudoID))
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
            return View(viewModel);
        }

        // GET: VisitasEstudo/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.VisitaEstudo == null)
            {
                return NotFound();
            }

            var visitaEstudo = await _context.VisitaEstudo
                .FirstOrDefaultAsync(m => m.VisitaEstudoID == id);
            if (visitaEstudo == null)
            {
                return NotFound();
            }

            return View(visitaEstudo);
        }

        // POST: VisitasEstudo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.VisitaEstudo == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.VisitaEstudo'  is null.");
            }
            var visitaEstudo = await _context.VisitaEstudo.FindAsync(id);
            if (visitaEstudo != null)
            {
                _context.VisitaEstudo.Remove(visitaEstudo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VisitaEstudoExists(Guid id)
        {
          return (_context.VisitaEstudo?.Any(e => e.VisitaEstudoID == id)).GetValueOrDefault();
        }

        private string UploadedFile(IFormFile logoPicture)
        {
            string uniqueFileName = "";

            if (logoPicture != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/uploads/visitasEstudo");
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
