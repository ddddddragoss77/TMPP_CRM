namespace TMPP_CRM.Domain.Decorator
{
    public class PushDecorator : NotificationDecorator
    {
        public PushDecorator(INotification wrapper) : base(wrapper)
        {
        }

        public override string Send(string message)
        {
            var baseResult = base.Send(message);
            var pushResult = $"Push Notification sent with message: {message}";
            return $"{baseResult}\n{pushResult}";
        }
    }
}
