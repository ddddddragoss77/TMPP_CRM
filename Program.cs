var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Clean Architecture - Dependency Injection
builder.Services.AddScoped(typeof(TMPP_CRM.Domain.Interfaces.IRepository<>), typeof(TMPP_CRM.Infrastructure.Persistence.InMemoryRepository<>));
builder.Services.AddScoped<TMPP_CRM.Application.Interfaces.ILeadService, TMPP_CRM.Application.Services.LeadService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
