﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.Models;
using ATLManager.ViewModels;
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using ATLManager.Services;

namespace ATLManager.Controllers
{
    public class ATLController : Controller
    {
        private readonly ATLManagerAuthContext _context;
        private readonly UserManager<ATLManagerUser> _userManager;
        private readonly IFileManager _fileManager;

        private readonly string FolderName = "atls";
        private readonly List<string> allowedPrefixesNIPC = new() { "5", "6", "7", "8", "9" };

		public ATLController(ATLManagerAuthContext context,
            UserManager<ATLManagerUser> userManager,
            IFileManager fileManager)
        {
            _context = context;
            _userManager = userManager;
            _fileManager = fileManager;
        }

        // GET: ATL
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == user.Id);

            if (userAccount == null)
            {
                return NotFound();
            }

            var atls = await (from atl in _context.ATL
                              join atlAdmin in _context.ATLAdmin on atl.AtlId equals atlAdmin.AtlId
                              join admin in _context.ContaAdministrativa on atlAdmin.ContaId equals admin.ContaId
                              where admin.ContaId == userAccount.ContaId
                              select atl).Include(a => a.Agrupamento).ToListAsync();

            return View(atls);
        }

        // GET: ATL/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.ATL == null)
            {
                return NotFound();
            }

            var atl = await _context.ATL
                .Include(a => a.Agrupamento)
                .FirstOrDefaultAsync(m => m.AtlId == id);

            if (atl == null)
            {
                return NotFound();
            }

            return View(atl);
        }

        // GET: ATL/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == user.Id);

            if (userAccount == null)
            {
                return NotFound();
            }

            var agrupamentos = await _context.Agrupamento
                    .Where(g => g.ContaId == userAccount.ContaId)
                    .ToListAsync();

            ViewData["AgrupamentoId"] = new SelectList(agrupamentos, "AgrupamentoID", "Name");
            return View(new ATLCreateViewModel());
        }

        // POST: ATL/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ATLCreateViewModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.NIPC) && string.IsNullOrEmpty(viewModel.AgrupamentoId.ToString()))
            {
                var validationMessage = "É necessário introduzir um NIPC ou Agrupamento";
                ModelState.AddModelError("NIPC", validationMessage);
                ModelState.AddModelError("AtlId", validationMessage);
            }

            if (!string.IsNullOrEmpty(viewModel.NIPC) && !string.IsNullOrEmpty(viewModel.AgrupamentoId.ToString()))
            {
                var validationMessage = "Apenas permitido introduzir um NIPC ou um Agrupamento";
                ModelState.AddModelError("NIPC", validationMessage);
                ModelState.AddModelError("AtlId", validationMessage);
            }

            if (!string.IsNullOrEmpty(viewModel.NIPC))
            {
				if (!allowedPrefixesNIPC.Contains(viewModel.NIPC.Trim().Substring(0, 1)))
				{
					var validationMessage = "NIPC requer que o primeiro dígito seja 5, 6, 7, 8 ou 9.";
					ModelState.AddModelError("NIPC", validationMessage);
				}

				if (_context.ATL.Any(a => a.NIPC == viewModel.NIPC)
                    || _context.Agrupamento.Any(a => a.NIPC == viewModel.NIPC))
                {
                    var validationMessage = "Outro ATL/Agrupamento já contém este NIPC";
                    ModelState.AddModelError("NIPC", validationMessage);
                }
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == user.Id);

            if (ModelState.IsValid)
            {
				var atl = new ATL
				{
					AtlId = Guid.NewGuid(),
					Name = viewModel.Name,
					Address = viewModel.Address,
                    City = viewModel.City,
                    PostalCode = viewModel.PostalCode,
					AgrupamentoId = viewModel.AgrupamentoId,
                    NIPC = viewModel.NIPC
				};

                string fileName = _fileManager.UploadFile(viewModel.LogoPicture, FolderName);
                if (fileName != null)
                {
                    atl.LogoPicture = fileName;
                }
                else
                {
                    atl.LogoPicture = "logo.png";
                }

                var atlAdmin = new ATLAdmin
                {
                    AtlId = atl.AtlId,
                    ContaId = userAccount.ContaId
                };

                _context.Add(atl);
                _context.Add(atlAdmin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            if (userAccount == null)
            {
                return NotFound();
            }

            var agrupamentos = await _context.Agrupamento
                    .Where(g => g.ContaId == userAccount.ContaId)
                    .ToListAsync();

            ViewData["AgrupamentoId"] = new SelectList(agrupamentos, "AgrupamentoID", "Name");

            return View(viewModel);
        }

        // GET: ATL/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.ATL == null)
            {
                return NotFound();
            }

            var atl = await _context.ATL.FindAsync(id);
            if (atl == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == user.Id);

            if (userAccount == null)
            {
                return NotFound();
            }

            var agrupamentos = await _context.Agrupamento
                    .Where(g => g.ContaId == userAccount.ContaId)
                    .ToListAsync();

            ViewData["AgrupamentoId"] = new SelectList(agrupamentos, "AgrupamentoID", "Name", atl.AgrupamentoId);
            return View(new ATLEditViewModel(atl));
        }

        // POST: ATL/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ATLEditViewModel viewModel)
        {
            if (id != viewModel.AtlId)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(viewModel.NIPC) && string.IsNullOrEmpty(viewModel.AgrupamentoId.ToString()))
            {
                var validationMessage = "É necessário introduzir um NIPC ou ATLId";
                ModelState.AddModelError("NIPC", validationMessage);             
                ModelState.AddModelError("AtlId", validationMessage);
            }
            
            if (!string.IsNullOrEmpty(viewModel.NIPC) && !string.IsNullOrEmpty(viewModel.AgrupamentoId.ToString()))
            {
                var validationMessage = "Apenas permitido introduzir um NIPC ou um ATLId";
                ModelState.AddModelError("NIPC", validationMessage);
                ModelState.AddModelError("AtlId", validationMessage);
            }

            if (!string.IsNullOrEmpty(viewModel.NIPC))
            {
				if (!allowedPrefixesNIPC.Contains(viewModel.NIPC.Trim().Substring(0, 1)))
				{
					var validationMessage = "NIPC requer que o primeiro dígito seja 5, 6, 7, 8 ou 9.";
					ModelState.AddModelError("NIPC", validationMessage);
				}

				var ATL = _context.ATL.Find(viewModel.AtlId);
                
				if (ATL.NIPC != viewModel.NIPC &&
					_context.ATL.Any(a => a.NIPC == viewModel.NIPC)
                    || _context.Agrupamento.Any(a => a.NIPC == viewModel.NIPC))
                {
                    var validationMessage = "Outro ATL/Agrupamento já contém este NIPC";
                    ModelState.AddModelError("NIPC", validationMessage);
                }
			}

            if (ModelState.IsValid)
            {
                try
                {
                    var atl = await _context.ATL.FindAsync(id);

                    if (atl != null)
                    {
                        atl.Name = viewModel.Name;
                        atl.Address = viewModel.Address;
                        atl.City = viewModel.City;
                        atl.PostalCode = viewModel.PostalCode;
                        atl.AgrupamentoId = viewModel.AgrupamentoId;
                        atl.NIPC = viewModel.NIPC;

                        string fileName = _fileManager.UploadFile(viewModel.LogoPicture, FolderName);
                        if (fileName != null)
                        {
                            atl.LogoPicture = fileName;
                        }

                        _context.Update(atl);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ATLExists(viewModel.AtlId))
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

            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userAccount = await _context.ContaAdministrativa
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == user.Id);

            if (userAccount == null)
            {
                return NotFound();
            }

            var agrupamentos = await _context.Agrupamento
                    .Where(g => g.ContaId == userAccount.ContaId)
                    .ToListAsync();

            ViewData["AgrupamentoId"] = new SelectList(agrupamentos, "AgrupamentoID", "Name", viewModel.AgrupamentoId);
            return View(viewModel);
        }

        // GET: ATL/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.ATL == null)
            {
                return NotFound();
            }

            var atl = await _context.ATL
                .Include(a => a.Agrupamento)
                .FirstOrDefaultAsync(m => m.AtlId == id);
            if (atl == null)
            {
                return NotFound();
            }

            return View(atl);
        }

        // POST: ATL/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.ATL == null)
            {
                return Problem("Entity set 'ATLManagerAuthContext.ATL'  is null.");
            }
            var atl = await _context.ATL.FindAsync(id);
            if (atl != null)
            {
                _context.ATL.Remove(atl);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool ATLExists(Guid id)
        {
          return _context.ATL.Any(e => e.AtlId == id);
        }
	}
}
