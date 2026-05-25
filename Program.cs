using Microsoft.EntityFrameworkCore;
using TMPP_CRM.Application.Interfaces;
using TMPP_CRM.Application.Services;
using TMPP_CRM.Domain.Interfaces;
using TMPP_CRM.Infrastructure.Data;
using TMPP_CRM.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// ─── Services ────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

// SQLite + EF Core
builder.Services.AddDbContext<CrmDbContext>(opt =>
    opt.UseSqlite("Data Source=crm.db"));

// Generic repository (EF-backed)
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

// Application services
builder.Services.AddScoped<ILeadService, LeadService>();

var app = builder.Build();

// ─── Ensure DB created + Seed ────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
    db.Database.EnsureCreated();
    await DbSeeder.SeedAsync(db);
}

// ─── Pipeline ────────────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
