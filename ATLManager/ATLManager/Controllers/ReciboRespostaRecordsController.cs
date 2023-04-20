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

        // GET: ReciboRespostaRecords/Details/5
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

        public IActionResult Download(string fileName)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "files/users/recibos");
            string filePath = Path.Combine(uploadsFolder, fileName);
            string fileCleanName = fileName.Substring(fileName.IndexOf("_id_") + 4);
            return File(System.IO.File.ReadAllBytes(filePath), "application/pdf", fileCleanName);
        }
    }
}
