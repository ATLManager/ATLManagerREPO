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
using ATLManager.Services;

namespace ATLManager.Controllers
{
    /// <summary>
    /// Controlador para o modelo 'Refeições'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class RefeicoesController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly IFileManager _fileManager;

        private readonly string FolderName = "refeicoes";

        public RefeicoesController(ATLManagerAuthContext context,
            UserManager<ATLManagerUser> userManager,
            IFileManager fileManager)
        {
            _context = context;
            _userManager = userManager;
            _fileManager = fileManager;
        }

        /// <summary>
        /// Retorna a visualização da lista de refeições com base no usuário atual.
        /// </summary>
        /// <returns>Ação do resultado de uma tarefa que representa a operação assíncrona.</returns>

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

                ViewBag.Educandos = educandos;
                return View(refeicoes);
			}
		}

        /// <summary>
        /// Retorna a visualização de detalhes de uma refeição com base no ID da refeição fornecido.
        /// </summary>
        /// <param name="id">O ID da refeição.</param>
        /// <returns>Ação do resultado de uma tarefa que representa a operação assíncrona.</returns>

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

        /// <summary>
        /// Retorna a visualização de criação de uma nova refeição.
        /// </summary>
        /// <returns>Ação do resultado.</returns>

        public IActionResult Create()
        {
            return View(new RefeicaoCreateViewModel());
        }

        /// <summary>
        /// Cria uma nova refeição com base nos dados fornecidos pelo ViewModel de criação de refeição.
        /// </summary>
        /// <param name="viewModel">O ViewModel de criação de refeição.</param>
        /// <returns>Ação do resultado de uma tarefa que representa a operação assíncrona.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RefeicaoCreateViewModel viewModel)
        {

            if ((viewModel.Data).CompareTo(DateTime.UtcNow) < 0)
            {
                var validationMessage = "Não é possível editar uma Visita de Estudo com uma data anterior à data atual";
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

                string fileName = _fileManager.UploadFile(viewModel.Picture, FolderName);

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

        /// <summary>
        /// Retorna a visualização de edição de uma refeição com base no ID da refeição fornecido.
        /// </summary>
        /// <param name="id">O ID da refeição.</param>
        /// <returns>Ação do resultado de uma tarefa que representa a operação assíncrona.</returns>

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

        /// <summary>
        /// Edita uma refeição existente com base nos dados fornecidos pelo ViewModel de edição de refeição.
        /// </summary>
        /// <param name="id">O ID da refeição.</param>
        /// <param name="viewModel">O ViewModel de edição de refeição.</param>
        /// <returns>Ação do resultado de uma tarefa que representa a operação assíncrona.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, RefeicaoEditViewModel viewModel)
        {
            if (id != viewModel.RefeicaoId)
            {
                return NotFound();
            }
            
            if (viewModel.Data != null)
            {
                if (((DateTime)viewModel.Data).CompareTo(DateTime.UtcNow) < 0)
                {
                    var validationMessage = "Não é possível editar uma Visita de Estudo com uma data anterior à data atual";
                    ModelState.AddModelError("Data", validationMessage);
                }
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
                            refeicao.Data = (DateTime)viewModel.Data;
                        }

                        string fileName = _fileManager.UploadFile(viewModel.Picture, FolderName);

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

        /// <summary>
        /// Este método processa o pedido para eliminar um objecto Refeicao com um determinado ID.
        /// </summary>
        /// <param name="id">O ID do objecto Refeicao a ser eliminado.</param>
        /// <returns>A vista que contém o objecto Refeicao a eliminar.</returns>
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

        /// <summary>
        /// Este método processa o pedido para eliminar um objecto Refeicao com um determinado ID.
        /// </summary>
        /// <param name="id">O ID do objecto Refeicao a ser eliminado.</param>
        /// <returns> Um redireccionamento para a vista do Índice após a eliminação ter sido concluída.</returns>
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

        /// <summary>
        /// Este método verifica se existe um objecto Refeicao na base de dados, com base no seu ID.
        /// </summary>
        /// <param name="id">O ID do objecto Refeicao a ser verificado.</param>
        /// <returns>True se o objecto Refeicao existir na base de dados, false caso contrário.</returns>
        private bool RefeicaoExists(Guid id)
        {
          return _context.Refeicao.Any(e => e.RefeicaoId == id);
        }

        [HttpGet]
        public async Task<IActionResult> GetRefeicoesByATLId(Guid atlid)
        {
            
            var refeicoes = await _context.Refeicao
                .Where(r => r.AtlId == atlid)
                .ToListAsync();

            return Json(refeicoes);
        }
    }
}
