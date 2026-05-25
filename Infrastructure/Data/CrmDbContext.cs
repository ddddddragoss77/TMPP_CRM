using Microsoft.EntityFrameworkCore;
using TMPP_CRM.Domain.Entities;

namespace TMPP_CRM.Infrastructure.Data
{
    public class CrmDbContext : DbContext
    {
        public CrmDbContext(DbContextOptions<CrmDbContext> options) : base(options) { }

        // Core CRM entities
        public DbSet<Client>  Clients  { get; set; }
        public DbSet<Deal>    Deals    { get; set; }
        public DbSet<Lead>    Leads    { get; set; }

        // Pattern-specific log tables
        public DbSet<PersistedOffer>      Offers          { get; set; }
        public DbSet<Transaction>         Transactions    { get; set; }
        public DbSet<NotificationLog>     Notifications   { get; set; }
        public DbSet<AccessLog>           AccessLogs      { get; set; }
        public DbSet<CommandHistoryEntry> CommandHistory  { get; set; }
        public DbSet<LeadSnapshot>        LeadSnapshots   { get; set; }
        public DbSet<DealEvent>           DealEvents      { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ignore navigation property from Deal to Client (not needed for SQLite)
            modelBuilder.Entity<Deal>().Ignore(d => d.Client);
            modelBuilder.Entity<Client>().Ignore(c => c.Deals);

            base.OnModelCreating(modelBuilder);
        }
    }
}
