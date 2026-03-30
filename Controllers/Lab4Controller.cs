using Microsoft.AspNetCore.Mvc;
using TMPP_CRM.Domain.Adapters;
using TMPP_CRM.Domain.Composite;
using TMPP_CRM.Domain.Facade;
using System;
using System.Collections.Generic;

namespace TMPP_CRM.Controllers
{
    public class Lab4Controller : Controller
    {
        public IActionResult Index() => View();

        // ─── Adapter ─────────────────────────────────────────────────────────────
        public IActionResult Adapter(string provider = "paypal", decimal amount = 150m)
        {
            IPaymentGateway gateway = provider.ToLower() switch
            {
                "stripe"    => new StripeAdapter(),
                "googlepay" => new GooglePayAdapter(),
                _           => new PayPalAdapter()
            };

            var result = gateway.ProcessPayment(amount, "RON", "Achizitie servicii CRM");
            var vm = new AdapterViewModel
            {
                SelectedProvider = provider,
                Result = result,
                Amount = amount,
                IsValid = gateway.ValidateTransaction(result.TransactionId)
            };

            return View(vm);
        }

        // ─── Composite ────────────────────────────────────────────────────────────
        public IActionResult Composite()
        {
            // Construim ierarhia meniului unui restaurant
            var meniuGlobal = new MenuGroup("Meniu Restaurant CRM Bistro", "Meniu complet al zilei");

            var micDejun = new MenuGroup("Mic Dejun", "Servit 07:00 – 11:00");
            micDejun.Add(new MenuItem("Ochiuri cu bacon",    28m, "2 oua, bacon crocant", "Mic Dejun"));
            micDejun.Add(new MenuItem("Pancakes cu frisca",  22m, "3 clatite, sirop artar", "Mic Dejun"));
            micDejun.Add(new MenuItem("Cafea Espresso",       9m, "Boabe arabica", "Bauturi"));
            micDejun.Add(new MenuItem("Suc de portocale",    12m, "Proaspat stors", "Bauturi"));

            var pranz = new MenuGroup("Pranz", "Servit 12:00 – 15:00");
            var supe = new MenuGroup("Supe", "Preparate la comanda");
            supe.Add(new MenuItem("Supa crema de ciuperci", 22m, "Cu crutoane", "Supe"));
            supe.Add(new MenuItem("Ciorba de burta",        25m, "Traditionala", "Supe"));
            var feluri = new MenuGroup("Feluri Principale");
            feluri.Add(new MenuItem("File de somon",        65m, "Cu legume la gratar", "Principale"));
            feluri.Add(new MenuItem("Pui cu rozmarin",      48m, "Cu cartofi natur", "Principale"));
            feluri.Add(new MenuItem("Risotto cu ciuperci",  42m, "Vegetal", "Principale"));
            pranz.Add(supe);
            pranz.Add(feluri);

            var deserturi = new MenuGroup("Deserturi", "Preparate de patisier");
            deserturi.Add(new MenuItem("Tiramisu",   28m, "Reteta italiana originala", "Desert"));
            deserturi.Add(new MenuItem("Cheesecake", 25m, "Cu coulis de fructe de padure", "Desert"));

            var combo = new MenuGroup("Meniu Combo Zilei ⭐", "Supa + Fel + Desert la pret special");
            combo.Add(new MenuItem("Ciorba de legume", 18m, "", "Supe"));
            combo.Add(new MenuItem("Cotlet de porc",   40m, "Cu piure", "Principale"));
            combo.Add(new MenuItem("Inghetata",        12m, "3 bile, topping la alegere", "Desert"));

            meniuGlobal.Add(micDejun);
            meniuGlobal.Add(pranz);
            meniuGlobal.Add(deserturi);
            meniuGlobal.Add(combo);

            return View(meniuGlobal);
        }

        // ─── Facade ───────────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Facade() => View(new FacadeViewModel());

        [HttpPost]
        public IActionResult Facade(FacadeViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var facade = new HotelBookingFacade();
            vm.Result = facade.BookRoom(vm.GuestName, vm.CheckIn, vm.CheckOut, vm.RoomType);
            return View(vm);
        }
    }

    // ─── ViewModels ──────────────────────────────────────────────────────────────

    public class AdapterViewModel
    {
        public string SelectedProvider { get; set; } = string.Empty;
        public PaymentResult Result { get; set; } = null!;
        public decimal Amount { get; set; }
        public bool IsValid { get; set; }
    }

    public class FacadeViewModel
    {
        public string GuestName { get; set; } = string.Empty;
        public DateTime CheckIn { get; set; } = DateTime.Today.AddDays(1);
        public DateTime CheckOut { get; set; } = DateTime.Today.AddDays(3);
        public string RoomType { get; set; } = "Standard";
        public BookingResult? Result { get; set; }
    }
}
