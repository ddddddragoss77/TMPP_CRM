using System;
using TMPP_CRM.Domain.Entities;

namespace TMPP_CRM.Domain.Observer
{
    public class LogObserver : IDealObserver
    {
        public string LastLog { get; private set; } = string.Empty;

        public void Update(Deal deal)
        {
            // Simulate logging to a file or database
            LastLog = $"[Log] {DateTime.Now}: Deal '{deal.Id}' updated. New Stage: {deal.Stage}";
            Console.WriteLine(LastLog);
        }
    }
}
