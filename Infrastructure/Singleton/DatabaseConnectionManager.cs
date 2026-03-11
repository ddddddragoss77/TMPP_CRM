using System;
using System.Threading;

namespace TMPP_CRM.Infrastructure.Singleton
{
    /// <summary>
    /// Singleton THREAD-SAFE pentru gestionarea conexiunii la baza de date CRM.
    /// 
    /// Garanteaza ca exista O SINGURA instanta accesibila global in intreaga aplicatie,
    /// reutilizata de toate modulele (Clienti, Deals, Leads, Rapoarte).
    /// 
    /// Thread-safety asigurata prin Lazy<T> cu LazyThreadSafetyMode.ExecutionAndPublication.
    /// </summary>
    public sealed class DatabaseConnectionManager
    {
        // ─── Singleton: instanta unica Lazy (thread-safe by default) ─────────────
        private static readonly Lazy<DatabaseConnectionManager> _instance =
            new Lazy<DatabaseConnectionManager>(
                () => new DatabaseConnectionManager(),
                LazyThreadSafetyMode.ExecutionAndPublication
            );

        // Contor pentru a demonstra ca se creeaza o singura instanta
        private static int _instanceCount = 0;

        // ─── Proprietati conexiune ───────────────────────────────────────────────
        public string ConnectionString { get; private set; }
        public bool IsConnected { get; private set; }
        public DateTime ConnectedAt { get; private set; }
        private int _activeTransactions = 0;

        // ─── Constructor privat: previne instantierea din exterior ───────────────
        private DatabaseConnectionManager()
        {
            int count = Interlocked.Increment(ref _instanceCount);
            if (count > 1)
                throw new InvalidOperationException("Singleton violat: s-a incercat crearea a doua instante!");

            // Configuratie implicita (in productie ar veni din appsettings.json)
            ConnectionString = "Server=localhost;Database=TMPP_CRM;Trusted_Connection=True;";
            IsConnected = false;
            ConnectedAt = DateTime.MinValue;
        }

        // ─── Acces la instanta unica ─────────────────────────────────────────────
        /// <summary>
        /// Returneaza singura instanta a DatabaseConnectionManager.
        /// Thread-safe: mai multe thread-uri pot apela simultan fara a crea instante duplicate.
        /// </summary>
        public static DatabaseConnectionManager Instance => _instance.Value;

        // ─── Metode de gestiune a conexiunii ────────────────────────────────────

        /// <summary>
        /// Deschide conexiunea la baza de date.
        /// </summary>
        public void Connect(string? connectionString = null)
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
                ConnectionString = connectionString;

            if (IsConnected)
            {
                Console.WriteLine("[DB Manager] Conexiunea este deja activa.");
                return;
            }

            // Simulare conexiune (in productie: SqlConnection.Open())
            IsConnected = true;
            ConnectedAt = DateTime.UtcNow;
            Console.WriteLine($"[DB Manager] Conexiune deschisa la: {ConnectedAt:HH:mm:ss}");
        }

        /// <summary>
        /// Inchide conexiunea la baza de date.
        /// </summary>
        public void Disconnect()
        {
            if (!IsConnected)
            {
                Console.WriteLine("[DB Manager] Nu exista o conexiune activa.");
                return;
            }

            IsConnected = false;
            Console.WriteLine($"[DB Manager] Conexiune inchisa. Durata sesiunii: " +
                              $"{(DateTime.UtcNow - ConnectedAt).TotalSeconds:F1}s");
        }

        /// <summary>
        /// Returneaza un rezumat al starii curente a conexiunii.
        /// </summary>
        public string GetStatus()
        {
            return $"[DB Manager] Status: {(IsConnected ? "CONECTAT" : "DECONECTAT")} | " +
                   $"Server: {ConnectionString.Split(';')[0]} | " +
                   $"Tranzactii active: {_activeTransactions}";
        }

        /// <summary>
        /// Incepe o tranzactie pe conexiunea curenta.
        /// </summary>
        public void BeginTransaction()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Nu exista conexiune activa pentru tranzactie.");

            Interlocked.Increment(ref _activeTransactions);
            Console.WriteLine($"[DB Manager] Tranzactie initiata. Total active: {_activeTransactions}");
        }

        /// <summary>
        /// Finalizeaza o tranzactie.
        /// </summary>
        public void CommitTransaction()
        {
            if (_activeTransactions == 0)
                throw new InvalidOperationException("Nu exista tranzactii active de confirmat.");

            Interlocked.Decrement(ref _activeTransactions);
            Console.WriteLine($"[DB Manager] Tranzactie confirmata. Ramase active: {_activeTransactions}");
        }
    }
}
