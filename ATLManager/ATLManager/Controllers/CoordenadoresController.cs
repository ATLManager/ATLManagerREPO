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

namespace ATLManager.Controllers
{
    public class CoordenadoresController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly IUserStore<ATLManagerUser> _userStore;
        private readonly IUserEmailStore<ATLManagerUser> _emailStore;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CoordenadoresController(ATLManagerAuthContext context, 
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

        // GET: Coordenador/Details/5
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

            var atls = await (from atl in _context.ATL
                              join atlAdmin in _context.ATLAdmin on atl.AtlId equals atlAdmin.AtlId
                              join admin in _context.ContaAdministrativa on atlAdmin.ContaId equals admin.ContaId
                              where admin.ContaId == userAccount.ContaId
                              select atl).Include(a => a.Agrupamento).ToListAsync();

            ViewData["AtlId"] = new SelectList(atls, "AtlId", "Name");
			return View(new CoordenadorCreateViewModel());
        }

        // POST: Coordenador/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

                    string fileName = UploadedFile(viewModel.ProfilePicture);
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

        // GET: Coordenador/Edit/5
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
				var coordenador = _context.ContaAdministrativa.Find(id);

				if (coordenador.CC != viewModel.CC &&
					(_context.ContaAdministrativa.Any(c => c.CC == viewModel.CC)
                    || _context.Educando.Any(e => e.CC == viewModel.CC)))
                {
                    var validationMessage = "Outra conta já contém este CC";
                    ModelState.AddModelError("CC", validationMessage);
                }
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

                        string fileName = UploadedFile(viewModel.ProfilePicture);
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

        // GET: Coordenador/Delete/5
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

        // POST: Coordenador/Delete/5
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
