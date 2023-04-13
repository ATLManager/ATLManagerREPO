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
using System.Diagnostics;
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;

namespace ATLManager.Controllers
{
    public class AgrupamentosController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

		private readonly List<string> allowedPrefixesNIPC = new() { "5", "6", "7", "8", "9" };

		public AgrupamentosController(ATLManagerAuthContext context,
            UserManager<ATLManagerUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Agrupamentos
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == user.Id);

            if (userAccount == null)
            {
                return NotFound();
            }

            var agrupamentos = await _context.Agrupamento
                    .Where(g => g.ContaId == userAccount.ContaId)
                    .ToListAsync();
            
            return View(agrupamentos);
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
            if (!string.IsNullOrEmpty(viewModel.NIPC))
            {
                if (!allowedPrefixesNIPC.Contains(viewModel.NIPC.Trim().Substring(0, 1)))
                {
                    var validationMessage = "NIPC requer que o primeiro dígito seja 5, 6, 7, 8 ou 9.";
                    ModelState.AddModelError("NIPC", validationMessage);
                }

				if (_context.Agrupamento.Any(a => a.NIPC == viewModel.NIPC))
				{
					var validationMessage = "Outro Agrupamento já contém este NIPC";
					ModelState.AddModelError("NIPC", validationMessage);
				}
			}

			if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var userAccount = await _context.ContaAdministrativa
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(m => m.UserId == user.Id);

                var agrupamento = new Agrupamento
                {
                    AgrupamentoID = Guid.NewGuid(),
                    Name = viewModel.Name,
                    Location = viewModel.Location,
                    NIPC = viewModel.NIPC,
                    ContaId = userAccount?.ContaId
                };

                string fileName = UploadedFile(viewModel.LogoPicture);

                if (fileName != null)
                {
                    agrupamento.LogoPicture = fileName;
                } 
                else
                {
                    agrupamento.LogoPicture = "logo.png";
                }

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

            if (!string.IsNullOrEmpty(viewModel.NIPC))
            {
				if (!allowedPrefixesNIPC.Contains(viewModel.NIPC.Trim().Substring(0, 1)))
				{
					var validationMessage = "NIPC requer que o primeiro dígito seja 5, 6, 7, 8 ou 9.";
					ModelState.AddModelError("NIPC", validationMessage);
				}

				var agrupamento = _context.Agrupamento.Find(viewModel.AgrupamentoId);
				if (agrupamento.NIPC != viewModel.NIPC &&
					_context.Agrupamento.Any(a => a.NIPC == viewModel.NIPC))
				{
					var validationMessage = "Outro Agrupamento já contém este NIPC";
					ModelState.AddModelError("NIPC", validationMessage);
				}
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

					    string fileName = UploadedFile(viewModel.LogoPicture);
                        if (fileName != null)
                        {
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
				string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/uploads/agrupamentos");
				uniqueFileName = Guid.NewGuid().ToString() + "_" + logoPicture.FileName;
				string filePath = Path.Combine(uploadsFolder, uniqueFileName);
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					logoPicture.CopyTo(fileStream);
				}
			}
			return uniqueFileName;
		}

		[HttpGet]
		public async Task<IActionResult> GetATLsByAgrupamento(Guid agrupamentoId)
		{
			var atls = await _context.ATL
				.Where(a => a.AgrupamentoId == agrupamentoId)
				.Select(a => new
				{
					a.AtlId,
					a.Name,
					a.Address,
					a.City,
					a.PostalCode
				})
				.ToListAsync();

			return Json(atls);
		}
        
		[HttpGet]
		public async Task<IActionResult> GetCoordenadoresByATL(Guid atlId)
		{
			var coordenadores = await _context.ContaAdministrativa
				.Include(ca => ca.User)
				.Where(ca => ca.AtlId == atlId)
				.Select(ca => new {
					FirstName = ca.User.FirstName,
					LastName = ca.User.LastName
				})
				.ToListAsync();

			return Json(new { Coordenadores = coordenadores });
		}



	}
}
