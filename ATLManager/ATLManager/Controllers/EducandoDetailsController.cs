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
    [Route("Educandos/Details/{id}")]
    public class EducandoDetailsController : Controller
    {
        private readonly ATLManagerAuthContext _context;

        public EducandoDetailsController(ATLManagerAuthContext context)
        {
            _context = context;
        }

        //// GET: EducandosSaude
        //public async Task<IActionResult> Index()
        //{
        //    var aTLManagerAuthContext = _context.EducandoSaude.Include(e => e.Educando);
        //    return View(await aTLManagerAuthContext.ToListAsync());
        //}

        // GET: Educandos/EducandoDetails/5
        public async Task<IActionResult> EducandoDetails(Guid? id)
        {
            if (id == null || _context.Educando == null)
            {
                return NotFound();
            }

            var educando = await _context.Educando
                .Include(e => e.Atl)
                .Include(e => e.Encarregado)
                .FirstOrDefaultAsync(m => m.EducandoId == id);
            if (educando == null)
            {
                return NotFound();
            }

            return View(educando);
        }

        //// GET: Educandos/Details/5/Saude
        //[Route("~/Saude")]
        //public async Task<IActionResult> DetailsSaude(Guid? id)
        //{
        //    if (id == null || _context.EducandoSaude == null)
        //    {
        //        return NotFound();
        //    }

        //    var educandoSaude = await _context.EducandoSaude
        //        .Include(e => e.Educando)
        //        .FirstOrDefaultAsync(m => m.EducandoSaudeId == id);
        //    if (educandoSaude == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(educandoSaude);
        //}

        //// GET: EducandosSaude/Create
        //public IActionResult Create()
        //{
        //    ViewData["EducandoId"] = new SelectList(_context.Educando, "EducandoId", "Apelido");
        //    return View();
        //}

        //// POST: EducandosSaude/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("EducandoSaudeId,EducandoId,BloodType,EmergencyContact,InsuranceName,InsuranceNumber,Allergies,Diseases,Medication,MedicalHistory")] EducandoSaude educandoSaude)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        educandoSaude.EducandoSaudeId = Guid.NewGuid();
        //        _context.Add(educandoSaude);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["EducandoId"] = new SelectList(_context.Educando, "EducandoId", "Apelido", educandoSaude.EducandoId);
        //    return View(educandoSaude);
        //}

        // GET: EducandosSaude/Edit/5/Saude/Edit
        [Route("~/Saude/Edit")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.EducandoSaude == null)
            {
                return NotFound();
            }

            var educandoSaude = await _context.EducandoSaude.FindAsync(id);
            if (educandoSaude == null)
            {
                return NotFound();
            }
            ViewData["EducandoId"] = new SelectList(_context.Educando, "EducandoId", "Apelido", educandoSaude.EducandoId);
            return View(educandoSaude);
        }

        // POST: EducandosSaude/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("~/Saude/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("EducandoSaudeId,EducandoId,BloodType,EmergencyContact,InsuranceName,InsuranceNumber,Allergies,Diseases,Medication,MedicalHistory")] EducandoSaude educandoSaude)
        {
            if (id != educandoSaude.EducandoSaudeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(educandoSaude);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EducandoSaudeExists(educandoSaude.EducandoSaudeId))
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
            ViewData["EducandoId"] = new SelectList(_context.Educando, "EducandoId", "Apelido", educandoSaude.EducandoId);
            return View(educandoSaude);
        }
        
        private bool EducandoSaudeExists(Guid id)
        {
          return _context.EducandoSaude.Any(e => e.EducandoSaudeId == id);
        }
    }
}
