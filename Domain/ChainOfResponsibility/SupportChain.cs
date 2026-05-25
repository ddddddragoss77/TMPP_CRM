namespace TMPP_CRM.Domain.ChainOfResponsibility
{
    // ─── Niveluri de suport ──────────────────────────────────────────────────────
    public enum SupportLevel { Level1, Level2, Level3, Manager }

    public class SupportTicket
    {
        public string Id          { get; init; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
        public string Title       { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public SupportLevel Level { get; init; }
        public string Category    { get; init; } = string.Empty;
    }

    public class HandlerResult
    {
        public string HandlerName { get; set; } = string.Empty;
        public string Action      { get; set; } = string.Empty; // Handled / Forwarded / Unhandled
        public string Message     { get; set; } = string.Empty;
        public bool   Handled     { get; set; }
    }

    // ─── Base Handler ────────────────────────────────────────────────────────────
    public abstract class SupportHandler
    {
        public string Name { get; protected set; } = string.Empty;
        protected SupportHandler? _next;

        public SupportHandler SetNext(SupportHandler next)
        {
            _next = next;
            return next;
        }

        public abstract List<HandlerResult> Handle(SupportTicket ticket);
    }

    // ─── Concrete Handlers ───────────────────────────────────────────────────────
    public class Level1Handler : SupportHandler
    {
        public Level1Handler() => Name = "Suport Tehnic L1 (Helpdesk)";

        public override List<HandlerResult> Handle(SupportTicket ticket)
        {
            var log = new List<HandlerResult>();
            if (ticket.Level == SupportLevel.Level1)
            {
                log.Add(new HandlerResult
                {
                    HandlerName = Name, Action = "Handled", Handled = true,
                    Message = $"✓ Ticket #{ticket.Id} rezolvat de L1: resetare parolă, ghiduri de utilizare, FAQ."
                });
            }
            else
            {
                log.Add(new HandlerResult
                {
                    HandlerName = Name, Action = "Forwarded", Handled = false,
                    Message = $"⟶ Complexitate depășește L1. Transmis spre {(_next?.Name ?? "nimeni")}."
                });
                if (_next != null) log.AddRange(_next.Handle(ticket));
            }
            return log;
        }
    }

    public class Level2Handler : SupportHandler
    {
        public Level2Handler() => Name = "Suport Tehnic L2 (Specialist IT)";

        public override List<HandlerResult> Handle(SupportTicket ticket)
        {
            var log = new List<HandlerResult>();
            if (ticket.Level == SupportLevel.Level2)
            {
                log.Add(new HandlerResult
                {
                    HandlerName = Name, Action = "Handled", Handled = true,
                    Message = $"✓ Ticket #{ticket.Id} rezolvat de L2: diagnostic rețea, configurare software, acces VPN."
                });
            }
            else
            {
                log.Add(new HandlerResult
                {
                    HandlerName = Name, Action = "Forwarded", Handled = false,
                    Message = $"⟶ Necesită expertiză avansată. Escaldat la {(_next?.Name ?? "nimeni")}."
                });
                if (_next != null) log.AddRange(_next.Handle(ticket));
            }
            return log;
        }
    }

    public class Level3Handler : SupportHandler
    {
        public Level3Handler() => Name = "Suport Tehnic L3 (Inginer Senior)";

        public override List<HandlerResult> Handle(SupportTicket ticket)
        {
            var log = new List<HandlerResult>();
            if (ticket.Level == SupportLevel.Level3)
            {
                log.Add(new HandlerResult
                {
                    HandlerName = Name, Action = "Handled", Handled = true,
                    Message = $"✓ Ticket #{ticket.Id} rezolvat de L3: bug critica în cod, probleme de arhitectura, securitate."
                });
            }
            else
            {
                log.Add(new HandlerResult
                {
                    HandlerName = Name, Action = "Forwarded", Handled = false,
                    Message = $"⟶ Incident de nivel managerial. Escaldat la {(_next?.Name ?? "Management")}."
                });
                if (_next != null) log.AddRange(_next.Handle(ticket));
            }
            return log;
        }
    }

    public class ManagerHandler : SupportHandler
    {
        public ManagerHandler() => Name = "Management & Board";

        public override List<HandlerResult> Handle(SupportTicket ticket)
        {
            return new List<HandlerResult>
            {
                new HandlerResult
                {
                    HandlerName = Name, Action = "Handled", Handled = true,
                    Message = $"✓ Ticket #{ticket.Id} preluat de Management: incident major, decizie executivă, escalare client enterprise."
                }
            };
        }
    }

    // ─── Factory pentru lanțul complet ──────────────────────────────────────────
    public static class SupportChainFactory
    {
        public static SupportHandler BuildChain()
        {
            var l1  = new Level1Handler();
            var l2  = new Level2Handler();
            var l3  = new Level3Handler();
            var mgr = new ManagerHandler();
            l1.SetNext(l2).SetNext(l3).SetNext(mgr);
            return l1;
        }
    }
}
