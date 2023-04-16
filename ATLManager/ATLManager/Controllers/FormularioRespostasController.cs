using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace ATLManager.Controllers
{
    public class FormularioRespostasController : Controller
    {
        private readonly ATLManagerAuthContext _context;

        public FormularioRespostasController(ATLManagerAuthContext context)
        {
            _context = context;
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
		[Authorize(Roles = "EncarregadoEducacao")]
		public async Task<IActionResult> Responder(Guid? id)
        {
            if (id == null || _context.FormularioResposta == null)
            {
                return NotFound();
            }

			var resposta = await _context.FormularioResposta
				.Include(f => f.Educando)
				.Include(f => f.Formulario)
				.FirstOrDefaultAsync(m => m.FormularioRespostaId == id);
			if (resposta == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formulario.FindAsync(resposta.FormularioId);
            var viewModel = new FormularioResponderViewModel
            {
                FormularioRespostaId = resposta.FormularioRespostaId,
                Name = formulario.Name,
                Educando = resposta.Educando.Name + " " + resposta.Educando.Apelido,
				Description = formulario.Description,
                DateLimit = formulario.DateLimit.ToShortDateString(),
                Authorized = resposta.Authorized
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
                    formularioResposta.ResponseDate = DateTime.UtcNow.Date;

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
