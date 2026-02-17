using System;

namespace TMPP_CRM.Domain.Entities
{
    public class CallActivity : Activity
    {
        public string PhoneNumber { get; set; }

        public override string GetActivityType()
        {
            return "Call";
        }
    }

    public class EmailActivity : Activity
    {
        public string EmailAddress { get; set; }
        public string Body { get; set; }

        public override string GetActivityType()
        {
            return "Email";
        }
    }

    public class MeetingActivity : Activity
    {
        public string Location { get; set; }
        public string Attendees { get; set; }

        public override string GetActivityType()
        {
            return "Meeting";
        }
    }
}
