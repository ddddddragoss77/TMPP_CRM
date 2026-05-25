namespace TMPP_CRM.Domain.Visitor
{
    // ─── Visitable elements ──────────────────────────────────────────────────────
    public interface IExportable
    {
        string Accept(IExportVisitor visitor);
        string ElementType { get; }
    }

    // ─── Visitor Interface ───────────────────────────────────────────────────────
    public interface IExportVisitor
    {
        string VisitDealDocument(DealDocument deal);
        string VisitLeadDocument(LeadDocument lead);
        string VisitInvoiceDocument(InvoiceDocument invoice);
    }

    // ─── Concrete Elements ───────────────────────────────────────────────────────
    public class DealDocument : IExportable
    {
        public string ElementType { get; } = "Deal";
        public string Title       { get; set; } = string.Empty;
        public string Client      { get; set; } = string.Empty;
        public decimal Value      { get; set; }
        public string Stage       { get; set; } = string.Empty;
        public string Agent       { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Accept(IExportVisitor visitor) => visitor.VisitDealDocument(this);
    }

    public class LeadDocument : IExportable
    {
        public string ElementType { get; } = "Lead";
        public string FullName    { get; set; } = string.Empty;
        public string Email       { get; set; } = string.Empty;
        public string Phone       { get; set; } = string.Empty;
        public string Status      { get; set; } = string.Empty;
        public string Source      { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Accept(IExportVisitor visitor) => visitor.VisitLeadDocument(this);
    }

    public class InvoiceDocument : IExportable
    {
        public string ElementType { get; } = "Factură";
        public string InvoiceNo   { get; set; } = string.Empty;
        public string Client      { get; set; } = string.Empty;
        public decimal Amount     { get; set; }
        public string Currency    { get; set; } = "RON";
        public string Status      { get; set; } = string.Empty;
        public DateTime IssuedAt  { get; set; } = DateTime.Now;

        public string Accept(IExportVisitor visitor) => visitor.VisitInvoiceDocument(this);
    }

    // ─── Concrete Visitors ───────────────────────────────────────────────────────
    public class PdfExportVisitor : IExportVisitor
    {
        public string FormatName  => "PDF";
        public string Icon        => "ph-file-pdf";
        public string Color       => "red";
        public string MimeType    => "application/pdf";

        public string VisitDealDocument(DealDocument d) =>
            $"""
            ╔══════════════════════════════════════════════════════╗
            ║              NEXUS CRM — DOCUMENT DEAL               ║
            ║                   [FORMAT PDF]                        ║
            ╚══════════════════════════════════════════════════════╝

            DEAL: {d.Title}
            ─────────────────────────────────────────────────────
            Client:      {d.Client}
            Valoare:     {d.Value:N2} lei
            Stadiu:      {d.Stage}
            Agent:       {d.Agent}
            Data:        {d.CreatedAt:dd.MM.yyyy}

            ─────────────────────────────────────────────────────
            [Semnatură digitală]  [Ștampilă CRM]
            """;

        public string VisitLeadDocument(LeadDocument l) =>
            $"""
            ╔══════════════════════════════════════════════════════╗
            ║            NEXUS CRM — FIȘĂ LEAD                    ║
            ║                   [FORMAT PDF]                        ║
            ╚══════════════════════════════════════════════════════╝

            LEAD: {l.FullName}
            ─────────────────────────────────────────────────────
            Email:       {l.Email}
            Telefon:     {l.Phone}
            Status:      {l.Status}
            Sursă:       {l.Source}
            Înregistrat: {l.CreatedAt:dd.MM.yyyy}

            [Semnatură digitală]  [Ștampilă CRM]
            """;

        public string VisitInvoiceDocument(InvoiceDocument i) =>
            $"""
            ╔══════════════════════════════════════════════════════╗
            ║            NEXUS CRM — FACTURĂ FISCALĂ              ║
            ║                   [FORMAT PDF]                        ║
            ╚══════════════════════════════════════════════════════╝

            FACTURĂ NR: {i.InvoiceNo}
            ─────────────────────────────────────────────────────
            Client:      {i.Client}
            Suma:        {i.Amount:N2} {i.Currency}
            Status:      {i.Status}
            Emisă:       {i.IssuedAt:dd.MM.yyyy}

            [Semnatură digitală]  [Ștampilă fiscală]
            """;
    }

    public class CsvExportVisitor : IExportVisitor
    {
        public string FormatName  => "CSV";
        public string Icon        => "ph-file-csv";
        public string Color       => "green";
        public string MimeType    => "text/csv";

        public string VisitDealDocument(DealDocument d) =>
            $"tip,titlu,client,valoare,stadiu,agent,data\n" +
            $"Deal,\"{d.Title}\",\"{d.Client}\",{d.Value},{d.Stage},{d.Agent},{d.CreatedAt:dd.MM.yyyy}";

        public string VisitLeadDocument(LeadDocument l) =>
            $"tip,nume_complet,email,telefon,status,sursa,data_inregistrare\n" +
            $"Lead,\"{l.FullName}\",{l.Email},{l.Phone},{l.Status},{l.Source},{l.CreatedAt:dd.MM.yyyy}";

        public string VisitInvoiceDocument(InvoiceDocument i) =>
            $"tip,numar_factura,client,suma,moneda,status,data_emitere\n" +
            $"Factură,{i.InvoiceNo},\"{i.Client}\",{i.Amount},{i.Currency},{i.Status},{i.IssuedAt:dd.MM.yyyy}";
    }

    public class XmlExportVisitor : IExportVisitor
    {
        public string FormatName  => "XML";
        public string Icon        => "ph-code";
        public string Color       => "orange";
        public string MimeType    => "application/xml";

        public string VisitDealDocument(DealDocument d) =>
            $"""
            <?xml version="1.0" encoding="UTF-8"?>
            <CrmExport type="Deal" generated="{DateTime.Now:o}">
              <Deal>
                <Title>{d.Title}</Title>
                <Client>{d.Client}</Client>
                <Value currency="RON">{d.Value}</Value>
                <Stage>{d.Stage}</Stage>
                <Agent>{d.Agent}</Agent>
                <CreatedAt>{d.CreatedAt:yyyy-MM-dd}</CreatedAt>
              </Deal>
            </CrmExport>
            """;

        public string VisitLeadDocument(LeadDocument l) =>
            $"""
            <?xml version="1.0" encoding="UTF-8"?>
            <CrmExport type="Lead" generated="{DateTime.Now:o}">
              <Lead>
                <FullName>{l.FullName}</FullName>
                <Email>{l.Email}</Email>
                <Phone>{l.Phone}</Phone>
                <Status>{l.Status}</Status>
                <Source>{l.Source}</Source>
                <CreatedAt>{l.CreatedAt:yyyy-MM-dd}</CreatedAt>
              </Lead>
            </CrmExport>
            """;

        public string VisitInvoiceDocument(InvoiceDocument i) =>
            $"""
            <?xml version="1.0" encoding="UTF-8"?>
            <CrmExport type="Invoice" generated="{DateTime.Now:o}">
              <Invoice>
                <Number>{i.InvoiceNo}</Number>
                <Client>{i.Client}</Client>
                <Amount currency="{i.Currency}">{i.Amount}</Amount>
                <Status>{i.Status}</Status>
                <IssuedAt>{i.IssuedAt:yyyy-MM-dd}</IssuedAt>
              </Invoice>
            </CrmExport>
            """;
    }

    // ─── Document collection that accepts visitors ────────────────────────────────
    public class DocumentCollection
    {
        private readonly List<IExportable> _documents = new();

        public void Add(IExportable doc) => _documents.Add(doc);

        public List<(string ElementType, string Content)> ExportAll(IExportVisitor visitor) =>
            _documents.Select(d => (d.ElementType, d.Accept(visitor))).ToList();

        public static DocumentCollection BuildSample() => new()
        {
            _documents =
            {
                new DealDocument { Title = "Contract Implementare ERP", Client = "Alfa Tech SRL", Value = 48500m, Stage = "Negotiation", Agent = "Mihai Popescu", CreatedAt = DateTime.Now.AddDays(-5) },
                new LeadDocument { FullName = "Ana Constantin", Email = "ana.c@example.ro", Phone = "+40720111222", Status = "Qualified", Source = "LinkedIn", CreatedAt = DateTime.Now.AddDays(-2) },
                new InvoiceDocument { InvoiceNo = "FC-2025-0047", Client = "Beta Systems SA", Amount = 12750m, Currency = "RON", Status = "Emisă", IssuedAt = DateTime.Now.AddDays(-1) },
            }
        };
    }
}
