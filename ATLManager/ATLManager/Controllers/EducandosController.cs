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
    public class EducandosController : Controller
    {
        private readonly ATLManagerAuthContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public EducandosController(ATLManagerAuthContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Educandos
        public async Task<IActionResult> Index()
        {
            var aTLManagerAuthContext = _context.Educando.Include(e => e.Atl).Include(e => e.Encarregado);
            return View(await aTLManagerAuthContext.ToListAsync());
        }

        // GET: Educandos/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Educando == null)
            {
                return NotFound();
            }

            var educando = await _context.Educando
                .Include(e => e.Atl)
                .Include(e => e.Encarregado)
                .FirstOrDefaultAsync(m => m.EducandoId == id);
            if (educando == null)
            {
                return NotFound();
            }

            return View(educando);
        }

        // GET: Educandos/Create
        public IActionResult Create()
        {
            ViewData["AtlId"] = new SelectList(_context.ATL, "AtlId", "Name");
            ViewData["EncarregadoId"] = new SelectList(_context.EncarregadoEducacao, "EncarregadoId", "FullName");
            return View(new EducandoCreateViewModel());
        }

        // POST: Educandos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EducandoCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
				string fileName = UploadedFile(viewModel.ProfilePicture);

                var educando = new Educando
                {
                    EducandoId = Guid.NewGuid(),
                    Name = viewModel.Name,
                    Apelido = viewModel.Apelido,
                    CC = viewModel.CC,
                    Genero = viewModel.Genero,
                    AtlId = viewModel.AtlId,
                    EncarregadoId = viewModel.EncarregadoId,
                    ProfilePicture = fileName
                };

                _context.Add(educando);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AtlId"] = new SelectList(_context.ATL, "AtlId", "Address", viewModel.AtlId);
            ViewData["EncarregadoId"] = new SelectList(_context.EncarregadoEducacao, "EncarregadoId", "Address", viewModel.EncarregadoId);
            return View(viewModel);
        }

        // GET: Educandos/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Educando == null)
            {
                return NotFound();
            }

            var educando = await _context.Educando.FindAsync(id);
            if (educando == null)
            {
                return NotFound();
            }
            ViewData["AtlId"] = new SelectList(_context.ATL, "AtlId", "Address", educando.AtlId);
            ViewData["EncarregadoId"] = new SelectList(_context.EncarregadoEducacao, "EncarregadoId", "Address", educando.EncarregadoId);
            return View(new EducandoEditViewModel(educando));
        }

        // POST: Educandos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EducandoEditViewModel viewModel)
        {
            if (id != viewModel.EducandoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var educando = await _context.Educando.FindAsync(id);

                    if (educando != null)
                    {
                        educando.Name = viewModel.Name;
                        educando.Apelido = viewModel.Apelido;
                        educando.CC = viewModel.CC;
                        educando.Genero = viewModel.Genero;
                        educando.AtlId = viewModel.AtlId;
                        educando.EncarregadoId = viewModel.EncarregadoId;

                        string fileName = UploadedFile(viewModel.ProfilePicture);
                        educando.ProfilePicture = fileName;

                        _context.Update(educando);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EducandoExists(viewModel.EducandoId))
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
            ViewData["AtlId"] = new SelectList(_context.ATL, "AtlId", "Address", viewModel.AtlId);
            ViewData["EncarregadoId"] = new SelectList(_context.EncarregadoEducacao, "EncarregadoId", "Address", viewModel.EncarregadoId);
            return View(viewModel);
        }

        // GET: Educandos/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Educando == null)
            {
                return NotFound();
            }

            var educando = await _context.Educando
                .Include(e => e.Atl)
                .Include(e => e.Encarregado)
                .FirstOrDefaultAsync(m => m.EducandoId == id);
            if (educando == null)
            {
                return NotFound();
            }

            return View(educando);
        }

        // POST: Educandos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Educando == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.Educando'  is null.");
            }
            var educando = await _context.Educando.FindAsync(id);
            if (educando != null)
            {
                _context.Educando.Remove(educando);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EducandoExists(Guid id)
        {
          return _context.Educando.Any(e => e.EducandoId == id);
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
