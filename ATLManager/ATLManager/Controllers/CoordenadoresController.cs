using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.Models;
using ATLManager.Areas.Identity.Data;
using ATLManager.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using ATLManager.Services;

namespace ATLManager.Controllers
{
    /// <summary>
    /// Controlador para o modelo 'Coordenadores'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class CoordenadoresController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly IUserStore<ATLManagerUser> _userStore;
        private readonly IUserEmailStore<ATLManagerUser> _emailStore;
        private readonly IFileManager _fileManager;

        private readonly string FolderName = "coordenadores";

        public CoordenadoresController(ATLManagerAuthContext context, 
            UserManager<ATLManagerUser> userManager,
            IUserStore<ATLManagerUser> userStore,
            IFileManager fileManager)
        {
            _context = context;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _fileManager = fileManager;
        }

        /// <summary>
        /// Retorna a exibição da lista de coordenadores que pertencem ao ATL.
        /// </summary>
        /// <returns>Uma View com a lista de coordenadores.</returns>

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var userAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            var usersCoordenadores = from user in _context.Users
                                     join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                     join role in _context.Roles on userRole.RoleId equals role.Id
                                     where role.Name == "Coordenador"
                                     select user;

			var coordenadores = from user in usersCoordenadores
								join profile in _context.ContaAdministrativa on user.Id equals profile.UserId
                                join atl in _context.ATL on profile.AtlId equals atl.AtlId
                                join atlAdmin in _context.ATLAdmin on atl.AtlId equals atlAdmin.AtlId
                                where atlAdmin.ContaId == userAccount.ContaId
								select new LowerAccountViewModel
								{
                                    User = user,
                                    Profile = profile,
                                    AtlName = atl.Name
								};

