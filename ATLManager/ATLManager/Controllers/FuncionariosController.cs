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

        // GET: Coordenador
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

        // GET: Coordenador/Details/5
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

        // GET: Coordenador/Create
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

        // POST: Coordenador/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Coordenador/Edit/5
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

        // POST: Coordenador/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Coordenador/Delete/5
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

        // POST: Coordenador/Delete/5
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

        private bool ContaAdministrativaExists(Guid id)
        {
          return _context.ContaAdministrativa.Any(e => e.ContaId == id);
        }

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

        private IUserEmailStore<ATLManagerUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ATLManagerUser>)_userStore;
        }

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
