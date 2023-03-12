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
using Microsoft.AspNetCore.Hosting;

namespace ATLManager.Controllers
{
    public class AgrupamentosController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AgrupamentosController(ATLManagerAuthContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Agrupamentos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Agrupamento.ToListAsync());
        }

        // GET: Agrupamentos/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Agrupamento == null)
            {
                return NotFound();
            }

            var agrupamento = await _context.Agrupamento
                .FirstOrDefaultAsync(m => m.AgrupamentoID == id);
            if (agrupamento == null)
            {
                return NotFound();
            }

            return View(agrupamento);
        }

        // GET: Agrupamentos/Create
        public IActionResult Create()
        {
            return View(new AgrupamentoCreateViewModel());
        }

        // POST: Agrupamentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AgrupamentoCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string fileName = UploadedFile(viewModel.LogoPicture);

                var agrupamento = new Agrupamento
                {
                    AgrupamentoID = Guid.NewGuid(),
                    Name = viewModel.Name,
                    Location = viewModel.Location,
                    NIPC = viewModel.NIPC,
                    LogoPicture = fileName
                };
                
                _context.Add(agrupamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Agrupamentos/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Agrupamento == null)
            {
                return NotFound();
            }

            var agrupamento = await _context.Agrupamento.FindAsync(id);
            if (agrupamento == null)
            {
                return NotFound();
            }
            return View(new AgrupamentoEditViewModel(agrupamento));
        }

        // POST: Agrupamentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AgrupamentoEditViewModel viewModel)
        {
            if (id != viewModel.AgrupamentoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var agrupamento = await _context.Agrupamento.FindAsync(id);

                    if (agrupamento != null)
                    {
                        agrupamento.Name = viewModel.Name;
                        agrupamento.Location = viewModel.Location;
                        agrupamento.NIPC = viewModel.NIPC;

                        if (viewModel.LogoPicture != null)
                        {
							string fileName = UploadedFile(viewModel.LogoPicture);

                            agrupamento.LogoPicture = fileName;
						}

					    _context.Update(agrupamento);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgrupamentoExists(viewModel.AgrupamentoId))
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

        // GET: Agrupamentos/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Agrupamento == null)
            {
                return NotFound();
            }

            var agrupamento = await _context.Agrupamento
                .FirstOrDefaultAsync(m => m.AgrupamentoID == id);
            if (agrupamento == null)
            {
                return NotFound();
            }

            return View(agrupamento);
        }

        // POST: Agrupamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Agrupamento == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.Agrupamento'  is null.");
            }
            var agrupamento = await _context.Agrupamento.FindAsync(id);
            if (agrupamento != null)
            {
                _context.Agrupamento.Remove(agrupamento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgrupamentoExists(Guid id)
        {
            return _context.Agrupamento.Any(e => e.AgrupamentoID == id);
        }

        private string UploadedFile(IFormFile logoPicture)
        {
            string uniqueFileName = null;

            if (logoPicture != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
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
