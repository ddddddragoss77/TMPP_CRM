namespace TMPP_CRM.Domain.Prototypes
{
    /// <summary>
    /// Sectiune dintr-un raport CRM (titlu + continut text).
    /// Este folosita pentru a demonstra diferenta dintre shallow si deep copy.
    /// </summary>
    public class ReportSection
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public ReportSection(string title, string content)
        {
            Title = title;
            Content = content;
        }

        /// <summary>
        /// Cloneaza sectiunea (obiect independent).
        /// </summary>
        public ReportSection Clone()
        {
            return new ReportSection(Title, Content);
        }

        public override string ToString() => $"[{Title}]: {Content}";
    }
}