			return View(coordenadores);
        }

        /// <summary>
        /// Retorna a exibição dos detalhes de um coordenador especificado pelo ID.
        /// </summary>
        /// <param name="id">O ID do coordenador.</param>
        /// <returns>Uma View com os detalhes do coordenador.</returns>

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.ContaAdministrativa == null)
            {
                return NotFound();
            }

            var coordenador = from user in _context.Users
                              join profile in _context.ContaAdministrativa on user.Id equals profile.UserId
                              join atl in _context.ATL on profile.AtlId equals atl.AtlId
                              where profile.ContaId == id
                              select new LowerAccountViewModel
                              {
                                  User = user,
                                  Profile = profile,
                                  AtlName = atl.Name
                              };

            if (coordenador == null)
            {
                return NotFound();
            }

            return View(await coordenador.FirstAsync());
        }

        /// <summary>
        /// Retorna a exibição do formulário de criação de um novo coordenador.
        /// </summary>
        /// <returns>Uma View com o formulário de criação.</returns>

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

            var atls = await (from atl in _context.ATL
                              join atlAdmin in _context.ATLAdmin on atl.AtlId equals atlAdmin.AtlId
                              join admin in _context.ContaAdministrativa on atlAdmin.ContaId equals admin.ContaId
                              where admin.ContaId == userAccount.ContaId
                              select atl).Include(a => a.Agrupamento).ToListAsync();

            ViewData["AtlId"] = new SelectList(atls, "AtlId", "Name");
			return View(new CoordenadorCreateViewModel());
        }

        /// <summary>
        /// Cria um novo coordenador com base nos dados fornecidos pelo formulário de criação.
        /// </summary>
        /// <param name="viewModel">O ViewModel contendo os dados do coordenador a ser criado.</param>
        /// <returns>Uma View com os detalhes do novo coordenador, se a criação for bem-sucedida, ou o formulário de criação com erros, caso contrário.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CoordenadorCreateViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.CC))
            {
                if (_context.ContaAdministrativa.Any(c => c.CC == viewModel.CC) 
                    || _context.Educando.Any(e => e.CC == viewModel.CC))
                {
                    var validationMessage = "Outro Coordenador já contém este CC";
                    ModelState.AddModelError("CC", validationMessage);
                }
            }

            // Obter a data de nascimento do ViewModel
            DateTime dataNascimento = viewModel.DateOfBirth;

            // Calcular a diferença entre a data de nascimento e a data atual
            TimeSpan diferenca = DateTime.Today - dataNascimento;

            // Verifique se a diferença em anos é maior ou igual a 18
            if (diferenca.TotalDays / 365.25 < 18)
            {
                var validationMessage = "A idade mínima para registar um coordenador é 18 anos";
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
					// Dar role de coordenador à conta
					await _userManager.AddToRoleAsync(user, "Coordenador");

                    // Aceder ao ATL pelo Id
                    var atl = await _context.ATL.Where(a => a.AtlId == viewModel.AtlId).FirstAsync();

                    // Criar o perfil
                    var coordenador = new ContaAdministrativa(user, atl, viewModel.DateOfBirth, viewModel.CC);

                    string fileName = _fileManager.UploadFile(viewModel.ProfilePicture, FolderName);
                    if (fileName != null)
                    {
                        coordenador.ProfilePicture = fileName;
                    }
                    else
                    {
                        coordenador.ProfilePicture = "logo.png";
                    }

                    _context.Add(coordenador);
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

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var userAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            if (userAccount == null)
            {
                return NotFound();
            }

            var atls = await (from atl in _context.ATL
                              join atlAdmin in _context.ATLAdmin on atl.AtlId equals atlAdmin.AtlId
                              join admin in _context.ContaAdministrativa on atlAdmin.ContaId equals admin.ContaId
                              where admin.ContaId == userAccount.ContaId
                              select atl).Include(a => a.Agrupamento).ToListAsync();

            ViewData["AtlId"] = new SelectList(atls, "AtlId", "Name", viewModel.AtlId);
            return View(viewModel);
        }

        /// <summary>
        /// Método de ação que exibe a tela de edição de um coordenador
        /// </summary>
        /// <param name="id">O id da conta do coordenador</param>
        /// <returns>Uma ActionResult</returns>

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.ContaAdministrativa == null)
            {
                return NotFound();
            }

            var coordenador = from user in _context.Users
                              join profile in _context.ContaAdministrativa on user.Id equals profile.UserId
                              join atl in _context.ATL on profile.AtlId equals atl.AtlId
                              where profile.ContaId == id
                              select new CoordenadorEditViewModel
                              {
                                  ContaId = profile.ContaId,
                                  FirstName = user.FirstName,
                                  LastName = user.LastName,
                                  AtlId = profile.AtlId.Value,
                                  DateOfBirth = profile.DateOfBirth.ToShortDateString(),
                                  CC = profile.CC,
                                  Email = user.Email
                              };

            if (coordenador == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var userAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            if (userAccount == null)
            {
                return NotFound();
            }

            var atls = await (from atl in _context.ATL
                              join atlAdmin in _context.ATLAdmin on atl.AtlId equals atlAdmin.AtlId
                              join admin in _context.ContaAdministrativa on atlAdmin.ContaId equals admin.ContaId
                              where admin.ContaId == userAccount.ContaId
                              select atl).Include(a => a.Agrupamento).ToListAsync();

            ViewData["AtlId"] = new SelectList(atls, "AtlId", "Name");
            return View(await coordenador.FirstAsync());
        }

        /// <summary>
        /// Método de ação que atualiza as informações de um coordenador
        /// </summary>
        /// <param name="id">O id da conta do coordenador</param>
        /// <param name="viewModel">O objeto view model com as informações do coordenador</param>
        /// <returns>Uma ActionResult</returns>

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
				var coordenador = _context.ContaAdministrativa.Find(id);

				if (coordenador.CC != viewModel.CC &&
					(_context.ContaAdministrativa.Any(c => c.CC == viewModel.CC)
                    || _context.Educando.Any(e => e.CC == viewModel.CC)))
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
                var validationMessage = "A idade mínima para registar um coordenador é 18 anos";
                ModelState.AddModelError("DateOfBirth", validationMessage);
            }



            if (ModelState.IsValid)
            {
                try
                {
                    var coordenador = await _context.ContaAdministrativa.FindAsync(id);
                    
                    if (coordenador != null)
                    {
                        if (viewModel.DateOfBirth != null)
                        {
						    coordenador.DateOfBirth = DateTime.Parse(viewModel.DateOfBirth);
                        }
                        coordenador.CC = viewModel.CC;
                        coordenador.AtlId = viewModel.AtlId;

                        string fileName = _fileManager.UploadFile(viewModel.ProfilePicture, FolderName);
                        if (fileName != null)
                        {
                            coordenador.ProfilePicture = fileName;
                        }

                        _context.Update(coordenador);
                        await _context.SaveChangesAsync();

                        var user = await _userManager.FindByIdAsync(coordenador.UserId);

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

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var userAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            if (userAccount == null)
            {
                return NotFound();
            }

            var atls = await (from atl in _context.ATL
                              join atlAdmin in _context.ATLAdmin on atl.AtlId equals atlAdmin.AtlId
                              join admin in _context.ContaAdministrativa on atlAdmin.ContaId equals admin.ContaId
                              where admin.ContaId == userAccount.ContaId
                              select atl).Include(a => a.Agrupamento).ToListAsync();

            ViewData["AtlId"] = new SelectList(atls, "AtlId", "Name", viewModel.AtlId);
            return View(viewModel);
        }

        /// <summary>
        /// Exclui um coordenador com base no ID fornecido.
        /// </summary>
        /// <param name="id">O ID do coordenador a ser excluído.</param>
        /// <returns>Um objeto do tipo IActionResult.</returns>

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.ContaAdministrativa == null)
            {
                return NotFound();
            }

            var coordenador = from user in _context.Users
                              join profile in _context.ContaAdministrativa on user.Id equals profile.UserId
                              join atl in _context.ATL on profile.AtlId equals atl.AtlId
                              where profile.ContaId == id
                              select new LowerAccountViewModel
                              {
                                  User = user,
                                  Profile = profile,
                                  AtlName = atl.Name
                              };

            if (coordenador == null)
            {
                return NotFound();
            }

            return View(await coordenador.FirstAsync());
        }

        /// <summary>
        /// Confirma a exclusão de um coordenador com base no ID fornecido.
        /// </summary>
        /// <param name="id">O ID do coordenador a ser excluído.</param>
        /// <returns>Um objeto do tipo IActionResult.</returns>

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.ContaAdministrativa == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.ContaAdministrativa' is null.");
            }
            var coordenador = await _context.ContaAdministrativa.FindAsync(id);
            if (coordenador != null)
            {
                var user = await _userManager.FindByIdAsync(coordenador.UserId);
                _context.ContaAdministrativa.Remove(coordenador);
                await _userManager.DeleteAsync(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se uma conta administrativa existe com base no ID fornecido.
        /// </summary>
        /// <param name="id">O ID da conta administrativa.</param>
        /// <returns>Um valor booleano que indica se a conta administrativa existe.</returns>

        private bool ContaAdministrativaExists(Guid id)
        {
          return _context.ContaAdministrativa.Any(e => e.ContaId == id);
        }

        /// <summary>
        /// Cria uma instância de ATLManagerUser.
        /// </summary>
        /// <returns>Um objeto do tipo ATLManagerUser.</returns>

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
        /// Obtém o armazenamento de e-mail do usuário.
        /// </summary>
        /// <returns>Um objeto do tipo IUserEmailStore&lt;ATLManagerUser&gt;.</returns>

        private IUserEmailStore<ATLManagerUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ATLManagerUser>)_userStore;
        }

        /// <summary>
        /// Faz upload de um arquivo para a pasta de upload de coordenadores.
        /// </summary>
        /// <param name="logoPicture">O arquivo a ser enviado.</param>
        /// <returns>O nome exclusivo do arquivo.</returns>

        private string UploadedFile(IFormFile logoPicture)
		{
			string uniqueFileName = null;

			if (logoPicture != null)
			{
				string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/uploads/coordenadores");
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
