﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using ATLManager.Models;
using ATLManager.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using ATLManager.Services;
using System.Linq;

namespace ATLManager.Areas.Identity.Pages.Account
{
	public class EERegisterModel : PageModel
    {
        private readonly SignInManager<ATLManagerUser> _signInManager;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly IUserStore<ATLManagerUser> _userStore;
        private readonly IUserEmailStore<ATLManagerUser> _emailStore;
        private readonly ILogger<EERegisterModel> _logger;
		private readonly ATLManagerAuthContext _context;
		private readonly IEmailSender _emailSender;
        private readonly LanguageService _language;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public EERegisterModel(
            UserManager<ATLManagerUser> userManager,
            IUserStore<ATLManagerUser> userStore,
            SignInManager<ATLManagerUser> signInManager,
            ILogger<EERegisterModel> logger,
			ATLManagerAuthContext context,
			IEmailSender emailSender, 
            LanguageService language,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
			_context = context;
			_emailSender = emailSender;
            _language = language;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public EEInputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class EEInputModel
        {
            [Required]
            [DataType(DataType.Text)]
            public string FirstName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            public string LastName { get; set; }

            [Required]
            [RegularExpression("^[1-9][0-9]{8}$")]
            public string Phone { get; set; }

            [Required]
            [StringLength(50, MinimumLength = 5)]
            public string Address { get; set; }

            [Required]
            [MaxLength(20)]
            public string City { get; set; }

            [Required]
            [RegularExpression(@"^\d{4}-\d{3}$", ErrorMessage = "Formato Incorreto - ex. 1234-123")]
            public string PostalCode { get; set; }

            [Required]
            [StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
            public string NIF { get; set; }

			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Required]
            [EmailAddress]
            [Display(Name = "Email")]
            [DataType(DataType.EmailAddress)]
			[RegularExpression(@"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?){1,2}$",
		        ErrorMessage = "Email Inválido")]
			public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
			ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (string.IsNullOrEmpty(Input.FirstName))
            {
                ModelState.AddModelError(string.Empty, _language.GetKey("txtNameRequired"));
            }
            if (string.IsNullOrEmpty(Input.LastName))
            {
                ModelState.AddModelError(string.Empty, _language.GetKey("txtLastNameRequired"));
            }
            if (string.IsNullOrEmpty(Input.Email))
            {
                ModelState.AddModelError(string.Empty, _language.GetKey("txtEmailRequired"));
            }
            if (string.IsNullOrEmpty(Input.Password))
            {
                ModelState.AddModelError(string.Empty, _language.GetKey("txtPasswordRequired"));
            }
            if (string.IsNullOrEmpty(Input.ConfirmPassword))
            {
                ModelState.AddModelError(string.Empty, _language.GetKey("txtConfirmPasswordRequired"));
            }

            if (!string.IsNullOrEmpty(Input.NIF))
            {
                if (_context.EncarregadoEducacao.Any(e => e.NIF == Input.NIF))
                {
                    var validationMessage = "Outro Encarregado já contém este NIF";
                    ModelState.AddModelError("NIF", validationMessage);
                }
            }

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await _userManager.SetPhoneNumberAsync(user, Input.Phone);
					await _userManager.AddToRoleAsync(user, "EncarregadoEducacao");

					var perfil = new EncarregadoEducacao(user.Id,
                        Input.Address, 
                        Input.City, 
                        Input.PostalCode, 
                        Input.NIF);

                    _context.Add(perfil);
					await _context.SaveChangesAsync();

					_logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId, code, returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "ATLManager - Confirme o seu email",
                        $"Obrigado por escolher o ATLManager. Por favor confirme a sua conta de Encarregado <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>aqui</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
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
                    $"Ensure that '{nameof(ATLManagerUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
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
    }
}
