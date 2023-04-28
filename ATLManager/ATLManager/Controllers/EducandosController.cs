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
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using ATLManager.Models.Historicos;
using ATLManager.Services;

namespace ATLManager.Controllers
{
    /// <summary>
    /// Controlador para o modelo Educando.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class EducandosController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileManager _fileManager;

        private readonly string FolderName = "educandos";

        public EducandosController(ATLManagerAuthContext context,
            UserManager<ATLManagerUser> userManager,
            IWebHostEnvironment webHostEnvironment,
            IFileManager fileManager)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _fileManager = fileManager;
        }

        // GET: Educandos
        /// <summary>
        /// Obtem uma lista de 'Educandos' e exibe a informação obtida numa view.
        /// Quando um utilizador tem o role de 'Coordenador', obtem os 'Educandos' do 'ATL' ao qual está registado e, 
        /// quando tem o role de 'EncarregadoEducacao', obtem os 'Educandos' aos quais lhe estão associados (ex. filhos).
        /// </summary>
        /// <returns>Uma view com a lista de educandos obtidos pelas queries.</returns>
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (HttpContext.User.IsInRole("Coordenador") || HttpContext.User.IsInRole("Funcionario"))
            {
                var currentUserAccount = await _context.ContaAdministrativa
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

                var educandos = await _context.Educando
                    .Include(e => e.Atl)
                    .Include(e => e.Encarregado)
                    .Include(e => e.Encarregado.User)
                    .Where(e => e.AtlId == currentUserAccount.AtlId)
                    .ToListAsync();

                return View(educandos);
            }
            else
            {
                var currentUserAccount = await _context.EncarregadoEducacao
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

                var educandos = await _context.Educando
                    .Include(e => e.Atl)
					.Include(e => e.Encarregado)
					.Include(e => e.Encarregado.User)
					.Where(e => e.EncarregadoId == currentUserAccount.EncarregadoId)
					.ToListAsync();

                return View(educandos);
            }
        }

        // GET: Educandos/Details/5
        /// <summary>
        /// Obtem os detalhes de um Educando e exibe a informação numa view.
        /// </summary>
        /// <param name="id">O Id do Educando a vizualizar.</param>
        /// <returns>Uma view 'Details' com a informação do Educando.</returns>
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Educando == null)
            {
                return NotFound();
            }

            var educando = await _context.Educando
                .Include(e => e.Atl)
                .Include(e => e.Encarregado)
                .Include(e => e.Encarregado.User)
                .FirstOrDefaultAsync(m => m.EducandoId == id);
            if (educando == null)
            {
                return NotFound();
            }

            return View(educando);
        }

		// GET: EducandoSaude/DetailsSaude/5
		/// <summary>
		/// Obtem os detalhes de saúde de um Educando e exibe a informação numa view.
		/// </summary>
		/// <param name="id">O Id do Educando a vizualizar</param>
		/// <returns>Uma view 'DetailsSaude' com a informação de 'EducandoSaude' de um Educando.</returns>
		public async Task<IActionResult> DetailsSaude(Guid? id)
		{
			if (id == null || _context.EducandoSaude == null)
			{
				return NotFound();
			}

			var educandoSaude = await _context.EducandoSaude
				.Include(e => e.Educando)
				.FirstOrDefaultAsync(m => m.EducandoId == id);
			if (educandoSaude == null)
			{
				return NotFound();
			}

			return View(educandoSaude);
		}


        /// Recebe o post de um form para atualizar os detalhes de saúde de um Educando e guarda
        /// a atualização na base de dados.
        /// </summary>
        /// <param name="id">O Id do Educando para o qual atualizar as informações de saúde na 'EducandoSaude'.</param>
        /// <param name="educandoSaude">O objeto "EducandoSaude" com os detlahes atualizados, submetido pelo form.</param>
        /// <returns>
        /// Se os dados do form forem válidos e o objeto for atualizado com sucesso, redireciona para a página 'Details'.
        /// Se os dados não forem válidos, redireciona para a mesma página.
        /// </returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> DetailsSaude(Guid id, [Bind("EducandoSaudeId,BloodType,EmergencyContact,InsuranceName,InsuranceNumber,Allergies,Diseases,Medication,MedicalHistory,EducandoId")] EducandoSaude educandoSaude)
        {
            if (id != educandoSaude.EducandoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(educandoSaude);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EducandoSaudeExists(educandoSaude.EducandoSaudeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id });
            }
            return View(educandoSaude);
        }


        // GET: EducandoSaude/DetailsResponsaveis/5
        /// <summary>
        /// Obtem uma lista dos responsáveis de um Educando e exibe a informação numa view.
        /// </summary>
        /// <param name="id">O Id do Educando a vizualizar</param>
        /// <returns>
        /// Uma view 'DetailsResponsaveis' com uma lista de 'EducandoResponsaveis' de um 'Educando
        /// </returns>

        public async Task<IActionResult> DetailsResponsaveis(Guid? id)
		{
			if (id == null || _context.Educando == null)
			{
				return NotFound();
			}

            var responsaveis = await _context.EducandoResponsavel
                .Include(e => e.Educando)
                .Where(e => e.EducandoId == id)
                .ToListAsync();

			if (responsaveis == null)
			{
				return NotFound();
			}

            ViewData["EducandoId"] = id;
			return View(responsaveis);
		}


        // GET: EducandoSaude/DetailsEncarregado/5
        /// <summary>
        /// Obtem os detalhes do Encarregado de Educação de um Educando e exibe a informação numa view.
        /// </summary>
        /// <param name="id">O Id do Educando do qual verificar o Encarregado de Educação.</param>
        /// <returns>Uma view 'DetailsEncarregado' com a informação do Encarregado de Educação.</returns>

        public async Task<IActionResult> DetailsEncarregado(Guid? id)
		{
			if (id == null || _context.EducandoSaude == null)
			{
				return NotFound();
			}

            var educando = await _context.Educando
                .Where(e => e.EducandoId == id)
                .FirstOrDefaultAsync();

			if (educando == null)
			{
				return NotFound();
			}

			var educandoEncarregado = await _context.EncarregadoEducacao
				.Include(e => e.User)
				.FirstOrDefaultAsync(e => e.EncarregadoId == educando.EncarregadoId);

			if (educandoEncarregado == null)
			{
				return NotFound();
			}

			ViewData["EducandoId"] = id;
			return View(educandoEncarregado);
		}


        /// <summary>
        /// Obtém uma lista de utilizadores EncarregadoEducacao.
        /// </summary>
        /// <returns>Uma lista de utilizadores EncarregadoEducacao.</returns>
        private async Task<List<EncarregadoEducacao>> GetEncarregadosAsync()
        {
            var encarregados = await _context.EncarregadoEducacao
                .Include(e => e.User)
                .ToListAsync();

            return encarregados;
        }



        /// <summary>
        /// Obtém uma lista de utilizadores EncarregadoEducacao filtrada por um termo de pesquisa.
        /// </summary>
        /// <param name="searchTerm">O termo de pesquisa.</param>
        /// <returns>Uma lista de utilizadores EncarregadoEducacao filtrada pelo termo de pesquisa.</returns>

        [HttpGet]
        public async Task<IActionResult> GetEncarregados(string searchTerm)
        {
            var allEncarregados = await GetEncarregadosAsync();

            var filteredEncarregados = allEncarregados
                .Where(e => e.User.FirstName.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase) || e.User.LastName.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase) || e.NIF.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(e => new { id = e.EncarregadoId, firstName = e.User.FirstName, lastName = e.User.LastName, nif = e.NIF })
                .ToList();

            return Json(filteredEncarregados);
        }





        /// <summary>
        /// Exibe o formulário para criar um novo Educando.
        /// </summary>
        /// <returns>O resultado da execução da tarefa.</returns>

        // GET: Educandos/Create
        public async Task<IActionResult> Create()
        {
			return View(new EducandoCreateViewModel());
        }

        /// <summary>
        /// Cria um novo Educando.
        /// </summary>
        /// <param name="viewModel">O objeto EducandoCreateViewModel que contém as informações do novo Educando.</param>
        /// <returns>O resultado da execução da tarefa.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        /// <summary>
        /// Cria um novo Educando a partir do modelo EducandoCreateViewModel.
        /// </summary>
        /// <param name="viewModel">O ViewModel que contém os dados do novo Educando.</param>
        /// <returns>Um objeto IActionResult redirecionando para a ação Index se o novo Educando foi criado com sucesso, 
        /// caso contrário, retorna a View com o modelo EducandoCreateViewModel e os dados da ViewData.</returns>
        public async Task<IActionResult> Create(EducandoCreateViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.CC))
            {
                if (_context.ContaAdministrativa.Any(c => c.CC == viewModel.CC)
                    || _context.Educando.Any(e => e.CC == viewModel.CC))
                {
                    var validationMessage = "Outro Educando já contém este CC";
                    ModelState.AddModelError("CC", validationMessage);
                }
            }

            if (DateTime.Compare(viewModel.BirthDate, DateTime.UtcNow.AddYears(-3)) >= 0)
            {
                var validationMessage = "O Educando não pode ter menos de 3 anos de idade";
                ModelState.AddModelError("BirthDate", validationMessage);
            }

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var userAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            if (userAccount == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var educando = new Educando
                {
                    EducandoId = Guid.NewGuid(),
                    Name = viewModel.Name,
                    Apelido = viewModel.Apelido,
                    CC = viewModel.CC,
                    Genero = viewModel.Genero,
                    AtlId = (Guid)userAccount.AtlId,
                    EncarregadoId = viewModel.EncarregadoId,
                    BirthDate = viewModel.BirthDate,
                };

				string photoFileName = _fileManager.UploadFile(viewModel.ProfilePicture, FolderName);

				if (photoFileName != null)
				{
					educando.ProfilePicture = photoFileName;
				}
				else
				{
					educando.ProfilePicture = "logo.png";
				}

                string boletinFileName = _fileManager.UploadFile(viewModel.BoletimVacinas, FolderName);

                if (photoFileName != null)
                {
                    educando.BoletimVacinas = boletinFileName;
                }

                string declaracaoFileName = _fileManager.UploadFile(viewModel.DeclaracaoMedica, FolderName);

                if (photoFileName != null)
                {
                    educando.DeclaracaoMedica = declaracaoFileName;
                }

                var educandoSaude = new EducandoSaude(educando.EducandoId);

                _context.Add(educando);
                _context.Add(educandoSaude);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

			var encarregados = await _context.EncarregadoEducacao
				.Include(e => e.User)
				.Select(e => new { e.EncarregadoId, Name = (e.User.FirstName + " " + e.User.LastName) })
				.ToListAsync();

			ViewData["EncarregadoId"] = new SelectList(encarregados, "EncarregadoId", "Name", viewModel.EncarregadoId);
            return View(viewModel);
        }


        /// <summary>
        /// Método assíncrono que edita um educando com o ID especificado.
        /// </summary>
        /// <param name="id">O ID do educando a ser editado.</param>
        /// <returns>Uma instância de Task que representa a operação assíncrona.</returns>

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

            var encarregados = await _context.EncarregadoEducacao
                .Include(e => e.User)
                .Select(e => new { e.EncarregadoId, Name = (e.User.FirstName + " " + e.User.LastName)})
                .ToListAsync();
            
            ViewData["EncarregadoId"] = new SelectList(encarregados, "EncarregadoId", "Name", educando.EncarregadoId);
            return View(new EducandoEditViewModel(educando));
        }

        /// <summary>
        /// Método assíncrono que edita um educando com o ID e os dados do modelo de visualização especificados.
        /// </summary>
        /// <param name="id">O ID do educando a ser editado.</param>
        /// <param name="viewModel">O modelo de visualização contendo os dados do educando a serem atualizados.</param>
        /// <returns>Uma instância de Task que representa a operação assíncrona.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EducandoEditViewModel viewModel)
        {
            if (id != viewModel.EducandoId)
            {
                return NotFound();
            }

			if (!string.IsNullOrEmpty(viewModel.CC))
			{
				var educando = _context.Educando.Find(viewModel.EducandoId);

				if (educando.CC != viewModel.CC &&
                    _context.Educando.Any(e => e.CC == viewModel.CC) ||
					_context.ContaAdministrativa.Any(c => c.CC == viewModel.CC)
)
				{
					var validationMessage = "Outra conta já contém este CC";
					ModelState.AddModelError("CC", validationMessage);
				}
			}

            if (viewModel.BirthDate != null)
            {
                if (DateTime.Compare((DateTime)viewModel.BirthDate, DateTime.UtcNow.AddYears(-3)) > 0)
                {
                    var validationMessage = "O Educando não pode ter menos de 3 anos de idade";
                    ModelState.AddModelError("BirthDate", validationMessage);
                }
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
                        educando.EncarregadoId = viewModel.EncarregadoId;
                        
                        if (viewModel.BirthDate != null)
                            educando.BirthDate = (DateTime)viewModel.BirthDate;

						string photoFileName = _fileManager.UploadFile(viewModel.ProfilePicture, FolderName);
                        if (photoFileName != null)
						{
							educando.ProfilePicture = photoFileName;
						}

                        string boletinFileName = _fileManager.UploadFile(viewModel.BoletimVacinas, FolderName);

                        if (boletinFileName != null)
                        {
                            educando.BoletimVacinas = boletinFileName;
                        }

                        string declaracaoFileName = _fileManager.UploadFile(viewModel.DeclaracaoMedica, FolderName);

                        if (declaracaoFileName != null)
                        {
                            educando.DeclaracaoMedica = declaracaoFileName;
                        }

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
			var encarregados = await _context.EncarregadoEducacao
				.Include(e => e.User)
				.Select(e => new { e.EncarregadoId, Name = (e.User.FirstName + " " + e.User.LastName) })
				.ToListAsync();

			ViewData["EncarregadoId"] = new SelectList(encarregados, "EncarregadoId", "Name", viewModel.EncarregadoId);
			return View(viewModel);
        }

        /// <summary>
        /// Método assíncrono que remove um educando da base de dados.
        /// </summary>
        /// <param name="id">O identificador do educando a ser removido.</param>
        /// <returns>Um objeto IActionResult representando o resultado da operação.</returns>


        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Educando == null)
            {
                return NotFound();
            }

            var educando = await _context.Educando
                .Include(e => e.Atl)
                .Include(e => e.Encarregado)
                .Include(e => e.Encarregado.User)
                .FirstOrDefaultAsync(m => m.EducandoId == id);

            if (educando == null)
            {
                return NotFound();
            }
            
            return View(educando);
        }

        /// <summary>
        /// Método assíncrono que remove um educando da base de dados após confirmação.
        /// </summary>
        /// <param name="id">O identificador do educando a ser removido.</param>
        /// <returns>Um objeto IActionResult representando o resultado da operação.</returns>

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Educando == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.Educando'  is null.");
            }
            var educando = await _context.Educando
                .Include(f => f.Encarregado)
                .Include(f => f.Encarregado.User)
                .Where(f => f.EducandoId == id)
                .FirstAsync();
            if (educando != null)
            {
                var record = new EducandoRecord()
                {
                    EducandoId = educando.EducandoId,
                    Name = educando.Name,
                    Apelido = educando.Apelido,
                    CC = educando.CC,
                    Genero = educando.Genero,
                    ProfilePicture = educando.ProfilePicture,
                    Encarregado = educando.Encarregado.User.FirstName + " " + educando.Encarregado.User.LastName,
                    AtlId = educando.AtlId,
                };

                _context.Add(record);
                _context.Educando.Remove(educando);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se existe um Educando com o Id especificado.
        /// </summary>
        /// <param name="id">O Id do Educando a ser verificado.</param>
        /// <returns>True se o Educando existir, caso contrário, False.</returns>

        private bool EducandoExists(Guid id)
        {
            return _context.Educando.Any(e => e.EducandoId == id);
        }

        /// <summary>
        /// Verifica se existe um registro de saúde de um Educando com o Id especificado.
        /// </summary>
        /// <param name="id">O Id do Educando a ser verificado.</param>
        /// <returns>True se o registro de saúde existir, caso contrário, False.</returns>

        private bool EducandoSaudeExists(Guid id)
        {
            return _context.EducandoSaude.Any(e => e.EducandoId == id);
        }

        /// <summary>
        /// Faz o download de um arquivo com o nome especificado da pasta de uploads de imagens de educandos.
        /// </summary>
        /// <param name="fileName">O nome do arquivo a ser baixado.</param>
        /// <returns>Um objeto FileResult contendo o arquivo baixado.</returns>

        public IActionResult Download(string fileName)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, @"files\users\educandos");
            string filePath = Path.Combine(uploadsFolder, fileName);
            string fileCleanName = fileName[(fileName.IndexOf("_id_") + 4)..];
            return File(System.IO.File.ReadAllBytes(filePath), "application/pdf", fileCleanName);
        }
    }
}
