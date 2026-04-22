namespace TMPP_CRM.Domain.Decorator
{
    public class EmailNotification : INotification
    {
        public string Send(string message)
        {
            return $"Email sent with message: {message}";
        }
    }
}
