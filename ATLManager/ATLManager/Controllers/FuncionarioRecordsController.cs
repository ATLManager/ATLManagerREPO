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
using Microsoft.AspNetCore.Identity;
using ATLManager.Areas.Identity.Data;

namespace ATLManager.Controllers
{
    /// <summary>
    /// Controlador para o modelo 'Históricos de Funcionários'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class FuncionarioRecordsController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;

        public FuncionarioRecordsController(ATLManagerAuthContext context, UserManager<ATLManagerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Retorna a página inicial que lista os funcionários .
        /// </summary>
        /// <returns>A página inicial com a lista de funcionários.</returns>

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            var funcionarios = await _context.FuncionarioRecord
                .Include(f => f.Atl)
                .Where(f => f.AtlId == currentUserAccount.AtlId)
                .ToListAsync();

            return View(funcionarios);
        }

        /// <summary>
        /// Retorna a página de detalhes do funcionário com o ID fornecido.
        /// </summary>
        /// <param name="id">O ID do funcionário.</param>
        /// <returns>A página de detalhes do funcionário.</returns>

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.FuncionarioRecord == null)
            {
                return NotFound();
            }

            var funcionarioRecord = await _context.FuncionarioRecord
                .Include(f => f.Atl)
                .FirstOrDefaultAsync(m => m.FuncionarioRecordId == id);
            if (funcionarioRecord == null)
            {
                return NotFound();
            }

            return View(funcionarioRecord);
        }

        /// <summary>
        /// Retorna a página de exclusão do funcionário com o ID fornecido.
        /// </summary>
        /// <param name="id">O ID do funcionário.</param>
        /// <returns>A página de exclusão do funcionário.</returns>

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.FuncionarioRecord == null)
            {
                return NotFound();
            }

            var funcionarioRecord = await _context.FuncionarioRecord
                .Include(f => f.Atl)
                .FirstOrDefaultAsync(m => m.FuncionarioRecordId == id);
            if (funcionarioRecord == null)
            {
                return NotFound();
            }

            return View(funcionarioRecord);
        }

        /// <summary>
        /// Remove o funcionário com o ID fornecido.
        /// </summary>
        /// <param name="id">O ID do funcionário.</param>
        /// <returns>Um redirecionamento para a página inicial.</returns>

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.FuncionarioRecord == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.FuncionarioRecord'  is null.");
            }
            var funcionarioRecord = await _context.FuncionarioRecord.FindAsync(id);
            if (funcionarioRecord != null)
            {
                _context.FuncionarioRecord.Remove(funcionarioRecord);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se um funcionário com o ID fornecido existe.
        /// </summary>
        /// <param name="id">O ID do funcionário.</param>
        /// <returns>True se o funcionário existe; False caso contrário.</returns>

        private bool FuncionarioRecordExists(Guid id)
        {
          return (_context.FuncionarioRecord?.Any(e => e.FuncionarioRecordId == id)).GetValueOrDefault();
        }
    }
}
