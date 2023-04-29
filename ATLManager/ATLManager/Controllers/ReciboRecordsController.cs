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

namespace ATLManager.Controllers
{
    /// <summary>
    /// Controlador para o modelo 'Históricos Recibos'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class ReciboRecordsController : Controller
    {
        private readonly ATLManagerAuthContext _context;
		private readonly UserManager<ATLManagerUser> _userManager;

		public ReciboRecordsController(ATLManagerAuthContext context, 
            UserManager<ATLManagerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Retorna uma lista de recibos associados à conta administrativa do utilizador.
        /// </summary>
        /// <returns>Retorna uma View contendo a lista de recibos.</returns>
        [Authorize(Roles ="Coordenador")]
        public async Task<IActionResult> Index()
        {
			var currentUser = await _userManager.GetUserAsync(HttpContext.User);

			var currentUserAccount = await _context.ContaAdministrativa
				.Include(f => f.User)
				.FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

			var recibos = await _context.ReciboRecord
				.Include(e => e.Atl)
				.Where(e => e.AtlId == currentUserAccount.AtlId)
				.ToListAsync();

			return View(recibos);
		}

        /// <summary>
        /// Retorna os detalhes de um recibo específico.
        /// </summary>
        /// <param name="id">O ID do recibo desejado.</param>
        /// <returns>Retorna uma View contendo os detalhes do recibo.</returns>

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.ReciboRecord == null)
            {
                return NotFound();
            }

            var reciboRecord = await _context.ReciboRecord
                .Include(r => r.Atl)
                .FirstOrDefaultAsync(m => m.ReciboRecordId == id);
            if (reciboRecord == null)
            {
                return NotFound();
            }

            return View(reciboRecord);
        }

        /// <summary>
        /// Retorna uma lista de respostas associadas a um recibo específico.
        /// </summary>
        /// <param name="id">O ID do recibo desejado.</param>
        /// <returns>Retorna uma View contendo a lista de respostas.</returns>

        public async Task<IActionResult> Respostas(Guid? id)
        {
            if (id == null || _context.ReciboRecord == null)
            {
                return NotFound();
            }

            var respostas = await _context.ReciboRespostaRecord
                .Where(f => f.ReciboRecordId == id)
                .ToListAsync();

            if (respostas == null)
            {
                return NotFound();
            }

            return View(respostas);
        }

        /// <summary>
        /// Exclui um recibo específico do banco de dados.
        /// </summary>
        /// <param name="id">O ID do recibo a ser excluído.</param>
        /// <returns>Retorna uma View de confirmação </returns>

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.ReciboRecord == null)
            {
                return NotFound();
            }

            var reciboRecord = await _context.ReciboRecord
                .Include(r => r.Atl)
                .FirstOrDefaultAsync(m => m.ReciboRecordId == id);
            if (reciboRecord == null)
            {
                return NotFound();
            }

            return View(reciboRecord);
        }

        /// <summary>
        /// Retorna uma View de confirmação de exclusão para um recibo específico.
        /// </summary>
        /// <param name="id">O ID do recibo desejado.</param>
        /// <returns>Retorna uma View de confirmação de exclusão.</returns>
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.ReciboRecord == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.ReciboRecord'  is null.");
            }
            var reciboRecord = await _context.ReciboRecord.FindAsync(id);
            if (reciboRecord != null)
            {
                _context.ReciboRecord.Remove(reciboRecord);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se um registo de Recibo com o identificador especificado existe no contexto atual.
        /// </summary>
        /// <param name="id">O identificador do registo de Recibo a ser verificado.</param>
        /// <returns>true se o registo existe, false caso contrário.</returns>
        private bool ReciboRecordExists(Guid id)
        {
          return (_context.ReciboRecord?.Any(e => e.ReciboRecordId == id)).GetValueOrDefault();
        }
    }
}
