using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using Microsoft.AspNetCore.Hosting;
using ATLManager.ViewModels;
using Microsoft.AspNetCore.Identity;
using ATLManager.Areas.Identity.Data;
using ATLManager.Models;
using ATLManager.Models.Historicos;

namespace ATLManager.Controllers
{
    public class AtividadesController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AtividadesController(ATLManagerAuthContext context, UserManager<ATLManagerUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Atividades
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (HttpContext.User.IsInRole("Coordenador") || HttpContext.User.IsInRole("Funcionario"))
            {
                var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

                var atividades = await _context.Atividade
                    .Where(a => a.AtlId == currentUserAccount.AtlId)
                    .ToListAsync();

                return View(atividades);
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

                var atividades = new List<Atividade>();

                foreach (var educando in educandos)
                {
                    var tempAtividades = await _context.Atividade
						.Include(a => a.Atl)
                        .Where(r => r.AtlId == educando.AtlId)
                        .ToListAsync();

                    atividades = atividades.Union(tempAtividades).ToList();
                }
                ViewData["EducandoId"] = new SelectList(educandos, "EducandoId", "Name");
                return View(atividades);
            }
        }

        // GET: Atividades/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Atividade == null)
            {
                return NotFound();
            }

            var atividade = await _context.Atividade
                .FirstOrDefaultAsync(m => m.AtividadeId == id);
            if (atividade == null)
            {
                return NotFound();
            }

            return View(atividade);
        }

        // GET: Atividades/Create
        public IActionResult Create()
        {
            return View(new AtividadeCreateViewModel());
        }

        // POST: Atividades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AtividadeCreateViewModel viewModel)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            
            DateTime dataAtual = DateTime.Now;

            DateTime dataViewModel = viewModel.StartDate;
            if (dataViewModel.CompareTo(dataAtual) < 0)
            {
                var validationMessage = "Não é possível criar uma Atividade com uma data anterior à data atual";
                ModelState.AddModelError("StartDate", validationMessage);
            }

            if (viewModel.EndDate < viewModel.StartDate)
            {
                var validationMessage = "Não é possível criar uma Atividade com uma data de término anterior à data de incício";
                ModelState.AddModelError("EndDate", validationMessage);
            }


            if (ModelState.IsValid)
            {
                string fileName = UploadedFile(viewModel.Picture);

                var atividade = new Atividade
                {
                    AtividadeId = Guid.NewGuid(),
                    Name = viewModel.Name,
                    StartDate = viewModel.StartDate,
                    EndDate = viewModel.EndDate,
                    Description = viewModel.Description,
                    AtlId = (Guid)currentUserAccount.AtlId 
                };

                if (fileName != null)
                {
                    atividade.Picture = fileName;
                }
                else
                {
                    atividade.Picture = "logo.png";
                }

                _context.Add(atividade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Atividades/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Atividade == null)
            {
                return NotFound();
            }

            var atividade = await _context.Atividade.FindAsync(id);
            if (atividade == null)
            {
                return NotFound();
            }

            var viewModel = new AtividadeEditViewModel
            {
                AtividadeId = atividade.AtividadeId,
                Name = atividade.Name,
                StartDate = atividade.StartDate,
                EndDate = atividade.EndDate,
                Description = atividade.Description
            };

            return View(viewModel);
        }

        // POST: Atividades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("AtividadeId,Name,StartDate,EndDate,Description,Picture")] AtividadeEditViewModel viewModel)
        {
            if (id != viewModel.AtividadeId)
            {
                return NotFound();
            }

            DateTime dataAtual = DateTime.Now;

            DateTime dataViewModel = (DateTime)viewModel.StartDate;
            if (dataViewModel.CompareTo(dataAtual) < 0)
            {
                var validationMessage = "Não é possível criar uma Atividade com uma data anterior à data atual";
                ModelState.AddModelError("StartDate", validationMessage);
            }

            if (viewModel.EndDate < viewModel.StartDate)
            {
                var validationMessage = "Não é possível criar uma Atividade com uma data de término anterior à data de incício";
                ModelState.AddModelError("EndDate", validationMessage);
            }


            if (ModelState.IsValid)
            {
                var atividade = await _context.Atividade.FindAsync(id);

                if (atividade == null) return NotFound();
                    
                atividade.Name = viewModel.Name;
                atividade.Description = viewModel.Description;

                if (viewModel.StartDate != null)
                    atividade.StartDate = (DateTime)viewModel.StartDate;

                if (viewModel.EndDate != null)
                    atividade.EndDate = (DateTime)viewModel.EndDate;


                string fileName = UploadedFile(viewModel.Picture);

                if (fileName != null)
                {
                    atividade.Picture = fileName;
                }

                try
                {
                    _context.Update(atividade);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AtividadeExists(viewModel.AtividadeId))
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

        // GET: Atividades/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Atividade == null)
            {
                return NotFound();
            }

            var atividade = await _context.Atividade
                .FirstOrDefaultAsync(m => m.AtividadeId == id);
            if (atividade == null)
            {
                return NotFound();
            }

            return View(atividade);
        }

        // POST: Atividades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Atividade == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.Atividade'  is null.");
            }
            var atividade = await _context.Atividade.FindAsync(id);
            if (atividade != null)
            {
                var record = new AtividadeRecord()
                {
                    AtividadeId = atividade.AtividadeId,
                    Name = atividade.Name,
                    StartDate = atividade.StartDate,
                    EndDate = atividade.EndDate,
                    Description = atividade.Description,
                    Picture = atividade.Picture,
                    AtlId = atividade.AtlId,
                };

                _context.Add(record);
                _context.Atividade.Remove(atividade);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AtividadeExists(Guid id)
        {
          return (_context.Atividade?.Any(e => e.AtividadeId == id)).GetValueOrDefault();
        }

		private string UploadedFile(IFormFile comprovativo)
		{
			string uniqueFileName = null;

			if (comprovativo != null)
			{
				string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/uploads/atividades");
				uniqueFileName = Guid.NewGuid().ToString() + "_id_" + comprovativo.FileName;
				string filePath = Path.Combine(uploadsFolder, uniqueFileName);
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					comprovativo.CopyTo(fileStream);
				}
			}
			return uniqueFileName;
		}
	}
}
