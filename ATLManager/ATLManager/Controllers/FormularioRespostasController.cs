using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.Models;

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
            ViewData["EducandoId"] = new SelectList(_context.Educando, "EducandoId", "Apelido", formularioResposta.EducandoId);
            ViewData["FormularioId"] = new SelectList(_context.Formulario, "FormularioId", "Description", formularioResposta.FormularioId);
            return View(formularioResposta);
        }

        // POST: FormularioRespostas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Responder(Guid id, [Bind("FormularioRespostaId,FormularioId,EducandoId,Authorized,ResponseDate")] FormularioResposta formularioResposta)
        {
            if (id != formularioResposta.FormularioRespostaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(formularioResposta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormularioRespostaExists(formularioResposta.FormularioRespostaId))
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
            ViewData["EducandoId"] = new SelectList(_context.Educando, "EducandoId", "Apelido", formularioResposta.EducandoId);
            ViewData["FormularioId"] = new SelectList(_context.Formulario, "FormularioId", "Description", formularioResposta.FormularioId);
            return View(formularioResposta);
        }

        private bool FormularioRespostaExists(Guid id)
        {
          return (_context.FormularioResposta?.Any(e => e.FormularioRespostaId == id)).GetValueOrDefault();
        }
    }
}
