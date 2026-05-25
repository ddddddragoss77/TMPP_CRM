namespace TMPP_CRM.Domain.TemplateMethod
{
    // ─── Data models ────────────────────────────────────────────────────────────
    public class ReportSection
    {
        public string Title   { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Icon    { get; set; } = "ph-file-text";
        public string Color   { get; set; } = "blue";
    }

    public class GeneratedReport
    {
        public string              ReportType  { get; set; } = string.Empty;
        public string              Title       { get; set; } = string.Empty;
        public string              Subtitle    { get; set; } = string.Empty;
        public string              GeneratedAt { get; set; } = DateTime.Now.ToString("dd MMMM yyyy, HH:mm");
        public List<ReportSection> Sections    { get; set; } = new();
        public string              Footer      { get; set; } = string.Empty;
        public string              Summary     { get; set; } = string.Empty;
        public List<string>        Steps       { get; set; } = new();
    }

    // ─── Abstract Template ───────────────────────────────────────────────────────
    public abstract class CrmReportGenerator
    {
        public string ReportName { get; protected set; } = string.Empty;
        public string Icon       { get; protected set; } = "ph-file-text";
        public string Color      { get; protected set; } = "blue";
        public string Description { get; protected set; } = string.Empty;

        // Template Method — defines the algorithm skeleton
        public GeneratedReport Generate(object? data = null)
        {
            var report = new GeneratedReport { ReportType = ReportName };
            report.Steps.Add($"1. Inițializare raport: {ReportName}");

            SetHeader(report);
            report.Steps.Add("2. Antet configurat");

            FetchData(report, data);
            report.Steps.Add("3. Date preluate din sursă");

            ProcessData(report);
            report.Steps.Add("4. Date procesate și agregate");

            BuildSections(report);
            report.Steps.Add("5. Secțiuni construite");

            SetFooter(report);
            report.Steps.Add("6. Subsol și sumar generat");

            return report;
        }

        protected abstract void SetHeader(GeneratedReport report);
        protected abstract void FetchData(GeneratedReport report, object? data);
        protected abstract void ProcessData(GeneratedReport report);
        protected abstract void BuildSections(GeneratedReport report);

        // Hook — can be overridden but has a default
        protected virtual void SetFooter(GeneratedReport report)
        {
            report.Footer = $"Raport generat automat de Nexus CRM la {DateTime.Now:dd.MM.yyyy HH:mm}.";
        }
    }

    // ─── Concrete: Sales Report ──────────────────────────────────────────────────
    public class SalesReportGenerator : CrmReportGenerator
    {
        public SalesReportGenerator()
        {
            ReportName  = "Raport Vânzări";
            Icon        = "ph-chart-line-up";
            Color       = "emerald";
            Description = "Analiză completă a performanței vânzărilor pe perioada selectată.";
        }

        protected override void SetHeader(GeneratedReport report)
        {
            report.Title    = "Raport de Performanță — Vânzări";
            report.Subtitle = $"Perioada: {DateTime.Now.AddDays(-30):dd MMM} – {DateTime.Now:dd MMM yyyy}";
        }

        protected override void FetchData(GeneratedReport report, object? data)
        {
            report.Summary = "Preluare deal-uri active, câștigate și pierdute din baza de date CRM.";
        }

        protected override void ProcessData(GeneratedReport report)
        {
            report.Summary += " | Calcul rate de conversie, valori medii și pipeline total.";
        }

        protected override void BuildSections(GeneratedReport report)
        {
            report.Sections = new List<ReportSection>
            {
                new() { Title = "Pipeline Total",       Content = "47 deal-uri active • Valoare: 234.500 lei",        Icon = "ph-funnel",        Color = "blue"    },
                new() { Title = "Deal-uri Câștigate",   Content = "12 închise cu succes • Rată conversie: 25,5%",     Icon = "ph-trophy",        Color = "emerald" },
                new() { Title = "Deal-uri Pierdute",    Content = "8 oportunități pierdute • Cauza: Preț (62%)",      Icon = "ph-x-circle",      Color = "red"     },
                new() { Title = "Top Agenți",           Content = "1. Mihai P. • 2. Ana C. • 3. Radu I.",            Icon = "ph-medal",         Color = "amber"   },
                new() { Title = "Previziune Q2",        Content = "Estimare: 180.000 lei • Probabilitate: 68%",       Icon = "ph-trend-up",      Color = "purple"  },
            };
        }

        protected override void SetFooter(GeneratedReport report)
        {
            report.Footer = $"Raport Vânzări — Confidențial. Generat la {DateTime.Now:dd.MM.yyyy HH:mm} de sistemul Nexus CRM v3.2.";
        }
    }

    // ─── Concrete: Leads Report ──────────────────────────────────────────────────
    public class LeadsReportGenerator : CrmReportGenerator
    {
        public LeadsReportGenerator()
        {
            ReportName  = "Raport Lead-uri";
            Icon        = "ph-user-focus";
            Color       = "blue";
            Description = "Monitorizarea și calificarea lead-urilor din toate sursele de achiziție.";
        }

        protected override void SetHeader(GeneratedReport report)
        {
            report.Title    = "Raport Calificare Lead-uri";
            report.Subtitle = $"Sursă: toate canalele • Actualizat: {DateTime.Now:dd MMM yyyy}";
        }

        protected override void FetchData(GeneratedReport report, object? data)
        {
            report.Summary = "Preluare lead-uri din: formulare web, campanii email, LinkedIn, referințe directe.";
        }

        protected override void ProcessData(GeneratedReport report)
        {
            report.Summary += " | Scorare automată, deduplicare și alocare agenți.";
        }

        protected override void BuildSections(GeneratedReport report)
        {
            report.Sections = new List<ReportSection>
            {
                new() { Title = "Lead-uri Noi",          Content = "83 lead-uri primite săptămâna aceasta",            Icon = "ph-user-plus",     Color = "blue"    },
                new() { Title = "Calificați (MQL)",      Content = "31 lead-uri calificate marketing (37,3%)",         Icon = "ph-check-square",  Color = "emerald" },
                new() { Title = "Surse Top",             Content = "Web: 42% • LinkedIn: 29% • Referințe: 18%",        Icon = "ph-globe",         Color = "indigo"  },
                new() { Title = "Timp Mediu Răspuns",    Content = "2h 14min (obiectiv: sub 3h) ✓",                    Icon = "ph-clock",         Color = "amber"   },
                new() { Title = "Rată Conversie Lead→Deal", Content = "Lead→Deal: 18,2% • Benchmark industrie: 15%",  Icon = "ph-arrows-split",  Color = "purple"  },
            };
        }
    }

    // ─── Concrete: Activity Report ───────────────────────────────────────────────
    public class ActivityReportGenerator : CrmReportGenerator
    {
        public ActivityReportGenerator()
        {
            ReportName  = "Raport Activitate";
            Icon        = "ph-activity";
            Color       = "violet";
            Description = "Jurnal complet al activităților echipei: apeluri, întâlniri, email-uri și sarcini.";
        }

        protected override void SetHeader(GeneratedReport report)
        {
            report.Title    = "Raport Activitate Echipă";
            report.Subtitle = $"Bilanț lunar • {DateTime.Now:MMMM yyyy}";
        }

        protected override void FetchData(GeneratedReport report, object? data)
        {
            report.Summary = "Preluare activități loggate: apeluri telefonice, demo-uri, email-uri și taskuri.";
        }

        protected override void ProcessData(GeneratedReport report)
        {
            report.Summary += " | Calculul KPI-urilor per agent și per canal de comunicare.";
        }

        protected override void BuildSections(GeneratedReport report)
        {
            report.Sections = new List<ReportSection>
            {
                new() { Title = "Total Activități",      Content = "312 activități loggate • +14% față de luna anterioară", Icon = "ph-chart-bar",    Color = "violet"  },
                new() { Title = "Apeluri Telefonice",    Content = "148 apeluri • Durată medie: 8 min 22 sec",              Icon = "ph-phone-call",   Color = "blue"    },
                new() { Title = "Întâlniri / Demo-uri",  Content = "27 demo-uri susținute • Prezență: 91%",                 Icon = "ph-video-camera", Color = "emerald" },
                new() { Title = "Email-uri Trimise",     Content = "215 email-uri • Rată deschidere: 34%",                  Icon = "ph-envelope",     Color = "amber"   },
                new() { Title = "Taskuri Completate",    Content = "89 taskuri finalizate din 94 planificate (94,6%)",       Icon = "ph-check-circle", Color = "green"   },
            };
        }

        protected override void SetFooter(GeneratedReport report)
        {
            report.Footer = $"Raport Activitate — Uz intern echipă. Export: {DateTime.Now:dd.MM.yyyy HH:mm}.";
        }
    }

    // ─── Factory ────────────────────────────────────────────────────────────────
    public static class ReportGeneratorFactory
    {
        public static CrmReportGenerator Create(string type) => type switch
        {
            "sales"    => new SalesReportGenerator(),
            "leads"    => new LeadsReportGenerator(),
            "activity" => new ActivityReportGenerator(),
            _          => new SalesReportGenerator()
        };

        public static List<(string Key, string Name, string Icon, string Color, string Description)> Available() =>
            new()
            {
                ("sales",    "Raport Vânzări",   "ph-chart-line-up", "emerald", "Performanța vânzărilor și pipeline"),
                ("leads",    "Raport Lead-uri",  "ph-user-focus",    "blue",    "Calificarea și conversia lead-urilor"),
                ("activity", "Raport Activitate","ph-activity",      "violet",  "Jurnalul complet al activităților echipei"),
            };
    }
}
