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
    public class EducandoResponsaveisController : Controller
    {
        private readonly ATLManagerAuthContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public EducandoResponsaveisController(ATLManagerAuthContext context, 
            IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
		}

		// GET: EducandoResponsaveis/Details/5
		public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.EducandoResponsavel == null)
            {
                return NotFound();
            }

            var educandoResponsavel = await _context.EducandoResponsavel
                .Include(e => e.Educando)
                .FirstOrDefaultAsync(m => m.EducandoResponsavelId == id);
            if (educandoResponsavel == null)
            {
                return NotFound();
            }

            return View(educandoResponsavel);
        }

        // GET: EducandoResponsaveis/Create
        public IActionResult Create(Guid id)
        {
            return View(new ResponsavelCreateViewModel(id));
        }

        // POST: EducandoResponsaveis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ResponsavelCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var responsavel = new EducandoResponsavel() 
                { 
                    EducandoId = viewModel.EducandoId,
                    Name = viewModel.Name,
                    Apelido = viewModel.Apelido,
                    CC = viewModel.CC,
                    Phone = viewModel.Phone,
                    Parentesco = viewModel.Parentesco,
                };

				string photoFileName = UploadedFile(viewModel.ProfilePicture);

				if (photoFileName != null)
				{
					responsavel.ProfilePicture = photoFileName;
				}
				else
				{
					responsavel.ProfilePicture = "logo.png";
				}

				_context.Add(responsavel);
                await _context.SaveChangesAsync();
                return RedirectToAction("DetailsResponsaveis", "Educandos", new { id = responsavel.EducandoId });
            }
            return View(viewModel);
        }

        // GET: EducandoResponsaveis/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.EducandoResponsavel == null)
            {
                return NotFound();
            }

            var responsavel = await _context.EducandoResponsavel.FindAsync(id);

            if (responsavel == null)
            {
                return NotFound();
            }

            var viewModel = new ResponsavelEditViewModel()
            {
                EducandoResponsavelId = responsavel.EducandoResponsavelId,
                EducandoId = responsavel.EducandoId,
                Name = responsavel.Name,
                Apelido = responsavel.Apelido,
                CC = responsavel.CC,
                Phone = responsavel.Phone,
                Parentesco = responsavel.Parentesco
            };

            return View(viewModel);
        }

        // POST: EducandoResponsaveis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ResponsavelEditViewModel viewModel)
        {
            if (id != viewModel.EducandoResponsavelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var responsavel = await _context.EducandoResponsavel.FindAsync(id);

                    responsavel.Name = viewModel.Name;
                    responsavel.Apelido = viewModel.Apelido;
                    responsavel.CC = viewModel.CC;
                    responsavel.Phone = viewModel.Phone;
                    responsavel.Parentesco = viewModel.Parentesco;

                    string photoFileName = UploadedFile(viewModel.ProfilePicture);

                    if (photoFileName != null)
                    {
                        responsavel.ProfilePicture = photoFileName;
                    }
                    else
                    {
                        responsavel.ProfilePicture = "logo.png";
                    }

                    _context.Update(responsavel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EducandoResponsavelExists(viewModel.EducandoResponsavelId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("DetailsResponsaveis", "Educandos", new { id = viewModel.EducandoId });
            }
            
            return View(viewModel);
        }

        // GET: EducandoResponsaveis/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.EducandoResponsavel == null)
            {
                return NotFound();
            }

            var educandoResponsavel = await _context.EducandoResponsavel
                .Include(e => e.Educando)
                .FirstOrDefaultAsync(m => m.EducandoResponsavelId == id);
            if (educandoResponsavel == null)
            {
                return NotFound();
            }

            return View(educandoResponsavel);
        }

        // POST: EducandoResponsaveis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.EducandoResponsavel == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.EducandoResponsavel'  is null.");
            }
            var educandoResponsavel = await _context.EducandoResponsavel.FindAsync(id);
            var educandoId = educandoResponsavel.EducandoId;
            if (educandoResponsavel != null)
            {
                _context.EducandoResponsavel.Remove(educandoResponsavel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("DetailsResponsaveis", "Educandos", new { id = educandoId });
        }

        private bool EducandoResponsavelExists(Guid id)
        {
          return (_context.EducandoResponsavel?.Any(e => e.EducandoResponsavelId == id)).GetValueOrDefault();
        }

		private string UploadedFile(IFormFile logoPicture)
		{
			string uniqueFileName = null;

			if (logoPicture != null)
			{
				string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/uploads/responsaveis");
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
