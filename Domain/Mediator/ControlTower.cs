namespace TMPP_CRM.Domain.Mediator
{
    // ─── Mediator Interface ──────────────────────────────────────────────────────
    public interface IATCMediator
    {
        void RegisterAircraft(Aircraft aircraft);
        MediatorEvent SendMessage(string fromCallsign, string message, string? toCallsign = null);
    }

    public class MediatorEvent
    {
        public string From      { get; set; } = string.Empty;
        public string To        { get; set; } = string.Empty; // "Tower" or specific callsign
        public string Message   { get; set; } = string.Empty;
        public string Response  { get; set; } = string.Empty;
        public DateTime SentAt  { get; set; } = DateTime.Now;
        public string Type      { get; set; } = "message"; // message / clearance / warning
    }

    // ─── Air Traffic Control Tower (Mediator) ────────────────────────────────────
    public class ControlTower : IATCMediator
    {
        private readonly List<Aircraft> _registered = new();
        private readonly List<string>   _activeRunways = new() { "RWY-01L", "RWY-19R" };
        private readonly Queue<string>  _landingQueue = new();

        public void RegisterAircraft(Aircraft aircraft)
        {
            aircraft.SetMediator(this);
            _registered.Add(aircraft);
        }

        public MediatorEvent SendMessage(string fromCallsign, string message, string? toCallsign = null)
        {
            var evt = new MediatorEvent
            {
                From    = fromCallsign,
                To      = toCallsign ?? "Tower",
                Message = message
            };

            if (message.Contains("REQUEST_LANDING"))
            {
                var runway = _activeRunways.FirstOrDefault() ?? "RWY-01L";
                evt.Response = $"Cleared to land on {runway}. Wind 270°/12kt. Squawk 7700.";
                evt.Type     = "clearance";
            }
            else if (message.Contains("REQUEST_TAKEOFF"))
            {
                evt.Response = $"Cleared for takeoff. Runway RWY-01L. Maintain runway heading, climb to FL150.";
                evt.Type     = "clearance";
            }
            else if (message.Contains("EMERGENCY"))
            {
                evt.Response = "All traffic hold position! Emergency declared. Clearing RWY-01L for priority landing.";
                evt.Type     = "warning";
                // Broadcast to all other aircraft
                foreach (var a in _registered.Where(a => a.Callsign != fromCallsign))
                    a.ReceiveInstruction("HOLD — Emergency in progress. ATC");
            }
            else
            {
                evt.Response = $"Roger {fromCallsign}, information Charlie. Altimeter 1013hPa.";
                evt.Type     = "message";
            }
            return evt;
        }
    }

    // ─── Colleague ───────────────────────────────────────────────────────────────
    public class Aircraft
    {
        private IATCMediator? _mediator;
        public string Callsign    { get; init; } = string.Empty;
        public string AircraftType { get; init; } = "B737";
        public string Origin      { get; init; } = string.Empty;
        public string Destination { get; init; } = string.Empty;
        public List<string> ReceivedInstructions { get; } = new();

        public void SetMediator(IATCMediator m) => _mediator = m;

        public MediatorEvent Send(string message, string? to = null) =>
            _mediator!.SendMessage(Callsign, message, to);

        public void ReceiveInstruction(string instruction) =>
            ReceivedInstructions.Add(instruction);
    }
}
