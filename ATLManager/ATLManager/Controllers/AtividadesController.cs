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
using ATLManager.Services;

namespace ATLManager.Controllers
{
    /// <summary>
    /// Controlador para o modelo 'Atividades'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class AtividadesController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly IFileManager _fileManager;

        private readonly string FolderName = "atividades";

        public AtividadesController(ATLManagerAuthContext context, 
            UserManager<ATLManagerUser> userManager,
            IFileManager fileManager)
        {
            _context = context;
            _userManager = userManager;
            _fileManager = fileManager;
        }

        /// <summary>
        /// Retorna uma lista de atividades dependendo das permissões do utilizador.
        /// </summary>
        /// <returns>Retorna uma IActionResult contendo uma lista de atividades.</returns>

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

        /// <summary>
        /// Retorna uma View com os detalhes de uma atividade específica.
        /// </summary>
        /// <param name="id">O ID da atividade.</param>
        /// <returns>Retorna uma IActionResult contendo uma View com os detalhes da atividade.</returns>

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

        /// <summary>
        /// Cria uma nova instância da classe IActionResult que representa a ação de criar.
        /// </summary>
        /// <returns>Uma nova instância da classe IActionResult que representa a ação de criar.</returns>

        public IActionResult Create()
        {
            return View(new AtividadeCreateViewModel());
        }

        /// <summary>
        /// Cria uma nova Atividade com base nas informações fornecidas pelo utilizador.
        /// </summary>
        /// <param name="viewModel">O ViewModel contendo as informações da atividade a ser criada.</param>
        /// <returns>Uma instância de IActionResult que redireciona para a página Index em caso de sucesso,
        /// ou exibe a página de criação novamente em caso de falha na validação do modelo.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AtividadeCreateViewModel viewModel)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);;

            if (viewModel.StartDate.CompareTo(DateTime.UtcNow) < 0)
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
                string fileName = _fileManager.UploadFile(viewModel.Picture, FolderName);

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

        // <summary>
        /// Método responsável por exibir a view de edição de uma atividade
        /// </summary>
        /// <param name="id">Id da atividade a ser editada</param>
        /// <returns>Retorna uma View com o ViewModel preenchido com os dados da atividade a ser editada</returns>
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

        /// <summary>
        /// Action para editar uma atividade existente
        /// </summary>
        /// <param name="id">Id da atividade</param>
        /// <param name="viewModel">Modelo de visualização para editar uma atividade</param>
        /// <returns>ActionResult</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("AtividadeId,Name,StartDate,EndDate,Description,Picture")] AtividadeEditViewModel viewModel)
        {
            if (id != viewModel.AtividadeId)
            {
                return NotFound();
            }
            
			if (viewModel.StartDate != null)
			{
				if (((DateTime)viewModel.StartDate).CompareTo(DateTime.UtcNow) < 0)
				{
					var validationMessage = "Não é possível criar uma Atividade com uma data anterior à data atual";
					ModelState.AddModelError("StartDate", validationMessage);
				}
			}

			if (viewModel.EndDate.HasValue && viewModel.StartDate.HasValue && viewModel.EndDate.Value < viewModel.StartDate.Value)
			{
				var validationMessage = "Não é possível criar uma Atividade com uma data de término anterior à data de início";
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

                string fileName = _fileManager.UploadFile(viewModel.Picture, FolderName);

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

        /// <summary>
        /// Action para excluir uma atividade
        /// </summary>
        /// <param name="id">Id da atividade</param>
        /// <returns>ActionResult</returns>

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

        /// <summary>
        /// Elimina uma actividade da base de dados.
        /// </summary>
        /// <param name="id">O ID da actividade a eliminar.</param>
        /// <returns>Uma IActionResult que representa o resultado da operação.</returns>

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

        /// <summary>
        /// Determina se existe na base de dados uma actividade com o ID especificado.
        /// </summary>
        /// <param name="id">O ID da actividade a verificar.</param>
        /// <returns>Verdadeiro se existir na base de dados uma actividade com o ID especificado; caso contrário, falso.</returns>
        p
        private bool AtividadeExists(Guid id)
        {
          return (_context.Atividade?.Any(e => e.AtividadeId == id)).GetValueOrDefault();
        }

        /// <summary>
        /// Carrega um ficheiro para o servidor e devolve o nome do ficheiro carregado.
        /// </summary>
        /// <param name="comprovativo">O ficheiro a carregar.</param>
        /// <returns>O nome do ficheiro carregado.</returns>

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
