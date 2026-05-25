using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPP_CRM.Domain.Bridge;
using TMPP_CRM.Domain.Decorator;
using TMPP_CRM.Domain.Entities;
using TMPP_CRM.Domain.Flyweight;
using TMPP_CRM.Domain.Proxy;
using TMPP_CRM.Infrastructure.Data;

namespace TMPP_CRM.Controllers
{
    public class Lab5Controller : Controller
    {
        private readonly CrmDbContext _db;
        public Lab5Controller(CrmDbContext db) => _db = db;

        public IActionResult Index() => View();

        // ─── Flyweight ─────────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Flyweight()
        {
            return View(new FlyweightViewModel
            {
                TextBlocks = new List<FlyweightTextBlock>
                {
                    new FlyweightTextBlock { Text = "Hello CRM",  FontSize = 12, Color = "Black" },
                    new FlyweightTextBlock { Text = "Promo 2026", FontSize = 14, Color = "Red"   }
                }
            });
        }

        [HttpPost]
        public IActionResult Flyweight(FlyweightViewModel vm)
        {
            if (vm.TextBlocks == null) vm.TextBlocks = new List<FlyweightTextBlock>();
            vm.TextBlocks = vm.TextBlocks.Where(t => !string.IsNullOrWhiteSpace(t.Text)).ToList();

            var factory = new CharacterFactory();
            var editor  = new TextEditor(factory);
            foreach (var block in vm.TextBlocks)
                editor.InsertText(block.Text, block.FontSize, block.Color);

            vm.TotalDocumentCharacters = editor.GetTotalCharactersInDocument();
            vm.TotalFactoryInstances   = factory.GetTotalCharactersCreated();
            vm.TotalUniqueChars        = vm.TotalFactoryInstances;
            vm.MemorySavedPercent      = vm.TotalDocumentCharacters > 0
                ? (int)(100.0 - (vm.TotalFactoryInstances * 100.0 / vm.TotalDocumentCharacters)) : 0;
            vm.UniqueCharList = string.Concat(vm.TextBlocks.Select(b => b.Text))
                                      .Distinct().Select(c => c.ToString()).ToList();
            vm.HasResult = true;
            return View(vm);
        }

