namespace TMPP_CRM.Domain.State
{
    // ─── State Interface ─────────────────────────────────────────────────────────
    public interface ITicketMachineState
    {
        string StateName    { get; }
        string Description  { get; }
        string IconClass    { get; }
        string ColorClass   { get; }
        string Action(TicketMachine machine);
        bool   CanAdvance   { get; }
    }

    // ─── Context ─────────────────────────────────────────────────────────────────
    public class TicketMachine
    {
        private ITicketMachineState _state;
        public decimal InsertedAmount { get; set; }
        public decimal TicketPrice    { get; set; } = 12.50m;
        public string  SelectedRoute  { get; set; } = string.Empty;
        public string  IssuedTicketId { get; set; } = string.Empty;
        public List<string> EventLog  { get; } = new();

        public TicketMachine() => _state = new WaitingForCoinsState();

        public ITicketMachineState CurrentState => _state;

        public void SetState(ITicketMachineState state)
        {
            EventLog.Add($"[{DateTime.Now:HH:mm:ss}] Tranziție: {_state.StateName} → {state.StateName}");
            _state = state;
        }

        public string Advance()
        {
            var result = _state.Action(this);
            EventLog.Add($"[{DateTime.Now:HH:mm:ss}] {_state.StateName}: {result}");
            return result;
        }
    }

    // ─── Concrete States ─────────────────────────────────────────────────────────
    public class WaitingForCoinsState : ITicketMachineState
    {
        public string StateName   => "Așteptare Monede";
        public string Description => "Automatul așteaptă inserarea monedelor sau cardului.";
        public string IconClass   => "ph-coins";
        public string ColorClass  => "blue";
        public bool   CanAdvance  => true;

        public string Action(TicketMachine machine)
        {
            machine.InsertedAmount = machine.TicketPrice;
            machine.SetState(new ValidatingPaymentState());
            return $"Suma de {machine.InsertedAmount:C} introdusă. Tranziție la validare.";
        }
    }

    public class ValidatingPaymentState : ITicketMachineState
    {
        public string StateName   => "Validare Plată";
        public string Description => "Procesarea plății prin terminalul bancar.";
        public string IconClass   => "ph-shield-check";
        public string ColorClass  => "amber";
        public bool   CanAdvance  => true;

        public string Action(TicketMachine machine)
        {
            if (machine.InsertedAmount >= machine.TicketPrice)
            {
                machine.SetState(new IssuingTicketState());
                return "Plată validată prin 3D-Secure. Suma suficientă. Tranziție la emitere bilet.";
            }
            machine.SetState(new WaitingForCoinsState());
            return "Sumă insuficientă. Revenire la starea inițială.";
        }
    }

    public class IssuingTicketState : ITicketMachineState
    {
        public string StateName   => "Emitere Bilet";
        public string Description => "Imprimarea și eliberarea biletului electronic.";
        public string IconClass   => "ph-ticket";
        public string ColorClass  => "emerald";
        public bool   CanAdvance  => true;

        public string Action(TicketMachine machine)
        {
            machine.IssuedTicketId = "BLT-" + new Random().Next(10000, 99999);
            machine.SetState(new TicketIssuedState());
            return $"Bilet {machine.IssuedTicketId} emis cu succes! Rută: {machine.SelectedRoute}.";
        }
    }

    public class TicketIssuedState : ITicketMachineState
    {
        public string StateName   => "Bilet Emis";
        public string Description => "Tranzacție completă. Automatul este pregătit pentru o nouă sesiune.";
        public string IconClass   => "ph-check-circle";
        public string ColorClass  => "green";
        public bool   CanAdvance  => false;

        public string Action(TicketMachine machine) => "Sesiune completă. Automatul se resetează.";
    }
}
