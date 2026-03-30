using System;

namespace TMPP_CRM.Domain.Facade
{
    /// <summary>
    /// Subsistem 3: Proceseaza plata rezervarii.
    /// </summary>
    public class PaymentService
    {
        public PaymentConfirmation ProcessPayment(string guestName, decimal totalAmount)
        {
            Console.WriteLine($"[Payment] Proceseaza plata {totalAmount:F2} RON pentru {guestName}...");

            if (totalAmount <= 0)
                return new PaymentConfirmation { IsSuccess = false, Message = "Suma invalida." };

            var confirmation = new PaymentConfirmation
            {
                IsSuccess = true,
                ReceiptNumber = $"REC-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}",
                AmountPaid = totalAmount,
                Message = $"Plata de {totalAmount:F2} RON confirmata."
            };

            Console.WriteLine($"[Payment] Confirmare: {confirmation.ReceiptNumber}");
            return confirmation;
        }
    }

    public class PaymentConfirmation
    {
        public bool IsSuccess { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
