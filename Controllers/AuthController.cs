using Microsoft.AspNetCore.Mvc;
using OfertareIndividuala.Data;
using OfertareIndividuala.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace OfertareIndividuala.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Utilizator_App
                    .FirstOrDefault(u => u.Nume_utilizator == model.Username && u.Password_utilizator == model.Password);

                if (user != null)
                {
                    // Stochează informațiile utilizatorului în sesiune
                    HttpContext.Session.SetInt32("UserId", user.Id_utilizator);
                    HttpContext.Session.SetString("UserType", user.Type_of_utilizator);

                    // Redirecționează în funcție de tipul utilizatorului
                    if (user.Type_of_utilizator == "Administrator")
                    {
                        return RedirectToAction("AdminPanel", "Admin");
                    }
                    else if (user.Type_of_utilizator == "UtilizatorDor2Dor")
                    {
                        return RedirectToAction("SelectClientType", "Client");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Tip de utilizator necunoscut.");
                        return View(model);
                    }
                }

                ModelState.AddModelError("", "Email sau parolă incorecte.");
            }
            return View(model);
        }

        // GET: /Auth/RecoverPassword
        public IActionResult RecoverPassword()
        {
            return View();
        }

        // POST: /Auth/RecoverPassword
        [HttpPost]
        public IActionResult RecoverPassword(RecoverPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Un link de resetare a parolei a fost trimis pe email.";
                return View();
            }
            return View(model);
        }

        // GET: /Auth/Logout
        public IActionResult Logout()
        {
            // Șterge toate datele din sesiune
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}