namespace TMPP_CRM.Domain.Interfaces
{
    // Abstract Products
    public interface IReportHeader
    {
        string Render();
    }

    public interface IReportBody
    {
        string Render();
    }

    public interface IReportFooter
    {
        string Render();
    }

    // Abstract Factory
    public interface IReportFactory
    {
        IReportHeader CreateHeader();
        IReportBody CreateBody();
        IReportFooter CreateFooter();
    }
}
