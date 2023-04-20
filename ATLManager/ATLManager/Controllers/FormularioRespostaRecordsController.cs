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
    public class FormularioRespostaRecordsController : Controller
    {
        private readonly ATLManagerAuthContext _context;

        public FormularioRespostaRecordsController(ATLManagerAuthContext context)
        {
            _context = context;
        }

        // GET: FormularioRespostaRecords/Details/5
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
