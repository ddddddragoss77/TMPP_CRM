namespace TMPP_CRM.Domain.Command
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}
