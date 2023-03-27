﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.Models;
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography.Pkcs;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using ATLManager.ViewModels;

namespace ATLManager.Controllers
{
    [Authorize(Roles = "Administrador, Coordenador")]
    public class FormulariosController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly IEmailSender _emailSender;

        public FormulariosController(ATLManagerAuthContext context, 
            UserManager<ATLManagerUser> userManager,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: Formularios
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            var formularios = await _context.Formulario
                .Include(a => a.Atl)
                .Where(r => r.AtlId == currentUserAccount.AtlId)
                .ToListAsync();

			return View(formularios);
        }

        // GET: Formularios/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Formulario == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formulario
                .Include(f => f.VisitaEstudo)
                .FirstOrDefaultAsync(m => m.FormularioId == id);
            if (formulario == null)
            {
                return NotFound();
            }

            return View(formulario);
        }

        // GET: Formularios/Respostas/5
        public async Task<IActionResult> Respostas(Guid? id)
        {
            if (id == null || _context.Formulario == null)
            {
                return NotFound();
            }

			var respostas = from resposta in _context.FormularioResposta
							join educando in _context.Educando on resposta.EducandoId equals educando.EducandoId
							select new FormularioRespostasViewModel
							{
								RespostaId = resposta.FormularioRespostaId,
                                FormularioId = resposta.FormularioId,
								EducandoName = educando.Name + " " + educando.Apelido,
                                Authorized = resposta.Authorized,
                                ResponseDate = ((DateTime)resposta.ResponseDate).ToShortDateString()
							};

			if (respostas == null)
            {
                return NotFound();
            }

            return View(respostas);
        }

        // GET: Formularios/Create
        [Authorize(Roles = "Coordenador")]
        public IActionResult Create()
        {
            ViewData["VisitaEstudoId"] = new SelectList(_context.VisitaEstudo, "VisitaEstudoID", "Name");
            return View();
        }

        // POST: Formularios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FormularioId,Name,Description,VisitaEstudoId,StartDate,DateLimit")] Formulario formulario)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var userAccount = await _context.ContaAdministrativa
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(m => m.UserId == user.Id);
                
                formulario.FormularioId = Guid.NewGuid();
                formulario.AtlId = userAccount?.AtlId;
                _context.Add(formulario);

                var educandos = await _context.Educando
                    .Include(c => c.Atl)
                    .Where(g => g.AtlId == userAccount.AtlId)
                    .ToListAsync(); // Carregue os dados antes de entrar no loop

                foreach (var educando in educandos)
                {
                    var resposta = new FormularioResposta(formulario.FormularioId, educando.EducandoId);
                    resposta.DateLimit = formulario.DateLimit;

                    // Obter Encarregado do Educando e a sua conta
                    var encarregado = await _context.EncarregadoEducacao
                        .FirstOrDefaultAsync(e => e.EncarregadoId == educando.EncarregadoId);
                    var encarregadoAccount = await _context.Users
                        .FirstOrDefaultAsync(e => e.Id == encarregado.UserId);

                    var userEmail = await _userManager.GetEmailAsync(encarregadoAccount);
                    var code = resposta.FormularioRespostaId.ToString();
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Action("Responder", "FormularioRespostas", new { id = resposta.FormularioRespostaId }, Request.Scheme);

                    await _emailSender.SendEmailAsync(userEmail, "Novo formulário por responder",
                        $"Por favor responda ao formulário <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>aqui</a>.");

                    _context.Add(resposta);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VisitaEstudoId"] = new SelectList(_context.VisitaEstudo, "VisitaEstudoID", "Name", formulario.VisitaEstudoId);
            return View(formulario);
        }

		// GET: Formularios/Edit/5
		public async Task<IActionResult> Edit(Guid? id)
		{
			if (id == null || _context.Formulario == null)
			{
				return NotFound();
			}

			var formulario = await _context.Formulario.FindAsync(id);
			if (formulario == null)
			{
				return NotFound();
			}

			var viewModel = new FormularioEditViewModel
			{
				FormularioId = formulario.FormularioId,
				Name = formulario.Name,
				VisitaEstudoId = formulario.VisitaEstudoId,
				Description = formulario.Description,
				StartDate = formulario.StartDate.ToShortDateString(),
				DateLimit = formulario.DateLimit.ToShortDateString(),
			};

			ViewData["VisitaEstudoId"] = new SelectList(_context.VisitaEstudo, "VisitaEstudoID", "Name", formulario.VisitaEstudoId);
			return View(viewModel);
		}

		// POST: Formularios/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, FormularioEditViewModel viewModel)
        {
            if (id != viewModel.FormularioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
					var formulario = await _context.Formulario.FindAsync(viewModel.FormularioId);

					if (formulario != null)
					{
						formulario.Name = viewModel.Name;
						formulario.Description = viewModel.Description;

						if (viewModel.StartDate != null)
							formulario.StartDate = DateTime.Parse(viewModel.StartDate);
						if (viewModel.DateLimit != null)
							formulario.DateLimit = DateTime.Parse(viewModel.DateLimit);

						_context.Update(formulario);
						await _context.SaveChangesAsync();
					}
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormularioExists(viewModel.FormularioId))
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
            ViewData["VisitaEstudoId"] = new SelectList(_context.VisitaEstudo, "VisitaEstudoID", "Name", viewModel.VisitaEstudoId);
            return View(viewModel);
        }

        // GET: Formularios/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Formulario == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formulario
                .Include(f => f.VisitaEstudo)
                .FirstOrDefaultAsync(m => m.FormularioId == id);
            if (formulario == null)
            {
                return NotFound();
            }

            return View(formulario);
        }

        // POST: Formularios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Formulario == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.Formulario'  is null.");
            }
            var formulario = await _context.Formulario.FindAsync(id);
            if (formulario != null)
            {
                _context.Formulario.Remove(formulario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FormularioExists(Guid id)
        {
          return (_context.Formulario?.Any(e => e.FormularioId == id)).GetValueOrDefault();
        }
    }
}