        // ─── Decorator ────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Decorator()
        {
            var vm = new DecoratorInputViewModel
            {
                RecentLogs = await _db.Notifications.OrderByDescending(n => n.CreatedAt).Take(10).ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Decorator(DecoratorInputViewModel vm)
        {
            INotification notification = new EmailNotification();
            if (vm.UseSms)  notification = new SmsDecorator(notification);
            if (vm.UsePush) notification = new PushDecorator(notification);

            var message = string.IsNullOrWhiteSpace(vm.Message) ? "Campanie de reduceri activa!" : vm.Message;
            vm.Result = notification.Send(message);

            // Save to DB
            var log = new NotificationLog
            {
                Message    = message,
                EmailSent  = true,
                SmsSent    = vm.UseSms,
                PushSent   = vm.UsePush,
                LayerCount = 1 + (vm.UseSms ? 1 : 0) + (vm.UsePush ? 1 : 0),
                Result     = vm.Result
            };
            _db.Notifications.Add(log);
            await _db.SaveChangesAsync();

            vm.HasResult  = true;
            vm.RecentLogs = await _db.Notifications.OrderByDescending(n => n.CreatedAt).Take(10).ToListAsync();
            return View(vm);
        }

        // ─── Bridge ───────────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Bridge() => View(new BridgeInputViewModel());

        [HttpPost]
        public IActionResult Bridge(BridgeInputViewModel vm)
        {
            IMediaDevice device   = vm.DeviceType == "Tv" ? new TvDevice() : new PhoneDevice();
            var fileName = string.IsNullOrWhiteSpace(vm.FileName)
                ? (vm.MediaType == "Video" ? "prezentare.mp4" : "interviu.mp3") : vm.FileName;

            MediaFile file = vm.MediaType == "Video"
                ? new VideoFile(device, fileName)
                : new AudioFile(device, fileName);

            vm.Result    = file.Play();
            vm.HasResult = true;
            return View(vm);
        }

        // ─── Proxy ───────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Proxy()
        {
            var vm = new ProxyInputViewModel
            {
                RecentLogs = await _db.AccessLogs.OrderByDescending(a => a.CreatedAt).Take(15).ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Proxy(ProxyInputViewModel vm)
        {
            var username = string.IsNullOrWhiteSpace(vm.Username) ? "testUser" : vm.Username;
            var session  = new UserSession(username, vm.Role ?? "Guest");
            ISensitiveResource proxy = new ResourceProxy(session);

            int customerId = vm.CustomerId > 0 ? vm.CustomerId : 42;
            decimal amount = vm.Amount > 0 ? vm.Amount : 2500m;
            int recordId   = vm.RecordId > 0 ? vm.RecordId : 1001;

            // READ attempt
            try { vm.ReadResult = proxy.GetCustomerData(customerId); }
            catch (Exception ex) { vm.ReadResult = "Eroare: " + ex.Message; }

            // WRITE attempt
            try { vm.WriteResult = proxy.UpdateFinancialRecords(recordId, amount); }
            catch (Exception ex) { vm.WriteResult = "Eroare: " + ex.Message; }

            // Save both access attempts to DB
            _db.AccessLogs.AddRange(
                new AccessLog
                {
                    Username   = username, Role = vm.Role ?? "Guest",
                    Operation  = "READ", CustomerId = customerId,
                    WasGranted = !vm.ReadResult.StartsWith("Eroare"),
                    Result     = vm.ReadResult, Amount = 0
                },
                new AccessLog
                {
                    Username   = username, Role = vm.Role ?? "Guest",
                    Operation  = "WRITE", CustomerId = recordId,
                    WasGranted = !vm.WriteResult.StartsWith("Eroare"),
                    Result     = vm.WriteResult, Amount = amount
                }
            );
            await _db.SaveChangesAsync();

            vm.HasResult  = true;
            vm.RecentLogs = await _db.AccessLogs.OrderByDescending(a => a.CreatedAt).Take(15).ToListAsync();
            return View(vm);
        }
    }

    // ─── ViewModels ──────────────────────────────────────────────────────────────
    public class FlyweightViewModel
    {
        public List<FlyweightTextBlock> TextBlocks              { get; set; } = new();
        public int                      TotalDocumentCharacters { get; set; }
        public int                      TotalFactoryInstances   { get; set; }
        public int                      TotalUniqueChars        { get; set; }
        public int                      MemorySavedPercent      { get; set; }
        public List<string>             UniqueCharList          { get; set; } = new();
        public bool                     HasResult               { get; set; }
    }

    public class FlyweightTextBlock
    {
        public string Text     { get; set; } = string.Empty;
        public int    FontSize { get; set; } = 12;
        public string Color    { get; set; } = "Black";
    }

    public class DecoratorInputViewModel
    {
        public string  Message    { get; set; } = string.Empty;
        public bool    UseEmail   { get; set; } = true;
        public bool    UseSms     { get; set; }
        public bool    UsePush    { get; set; }
        public bool    HasResult  { get; set; }
        public string  Result     { get; set; } = string.Empty;
        public List<NotificationLog> RecentLogs { get; set; } = new();
    }

    public class BridgeInputViewModel
    {
        public string MediaType  { get; set; } = "Audio";
        public string DeviceType { get; set; } = "Phone";
        public string FileName   { get; set; } = string.Empty;
        public bool   HasResult  { get; set; }
        public string Result     { get; set; } = string.Empty;
    }

    public class ProxyInputViewModel
    {
        public string  Username   { get; set; } = string.Empty;
        public string  Role       { get; set; } = "Guest";
        public int     CustomerId { get; set; } = 42;
        public decimal Amount     { get; set; } = 2500m;
        public int     RecordId   { get; set; } = 1001;
        public bool    HasResult  { get; set; }
        public string  ReadResult  { get; set; } = string.Empty;
        public string  WriteResult { get; set; } = string.Empty;
        public List<AccessLog> RecentLogs { get; set; } = new();
    }

    // Legacy stubs kept so existing views that reference these compile
    public class DecoratorViewModel { public bool UseSms{get;set;} public bool UsePush{get;set;} public string Result{get;set;}=string.Empty; }
    public class BridgeViewModel    { public string MediaType{get;set;}=string.Empty; public string DeviceType{get;set;}=string.Empty; public string Result{get;set;}=string.Empty; }
    public class ProxyViewModel     { public string Role{get;set;}=string.Empty; public string ReadResult{get;set;}=string.Empty; public string WriteResult{get;set;}=string.Empty; }
}
