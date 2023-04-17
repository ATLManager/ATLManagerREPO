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
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace ATLManager.Controllers
{
    public class RecibosController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly IEmailSender _emailSender;

        public RecibosController(ATLManagerAuthContext context,
            UserManager<ATLManagerUser> userManager,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: Reciboes
        [Authorize(Roles = "Coordenador,Funcionario")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var currentUserAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            var recibos = await _context.Recibo
                .Include(e => e.Atl)
                .Where(e => e.AtlId == currentUserAccount.AtlId)
                .ToListAsync();

            return View(recibos);
        }

        [Authorize(Roles = "EncarregadoEducacao")]
        public async Task<IActionResult> IndexEE()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var currentUserAccount = await _context.EncarregadoEducacao
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == currentUser.Id);

            var educandos = await _context.Educando
                .Include(e => e.Atl)
                .Include(e => e.Encarregado)
                .Where(e => e.EncarregadoId == currentUserAccount.EncarregadoId)
                .ToListAsync();

            var recibos = new List<ReciboRespostasViewModel>();

            foreach (var educando in educandos)
            {
                var respostas = await (from resposta in _context.ReciboResposta
                                       join educandoTable in _context.Educando on resposta.EducandoId equals educandoTable.EducandoId
                                       where resposta.EducandoId == educando.EducandoId
                                       select new ReciboRespostasViewModel
                                       {
                                           RespostaId = resposta.ReciboRespostaId,
                                           ReciboId = resposta.ReciboId,
                                           EducandoName = educandoTable.Name + " " + educandoTable.Apelido,
                                           Authorized = resposta.Authorized,
                                           ResponseDate = ((DateTime)resposta.ResponseDate).ToShortDateString(),
                                           ComprovativoPath = resposta.ComprovativoPath
                                       }).ToListAsync();

                recibos = recibos.Union(respostas).ToList();
            }

            ViewData["EducandoId"] = new SelectList(educandos, "EducandoId", "Name");
            return View(recibos);
        }

        // GET: Reciboes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Recibo == null)
            {
                return NotFound();
            }

            var recibo = await _context.Recibo
                .FirstOrDefaultAsync(m => m.ReciboId == id);
            if (recibo == null)
            {
                return NotFound();
            }

            return View(recibo);
        }

        public async Task<IActionResult> Respostas(Guid? id)
        {
            if (id == null || _context.Recibo == null)
            {
                return NotFound();
            }

            var respostas = await (from resposta in _context.ReciboResposta
                                   join educando in _context.Educando on resposta.EducandoId equals educando.EducandoId
                                   where resposta.ReciboId == id
                                   select new ReciboRespostasViewModel
                                   {
                                       RespostaId = resposta.ReciboRespostaId,
                                       ReciboId = resposta.ReciboId,
                                       EducandoName = educando.Name + " " + educando.Apelido,
                                       Authorized = resposta.Authorized,
                                       ResponseDate = ((DateTime)resposta.ResponseDate).ToShortDateString(),
                                       ComprovativoPath = resposta.ComprovativoPath
                                   }).ToListAsync();

            if (respostas == null)
            {
                return NotFound();
            }

            return View(respostas);
        }

        // GET: Reciboes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reciboes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,NIB,Description,DateLimit")] Recibo recibo)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == user.Id);

            if (ModelState.IsValid)
            {
                recibo.EmissionDate = DateTime.UtcNow.Date;
                recibo.AtlId = userAccount.AtlId;
                _context.Add(recibo);

                var educandos = await _context.Educando
                    .Include(c => c.Atl)
                    .Where(g => g.AtlId == userAccount.AtlId)
                    .ToListAsync();

                foreach (var educando in educandos)
                {
                    var resposta = new ReciboResposta(recibo.ReciboId, educando.EducandoId)
                    {
                        Name = recibo.Name,
                        Price = recibo.Price,
                        NIB = recibo.NIB,
                        Description = recibo.Description,
                        DateLimit = recibo.DateLimit
                    };

                    // Obter Encarregado do Educando e a sua conta
                    var encarregado = await _context.EncarregadoEducacao
                        .FirstOrDefaultAsync(e => e.EncarregadoId == educando.EncarregadoId);
                    var encarregadoAccount = await _context.Users
                        .FirstOrDefaultAsync(e => e.Id == encarregado.UserId);

                    var userEmail = await _userManager.GetEmailAsync(encarregadoAccount);
                    var code = resposta.ReciboRespostaId.ToString();
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Action("Responder", "ReciboRespostas", new { id = resposta.ReciboRespostaId }, Request.Scheme);

                    await _emailSender.SendEmailAsync(userEmail, "Novo recibo por responder",
                        $"Por favor responda ao recibo <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>aqui</a>.");

                    _context.Add(resposta);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(recibo);
        }

        // GET: Recibos/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Recibo == null)
            {
                return NotFound();
            }

            var recibo = await _context.Recibo.FindAsync(id);
            if (recibo == null)
            {
                return NotFound();
            }
            return View(recibo);
        }

        // POST: Recibos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ReciboId,Name,Price,NIB,Description,DateLimit")] Recibo recibo)
        {
            if (id != recibo.ReciboId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recibo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReciboExists(recibo.ReciboId))
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
            return View(recibo);
        }

        // GET: Reciboes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Recibo == null)
            {
                return NotFound();
            }

            var recibo = await _context.Recibo
                .FirstOrDefaultAsync(m => m.ReciboId == id);
            if (recibo == null)
            {
                return NotFound();
            }

            return View(recibo);
        }

        // POST: Reciboes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Recibo == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.Recibo'  is null.");
            }
            var recibo = await _context.Recibo.FindAsync(id);
            if (recibo != null)
            {
                _context.Recibo.Remove(recibo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReciboExists(Guid id)
        {
          return (_context.Recibo?.Any(e => e.ReciboId == id)).GetValueOrDefault();
        }
    }
}
