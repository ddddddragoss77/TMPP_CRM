using TMPP_CRM.Domain.Interfaces;

namespace TMPP_CRM.Domain.Entities.Reports
{
    // Concrete Products for CSV
    public class CsvReportHeader : IReportHeader
    {
        public string Render() => "ID,Name,Value";
    }

    public class CsvReportBody : IReportBody
    {
        public string Render() => "1,Item A,100\n2,Item B,200\n3,Item C,300";
    }

    public class CsvReportFooter : IReportFooter
    {
        public string Render() => "Total,Items,3";
    }

    // Concrete Products for JSON
    public class JsonReportHeader : IReportHeader
    {
        public string Render() => "{ \"report\": { \"header\": \"Sales Report\",";
    }

    public class JsonReportBody : IReportBody
    {
        public string Render() => "\"data\": [ { \"id\": 1, \"name\": \"Item A\", \"value\": 100 }, { \"id\": 2, \"name\": \"Item B\", \"value\": 200 } ],";
    }

    public class JsonReportFooter : IReportFooter
    {
        public string Render() => "\"footer\": { \"total_items\": 2 } } }";
    }
}
