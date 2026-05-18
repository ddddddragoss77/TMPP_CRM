using TMPP_CRM.Domain.Entities;
using TMPP_CRM.Domain.Enums;

namespace TMPP_CRM.Domain.Command
{
    public class ChangeDealStageCommand : ICommand
    {
        private readonly Deal _deal;
        private readonly DealStage _newStage;
        private DealStage _previousStage;

        public ChangeDealStageCommand(Deal deal, DealStage newStage)
        {
            _deal = deal;
            _newStage = newStage;
        }

        public void Execute()
        {
            _previousStage = _deal.Stage;
            _deal.Stage = _newStage;
        }

        public void Undo()
        {
            _deal.Stage = _previousStage;
        }
    }
}
