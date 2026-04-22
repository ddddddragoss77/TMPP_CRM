using System;

namespace TMPP_CRM.Domain.Facade
{
    /// <summary>
    /// Subsistem 2: Verifica disponibilitatea si blocheaza camera pentru intervalul ales.
    /// </summary>
    public class AvailabilityService
    {
        public bool CheckAndReserve(string roomNumber, DateTime checkIn, DateTime checkOut)
        {
            Console.WriteLine($"[Availability] Verifica camera {roomNumber}: {checkIn:dd.MM.yyyy} – {checkOut:dd.MM.yyyy}...");

            if (checkOut <= checkIn)
            {
                Console.WriteLine("[Availability] Eroare: data checkout trebuie sa fie dupa checkin.");
                return false;
            }

            // Simulare: camera 999 e mereu indisponibila (pentru teste)
            if (roomNumber == "999")
            {
                Console.WriteLine($"[Availability] Camera {roomNumber} nu este disponibila.");
                return false;
            }

            Console.WriteLine($"[Availability] Camera {roomNumber} blocata cu succes pentru {(checkOut - checkIn).Days} nopti.");
            return true;
        }
    }
}
