using Microsoft.AspNetCore.Mvc;
using OfertareIndividuala.Data;
using OfertareIndividuala.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace OfertareIndividuala.Controllers
{
    public class ClientController : Controller
    {
        private readonly AppDbContext _context;

        public ClientController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Client/SelectClientType
        public IActionResult SelectClientType()
        {
            return View();
        }

        // POST: /Client/SelectClientType
        [HttpPost]
        public IActionResult SelectClientType(ClientTypeModel model)
        {
            if (ModelState.IsValid)
            {
                switch (model.ClientType)
                {
                    case "New":
                        return RedirectToAction("NewClientForm");
                    case "NewNoData":
                        return RedirectToAction("NewNoDataForm");
                    case "Starnet":
                        return RedirectToAction("StarnetClientForm");
                    default:
                        ModelState.AddModelError("", "Tip de client invalid.");
                        break;
                }
            }
            return View(model);
        }

        // GET: /Client/NewClientForm
        public IActionResult NewClientForm()
        {
            return View();
        }

        // GET: /Client/NewNoDataForm
        public IActionResult NewNoDataForm()
        {
            return View(new QuestionnaireModel());
        }

        // POST: /Client/NewNoDataForm
        [HttpPost]
        public IActionResult NewNoDataForm(QuestionnaireModel model)
        {
            if (ModelState.IsValid)
            {
                // Obține UserId din sesiune
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return RedirectToAction("Login", "Auth");
                }

                // Salvează răspunsurile în baza de date
                var response = new QuestionnaireResponse
                {
                    UserId = userId.Value,
                    SmartTvOffer = model.SmartTvOffer,
                    ElectronicsDiscount = model.ElectronicsDiscount,
                    WifiSatisfaction = model.WifiSatisfaction,
                    CreatedAt = DateTime.Now
                };
                _context.QuestionnaireResponses.Add(response);
                _context.SaveChanges();

                // Generează ofertele
                var offers = GenerateOffers(model);
                ViewBag.Offers = offers;
                ViewBag.QuestionnaireResponseId = response.Id;
                return View("OfferResults");
            }
            return View(model);
        }

        // GET: /Client/AddClientDetails
        public IActionResult AddClientDetails(int questionnaireResponseId)
        {
            ViewBag.QuestionnaireResponseId = questionnaireResponseId;
            return View(new ClientNouFaraDateModel());
        }

        // POST: /Client/AddClientDetails
        [HttpPost]
        public IActionResult AddClientDetails(ClientNouFaraDateModel model, int questionnaireResponseId)
        {
            if (ModelState.IsValid)
            {
                // Obține UserId din sesiune
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return RedirectToAction("Login", "Auth");
                }

                var client = new ClientNouFaraDate
                {
                    Nume_prenume_client = model.Nume_prenume_client,
                    Nr_telefon_client = model.Nr_telefon_client,
                    E_mail_client = model.E_mail_client,
                    QuestionnaireResponsesId = questionnaireResponseId,
                    Utilizator_App_Id = userId.Value
                };
                _context.Client_nou_fara_date.Add(client);
                _context.SaveChanges();

                // Actualizează QuestionnaireResponses cu ClientNouId
                var response = _context.QuestionnaireResponses.Find(questionnaireResponseId);
                if (response != null)
                {
                    response.ClientNouId = client.Id_client_nou_fara_date;
                    _context.SaveChanges();
                }

                // Setează mesajul de succes
                TempData["SuccessMessage"] = "Datele clientului au fost adăugate cu succes!";

                // Redirecționează către SelectClientType
                return RedirectToAction("SelectClientType");
            }
            ViewBag.QuestionnaireResponseId = questionnaireResponseId;
            return View(model);
        }

        // GET: /Client/StarnetClientForm
        public IActionResult StarnetClientForm()
        {
            return View();
        }

        // Metodă pentru generarea ofertelor din baza de date
        private List<OfferModel> GenerateOffers(QuestionnaireModel model)
        {
            var offers = new List<OfferModel>();

            // Logică bazată pe răspunsuri
            if (model.SmartTvOffer == "Yes")
            {
                // Oferte SMART TV (Type_Of_Oferta = 1)
                offers.AddRange(_context.Oferte_Speciale
                    .Where(o => o.Type_Of_Oferta == 1)
                    .Select(o => new OfferModel
                    {
                        Id_Oferta_Speciala = o.Id_Oferta_Speciala,
                        Denumire_Oferta_Speciala = o.Denumire_Oferta_Speciala,
                        Type_Of_Oferta = o.Type_Of_Oferta,
                        Suma_Oferta = o.Suma_Oferta,
                        Suma_Magazin = o.Suma_Magazin
                    }));
            }

            if (model.ElectronicsDiscount == "Yes")
            {
                // Oferte reduceri (Type_Of_Oferta = 2)
                offers.AddRange(_context.Oferte_Speciale
                    .Where(o => o.Type_Of_Oferta == 2)
                    .Select(o => new OfferModel
                    {
                        Id_Oferta_Speciala = o.Id_Oferta_Speciala,
                        Denumire_Oferta_Speciala = o.Denumire_Oferta_Speciala,
                        Type_Of_Oferta = o.Type_Of_Oferta,
                        Suma_Oferta = o.Suma_Oferta,
                        Suma_Magazin = o.Suma_Magazin
                    }));
            }

            if (model.WifiSatisfaction == "No")
            {
                // Oferte Wi-Fi (Type_Of_Oferta = 3, 4, 5, 7)
                offers.AddRange(_context.Oferte_Speciale
                    .Where(o => new[] { 3, 4, 5, 7 }.Contains(o.Type_Of_Oferta))
                    .Select(o => new OfferModel
                    {
                        Id_Oferta_Speciala = o.Id_Oferta_Speciala,
                        Denumire_Oferta_Speciala = o.Denumire_Oferta_Speciala,
                        Type_Of_Oferta = o.Type_Of_Oferta,
                        Suma_Oferta = o.Suma_Oferta,
                        Suma_Magazin = o.Suma_Magazin
                    }));
            }

            // Ofertă implicită dacă nu există alte oferte
            if (!offers.Any())
            {
                offers.AddRange(_context.Oferte_Speciale
                    .Where(o => o.Type_Of_Oferta == 2)
                    .Select(o => new OfferModel
                    {
                        Id_Oferta_Speciala = o.Id_Oferta_Speciala,
                        Denumire_Oferta_Speciala = o.Denumire_Oferta_Speciala,
                        Type_Of_Oferta = o.Type_Of_Oferta,
                        Suma_Oferta = o.Suma_Oferta,
                        Suma_Magazin = o.Suma_Magazin
                    }));
            }

            return offers;
        }
    }
}