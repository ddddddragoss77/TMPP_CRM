using TMPP_CRM.Domain.Interfaces;
using TMPP_CRM.Domain.Entities.Reports;

namespace TMPP_CRM.Domain.Factories
{
    // Concrete Factory for CSV
    public class CsvReportFactory : IReportFactory
    {
        public IReportHeader CreateHeader() => new CsvReportHeader();
        public IReportBody CreateBody() => new CsvReportBody();
        public IReportFooter CreateFooter() => new CsvReportFooter();
    }

    // Concrete Factory for JSON
    public class JsonReportFactory : IReportFactory
    {
        public IReportHeader CreateHeader() => new JsonReportHeader();
        public IReportBody CreateBody() => new JsonReportBody();
        public IReportFooter CreateFooter() => new JsonReportFooter();
    }
}
