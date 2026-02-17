using System;
using TMPP_CRM.Domain.Entities;

namespace TMPP_CRM.Domain.Factories
{
    public abstract class ActivityFactory
    {
        public abstract Activity CreateActivity();
    }

    public class CallActivityFactory : ActivityFactory
    {
        private readonly string _phoneNumber;
        private readonly string _subject;

        public CallActivityFactory(string phoneNumber, string subject)
        {
            _phoneNumber = phoneNumber;
            _subject = subject;
        }

        public override Activity CreateActivity()
        {
            return new CallActivity
            {
                Subject = _subject,
                PhoneNumber = _phoneNumber,
                Description = "Created via Factory Method",
                DueDate = DateTime.Now.AddDays(1)
            };
        }
    }

    public class EmailActivityFactory : ActivityFactory
    {
        private readonly string _emailAddress;
        private readonly string _subject;
        private readonly string _body;

        public EmailActivityFactory(string emailAddress, string subject, string body)
        {
            _emailAddress = emailAddress;
            _subject = subject;
            _body = body;
        }

        public override Activity CreateActivity()
        {
            return new EmailActivity
            {
                Subject = _subject,
                EmailAddress = _emailAddress,
                Body = _body,
                Description = "Created via Factory Method",
                DueDate = DateTime.Now.AddDays(2)
            };
        }
    }

    public class MeetingActivityFactory : ActivityFactory
    {
        private readonly string _location;
        private readonly string _attendees;
        private readonly string _subject;

        public MeetingActivityFactory(string location, string attendees, string subject)
        {
            _location = location;
            _attendees = attendees;
            _subject = subject;
        }

        public override Activity CreateActivity()
        {
            return new MeetingActivity
            {
                Subject = _subject,
                Location = _location,
                Attendees = _attendees,
                Description = "Created via Factory Method",
                DueDate = DateTime.Now.AddDays(3)
            };
        }
    }
}
