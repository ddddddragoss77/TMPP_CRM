namespace TMPP_CRM.Domain.Decorator
{
    public class SmsDecorator : NotificationDecorator
    {
        public SmsDecorator(INotification wrapper) : base(wrapper)
        {
        }

        public override string Send(string message)
        {
            var baseResult = base.Send(message);
            var smsResult = $"SMS sent with message: {message}";
            return $"{baseResult}\n{smsResult}";
        }
    }
}
