using Microsoft.AspNetCore.Mvc;
using OfertareIndividuala.Data;
using OfertareIndividuala.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OfertareIndividuala.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/AdminPanel
        public IActionResult AdminPanel()
        {
            if (!IsAdministrator()) return RedirectToAction("Login", "Auth");
            return View();
        }

        // GET: /Admin/ViewRecords
        public IActionResult ViewRecords()
        {
            if (!IsAdministrator()) return RedirectToAction("Login", "Auth");

            var records = from qr in _context.QuestionnaireResponses
                          join c in _context.Client_nou_fara_date on qr.ClientNouId equals c.Id_client_nou_fara_date into clientGroup
                          from c in clientGroup.DefaultIfEmpty()
                          join u in _context.Utilizator_App on qr.UserId equals u.Id_utilizator
                          where u.Type_of_utilizator == "UtilizatorDor2Dor"
                          select new RecordViewModel
                          {
                              QuestionnaireResponseId = qr.Id,
                              UserEmail = u.Nume_utilizator,
                              ClientName = c != null ? c.Nume_prenume_client : "N/A",
                              ClientPhone = c != null ? c.Nr_telefon_client : "N/A",
                              ClientEmail = c != null ? c.E_mail_client : "N/A",
                              SmartTvOffer = qr.SmartTvOffer,
                              ElectronicsDiscount = qr.ElectronicsDiscount,
                              WifiSatisfaction = qr.WifiSatisfaction,
                              CreatedAt = qr.CreatedAt
                          };

            return View(records.ToList());
        }

        // GET: /Admin/ManageUsers
        public IActionResult ManageUsers()
        {
            if (!IsAdministrator()) return RedirectToAction("Login", "Auth");

            var users = _context.Utilizator_App
                .Where(u => u.Type_of_utilizator == "UtilizatorDor2Dor")
                .Select(u => new UserViewModel
                {
                    Id_utilizator = u.Id_utilizator,
                    Nume_utilizator = u.Nume_utilizator,
                    Type_of_utilizator = u.Type_of_utilizator
                })
                .ToList();

            return View(users);
        }

        // GET: /Admin/AddUser
        public IActionResult AddUser()
        {
            if (!IsAdministrator()) return RedirectToAction("Login", "Auth");
            return View(new UserViewModel());
        }

        // POST: /Admin/AddUser
        [HttpPost]
        public IActionResult AddUser(UserViewModel model)
        {
            if (!IsAdministrator()) return RedirectToAction("Login", "Auth");

            if (ModelState.IsValid)
            {
                if (_context.Utilizator_App.Any(u => u.Nume_utilizator == model.Nume_utilizator))
                {
                    ModelState.AddModelError("Nume_utilizator", "Acest email este deja utilizat.");
                    return View(model);
                }

                if (model.Type_of_utilizator != "Administrator" && model.Type_of_utilizator != "UtilizatorDor2Dor")
                {
                    ModelState.AddModelError("Type_of_utilizator", "Tipul utilizatorului trebuie să fie Administrator sau UtilizatorDor2Dor.");
                    return View(model);
                }

                var newUser = new User
                {
                    Nume_utilizator = model.Nume_utilizator,
                    Password_utilizator = model.Password_utilizator,
                    Type_of_utilizator = model.Type_of_utilizator
                };

                _context.Utilizator_App.Add(newUser);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Utilizatorul a fost adăugat cu succes!";
                return RedirectToAction("ManageUsers");
            }

            return View(model);
        }

        // GET: /Admin/EditUser
        public IActionResult EditUser(int id)
        {
            if (!IsAdministrator()) return RedirectToAction("Login", "Auth");

            var user = _context.Utilizator_App.Find(id);
            if (user == null || user.Type_of_utilizator != "UtilizatorDor2Dor")
            {
                return NotFound();
            }

            var model = new UserViewModel
            {
                Id_utilizator = user.Id_utilizator,
                Nume_utilizator = user.Nume_utilizator,
                Password_utilizator = user.Password_utilizator,
                Type_of_utilizator = user.Type_of_utilizator
            };

            return View(model);
        }

        // POST: /Admin/EditUser
        [HttpPost]
        public IActionResult EditUser(UserViewModel model)
        {
            if (!IsAdministrator()) return RedirectToAction("Login", "Auth");

            if (ModelState.IsValid)
            {
                var user = _context.Utilizator_App.Find(model.Id_utilizator);
                if (user == null || user.Type_of_utilizator != "UtilizatorDor2Dor")
                {
                    return NotFound();
                }

                if (_context.Utilizator_App.Any(u => u.Nume_utilizator == model.Nume_utilizator && u.Id_utilizator != model.Id_utilizator))
                {
                    ModelState.AddModelError("Nume_utilizator", "Acest email este deja utilizat.");
                    return View(model);
                }

                user.Nume_utilizator = model.Nume_utilizator;
                user.Password_utilizator = model.Password_utilizator;
                user.Type_of_utilizator = "UtilizatorDor2Dor";

                _context.SaveChanges();

                TempData["SuccessMessage"] = "Utilizatorul a fost actualizat cu succes!";
                return RedirectToAction("ManageUsers");
            }

            return View(model);
        }

        // POST: /Admin/DeleteUser
        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            if (!IsAdministrator()) return RedirectToAction("Login", "Auth");

            var user = _context.Utilizator_App.Find(id);
            if (user == null || user.Type_of_utilizator != "UtilizatorDor2Dor")
            {
                return NotFound();
            }

            if (_context.QuestionnaireResponses.Any(qr => qr.UserId == id) ||
                _context.Client_nou_fara_date.Any(c => c.Utilizator_App_Id == id))
            {
                TempData["ErrorMessage"] = "Utilizatorul nu poate fi șters deoarece are înregistrări asociate.";
                return RedirectToAction("ManageUsers");
            }

            _context.Utilizator_App.Remove(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Utilizatorul a fost șters cu succes!";
            return RedirectToAction("ManageUsers");
        }

        // GET: /Admin/ManageOffers
        public IActionResult ManageOffers()
        {
            if (!IsAdministrator()) return RedirectToAction("Login", "Auth");

            var offers = _context.Oferte_Speciale
                .Select(o => new OfferModel
                {
                    Id_Oferta_Speciala = o.Id_Oferta_Speciala,
                    Denumire_Oferta_Speciala = o.Denumire_Oferta_Speciala,
                    Type_Of_Oferta = o.Type_Of_Oferta,
                    Suma_Oferta = o.Suma_Oferta,
                    Suma_Magazin = o.Suma_Magazin
                })
                .ToList();

            return View(offers);
        }

        // GET: /Admin/AddOffer
        public IActionResult AddOffer()
        {
            if (!IsAdministrator()) return RedirectToAction("Login", "Auth");
            return View(new OfferModel());
        }

        // POST: /Admin/AddOffer
        [HttpPost]
        public IActionResult AddOffer(OfferModel model)
        {
            if (!IsAdministrator()) return RedirectToAction("Login", "Auth");

            if (ModelState.IsValid)
            {
                var newOffer = new SpecialOffer
                {
                    Denumire_Oferta_Speciala = model.Denumire_Oferta_Speciala,
                    Type_Of_Oferta = model.Type_Of_Oferta,
                    Suma_Oferta = model.Suma_Oferta,
                    Suma_Magazin = model.Suma_Magazin
                };

                _context.Oferte_Speciale.Add(newOffer);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Oferta a fost adăugată cu succes!";
                return RedirectToAction("ManageOffers");
            }

            return View(model);
        }

        // GET: /Admin/EditOffer
        public IActionResult EditOffer(int id)
        {
            if (!IsAdministrator()) return RedirectToAction("Login", "Auth");

            var offer = _context.Oferte_Speciale.Find(id);
            if (offer == null)
            {
                return NotFound();
            }

            var model = new OfferModel
            {
                Id_Oferta_Speciala = offer.Id_Oferta_Speciala,
                Denumire_Oferta_Speciala = offer.Denumire_Oferta_Speciala,
                Type_Of_Oferta = offer.Type_Of_Oferta,
                Suma_Oferta = offer.Suma_Oferta,
                Suma_Magazin = offer.Suma_Magazin
            };

            return View(model);
        }

        // POST: /Admin/EditOffer
        [HttpPost]
        public IActionResult EditOffer(OfferModel model)
        {
            if (!IsAdministrator()) return RedirectToAction("Login", "Auth");

            if (ModelState.IsValid)
            {
                var offer = _context.Oferte_Speciale.Find(model.Id_Oferta_Speciala);
                if (offer == null)
                {
                    return NotFound();
                }

                offer.Denumire_Oferta_Speciala = model.Denumire_Oferta_Speciala;
                offer.Type_Of_Oferta = model.Type_Of_Oferta;
                offer.Suma_Oferta = model.Suma_Oferta;
                offer.Suma_Magazin = model.Suma_Magazin;

                _context.SaveChanges();

                TempData["SuccessMessage"] = "Oferta a fost actualizată cu succes!";
                return RedirectToAction("ManageOffers");
            }

            return View(model);
        }

        // POST: /Admin/DeleteOffer
        [HttpPost]
        public IActionResult DeleteOffer(int id)
        {
            if (!IsAdministrator()) return RedirectToAction("Login", "Auth");

            var offer = _context.Oferte_Speciale.Find(id);
            if (offer == null)
            {
                return NotFound();
            }

            _context.Oferte_Speciale.Remove(offer);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Oferta a fost ștearsă cu succes!";
            return RedirectToAction("ManageOffers");
        }

        private bool IsAdministrator()
        {
            return HttpContext.Session.GetString("UserType") == "Administrator";
        }
    }

    public class RecordViewModel
    {
        public int QuestionnaireResponseId { get; set; }
        public string UserEmail { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public string ClientEmail { get; set; }
        public string SmartTvOffer { get; set; }
        public string ElectronicsDiscount { get; set; }
        public string WifiSatisfaction { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserViewModel
    {
        public int Id_utilizator { get; set; }

        [Required(ErrorMessage = "Emailul este obligatoriu.")]
        [EmailAddress(ErrorMessage = "Introduceți un email valid.")]
        [RegularExpression(@"^[^@\s]+@starnet\.md$", ErrorMessage = "Emailul trebuie să fie de forma @starnet.md.")]
        public string Nume_utilizator { get; set; }

        [Required(ErrorMessage = "Parola este obligatorie.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Parola trebuie să aibă cel puțin 8 caractere.")]
        [DataType(DataType.Password)]
        public string Password_utilizator { get; set; }

        [Required(ErrorMessage = "Confirmarea parolei este obligatorie.")]
        [Compare("Password_utilizator", ErrorMessage = "Parolele nu coincid.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Tipul utilizatorului este obligatoriu.")]
        public string Type_of_utilizator { get; set; }
    }
}