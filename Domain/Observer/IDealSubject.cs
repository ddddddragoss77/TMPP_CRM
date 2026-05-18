namespace TMPP_CRM.Domain.Observer
{
    public interface IDealSubject
    {
        void Attach(IDealObserver observer);
        void Detach(IDealObserver observer);
        void Notify();
    }
}
