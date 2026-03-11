using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;

namespace TMPP_CRM.Domain.Prototypes
{
    /// <summary>
    /// Sablon de raport CRM care implementeaza Prototype Pattern.
    /// Permite duplicarea rapida a rapoartelor existente cu continut si setari identice.
    /// 
    /// SHALLOW COPY: noul obiect partajeaza lista de sectiuni cu originalul.
    /// DEEP COPY:    noul obiect este complet independent (lista si sectiunile sunt copiate).
    /// </summary>
    public class ReportTemplate : IPrototype<ReportTemplate>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Setari de formatare (ex: { "font": "Arial", "fontSize": "12", "color": "black" }).
        /// </summary>
        public Dictionary<string, string> FormattingSettings { get; set; } = new();

        /// <summary>
        /// Sectiunile raportului (obiect de referinta – relevant pentru demo shallow vs deep).
        /// </summary>
        public List<ReportSection> Sections { get; set; } = new();

        // ─── Constructori ───────────────────────────────────────────────────────

        public ReportTemplate() { }

        public ReportTemplate(string title, string author)
        {
            Title = title;
            Author = author;
        }

        // ─── Prototype: Shallow Copy ─────────────────────────────────────────────

        /// <summary>
        /// Copie superficiala: creeaza un nou ReportTemplate, dar Sections si FormattingSettings
        /// sunt ACELEASI referinte ca in original. Modificarile la sectiuni afecteaza ambele obiecte!
        /// </summary>
        public ReportTemplate ShallowCopy()
        {
            var clone = (ReportTemplate)this.MemberwiseClone();
            clone.Id = Guid.NewGuid();               // ID unic pentru clona
            clone.CreatedAt = DateTime.UtcNow;       // Data noua
            clone.Title = $"{Title} (copie)";
            // Sections si FormattingSettings sunt PARTAJATE (shallow)
            return clone;
        }

        // ─── Prototype: Deep Copy ────────────────────────────────────────────────

        /// <summary>
        /// Copie profunda: noul obiect este complet independent.
        /// Sectiunile si setarile sunt copiate recursiv.
        /// </summary>
        public ReportTemplate DeepCopy()
        {
            return new ReportTemplate
            {
                Id = Guid.NewGuid(),
                Title = $"{Title} (copie completa)",
                Author = Author,
                CreatedAt = DateTime.UtcNow,
                // Deep copy pentru Dictionary
                FormattingSettings = new Dictionary<string, string>(FormattingSettings),
                // Deep copy pentru fiecare sectiune
                Sections = Sections.Select(s => s.Clone()).ToList()
            };
        }

        public override string ToString()
        {
            return $"[Raport] '{Title}' | Autor: {Author} | " +
                   $"Sectiuni: {Sections.Count} | Creat: {CreatedAt:dd.MM.yyyy HH:mm}";
        }
    }
}
