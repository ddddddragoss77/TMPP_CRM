using Microsoft.AspNetCore.Mvc;
using TMPP_CRM.Domain.ChainOfResponsibility;
using TMPP_CRM.Domain.Mediator;
using TMPP_CRM.Domain.State;
using TMPP_CRM.Domain.TemplateMethod;
using TMPP_CRM.Domain.Visitor;

namespace TMPP_CRM.Controllers
{
    public class Lab7Controller : Controller
    {
        public IActionResult Index() => View();

        // ─── Chain of Responsibility ─────────────────────────────────────────────
        [HttpGet]
        public IActionResult ChainOfResponsibility() => View(new ChainViewModel());

        [HttpPost]
        public IActionResult ChainOfResponsibility(ChainViewModel vm)
        {
            var chain  = SupportChainFactory.BuildChain();
            var ticket = new SupportTicket
            {
                Title       = vm.Title,
                Description = vm.Description,
                Level       = vm.Level,
                Category    = vm.Category
            };
            vm.Results  = chain.Handle(ticket);
            vm.Ticket   = ticket;
            vm.HasResult = true;
            return View(vm);
        }

        // ─── State ───────────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult State()
        {
            var machine = BuildMachineFromStep(0, string.Empty);
            return View(new StateViewModel { Machine = machine, StepsCompleted = 0 });
        }

        [HttpPost]
        public IActionResult State(StateViewModel vm)
        {
            var machine  = BuildMachineFromStep(vm.StepsCompleted, vm.SelectedRoute);
            var message  = string.Empty;

            if (machine.CurrentState.CanAdvance)
            {
                message = machine.Advance();
                vm.StepsCompleted++;
                vm.LastMessage = message;
            }

            vm.Machine   = machine;
            vm.HasResult = true;
            return View(vm);
        }

        private static TicketMachine BuildMachineFromStep(int stepsCompleted, string route)
        {
            var machine = new TicketMachine();
            if (!string.IsNullOrWhiteSpace(route)) machine.SelectedRoute = route;
            for (int i = 0; i < stepsCompleted && machine.CurrentState.CanAdvance; i++)
                machine.Advance();
            return machine;
        }

        // ─── Mediator ────────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Mediator() => View(new MediatorViewModel());

        [HttpPost]
        public IActionResult Mediator(MediatorViewModel vm)
        {
            var tower = new ControlTower();

            var aircraft = (vm.Callsigns ?? new List<string>())
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select((c, i) => new Aircraft
                {
                    Callsign     = c.Trim().ToUpper(),
                    AircraftType = vm.AircraftTypes?.ElementAtOrDefault(i) ?? "B737",
                    Origin       = vm.Origins?.ElementAtOrDefault(i) ?? "OTP",
                    Destination  = vm.Destinations?.ElementAtOrDefault(i) ?? "CLJ"
                }).ToList();

            foreach (var a in aircraft)
                tower.RegisterAircraft(a);

            vm.Events = new List<MediatorEvent>();

            foreach (var msg in vm.Messages ?? new List<string>())
            {
                if (string.IsNullOrWhiteSpace(msg)) continue;
                var parts    = msg.Split('|');
                var callsign = parts[0].Trim().ToUpper();
                var message  = parts.Length > 1 ? parts[1].Trim() : msg.Trim();
                var aircraft_ = aircraft.FirstOrDefault(a => a.Callsign == callsign);
                if (aircraft_ != null)
                    vm.Events.Add(aircraft_.Send(message));
            }

            vm.RegisteredAircraft = aircraft;
            vm.HasResult = true;
            return View(vm);
        }

        // ─── Template Method ─────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult TemplateMethod()
        {
            var vm = new TemplateMethodViewModel
            {
                AvailableReports = ReportGeneratorFactory.Available()
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult TemplateMethod(TemplateMethodViewModel vm)
        {
            vm.AvailableReports = ReportGeneratorFactory.Available();
            var generator = ReportGeneratorFactory.Create(vm.SelectedType);
            vm.Report     = generator.Generate();
            vm.HasResult  = true;
            return View(vm);
        }

        // ─── Visitor ─────────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Visitor()
        {
            var vm = new VisitorViewModel
            {
                Formats  = GetFormats(),
                Documents = DocumentCollection.BuildSample()
                               .ExportAll(new PdfExportVisitor())
                               .Select(r => r.ElementType).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Visitor(VisitorViewModel vm)
        {
            vm.Formats = GetFormats();

            IExportVisitor visitor = vm.SelectedFormat switch
            {
                "csv" => new CsvExportVisitor(),
                "xml" => new XmlExportVisitor(),
                _     => new PdfExportVisitor()
            };

            var collection = DocumentCollection.BuildSample();
            vm.Results     = collection.ExportAll(visitor);
            vm.HasResult   = true;
            vm.Documents   = vm.Results.Select(r => r.ElementType).ToList();
            return View(vm);
        }

        private static List<(string Key, string Name, string Icon, string Color)> GetFormats() =>
            new()
            {
                ("pdf", "PDF",  "ph-file-pdf", "red"),
                ("csv", "CSV",  "ph-file-csv", "green"),
                ("xml", "XML",  "ph-code",     "orange"),
            };
    }

    // ─── ViewModels ─────────────────────────────────────────────────────────────
    public class ChainViewModel
    {
        public string        Title       { get; set; } = "Eroare rețea intermitentă";
        public string        Description { get; set; } = "VPN-ul cade la fiecare 10 minute.";
        public SupportLevel  Level       { get; set; } = SupportLevel.Level1;
        public string        Category    { get; set; } = "Rețea";
        public bool          HasResult   { get; set; }
        public SupportTicket? Ticket     { get; set; }
        public List<HandlerResult> Results { get; set; } = new();
    }

    public class StateViewModel
    {
        public TicketMachine Machine       { get; set; } = new();
        public string         SelectedRoute { get; set; } = "Chișinău – București";
        public int            StepsCompleted { get; set; }
        public string         LastMessage   { get; set; } = string.Empty;
        public bool           HasResult     { get; set; }
    }

    public class MediatorViewModel
    {
        public List<string>?  Callsigns     { get; set; } = new() { "ROT401", "WZZ1234" };
        public List<string>?  AircraftTypes { get; set; } = new() { "B737",   "A320"    };
        public List<string>?  Origins       { get; set; } = new() { "OTP",    "CLJ"     };
        public List<string>?  Destinations  { get; set; } = new() { "CDG",    "LHR"     };
        public List<string>?  Messages      { get; set; } = new() { "ROT401|REQUEST_LANDING", "WZZ1234|REQUEST_TAKEOFF" };
        public List<Aircraft> RegisteredAircraft { get; set; } = new();
        public List<MediatorEvent> Events   { get; set; } = new();
        public bool           HasResult     { get; set; }
    }

    public class TemplateMethodViewModel
    {
        public string         SelectedType     { get; set; } = "sales";
        public GeneratedReport? Report         { get; set; }
        public bool           HasResult        { get; set; }
        public List<(string Key, string Name, string Icon, string Color, string Description)> AvailableReports { get; set; } = new();
    }

    public class VisitorViewModel
    {
        public string         SelectedFormat  { get; set; } = "pdf";
        public List<string>   Documents       { get; set; } = new();
        public List<(string ElementType, string Content)> Results { get; set; } = new();
        public bool           HasResult       { get; set; }
        public List<(string Key, string Name, string Icon, string Color)> Formats { get; set; } = new();
    }
}
