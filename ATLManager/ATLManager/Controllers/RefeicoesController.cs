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
using NuGet.ContentModel;
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using ATLManager.Models.Historicos;

namespace ATLManager.Controllers
{
    public class RefeicoesController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RefeicoesController(ATLManagerAuthContext context,
            UserManager<ATLManagerUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Refeicoes
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

			if (HttpContext.User.IsInRole("Coordenador") || HttpContext.User.IsInRole("Funcionario"))
			{
				var currentUserAccount = await _context.ContaAdministrativa
				.Include(f => f.User)
				.FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

				var refeicoes = await _context.Refeicao
					.Include(a => a.Atl)
					.Where(r => r.AtlId == currentUserAccount.AtlId)
					.ToListAsync();

				return View(refeicoes);
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

				var refeicoes = new List<Refeicao>();

				foreach (var educando in educandos)
				{
					var tempRefeicoes = await _context.Refeicao
						.Include(a => a.Atl)
						.Where(r => r.AtlId == educando.AtlId)
						.ToListAsync();

					refeicoes = refeicoes.Union(tempRefeicoes).ToList();
				}
				ViewData["EducandoId"] = new SelectList(educandos, "EducandoId", "Name");
				return View(refeicoes);
			}
		}

        // GET: Refeicoes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Refeicao == null)
            {
                return NotFound();
            }

            var refeicao = await _context.Refeicao
                .FirstOrDefaultAsync(m => m.RefeicaoId == id);
            if (refeicao == null)
            {
                return NotFound();
            }

            return View(refeicao);
        }

        // GET: Refeicoes/Create
        public IActionResult Create()
        {
            return View(new RefeicaoCreateViewModel());
        }

        // POST: Refeicoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RefeicaoCreateViewModel viewModel)
        {

            DateTime dataAtual = DateTime.Now;

            DateTime dataViewModel = viewModel.Data;
            if (dataViewModel.CompareTo(dataAtual) < 0)
            {
                var validationMessage = "Não é possível criar uma Visita de Estudo com uma data anterior à data atual";
                ModelState.AddModelError("Data", validationMessage);
            }

            if (ModelState.IsValid)
            {
                    var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                var currentUserAccount = await _context.ContaAdministrativa
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

                var refeicao = new Refeicao
                {
                    RefeicaoId = Guid.NewGuid(),
                    Name = viewModel.Name,
                    Categoria = viewModel.Categoria,
                    Data = viewModel.Data,
                    Descricao = viewModel.Descricao,
                    Proteina = viewModel.Proteina,
                    HidratosCarbono = viewModel.HidratosCarbono,
                    VR = viewModel.VR,
                    Acucar = viewModel.Acucar,
                    Lipidos = viewModel.Lipidos,
                    ValorEnergetico = viewModel.ValorEnergetico,
                    AGSat = viewModel.AGSat,
                    Sal = viewModel.Sal,
                    AtlId = (Guid)currentUserAccount.AtlId
                };

                string fileName = UploadedFile(viewModel.Picture);

                if (fileName != null)
                {
                    refeicao.Picture = fileName;
                }
                else
                {
                    refeicao.Picture = "logo.png";
                }

                _context.Add(refeicao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

    // GET: Refeicoes/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Refeicao == null)
            {
                return NotFound();
            }

            var refeicao = await _context.Refeicao.FindAsync(id);
            if (refeicao == null)
            {
                return NotFound();
            }
            return View(new RefeicaoEditViewModel(refeicao));
        }

        // POST: Refeicoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, RefeicaoEditViewModel viewModel)
        {
            if (id != viewModel.RefeicaoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var refeicao = await _context.Refeicao.FindAsync(id);

                    if (refeicao != null)
                    {
                        refeicao.Name = viewModel.Name;
                        refeicao.Categoria = viewModel.Categoria;
                        refeicao.Descricao = viewModel.Descricao;
                        refeicao.Proteina = viewModel.Proteina;
                        refeicao.HidratosCarbono = viewModel.HidratosCarbono;
                        refeicao.VR = viewModel.VR;
                        refeicao.Acucar = viewModel.Acucar;
                        refeicao.Lipidos = viewModel.Lipidos;
                        refeicao.ValorEnergetico = viewModel.ValorEnergetico;
                        refeicao.AGSat = viewModel.AGSat;
                        refeicao.Sal = viewModel.Sal;

                        if (viewModel.Data != null)
                        {
                            refeicao.Data = DateTime.Parse(viewModel.Data);
                        }

                        string fileName = UploadedFile(viewModel.Picture);

                        if (fileName != null)
                        {
                            refeicao.Picture = fileName;
                        }

                        _context.Update(refeicao);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RefeicaoExists(viewModel.RefeicaoId))
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

        // GET: Refeicoes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Refeicao == null)
            {
                return NotFound();
            }

            var refeicao = await _context.Refeicao
                .FirstOrDefaultAsync(m => m.RefeicaoId == id);
            if (refeicao == null)
            {
                return NotFound();
            }

            return View(refeicao);
        }

        // POST: Refeicoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Refeicao == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.Refeicao'  is null.");
            }
            var refeicao = await _context.Refeicao.FindAsync(id);
            if (refeicao != null)
            {
                var record = new RefeicaoRecord()
                {
                    RefeicaoId = refeicao.RefeicaoId,
                    Name = refeicao.Name,
                    Categoria = refeicao.Categoria,
                    Data = refeicao.Data.Date,
                    Descricao = refeicao.Descricao,
                    Proteina = refeicao.Proteina,
                    HidratosCarbono = refeicao.Proteina,
                    VR = refeicao.VR,
                    Acucar = refeicao.Acucar,
                    Lipidos = refeicao.Lipidos,
                    ValorEnergetico = refeicao.ValorEnergetico,
                    AGSat = refeicao.AGSat,
                    Sal = refeicao.Sal,
                    Picture = refeicao.Picture,
                    AtlId = refeicao.AtlId,
                };

                _context.Add(record);
                _context.Refeicao.Remove(refeicao);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RefeicaoExists(Guid id)
        {
          return _context.Refeicao.Any(e => e.RefeicaoId == id);
        }

        private string UploadedFile(IFormFile logoPicture)
        {
            string uniqueFileName = null;

            if (logoPicture != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/uploads/refeicoes");
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
