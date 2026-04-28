namespace TMPP_CRM.Domain.Decorator
{
    public abstract class NotificationDecorator : INotification
    {
        protected INotification _wrapper;

        public NotificationDecorator(INotification wrapper)
        {
            _wrapper = wrapper;
        }

        public virtual string Send(string message)
        {
            return _wrapper.Send(message);
        }
    }
}
