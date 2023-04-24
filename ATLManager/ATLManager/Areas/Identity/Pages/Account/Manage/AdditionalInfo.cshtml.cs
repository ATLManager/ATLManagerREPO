// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ATLManager.Areas.Identity.Data;
using ATLManager.Attributes;
using ATLManager.Data;
using ATLManager.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Xunit.Sdk;

namespace ATLManager.Areas.Identity.Pages.Account.Manage
{
    public class AdditionalInfoModel : PageModel
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly SignInManager<ATLManagerUser> _signInManager;

		public AdditionalInfoModel(
            ATLManagerAuthContext context,
            UserManager<ATLManagerUser> userManager,
            SignInManager<ATLManagerUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DisplayName("Morada")]
            [StringLength(50, MinimumLength = 5)]
            public string Address { get; set; }

            [Required]
            [DisplayName("Código Postal")]
            [RegularExpression(@"^\d{4}-\d{3}$", ErrorMessage = "Formato Incorreto - ex. 1234-123")]
            public string PostalCode { get; set; }

            [Required]
            [MaxLength(20)]
            [DisplayName("Cidade")]
            public string City { get; set; }

            [Required]
            [StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
            [RegularExpression("^[0-9]*$", ErrorMessage = "Este campo deve conter apenas dígitos")]
            public string NIF { get; set; }
        }

        private async Task LoadAsync(EncarregadoEducacao encarregado)
        {
            Input = new InputModel
            {
                Address = encarregado.Address,
                PostalCode = encarregado.PostalCode,
                City = encarregado.City,
                NIF = encarregado.NIF,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var encarregado = await _context.EncarregadoEducacao
                .FirstOrDefaultAsync(e => e.UserId == user.Id);
            if (encarregado == null)
            {
                return NotFound();
            }

            await LoadAsync(encarregado);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var encarregado = await _context.EncarregadoEducacao
                .FirstOrDefaultAsync(e => e.UserId == user.Id);
            if (encarregado == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(encarregado);
                return Page();
            }

            if (Input.Address != encarregado.Address)
            {
                encarregado.Address = Input.Address;
			}

            if (Input.PostalCode != encarregado.PostalCode)
            {
                encarregado.PostalCode = Input.PostalCode;
            }

            if (Input.City != encarregado.City)
            {
                encarregado.City = Input.City;
            }

            if (Input.NIF != encarregado.NIF)
            {
                encarregado.NIF = Input.NIF;
            }

            _context.Update(encarregado);
            await _context.SaveChangesAsync();
			await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
	}
}
