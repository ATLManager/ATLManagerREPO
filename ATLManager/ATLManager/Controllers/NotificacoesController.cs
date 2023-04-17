using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.Models;
using Microsoft.AspNetCore.Identity;
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ATLManager.Controllers
{
    public class NotificacoesController : Controller, INotificacoesController
    {
        private readonly ATLManagerAuthContext _context;
		private readonly UserManager<ATLManagerUser> _userManager;

		public NotificacoesController(ATLManagerAuthContext context, UserManager<ATLManagerUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		// GET: Notificacao
		public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var userId = currentUser.Id;

            var notificacoes = await _context.Notificacoes
                .Where(n => n.UserId == userId)
                .Include(n => n.User)
                .ToListAsync();

            return View(notificacoes);
        }

		// GET: Notificacao/Details/5
		public async Task<IActionResult> Details(Guid? id)
		{
			if (id == null || _context.Notificacoes == null)
			{
				return NotFound();
			}

			var notificacao = await _context.Notificacoes
				.Include(n => n.User)
				.FirstOrDefaultAsync(m => m.NotificacaoId == id);
			if (notificacao == null)
			{
				return NotFound();
			}

            // Marcar notificação como lida
            notificacao.Lida = true;
            _context.Update(notificacao);
            await _context.SaveChangesAsync();
            
            return View(notificacao);
		}

		// GET: Notificacao/Create
		public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Notificacao/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NotificacaoId,UserId,Titulo,Mensagem")] Notificacao notificacao)
        {
            var modelStateWithoutUser = new ModelStateDictionary(ModelState);
            modelStateWithoutUser.Remove("User");

            if (modelStateWithoutUser.IsValid)
            {
                notificacao.NotificacaoId = Guid.NewGuid();
                _context.Add(notificacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", notificacao.UserId);
            return View(notificacao);
        }

        // GET: Notificacao/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Notificacoes == null)
            {
                return NotFound();
            }

            var notificacao = await _context.Notificacoes.FindAsync(id);
            if (notificacao == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", notificacao.UserId);
            return View(notificacao);
        }

        // POST: Notificacao/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("NotificacaoId,UserId,Titulo,Mensagem,Lida,DataNotificacao")] Notificacao notificacao)
        {
            if (id != notificacao.NotificacaoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notificacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificacaoExists(notificacao.NotificacaoId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", notificacao.UserId);
            return View(notificacao);
        }

		// GET: Notificacao/Delete/5
		public async Task<IActionResult> Delete(Guid? id)
		{
			if (id == null || _context.Notificacoes == null)
			{
				return NotFound();
			}

			var notificacao = await _context.Notificacoes
				.Include(n => n.User)
				.FirstOrDefaultAsync(m => m.NotificacaoId == id);
			if (notificacao == null)
			{
				return NotFound();
			}

			return View(notificacao);
		}

		// POST: Notificacao/Delete/5
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Notificacoes == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.Notificacoes'  is null.");
            }
            var notificacao = await _context.Notificacoes.FindAsync(id);
            if (notificacao != null)
            {
                _context.Notificacoes.Remove(notificacao);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificacaoExists(Guid id)
        {
          return (_context.Notificacoes?.Any(e => e.NotificacaoId == id)).GetValueOrDefault();
        }
        
		public async Task CreateNotification(string userId, string titulo, string mensagem)
		{
			var notificacao = new Notificacao(userId, titulo, mensagem);
			_context.Add(notificacao);
			await _context.SaveChangesAsync();
		}

        public async Task<List<Notificacao>> GetUserNotifications(string userId)
        {
            return await _context.Notificacoes
                .Where(n => n.UserId == userId && !n.Lida)
                .ToListAsync();
        }
    }
}
