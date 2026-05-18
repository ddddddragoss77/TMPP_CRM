using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TMPP_CRM.Domain.Command;
using TMPP_CRM.Domain.Entities;
using TMPP_CRM.Domain.Enums;
using TMPP_CRM.Domain.Iterator;
using TMPP_CRM.Domain.Memento;
using TMPP_CRM.Domain.Observer;
using TMPP_CRM.Domain.Strategy;

namespace TMPP_CRM.Controllers
{
    public class Lab6Controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Strategy()
        {
            var deal = new Deal { Title = "Software License Deal", Value = 1000m };
            
            var percentageStrategy = new PercentageDiscountStrategy(10);
            var fixedStrategy = new FixedAmountDiscountStrategy(150);
            var noDiscount = new NoDiscountStrategy();

            ViewBag.OriginalValue = deal.Value;
            ViewBag.PercentageDiscount = deal.GetDiscountedValue(percentageStrategy);
            ViewBag.FixedDiscount = deal.GetDiscountedValue(fixedStrategy);
            ViewBag.NoDiscount = deal.GetDiscountedValue(noDiscount);

            return View(deal);
        }

        public IActionResult Observer()
        {
            var deal = new Deal { Title = "Important Deal", Stage = DealStage.New };
            var notifier = new DealNotifier(deal);
            
            var logObserver = new LogObserver();
            var emailObserver = new EmailNotificationObserver();

            notifier.Attach(logObserver);
            notifier.Attach(emailObserver);

            notifier.ChangeDealStage(DealStage.Negotiation);
            ViewBag.Log1 = logObserver.LastLog;

            notifier.ChangeDealStage(DealStage.Won);
            ViewBag.Log2 = logObserver.LastLog;

            return View(deal);
        }

        public IActionResult Command()
        {
            var deal = new Deal { Title = "Command Deal", Stage = DealStage.New };
            var invoker = new CommandInvoker();

            var commandToNegotiation = new ChangeDealStageCommand(deal, DealStage.Negotiation);
            var commandToWon = new ChangeDealStageCommand(deal, DealStage.Won);

            ViewBag.InitialStage = deal.Stage;

            invoker.ExecuteCommand(commandToNegotiation);
            ViewBag.AfterFirstCommand = deal.Stage;

            invoker.ExecuteCommand(commandToWon);
            ViewBag.AfterSecondCommand = deal.Stage;

            invoker.UndoLastCommand();
            ViewBag.AfterFirstUndo = deal.Stage;

            invoker.UndoLastCommand();
            ViewBag.AfterSecondUndo = deal.Stage;

            return View(deal);
        }

        public IActionResult Memento()
        {
            var lead = new Lead { FirstName = "John", LastName = "Doe", Email = "john@doe.com", Phone = "123", Status = "New" };
            var history = new LeadHistory();

            ViewBag.InitialState = $"{lead.FirstName} {lead.LastName} - {lead.Status}";

            // Save State
            history.Save(lead.SaveState());

            // Modify State
            lead.FirstName = "Johnny";
            lead.Status = "Contacted";
            ViewBag.ModifiedState = $"{lead.FirstName} {lead.LastName} - {lead.Status}";

            // Restore State
            var previousState = history.Undo();
            if (previousState != null)
            {
                lead.RestoreState(previousState);
            }
            ViewBag.RestoredState = $"{lead.FirstName} {lead.LastName} - {lead.Status}";

            return View(lead);
        }

        public IActionResult Iterator()
        {
            var collection = new DealCollection();
            collection.AddDeal(new Deal { Title = "Deal A", Value = 100 });
            collection.AddDeal(new Deal { Title = "Deal B", Value = 200 });
            collection.AddDeal(new Deal { Title = "Deal C", Value = 300 });

            var iterator = collection.CreateIterator();
            var resultList = new List<string>();

            while (iterator.HasNext())
            {
                var deal = iterator.Next();
                resultList.Add($"{deal.Title} - ${deal.Value}");
            }

            ViewBag.DealList = resultList;
            return View();
        }
    }
}
