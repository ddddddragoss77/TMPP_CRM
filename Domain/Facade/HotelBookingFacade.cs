using System;

namespace TMPP_CRM.Domain.Facade
{
    /// <summary>
    /// FACADE: Expune o interfata simplificata pentru rezervarea unei camere de hotel.
    /// Orchestreaza intern 4 subsisteme complexe: RoomSearch, Availability, Payment, Notification.
    /// Clientul apeleaza o singura metoda BookRoom() fara a cunoaste subsistemele.
    /// </summary>
    public class HotelBookingFacade
    {
        private readonly RoomSearchService _roomSearch;
        private readonly AvailabilityService _availability;
        private readonly PaymentService _payment;
        private readonly NotificationService _notification;

        public HotelBookingFacade(
            RoomSearchService? roomSearch = null,
            AvailabilityService? availability = null,
            PaymentService? payment = null,
            NotificationService? notification = null)
        {
            _roomSearch     = roomSearch     ?? new RoomSearchService();
            _availability   = availability   ?? new AvailabilityService();
            _payment        = payment        ?? new PaymentService();
            _notification   = notification   ?? new NotificationService();
        }

        /// <summary>
        /// Rezerva o camera – singura metoda pe care clientul trebuie sa o cunoasca.
        /// Intern orchestreaza: cautare → disponibilitate → plata → notificare.
        /// </summary>
        public BookingResult BookRoom(string guestName, DateTime checkIn, DateTime checkOut, 
                                      string roomType = "Standard", int guests = 1)
        {
            Console.WriteLine($"\n=== Hotel Booking Facade: BookRoom() ===");

            // Validari de baza
            if (string.IsNullOrWhiteSpace(guestName))
                return Fail("Numele oaspetelui este obligatoriu.");
            if (checkOut <= checkIn)
                return Fail("Data de check-out trebuie sa fie dupa data de check-in.");
            if (checkIn < DateTime.Today)
                return Fail("Data de check-in nu poate fi in trecut.");

            // Pasul 1: Cauta camera
            var room = _roomSearch.FindRoom(roomType, guests);
            if (room == null)
                return Fail($"Nu exista camere de tip '{roomType}' disponibile pentru {guests} persoane.");

            // Pasul 2: Verifica disponibilitatea si blocheaza
            if (!_availability.CheckAndReserve(room.RoomNumber, checkIn, checkOut))
                return Fail($"Camera {room.RoomNumber} nu este disponibila in intervalul ales.");

            // Pasul 3: Calculeaza suma si proceseaza plata
            int nights = (checkOut - checkIn).Days;
            decimal total = room.PricePerNight * nights;
            var payment = _payment.ProcessPayment(guestName, total);
            if (!payment.IsSuccess)
                return Fail($"Plata esuata: {payment.Message}");

            // Pasul 4: Trimite notificare de confirmare
            _notification.SendConfirmation(guestName, room.RoomNumber, checkIn, checkOut, payment.ReceiptNumber);

            Console.WriteLine($"=== Rezervare finalizata cu succes! ===\n");

            return new BookingResult
            {
                IsSuccess = true,
                GuestName = guestName,
                RoomNumber = room.RoomNumber,
                RoomType = room.Type,
                CheckIn = checkIn,
                CheckOut = checkOut,
                Nights = nights,
                TotalAmount = total,
                ReceiptNumber = payment.ReceiptNumber,
                Message = $"Rezervare confirmata! Camera {room.RoomNumber} ({room.Type}), {nights} nopti, {total:F2} RON."
            };
        }

        private static BookingResult Fail(string reason) => new() { IsSuccess = false, Message = reason };
    }

    public class BookingResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Nights { get; set; }
        public decimal TotalAmount { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
    }
}
