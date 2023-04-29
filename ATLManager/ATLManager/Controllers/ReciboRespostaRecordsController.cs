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
using Microsoft.AspNetCore.Hosting;

namespace ATLManager.Controllers
{
    /// <summary>
    /// Controlador para o modelo 'Históricos de Resposta de Recibos'.
    /// Contém as ações básicas de CRUD e outras ações de detalhes para outros aspetos relacionados ao modelo.
    /// </summary>
    public class ReciboRespostaRecordsController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReciboRespostaRecordsController(ATLManagerAuthContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Devolve a vista de detalhes de um ReciboRespostaRecord com base no id fornecido.
        /// </summary>
        /// <param name="id">O id do ReciboRespostaRecord para mostrar os detalhes.</param>
        /// <returns>A vista de pormenor do ReciboRespostaRecord, se encontrado, ou um resultado NotFound.</returns>
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.ReciboRespostaRecord == null)
            {
                return NotFound();
            }

            var reciboRespostaRecord = await _context.ReciboRespostaRecord
                .Include(r => r.ReciboRecord)
                .FirstOrDefaultAsync(m => m.ReciboRespostaRecordId == id);
            if (reciboRespostaRecord == null)
            {
                return NotFound();
            }

            return View(reciboRespostaRecord);
        }

        /// <summary>
        /// Devolve um ficheiro descarregado do ficheiro com o nome de ficheiro indicado.
        /// </summary>
        /// <param name="fileName">O nome do ficheiro a descarregar.</param>
        /// <returns>O ficheiro como ficheiro descarregável.</returns>
        public IActionResult Download(string fileName)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "files/users/recibos");
            string filePath = Path.Combine(uploadsFolder, fileName);
            string fileCleanName = fileName.Substring(fileName.IndexOf("_id_") + 4);
            return File(System.IO.File.ReadAllBytes(filePath), "application/pdf", fileCleanName);
        }
    }
}
