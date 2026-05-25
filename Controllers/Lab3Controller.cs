using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPP_CRM.Domain.Builders;
using TMPP_CRM.Domain.Entities;
using TMPP_CRM.Domain.Prototypes;
using TMPP_CRM.Infrastructure.Data;
using TMPP_CRM.Infrastructure.Singleton;

namespace TMPP_CRM.Controllers
{
    public class Lab3Controller : Controller
    {
        private readonly CrmDbContext _db;
        public Lab3Controller(CrmDbContext db) => _db = db;

        public IActionResult Index() => View();

        // ─── Builder ─────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Builder()
        {
            var vm = new BuilderViewModel
            {
                SavedOffers = await _db.Offers.OrderByDescending(o => o.CreatedAt).ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Builder(BuilderViewModel vm)
        {
            vm.SavedOffers = await _db.Offers.OrderByDescending(o => o.CreatedAt).ToListAsync();
            if (!ModelState.IsValid) return View(vm);

            var builder  = new OfferBuilder();
            var director = new OfferDirector(builder);
            var clientName = string.IsNullOrWhiteSpace(vm.ClientName) ? "Client Demo" : vm.ClientName;

            Offer builtOffer = vm.OfferType?.ToLower() switch
            {
                "premium" => director.BuildPremiumOffer(clientName),
                "custom"  => director.BuildCustomOffer(
                                "Ofertă Personalizată Enterprise",
                                clientName,
                                (vm.CustomProducts ?? new List<string>())
                                    .Where(p => !string.IsNullOrWhiteSpace(p)).ToArray(),
                                vm.CustomDiscount,
                                vm.CustomValidity),
                _         => director.BuildStandardOffer(clientName)
            };

            // Persists to DB
            var persisted = new PersistedOffer
            {
                Title       = builtOffer.Title,
                ClientName  = builtOffer.ClientName,
                OfferType   = vm.OfferType ?? "standard",
                Discount    = builtOffer.Discount,
                ValidityDays = builtOffer.ValidityDays,
                Notes       = builtOffer.Notes
            };
            persisted.SetProducts(builtOffer.Products);

            _db.Offers.Add(persisted);
            await _db.SaveChangesAsync();

            vm.HasResult    = true;
            vm.Result       = builtOffer;
            vm.PersistedId  = persisted.Id;
            vm.SavedOffers  = await _db.Offers.OrderByDescending(o => o.CreatedAt).ToListAsync();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOffer(Guid id)
        {
            var offer = await _db.Offers.FindAsync(id);
            if (offer != null) { _db.Offers.Remove(offer); await _db.SaveChangesAsync(); }
            return RedirectToAction("Builder");
        }

        // ─── Prototype ─────────────────────────────────────────────────────────────
        public IActionResult Prototype()
        {
            var original = new ReportTemplate("Raport Lunar Vanzari", "Ion Popescu");
            original.FormattingSettings["font"]     = "Arial";
            original.FormattingSettings["fontSize"] = "12";
            original.FormattingSettings["color"]    = "black";
            original.Sections.Add(new ReportSection("Introducere",  "Rezumatul activitatii lunare."));
            original.Sections.Add(new ReportSection("Analiza Date", "Grafice si tabele cu vanzari."));
            original.Sections.Add(new ReportSection("Concluzii",    "Recomandari pentru luna urmatoare."));

            var shallowCopy = original.ShallowCopy();
            var deepCopy    = original.DeepCopy();
            shallowCopy.Sections[0].Content = "[MODIFICAT în shallow copy]";

            return View(new PrototypeViewModel
            {
                Original    = original,
                ShallowCopy = shallowCopy,
                DeepCopy    = deepCopy,
                OriginalSection0Content = original.Sections[0].Content
            });
        }

        // ─── Singleton ─────────────────────────────────────────────────────────────
        public IActionResult Singleton()
        {
            var db = DatabaseConnectionManager.Instance;
            db.Connect("Server=localhost;Database=TMPP_CRM;Trusted_Connection=True;");
            db.BeginTransaction();
            db.CommitTransaction();

            return View(new SingletonViewModel
            {
                Status           = db.GetStatus(),
                IsConnected      = db.IsConnected,
                ConnectionString = db.ConnectionString,
                ConnectedAt      = db.ConnectedAt,
                HashCode1        = DatabaseConnectionManager.Instance.GetHashCode(),
                HashCode2        = DatabaseConnectionManager.Instance.GetHashCode()
            });
        }
    }

    // ─── ViewModels ──────────────────────────────────────────────────────────────
    public class BuilderViewModel
    {
        public string       OfferType      { get; set; } = "standard";
        public string       ClientName     { get; set; } = string.Empty;
        public List<string> CustomProducts { get; set; } = new() { "", "", "" };
        public decimal      CustomDiscount { get; set; } = 10m;
        public int          CustomValidity { get; set; } = 30;
        public bool         HasResult      { get; set; }
        public Offer?       Result         { get; set; }
        public Guid         PersistedId    { get; set; }
        public List<PersistedOffer> SavedOffers { get; set; } = new();
    }

    public class PrototypeViewModel
    {
        public ReportTemplate Original    { get; set; } = null!;
        public ReportTemplate ShallowCopy { get; set; } = null!;
        public ReportTemplate DeepCopy    { get; set; } = null!;
        public string OriginalSection0Content { get; set; } = string.Empty;
    }

    public class SingletonViewModel
    {
        public string   Status           { get; set; } = string.Empty;
        public bool     IsConnected      { get; set; }
        public string   ConnectionString { get; set; } = string.Empty;
        public DateTime ConnectedAt      { get; set; }
        public int      HashCode1        { get; set; }
        public int      HashCode2        { get; set; }
    }
}
