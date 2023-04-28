using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.Models;
using ATLManager.Areas.Identity.Data;
using ATLManager.ViewModels;
using Microsoft.AspNetCore.Identity;
using ATLManager.Models.Historicos;

namespace ATLManager.Controllers
{
    /// <summary>
    /// Controlador para o modelo 'Históricos Funcionários'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class FuncionariosController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly IUserStore<ATLManagerUser> _userStore;
        private readonly IUserEmailStore<ATLManagerUser> _emailStore;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FuncionariosController(ATLManagerAuthContext context, 
            UserManager<ATLManagerUser> userManager,
            IUserStore<ATLManagerUser> userStore,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Retorna uma visualização que lista os funcionários da ATL .
        /// </summary>
        /// <returns>Uma visualização que lista os funcionários da ATL.</returns>

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            var usersFuncionarios = from user in _context.Users
                                     join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                     join role in _context.Roles on userRole.RoleId equals role.Id
                                     where role.Name == "Funcionario"
                                     select user;

			var funcionarios = from user in usersFuncionarios
							   join profile in _context.ContaAdministrativa on user.Id equals profile.UserId
                               join atl in _context.ATL on profile.AtlId equals atl.AtlId
                               where atl.AtlId == currentUserAccount.AtlId
							   select new LowerAccountViewModel
							   {
                                   User = user,
                                   Profile = profile,
                                   AtlName = atl.Name
							   };

			return View(funcionarios);
        }

