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

namespace ATLManager.Controllers
{
    /// <summary>
    /// Controlador para o modelo 'Históricos de Resposta de Formulários'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class FormularioRespostaRecordsController : Controller
    {
        private readonly ATLManagerAuthContext _context;

        public FormularioRespostaRecordsController(ATLManagerAuthContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Exibe os detalhes de um registro de formulário de resposta com base no ID.
        /// </summary>
        /// <param name="id">O ID do registro de formulário de resposta.</param>
        /// <returns>Um objeto IActionResult que exibe a visualização dos detalhes do registro de formulário de resposta.</returns>

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.FormularioRespostaRecord == null)
            {
                return NotFound();
            }

            var formularioRespostaRecord = await _context.FormularioRespostaRecord
                .Include(f => f.FormularioRecord)
                .FirstOrDefaultAsync(m => m.FormularioRespostaRecordId == id);
            if (formularioRespostaRecord == null)
            {
                return NotFound();
            }

            return View(formularioRespostaRecord);
        }
    }
}
