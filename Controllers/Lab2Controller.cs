using Microsoft.AspNetCore.Mvc;
using TMPP_CRM.Domain.Factories;
using TMPP_CRM.Domain.Entities;
using TMPP_CRM.Domain.Interfaces;
using System.Collections.Generic;

namespace TMPP_CRM.Controllers
{
    public class Lab2Controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FactoryMethod()
        {
            var activities = new List<Activity>();

            // Using Factory Method to create different activities
            ActivityFactory callFactory = new CallActivityFactory("0744123456", "Initial Contact");
            activities.Add(callFactory.CreateActivity());

            ActivityFactory emailFactory = new EmailActivityFactory("client@example.com", "Offer Proposal", "Here is our offer...");
            activities.Add(emailFactory.CreateActivity());

            ActivityFactory meetingFactory = new MeetingActivityFactory("Conference Room A", "John, Alice", "Project Kickoff");
            activities.Add(meetingFactory.CreateActivity());

            return View(activities);
        }

        public IActionResult AbstractFactory(string type = "csv")
        {
            IReportFactory factory;

            if (type.ToLower() == "json")
            {
                factory = new JsonReportFactory();
            }
            else
            {
                factory = new CsvReportFactory();
            }

            var reportModel = new ReportViewModel
            {
                Header = factory.CreateHeader().Render(),
                Body = factory.CreateBody().Render(),
                Footer = factory.CreateFooter().Render(),
                Type = type.ToUpper()
            };

            return View(reportModel);
        }
    }

    public class ReportViewModel
    {
        public string Header { get; set; }
        public string Body { get; set; }
        public string Footer { get; set; }
        public string Type { get; set; }
    }
}