        /// <summary>
        /// Retorna uma visualização que exibe detalhes de um funcionário específico.
        /// </summary>
        /// <param name="id">O ID da conta do funcionário.</param>
        /// <returns>Uma visualização que exibe detalhes de um funcionário específico.</returns>

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.ContaAdministrativa == null)
            {
                return NotFound();
            }

            var funcionario = from user in _context.Users
                              join profile in _context.ContaAdministrativa on user.Id equals profile.UserId
                              join atl in _context.ATL on profile.AtlId equals atl.AtlId
                              where profile.ContaId == id
                              select new LowerAccountViewModel
                              {
                                  User = user,
                                  Profile = profile,
                                  AtlName = atl.Name
                              };

            if (funcionario == null)
            {
                return NotFound();
            }

            return View(await funcionario.FirstAsync());
        }

        /// <summary>
        /// Retorna uma visualização para criar um novo funcionário.
        /// </summary>
        /// <returns>Uma visualização para criar um novo funcionário.</returns>

        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var userAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            if (userAccount == null)
            {
                return NotFound();
            }

            var atls = await _context.ATL.Where(a => a.AtlId == userAccount.AtlId).ToListAsync();

            ViewData["AtlId"] = new SelectList(atls, "AtlId", "Name");

			return View(new FuncionarioCreateViewModel());
        }

        /// <summary>
        /// Cria um novo funcionário com base nas informações fornecidas pelo utilizador e adiciona-o.
        /// </summary>
        /// <param name="viewModel">Os dados do novo funcionário.</param>
        /// <returns>Redireciona para a ação Index se o funcionário for criado com sucesso. Retorna a visualização Create com mensagens de erro, caso contrário.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FuncionarioCreateViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.CC))
            {
                if (_context.ContaAdministrativa.Any(c => c.CC == viewModel.CC)
                    || _context.Educando.Any(e => e.CC == viewModel.CC))
                {
                    var validationMessage = "Outro Agrupamento já contém este CC";
                    ModelState.AddModelError("CC", validationMessage);
                }
            }

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var userAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            if (userAccount == null)
            {
                return NotFound();
            }

            // Obter a data de nascimento do ViewModel
            DateTime dataNascimento = viewModel.DateOfBirth;

            // Calcular a diferença entre a data de nascimento e a data atual
            TimeSpan diferenca = DateTime.Today - dataNascimento;

            // Verifique se a diferença em anos é maior ou igual a 18
            if (diferenca.TotalDays / 365.25 < 18)
            {
                var validationMessage = "A idade mínima para registar um funcionário é 18 anos";
                ModelState.AddModelError("DateOfBirth", validationMessage);
            }

            if (ModelState.IsValid)
            {
                // Criar user
                var user = CreateUser();
                user.FirstName = viewModel.FirstName;
                user.LastName = viewModel.LastName;
                await _userStore.SetUserNameAsync(user, viewModel.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, viewModel.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, viewModel.Password);

                if (result.Succeeded)
                {
                    // Dar role de funcionario à conta
                    await _userManager.AddToRoleAsync(user, "Funcionario");

                    // Criar o perfil
                    var funcionario = new ContaAdministrativa(user, atlId: (Guid)userAccount.AtlId, viewModel.DateOfBirth, viewModel.CC);

                    string fileName = UploadedFile(viewModel.ProfilePicture);
					if (fileName != null)
					{
						funcionario.ProfilePicture = fileName;
					}
					else
					{
						funcionario.ProfilePicture = "logo.png";
					}
					_context.Add(funcionario);
                    await _context.SaveChangesAsync();
                
                    // Ativar a conta
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await _userManager.ConfirmEmailAsync(user, code);

                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(viewModel);
        }

        /// <summary>
        /// Método que edita um funcionário com o ID fornecido.
        /// </summary>
        /// <param name="id">O ID do funcionário a ser editado.</param>
        /// <returns>Retorna uma tarefa que representa a operação assíncrona.</returns>

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.ContaAdministrativa == null)
            {
                return NotFound();
            }

            var funcionario = from user in _context.Users
                              join profile in _context.ContaAdministrativa on user.Id equals profile.UserId
                              join atl in _context.ATL on profile.AtlId equals atl.AtlId
                              where profile.ContaId == id
                              select new FuncionarioEditViewModel
                              {
                                  ContaId = profile.ContaId,
                                  FirstName = user.FirstName,
                                  LastName = user.LastName,
                                  DateOfBirth = profile.DateOfBirth.ToShortDateString(),
                                  CC = profile.CC,
                                  Email = user.Email
                              };

            if (funcionario == null)
            {
                return NotFound();
            }

            return View(await funcionario.FirstAsync());
        }

        /// <summary>
        /// Método que edita um coordenador com o ID fornecido.
        /// </summary>
        /// <param name="id">O ID do coordenador a ser editado.</param>
        /// <param name="viewModel">O objeto ViewModel contendo as informações do coordenador.</param>
        /// <returns>Retorna uma tarefa que representa a operação assíncrona.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CoordenadorEditViewModel viewModel)
        {
            if (id != viewModel.ContaId)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(viewModel.CC))
            {
				var funcionario = _context.ContaAdministrativa.Find(viewModel.ContaId);

				if (funcionario.CC != viewModel.CC &&
					_context.ContaAdministrativa.Any(c => c.CC == viewModel.CC)
                    || _context.Educando.Any(e => e.CC == viewModel.CC))
                {
                    var validationMessage = "Outra conta já contém este CC";
                    ModelState.AddModelError("CC", validationMessage);
                }
            }


            // Obter a data de nascimento do ViewModel
            DateTime dataNascimento = DateTime.Parse(viewModel.DateOfBirth);

            // Calcular a diferença entre a data de nascimento e a data atual
            TimeSpan diferenca = DateTime.Today - dataNascimento;

            // Verifique se a diferença em anos é maior ou igual a 18
            if (diferenca.TotalDays / 365.25 < 18)
            {
                var validationMessage = "A idade mínima para registar um funcionário é 18 anos";
                ModelState.AddModelError("DateOfBirth", validationMessage);
            }


            if (ModelState.IsValid)
            {
                try
                {
                    var funcionario = await _context.ContaAdministrativa.FindAsync(id);
                    
                    if (funcionario != null)
                    {
                        if (viewModel.DateOfBirth != null)
                        {
                            funcionario.DateOfBirth = DateTime.Parse(viewModel.DateOfBirth);
                        }
                        funcionario.CC = viewModel.CC;

                        string fileName = UploadedFile(viewModel.ProfilePicture);
						if (fileName != null)
						{
							funcionario.ProfilePicture = fileName;
						}

						_context.Update(funcionario);
                        await _context.SaveChangesAsync();

                        var user = await _userManager.FindByIdAsync(funcionario.UserId);

                        user.FirstName = viewModel.FirstName;
                        user.LastName = viewModel.LastName;

                        await _userStore.SetUserNameAsync(user, viewModel.Email, CancellationToken.None);
                        await _emailStore.SetEmailAsync(user, viewModel.Email, CancellationToken.None);
                        await _userManager.UpdateAsync(user);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContaAdministrativaExists(viewModel.ContaId))
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
        /// Método que exclui um funcionário com o ID fornecido.
        /// </summary>
        /// <param name="id">O ID do funcionário a ser excluído.</param>
        /// <returns>Retorna uma tarefa que representa a operação assíncrona.</returns>

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.ContaAdministrativa == null)
            {
                return NotFound();
            }

            var funcionario = from user in _context.Users
                              join profile in _context.ContaAdministrativa on user.Id equals profile.UserId
                              join atl in _context.ATL on profile.AtlId equals atl.AtlId
                              where profile.ContaId == id
                              select new LowerAccountViewModel
                              {
                                  User = user,
                                  Profile = profile,
                                  AtlName = atl.Name
                              };

            if (funcionario == null)
            {
                return NotFound();
            }

            return View(await funcionario.FirstAsync());
        }

        /// <summary>
        /// Confirma a exclusão de uma conta administrativa.
        /// </summary>
        /// <param name="id">O ID da conta administrativa a ser excluída.</param>
        /// <returns>Um objeto IActionResult.</returns>

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.ContaAdministrativa == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.ContaAdministrativa' is null.");
            }

            var funcionario = await _context.ContaAdministrativa
                .Include(f => f.User)
                .Where(f => f.ContaId == id)
                .FirstAsync();

            if (funcionario != null)
            {
                var record = new FuncionarioRecord()
                {
                    ContaId = funcionario.ContaId,
                    FirstName = funcionario.User.FirstName,
                    LastName = funcionario.User.LastName,
                    Email = funcionario.User.Email,
                    DateOfBirth = funcionario.DateOfBirth,
                    CC = funcionario.CC,
                    ProfilePicture = funcionario.ProfilePicture,
                    AtlId = funcionario.AtlId,
                };

                var user = await _userManager.FindByIdAsync(funcionario.UserId);
                _context.Add(record);
                _context.ContaAdministrativa.Remove(funcionario);
                await _userManager.DeleteAsync(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se uma conta administrativa existe.
        /// </summary>
        /// <param name="id">O ID da conta administrativa a ser verificada.</param>
        /// <returns>True se a conta existir, false caso contrário.</returns>

        private bool ContaAdministrativaExists(Guid id)
        {
          return _context.ContaAdministrativa.Any(e => e.ContaId == id);
        }

        /// <summary>
        /// Cria uma nova instância de ATLManagerUser.
        /// </summary>
        /// <returns>Um objeto ATLManagerUser.</returns>
        private ATLManagerUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ATLManagerUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ATLManagerUser)}'. " +
                    $"Ensure that '{nameof(ATLManagerUser)}' is not an abstract class and has a parameterless constructor");
            }
        }

        /// <summary>
        /// Obtém o IUserEmailStore correspondente a ATLManagerUser.
        /// </summary>
        /// <returns>Um objeto IUserEmailStore&lt;ATLManagerUser;.</returns>

        private IUserEmailStore<ATLManagerUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ATLManagerUser>)_userStore;
        }

        /// <summary>
        /// Método que faz o upload de um arquivo enviado via formulário web para a pasta de uploads de imagens de funcionários.
        /// </summary>
        /// <param name="logoPicture">O arquivo enviado pelo formulário web.</param>
        /// <returns>O nome único do arquivo gerado após o upload.</returns>
        private string UploadedFile(IFormFile logoPicture)
        {
            string uniqueFileName = null;

            if (logoPicture != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/uploads/funcionarios");
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
