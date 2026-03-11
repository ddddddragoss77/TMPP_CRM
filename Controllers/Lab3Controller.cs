using Microsoft.AspNetCore.Mvc;
using TMPP_CRM.Domain.Builders;
using TMPP_CRM.Domain.Prototypes;
using TMPP_CRM.Infrastructure.Singleton;
using System.Collections.Generic;

namespace TMPP_CRM.Controllers
{
    public class Lab3Controller : Controller
    {
        // ─── Index ───────────────────────────────────────────────────────────────
        public IActionResult Index()
        {
            return View();
        }

        // ─── UML ─────────────────────────────────────────────────────────────────
        public IActionResult Uml()
        {
            return View();
        }

        // ─── Builder ─────────────────────────────────────────────────────────────
        public IActionResult Builder(string type = "standard", string client = "Client Demo")
        {
            var builder = new OfferBuilder();
            var director = new OfferDirector(builder);

            Offer offer = type.ToLower() switch
            {
                "premium" => director.BuildPremiumOffer(client),
                "custom"  => director.BuildCustomOffer(
                                "Oferta Personalizata",
                                client,
                                new[] { "Modul A", "Modul B", "Modul C" },
                                12, 45),
                _         => director.BuildStandardOffer(client)
            };

            ViewBag.SelectedType = type;
            return View(offer);
        }

        // ─── Prototype ───────────────────────────────────────────────────────────
        public IActionResult Prototype()
        {
            // Creem template-ul original
            var original = new ReportTemplate("Raport Lunar Vanzari", "Ion Popescu");
            original.FormattingSettings["font"]     = "Arial";
            original.FormattingSettings["fontSize"] = "12";
            original.FormattingSettings["color"]    = "black";
            original.Sections.Add(new ReportSection("Introducere",   "Rezumatul activitatii lunare."));
            original.Sections.Add(new ReportSection("Analiza Date",  "Grafice si tabele cu vanzari."));
            original.Sections.Add(new ReportSection("Concluzii",     "Recomandari pentru luna urmatoare."));

            var shallowCopy = original.ShallowCopy();
            var deepCopy    = original.DeepCopy();

            // Modificam sectiunea 0 a shallow copy - pentru a demonstra efectul
            shallowCopy.Sections[0].Content = "[MODIFICAT în shallow copy]";

            var vm = new PrototypeViewModel
            {
                Original    = original,
                ShallowCopy = shallowCopy,
                DeepCopy    = deepCopy,
                // Demonstreaza daca originalul a fost afectat
                OriginalSection0Content = original.Sections[0].Content
            };

            return View(vm);
        }

        // ─── Singleton ───────────────────────────────────────────────────────────
        public IActionResult Singleton()
        {
            var db = DatabaseConnectionManager.Instance;

            // Simuleaza operatii
            db.Connect("Server=localhost;Database=TMPP_CRM;Trusted_Connection=True;");
            db.BeginTransaction();
            db.CommitTransaction();

            var vm = new SingletonViewModel
            {
                Status           = db.GetStatus(),
                IsConnected      = db.IsConnected,
                ConnectionString = db.ConnectionString,
                ConnectedAt      = db.ConnectedAt,
                HashCode1        = DatabaseConnectionManager.Instance.GetHashCode(),
                HashCode2        = DatabaseConnectionManager.Instance.GetHashCode()
            };

            return View(vm);
        }
    }

    // ─── ViewModels ──────────────────────────────────────────────────────────────

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
