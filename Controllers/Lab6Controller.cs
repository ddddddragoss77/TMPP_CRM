using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPP_CRM.Domain.Command;
using TMPP_CRM.Domain.Entities;
using TMPP_CRM.Domain.Enums;
using TMPP_CRM.Domain.Iterator;
using TMPP_CRM.Domain.Memento;
using TMPP_CRM.Domain.Observer;
using TMPP_CRM.Domain.Strategy;
using TMPP_CRM.Infrastructure.Data;

namespace TMPP_CRM.Controllers
{
    public class Lab6Controller : Controller
    {
        private readonly CrmDbContext _db;
        public Lab6Controller(CrmDbContext db) => _db = db;

        public IActionResult Index() => View();

        // ─── Strategy ────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Strategy()
        {
            var vm = new StrategyViewModel
            {
                Deals = await _db.Deals.OrderBy(d => d.Title).ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Strategy(StrategyViewModel vm)
        {
            vm.Deals = await _db.Deals.OrderBy(d => d.Title).ToListAsync();
            if (!ModelState.IsValid) return View(vm);

            // Load real deal from DB
            Deal? deal = vm.SelectedDealId != Guid.Empty
                ? await _db.Deals.FindAsync(vm.SelectedDealId)
                : null;

            if (deal == null)
            {
                deal = new Deal { Title = vm.DealTitle, Value = vm.DealValue };
            }
            else
            {
                vm.DealTitle = deal.Title;
                vm.DealValue = deal.Value;
            }

            vm.NoDiscountResult = deal.GetDiscountedValue(new NoDiscountStrategy());
            vm.PercentageResult = deal.GetDiscountedValue(new PercentageDiscountStrategy(vm.PercentageRate));
            vm.FixedResult      = deal.GetDiscountedValue(new FixedAmountDiscountStrategy(vm.FixedAmount));
            vm.HasResult        = true;
            return View(vm);
        }

        // ─── Observer ────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Observer()
        {
            var vm = new ObserverViewModel
            {
                Deals    = await _db.Deals.OrderBy(d => d.Title).ToListAsync(),
                EventLog = await _db.DealEvents.OrderByDescending(e => e.CreatedAt).Take(20).ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Observer(ObserverViewModel vm)
        {
            vm.Deals = await _db.Deals.OrderBy(d => d.Title).ToListAsync();

            // Load real deal from DB
            Deal? dbDeal = vm.SelectedDealId != Guid.Empty
                ? await _db.Deals.FindAsync(vm.SelectedDealId)
                : null;

            if (dbDeal == null)
            {
                dbDeal = new Deal { Title = vm.DealTitle, Stage = DealStage.New };
            }

            vm.DealTitle = dbDeal.Title;

            var domainDeal = new Deal { Title = dbDeal.Title, Stage = dbDeal.Stage };
            var notifier   = new DealNotifier(domainDeal);
            var logObs     = new LogObserver();
            var emailObs   = new EmailNotificationObserver();
            notifier.Attach(logObs);
            notifier.Attach(emailObs);

            vm.Steps = new List<ObserverStep>();

            foreach (var stageName in vm.Stages ?? new List<string>())
            {
                if (Enum.TryParse<DealStage>(stageName, out var stage))
                {
                    notifier.ChangeDealStage(stage);

                    // Persist event to DB
                    var evt = new DealEvent
                    {
                        DealId    = dbDeal.Id,
                        DealTitle = dbDeal.Title,
                        NewStage  = stageName,
                        EventLog  = logObs.LastLog
                    };
                    _db.DealEvents.Add(evt);

                    vm.Steps.Add(new ObserverStep { Stage = stageName, Log = logObs.LastLog });
                }
            }

            // Update deal stage in DB
            if (vm.Stages?.Any() == true && Enum.TryParse<DealStage>(vm.Stages.Last(), out var finalStage))
            {
                dbDeal.Stage = finalStage;
                _db.Deals.Update(dbDeal);
            }

            await _db.SaveChangesAsync();
            vm.FinalStage = domainDeal.Stage.ToString();
            vm.HasResult  = true;
            vm.EventLog   = await _db.DealEvents.OrderByDescending(e => e.CreatedAt).Take(20).ToListAsync();
            return View(vm);
        }

        // ─── Command ─────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Command()
        {
            var vm = new CommandViewModel
            {
                Deals        = await _db.Deals.OrderBy(d => d.Title).ToListAsync(),
                PersistedLog = await _db.CommandHistory.OrderByDescending(c => c.CreatedAt).Take(20).ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Command(CommandViewModel vm)
        {
            vm.Deals = await _db.Deals.OrderBy(d => d.Title).ToListAsync();

            Deal? dbDeal = vm.SelectedDealId != Guid.Empty
                ? await _db.Deals.FindAsync(vm.SelectedDealId)
                : null;

            if (dbDeal == null)
                dbDeal = new Deal { Title = vm.DealTitle, Stage = DealStage.New };

            vm.DealTitle = dbDeal.Title;

            var domainDeal = new Deal { Title = dbDeal.Title, Stage = dbDeal.Stage };
            var invoker    = new CommandInvoker();

            vm.AuditTrail = new List<CommandStep>
            {
                new CommandStep { Action = "CREATE", Stage = domainDeal.Stage.ToString(), Actor = "System", Type = "create" }
            };

            var stagesToExecute = vm.ExecuteStages ?? new List<string>();
            foreach (var stageName in stagesToExecute)
            {
                if (Enum.TryParse<DealStage>(stageName, out var stage))
                {
                    var prevStage = domainDeal.Stage.ToString();
                    var cmd = new ChangeDealStageCommand(domainDeal, stage);
                    invoker.ExecuteCommand(cmd);
                    var entry = new CommandHistoryEntry
                    {
                        DealId = dbDeal.Id, DealTitle = dbDeal.Title,
                        Action = "EXECUTE", PreviousStage = prevStage,
                        NewStage = domainDeal.Stage.ToString(), Actor = "Agent Vânzări"
                    };
                    _db.CommandHistory.Add(entry);
                    vm.AuditTrail.Add(new CommandStep { Action = "EXECUTE", Stage = domainDeal.Stage.ToString(), Actor = "Agent Vânzări", Type = "execute" });
                }
            }

            for (int i = 0; i < vm.UndoCount; i++)
            {
                var prevStage = domainDeal.Stage.ToString();
                invoker.UndoLastCommand();
                var entry = new CommandHistoryEntry
                {
                    DealId = dbDeal.Id, DealTitle = dbDeal.Title,
                    Action = "UNDO", PreviousStage = prevStage,
                    NewStage = domainDeal.Stage.ToString(), Actor = "Supervisor"
                };
                _db.CommandHistory.Add(entry);
                vm.AuditTrail.Add(new CommandStep { Action = "UNDO", Stage = domainDeal.Stage.ToString(), Actor = "Supervisor", Type = "undo" });
            }

            // Update real deal in DB
            dbDeal.Stage = domainDeal.Stage;
            _db.Deals.Update(dbDeal);
            await _db.SaveChangesAsync();

            vm.FinalStage    = domainDeal.Stage.ToString();
            vm.HasResult     = true;
            vm.PersistedLog  = await _db.CommandHistory.OrderByDescending(c => c.CreatedAt).Take(20).ToListAsync();
            return View(vm);
        }

        // ─── Memento ─────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Memento()
        {
            var leads     = await _db.Leads.OrderBy(l => l.LastName).ToListAsync();
            var snapshots = await _db.LeadSnapshots.OrderByDescending(s => s.CreatedAt).Take(10).ToListAsync();
            var vm = new MementoViewModel { Leads = leads, Snapshots = snapshots };
            if (leads.Any())
            {
                var first = leads.First();
                vm.LeadId = first.Id; vm.FirstName = first.FirstName; vm.LastName = first.LastName;
                vm.Email  = first.Email; vm.Phone = first.Phone; vm.Status = first.Status;
            }
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Memento(MementoViewModel vm)
        {
            vm.Leads     = await _db.Leads.OrderBy(l => l.LastName).ToListAsync();
            vm.Snapshots = await _db.LeadSnapshots.OrderByDescending(s => s.CreatedAt).Take(10).ToListAsync();

            Lead? lead = vm.LeadId != Guid.Empty ? await _db.Leads.FindAsync(vm.LeadId) : null;
            if (lead == null) return View(vm);

            // Save snapshot BEFORE edit
            var snapshot = new LeadSnapshot
            {
                LeadId    = lead.Id,
                FirstName = lead.FirstName, LastName = lead.LastName,
                Email     = lead.Email,     Phone    = lead.Phone,
                Status    = lead.Status,
                Label     = $"Snapshot {DateTime.Now:HH:mm:ss}"
            };
            _db.LeadSnapshots.Add(snapshot);

            vm.SnapshotState = $"{lead.FirstName} {lead.LastName} | {lead.Email} | {lead.Phone} | {lead.Status}";

            // Apply modifications
            if (!string.IsNullOrWhiteSpace(vm.NewFirstName)) lead.FirstName = vm.NewFirstName;
            if (!string.IsNullOrWhiteSpace(vm.NewLastName))  lead.LastName  = vm.NewLastName;
            if (!string.IsNullOrWhiteSpace(vm.NewEmail))     lead.Email     = vm.NewEmail;
            if (!string.IsNullOrWhiteSpace(vm.NewPhone))     lead.Phone     = vm.NewPhone;
            if (!string.IsNullOrWhiteSpace(vm.NewStatus))    lead.Status    = vm.NewStatus;

            vm.ModifiedState = $"{lead.FirstName} {lead.LastName} | {lead.Email} | {lead.Phone} | {lead.Status}";

            _db.Leads.Update(lead);
            await _db.SaveChangesAsync();

            vm.HasResult = true;
            vm.Snapshots = await _db.LeadSnapshots.OrderByDescending(s => s.CreatedAt).Take(10).ToListAsync();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> RestoreSnapshot(Guid snapshotId)
        {
            var snap = await _db.LeadSnapshots.FindAsync(snapshotId);
            if (snap == null) return RedirectToAction("Memento");

            var lead = await _db.Leads.FindAsync(snap.LeadId);
            if (lead != null)
            {
                lead.FirstName = snap.FirstName; lead.LastName = snap.LastName;
                lead.Email     = snap.Email;     lead.Phone    = snap.Phone;
                lead.Status    = snap.Status;
                _db.Leads.Update(lead);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Memento");
        }

        // ─── Iterator ────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Iterator()
        {
            var deals = await _db.Deals.OrderBy(d => d.Title).ToListAsync();
            var vm = new IteratorViewModel
            {
                Deals = deals.Select(d => new IteratorDealInput { Title = d.Title, Value = d.Value }).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Iterator(IteratorViewModel vm)
        {
            // Load from DB, apply optional filter
            IQueryable<Deal> query = _db.Deals;

            if (!string.IsNullOrWhiteSpace(vm.FilterTitle))
                query = query.Where(d => d.Title.Contains(vm.FilterTitle));

            if (vm.MinValue > 0)
                query = query.Where(d => d.Value >= vm.MinValue);

            var dbDeals = await query.OrderBy(d => d.Title).ToListAsync();

            // Use the Iterator pattern on results
            var collection = new DealCollection();
            foreach (var d in dbDeals)
                collection.AddDeal(new Deal { Title = d.Title, Value = d.Value });

            var iterator = collection.CreateIterator();
            vm.IteratedDeals = new List<IteratedDealResult>();
            int idx = 0;
            while (iterator.HasNext())
            {
                var deal = iterator.Next();
                vm.IteratedDeals.Add(new IteratedDealResult
                {
                    Index   = idx++,
                    Title   = deal.Title,
                    Value   = deal.Value,
                    HasNext = iterator.HasNext()
                });
            }

            vm.Deals      = dbDeals.Select(d => new IteratorDealInput { Title = d.Title, Value = d.Value }).ToList();
            vm.TotalValue = dbDeals.Sum(d => d.Value);
            vm.TotalCount = dbDeals.Count;
            vm.HasResult  = true;
            return View(vm);
        }
    }

    // ─── ViewModels ────────────────────────────────────────────────────────────
    public class StrategyViewModel
    {
        public List<Deal> Deals          { get; set; } = new();
        public Guid    SelectedDealId   { get; set; }
        public string  DealTitle        { get; set; } = string.Empty;
        public decimal DealValue        { get; set; } = 5000m;
        public decimal PercentageRate   { get; set; } = 10m;
        public decimal FixedAmount      { get; set; } = 500m;
        public decimal NoDiscountResult { get; set; }
        public decimal PercentageResult { get; set; }
        public decimal FixedResult      { get; set; }
        public bool    HasResult        { get; set; }
    }

    public class ObserverViewModel
    {
        public List<Deal>     Deals          { get; set; } = new();
        public Guid           SelectedDealId { get; set; }
        public string         DealTitle      { get; set; } = string.Empty;
        public List<string>?  Stages         { get; set; }
        public string         FinalStage     { get; set; } = string.Empty;
        public bool           HasResult      { get; set; }
        public List<ObserverStep>  Steps     { get; set; } = new();
        public List<DealEvent>     EventLog  { get; set; } = new();
    }

    public class ObserverStep { public string Stage { get; set; } = string.Empty; public string Log { get; set; } = string.Empty; }

    public class CommandViewModel
    {
        public List<Deal>     Deals          { get; set; } = new();
        public Guid           SelectedDealId { get; set; }
        public string         DealTitle      { get; set; } = string.Empty;
        public List<string>?  ExecuteStages  { get; set; }
        public int            UndoCount      { get; set; }
        public string         FinalStage     { get; set; } = string.Empty;
        public bool           HasResult      { get; set; }
        public List<CommandStep>         AuditTrail   { get; set; } = new();
        public List<CommandHistoryEntry> PersistedLog { get; set; } = new();
    }

    public class CommandStep { public string Action{get;set;}=string.Empty; public string Stage{get;set;}=string.Empty; public string Actor{get;set;}=string.Empty; public string Type{get;set;}=string.Empty; }

    public class MementoViewModel
    {
        public List<Lead>         Leads     { get; set; } = new();
        public List<LeadSnapshot> Snapshots { get; set; } = new();
        public Guid   LeadId    { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName  { get; set; } = string.Empty;
        public string Email     { get; set; } = string.Empty;
        public string Phone     { get; set; } = string.Empty;
        public string Status    { get; set; } = "New";
        public string NewFirstName { get; set; } = string.Empty;
        public string NewLastName  { get; set; } = string.Empty;
        public string NewEmail     { get; set; } = string.Empty;
        public string NewPhone     { get; set; } = string.Empty;
        public string NewStatus    { get; set; } = string.Empty;
        public string SnapshotState { get; set; } = string.Empty;
        public string ModifiedState { get; set; } = string.Empty;
        public bool   HasResult     { get; set; }
    }

    public class IteratorViewModel
    {
        public List<IteratorDealInput>  Deals        { get; set; } = new();
        public List<IteratedDealResult> IteratedDeals { get; set; } = new();
        public string  FilterTitle { get; set; } = string.Empty;
        public decimal MinValue    { get; set; }
        public decimal TotalValue  { get; set; }
        public int     TotalCount  { get; set; }
        public bool    HasResult   { get; set; }
    }

    public class IteratorDealInput  { public string Title { get; set; } = string.Empty; public decimal Value { get; set; } }
    public class IteratedDealResult { public int Index { get; set; } public string Title { get; set; } = string.Empty; public decimal Value { get; set; } public bool HasNext { get; set; } }
}
