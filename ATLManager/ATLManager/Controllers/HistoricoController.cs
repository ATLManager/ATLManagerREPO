﻿using Microsoft.AspNetCore.Mvc;

namespace ATLManager.Controllers
{
    public class HistoricoController : Controller
    {
        public IActionResult Escolher()
        {
            return View();
        }
    }
}