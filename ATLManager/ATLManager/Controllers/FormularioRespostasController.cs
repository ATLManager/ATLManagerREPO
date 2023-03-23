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

namespace ATLManager.Controllers
{
    public class FormularioRespostasController : Controller
    {
        private readonly ATLManagerAuthContext _context;

        public FormularioRespostasController(ATLManagerAuthContext context)
        {
            _context = context;
        }

        // GET: FormularioRespostas
        public async Task<IActionResult> Index()
        {
            var aTLManagerAuthContext = _context.FormularioResposta.Include(f => f.Educando).Include(f => f.Formulario);
            return View(await aTLManagerAuthContext.ToListAsync());
        }

        // GET: FormularioRespostas/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.FormularioResposta == null)
            {
                return NotFound();
            }

            var formularioResposta = await _context.FormularioResposta
                .Include(f => f.Educando)
                .Include(f => f.Formulario)
                .FirstOrDefaultAsync(m => m.FormularioRespostaId == id);
            if (formularioResposta == null)
            {
                return NotFound();
            }

            return View(formularioResposta);
        }

        // GET: FormularioRespostas/Edit/5
        public async Task<IActionResult> Responder(Guid? id)
        {
            if (id == null || _context.FormularioResposta == null)
            {
                return NotFound();
            }

            var formularioResposta = await _context.FormularioResposta.FindAsync(id);
            if (formularioResposta == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formulario.FindAsync(formularioResposta.FormularioId);
            var viewModel = new FormularioResponderViewModel
            {
                FormularioRespostaId = formularioResposta.FormularioRespostaId,
                Name = formulario.Name,
                Description = formulario.Description,
                DateLimit = formulario.DateLimit.ToShortDateString(),
                Authorized = formularioResposta.Authorized
            };

            return View(viewModel);
        }

        // POST: FormularioRespostas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Responder(Guid id, FormularioResponderViewModel viewModel)
        {
            if (id != viewModel.FormularioRespostaId)
            {
                return NotFound();
            }

            ModelState.Remove("Name");
            ModelState.Remove("Description");
            ModelState.Remove("DateLimit");

            if (ModelState.IsValid)
            {
                try
                {
                    var formularioResposta = await _context.FormularioResposta.FindAsync(id);

                    formularioResposta.Authorized = viewModel.Authorized;
                    formularioResposta.ResponseDate = DateTime.UtcNow;

					_context.Update(formularioResposta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormularioRespostaExists(viewModel.FormularioRespostaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Obrigado));
            }
            return View(viewModel);
        }

		public IActionResult Obrigado()
		{
			return View();
		}

		private bool FormularioRespostaExists(Guid id)
        {
          return (_context.FormularioResposta?.Any(e => e.FormularioRespostaId == id)).GetValueOrDefault();
        }
    }
}
