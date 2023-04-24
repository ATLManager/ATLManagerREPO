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
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using ATLManager.Models.Historicos;

namespace ATLManager.Controllers
{
    public class VisitasEstudoController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VisitasEstudoController(ATLManagerAuthContext context,
            UserManager<ATLManagerUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: VisitasEstudo
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            
            if (HttpContext.User.IsInRole("Coordenador") || HttpContext.User.IsInRole("Funcionario"))
            { 
                var currentUserAccount = await _context.ContaAdministrativa
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

                var visitas = await _context.VisitaEstudo
                    .Include(a => a.Atl)
                    .Where(r => r.AtlId == currentUserAccount.AtlId)
                    .ToListAsync();

                return View(visitas);
            }
            else
            {
                var currentUserAccount = await _context.EncarregadoEducacao
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

                var educandos = await _context.Educando
                    .Include(e => e.Atl)
                    .Include(e => e.Encarregado)
                    .Where(e => e.EncarregadoId == currentUserAccount.EncarregadoId)
                    .ToListAsync();

                var visitas = new List<VisitaEstudo>();

                foreach (var educando in educandos)
                {
				    var tempVisitas = await _context.VisitaEstudo
					    .Include(a => a.Atl)
					    .Where(r => r.AtlId == educando.AtlId)
					    .ToListAsync();

                    visitas = visitas.Union(tempVisitas).ToList();
                }

				ViewData["EducandoId"] = new SelectList(educandos, "EducandoId", "Name");
				return View(visitas);
            }
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

            DateTime dataAtual = DateTime.Now;

            DateTime dataViewModel = viewModel.Date;
            if (dataViewModel.CompareTo(dataAtual) < 0)
            {
                var validationMessage = "Não é possível criar uma Visita de Estudo com uma data anterior à data atual";
                ModelState.AddModelError("Date", validationMessage);
            }



            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                var currentUserAccount = await _context.ContaAdministrativa
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

                var visitaEstudo = new VisitaEstudo
                {
                    VisitaEstudoID = Guid.NewGuid(),
                    Name = viewModel.Name,
                    Date = viewModel.Date,
                    Description = viewModel.Descripton,
                    Location = viewModel.Location,
                    AtlId = currentUserAccount.AtlId
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


            DateTime dataAtual = DateTime.Now;

            DateTime dataViewModel = DateTime.Parse(viewModel.Date);
            if (dataViewModel.CompareTo(dataAtual) < 0)
            {
                var validationMessage = "Não é possível criar uma Visita de Estudo com uma data anterior à data atual";
                ModelState.AddModelError("Date", validationMessage);
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
                        visitaEstudo.Description = viewModel.Descripton;


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
                var record = new VisitaEstudoRecord()
                {
                    VisitaEstudoID = id,
                    Name = visitaEstudo.Name,
                    Location = visitaEstudo.Location,
                    Description = visitaEstudo.Description,
                    Date = visitaEstudo.Date.Date,
                    Picture = visitaEstudo.Picture,
                    AtlId = visitaEstudo.AtlId
                };

                _context.Add(record);
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
            string uniqueFileName = null;

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
