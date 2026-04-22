using System;

namespace TMPP_CRM.Domain.Facade
{
    /// <summary>
    /// Subsistem 4: Trimite notificari de confirmare (email/SMS).
    /// </summary>
    public class NotificationService
    {
        public void SendConfirmation(string guestName, string roomNumber, 
                                     DateTime checkIn, DateTime checkOut, string receiptNumber)
        {
            Console.WriteLine($"[Notification] Trimite confirmare rezervare...");
            Console.WriteLine($"  📧 Email catre {guestName}:");
            Console.WriteLine($"     Camera: {roomNumber} | {checkIn:dd.MM.yyyy} – {checkOut:dd.MM.yyyy}");
            Console.WriteLine($"     Nr. chitanta: {receiptNumber}");
        }

        public void SendReminder(string guestName, DateTime checkIn)
        {
            Console.WriteLine($"[Notification] Reminder trimis catre {guestName} – check-in: {checkIn:dd.MM.yyyy}");
        }
    }
}
