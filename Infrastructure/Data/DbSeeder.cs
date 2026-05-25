using System;
using System.Linq;
using System.Threading.Tasks;
using TMPP_CRM.Domain.Entities;
using TMPP_CRM.Domain.Enums;
using TMPP_CRM.Infrastructure.Data;

namespace TMPP_CRM.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(CrmDbContext db)
        {
            // Only seed if empty
            if (db.Clients.Any()) return;

            // ─── Clients ────────────────────────────────────────────────────────
            var c1 = new Client { CompanyName = "TechCorp SRL",     ContactName = "Andrei Ionescu",  Email = "andrei@techcorp.ro",    Phone = "+40721100001" };
            var c2 = new Client { CompanyName = "DigitalWave SA",   ContactName = "Maria Popescu",   Email = "maria@digitalwave.ro",  Phone = "+40721100002" };
            var c3 = new Client { CompanyName = "CloudSoft ONG",    ContactName = "Radu Constantin", Email = "radu@cloudsoft.ro",     Phone = "+40721100003" };
            var c4 = new Client { CompanyName = "InnovateMD SRL",   ContactName = "Elena Ciobanu",   Email = "elena@innovatemd.ro",   Phone = "+40721100004" };
            db.Clients.AddRange(c1, c2, c3, c4);

            // ─── Deals ──────────────────────────────────────────────────────────
            var d1 = new Deal { Title = "Implementare CRM TechCorp",   Value = 15000m, Stage = DealStage.Qualified,    ClientId = c1.Id };
            var d2 = new Deal { Title = "Licențe SaaS DigitalWave",    Value = 8500m,  Stage = DealStage.ProposalSent, ClientId = c2.Id };
            var d3 = new Deal { Title = "Suport Premium CloudSoft",    Value = 4200m,  Stage = DealStage.Contacted,    ClientId = c3.Id };
            var d4 = new Deal { Title = "Migrare Cloud InnovateMD",    Value = 22000m, Stage = DealStage.Negotiation,  ClientId = c4.Id };
            var d5 = new Deal { Title = "Training Echipă TechCorp Q2", Value = 3600m,  Stage = DealStage.New,          ClientId = c1.Id };
            db.Deals.AddRange(d1, d2, d3, d4, d5);

            // ─── Leads ──────────────────────────────────────────────────────────
            var l1 = new Lead { FirstName = "Ion",    LastName = "Popescu",    Email = "ion.popescu@gmail.com",    Phone = "+40731200001", Status = "New" };
            var l2 = new Lead { FirstName = "Ana",    LastName = "Dumitrescu", Email = "ana.dum@yahoo.com",        Phone = "+40731200002", Status = "Contacted" };
            var l3 = new Lead { FirstName = "Mihai",  LastName = "Georgescu",  Email = "mihai.geo@outlook.com",    Phone = "+40731200003", Status = "Qualified" };
            db.Leads.AddRange(l1, l2, l3);

            await db.SaveChangesAsync();
        }
    }
}
