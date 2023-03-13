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

namespace ATLManager.Controllers
{
    public class RefeicoesController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RefeicoesController(ATLManagerAuthContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Refeicoes
        public async Task<IActionResult> Index()
        {
              return View(await _context.Refeicao.ToListAsync());
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
            string fileName = UploadedFile(viewModel.Picture);

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
                Lipidos = viewModel.Lipidos\,
                ValorEnergetico = viewModel.ValorEnergetico,
                AGSat = viewModel.AGSat,
                Sal = viewModel.Sal,
                Picture = fileName
            };

            _context.Add(refeicao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
                        refeicao.Data = viewModel.Data;
                        refeicao.Descricao = viewModel.Descricao;
                        refeicao.Proteina = viewModel.Proteina;
                        refeicao.HidratosCarbono = viewModel.HidratosCarbono;
                        refeicao.VR = viewModel.VR;
                        refeicao.Acucar = viewModel.Acucar;
                        refeicao.Lipidos = viewModel.Lipidos;
                        refeicao.ValorEnergetico = viewModel.ValorEnergetico;
                        refeicao.AGSat = viewModel.AGSat;
                        refeicao.Sal = viewModel.Sal;


                        string fileName = UploadedFile(viewModel.Picture);
                        refeicao.Picture = fileName;

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
