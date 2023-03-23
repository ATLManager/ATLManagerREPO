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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography.Pkcs;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;

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
            var aTLManagerAuthContext = _context.Formulario.Include(f => f.VisitaEstudo);
            return View(await aTLManagerAuthContext.ToListAsync());
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
                formulario.FormularioId = Guid.NewGuid();
                _context.Add(formulario);

                var user = await _userManager.GetUserAsync(HttpContext.User);
                var userAccount = await _context.ContaAdministrativa
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(m => m.UserId == user.Id);
                var educandos = _context.Educando
                    .Include(c => c.Atl)
                    .Where(g => g.AtlId == userAccount.AtlId);

                foreach(var educando in educandos)
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
            ViewData["VisitaEstudoId"] = new SelectList(_context.VisitaEstudo, "VisitaEstudoID", "Name", formulario.VisitaEstudoId);
            return View(formulario);
        }

        // POST: Formularios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("FormularioId,Name,Description,VisitaEstudoId,StartDate,DateLimit")] Formulario formulario)
        {
            if (id != formulario.FormularioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(formulario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormularioExists(formulario.FormularioId))
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
            ViewData["VisitaEstudoId"] = new SelectList(_context.VisitaEstudo, "VisitaEstudoID", "Name", formulario.VisitaEstudoId);
            return View(formulario);
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
