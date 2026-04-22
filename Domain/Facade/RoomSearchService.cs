using System;
using System.Collections.Generic;

namespace TMPP_CRM.Domain.Facade
{
    /// <summary>
    /// Subsistem 1: Cauta camere disponibile dupa criterii.
    /// </summary>
    public class RoomSearchService
    {
        private static readonly List<RoomInfo> _rooms = new()
        {
            new RoomInfo { RoomNumber = "101", Type = "Standard", PricePerNight = 250m, MaxGuests = 2 },
            new RoomInfo { RoomNumber = "201", Type = "Deluxe",   PricePerNight = 400m, MaxGuests = 2 },
            new RoomInfo { RoomNumber = "301", Type = "Suite",    PricePerNight = 800m, MaxGuests = 4 },
            new RoomInfo { RoomNumber = "102", Type = "Standard", PricePerNight = 250m, MaxGuests = 2 },
        };

        public RoomInfo? FindRoom(string roomType, int guests = 1)
        {
            Console.WriteLine($"[RoomSearch] Cauta camera '{roomType}' pentru {guests} persoane...");
            var room = _rooms.Find(r =>
                r.Type.Equals(roomType, StringComparison.OrdinalIgnoreCase) &&
                r.MaxGuests >= guests);

            Console.WriteLine(room != null
                ? $"[RoomSearch] Camera gasita: {room.RoomNumber} ({room.Type})"
                : $"[RoomSearch] Nicio camera '{roomType}' disponibila.");
            return room;
        }
    }

    public class RoomInfo
    {
        public string RoomNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int MaxGuests { get; set; }
    }
}
