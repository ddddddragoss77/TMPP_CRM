using System;
using TMPP_CRM.Domain.Entities;

namespace TMPP_CRM.Domain.Observer
{
    public class EmailNotificationObserver : IDealObserver
    {
        public void Update(Deal deal)
        {
            // Simulate sending email
            Console.WriteLine($"[Email] Deal '{deal.Title}' stage changed to {deal.Stage}. Notification sent to manager.");
        }
    }
}
