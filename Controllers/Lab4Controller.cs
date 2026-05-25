using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPP_CRM.Domain.Adapters;
using TMPP_CRM.Domain.Composite;
using TMPP_CRM.Domain.Entities;
using TMPP_CRM.Domain.Facade;
using TMPP_CRM.Infrastructure.Data;

namespace TMPP_CRM.Controllers
{
    public class Lab4Controller : Controller
    {
        private readonly CrmDbContext _db;
        public Lab4Controller(CrmDbContext db) => _db = db;

        public IActionResult Index() => View();

        // ─── Adapter ─────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Adapter()
        {
            var vm = new AdapterInputViewModel
            {
                History = await _db.Transactions.OrderByDescending(t => t.CreatedAt).Take(10).ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Adapter(AdapterInputViewModel vm)
        {
            vm.History = await _db.Transactions.OrderByDescending(t => t.CreatedAt).Take(10).ToListAsync();
            if (!ModelState.IsValid) return View(vm);

            IPaymentGateway gateway = vm.Provider?.ToLower() switch
            {
                "stripe"    => new StripeAdapter(),
                "googlepay" => new GooglePayAdapter(),
                _           => new PayPalAdapter()
            };

            var desc = string.IsNullOrWhiteSpace(vm.Description) ? "Achizitie servicii CRM" : vm.Description;
            var result = gateway.ProcessPayment(vm.Amount, vm.Currency ?? "RON", desc);

            // Save to DB
            var tx = new Transaction
            {
                Provider       = vm.Provider ?? "paypal",
                Amount         = vm.Amount,
                Currency       = vm.Currency ?? "RON",
                Description    = desc,
                IsSuccess      = result.IsSuccess,
                TransactionRef = result.TransactionId,
                Result         = $"{result.Provider} — {(result.IsSuccess ? "APROBAT" : "RESPINS")} — {result.Amount:F2} {result.Currency}"
            };
            _db.Transactions.Add(tx);
            await _db.SaveChangesAsync();

            vm.Result    = result;
            vm.IsValid   = gateway.ValidateTransaction(result.TransactionId);
            vm.HasResult = true;
            vm.History   = await _db.Transactions.OrderByDescending(t => t.CreatedAt).Take(10).ToListAsync();
            return View(vm);
        }

        // ─── Composite ────────────────────────────────────────────────────────────
        public IActionResult Composite()
        {
            var meniuGlobal = new MenuGroup("Meniu Restaurant CRM Bistro", "Meniu complet al zilei");
            var micDejun    = new MenuGroup("Mic Dejun", "Servit 07:00 – 11:00");
            micDejun.Add(new MenuItem("Ochiuri cu bacon",   28m, "2 oua, bacon crocant",   "Mic Dejun"));
            micDejun.Add(new MenuItem("Pancakes cu frisca", 22m, "3 clatite, sirop artar", "Mic Dejun"));
            micDejun.Add(new MenuItem("Cafea Espresso",      9m, "Boabe arabica",          "Bauturi"));
            micDejun.Add(new MenuItem("Suc de portocale",   12m, "Proaspat stors",         "Bauturi"));
            var pranz  = new MenuGroup("Pranz", "Servit 12:00 – 15:00");
            var supe   = new MenuGroup("Supe", "Preparate la comanda");
            supe.Add(new MenuItem("Supa crema de ciuperci", 22m, "Cu crutoane",   "Supe"));
            supe.Add(new MenuItem("Ciorba de burta",        25m, "Traditionala",  "Supe"));
            var feluri = new MenuGroup("Feluri Principale");
            feluri.Add(new MenuItem("File de somon",       65m, "Cu legume la gratar", "Principale"));
            feluri.Add(new MenuItem("Pui cu rozmarin",     48m, "Cu cartofi natur",    "Principale"));
            feluri.Add(new MenuItem("Risotto cu ciuperci", 42m, "Vegetal",             "Principale"));
            pranz.Add(supe); pranz.Add(feluri);
            var deserturi = new MenuGroup("Deserturi", "Preparate de patisier");
            deserturi.Add(new MenuItem("Tiramisu",   28m, "Reteta italiana originala",        "Desert"));
            deserturi.Add(new MenuItem("Cheesecake", 25m, "Cu coulis de fructe de padure",   "Desert"));
            var combo = new MenuGroup("Meniu Combo Zilei ⭐", "Supa + Fel + Desert la pret special");
            combo.Add(new MenuItem("Ciorba de legume", 18m, "",               "Supe"));
            combo.Add(new MenuItem("Cotlet de porc",   40m, "Cu piure",       "Principale"));
            combo.Add(new MenuItem("Inghetata",        12m, "3 bile, topping","Desert"));
            meniuGlobal.Add(micDejun); meniuGlobal.Add(pranz);
            meniuGlobal.Add(deserturi); meniuGlobal.Add(combo);
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
    public class AdapterInputViewModel
    {
        public string         Provider    { get; set; } = "paypal";
        public decimal        Amount      { get; set; } = 150m;
        public string         Currency    { get; set; } = "RON";
        public string         Description { get; set; } = string.Empty;
        public bool           HasResult   { get; set; }
        public PaymentResult? Result      { get; set; }
        public bool           IsValid     { get; set; }
        public List<Transaction> History  { get; set; } = new();
    }

    public class FacadeViewModel
    {
        public string    GuestName { get; set; } = string.Empty;
        public DateTime  CheckIn   { get; set; } = DateTime.Today.AddDays(1);
        public DateTime  CheckOut  { get; set; } = DateTime.Today.AddDays(3);
        public string    RoomType  { get; set; } = "Standard";
        public BookingResult? Result { get; set; }
    }
}
