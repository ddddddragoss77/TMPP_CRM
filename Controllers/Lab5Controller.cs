using Microsoft.AspNetCore.Mvc;
using TMPP_CRM.Domain.Flyweight;
using TMPP_CRM.Domain.Decorator;
using TMPP_CRM.Domain.Bridge;
using TMPP_CRM.Domain.Proxy;
using System;

namespace TMPP_CRM.Controllers
{
    public class Lab5Controller : Controller
    {
        public IActionResult Index() => View();

        // ─── Flyweight ─────────────────────────────────────────────────────────────
        public IActionResult Flyweight()
        {
            var factory = new CharacterFactory();
            var editor = new TextEditor(factory);

            editor.InsertText("Hello CRM", 12, "Black");
            editor.InsertText("Promo 2026", 14, "Red");

            var vm = new FlyweightViewModel
            {
                TotalDocumentCharacters = editor.GetTotalCharactersInDocument(),
                TotalFactoryInstances = factory.GetTotalCharactersCreated()
            };

            return View(vm);
        }

        // ─── Decorator ────────────────────────────────────────────────────────────
        public IActionResult Decorator(bool useSms = false, bool usePush = false)
        {
            INotification notification = new EmailNotification();
            
            if (useSms)
                notification = new SmsDecorator(notification);
            
            if (usePush)
                notification = new PushDecorator(notification);

            var result = notification.Send("Campanie de reduceri activa!");

            var vm = new DecoratorViewModel
            {
                UseSms = useSms,
                UsePush = usePush,
                Result = result
            };

            return View(vm);
        }

        // ─── Bridge ───────────────────────────────────────────────────────────────
        public IActionResult Bridge(string mediaType = "Audio", string deviceType = "Phone")
        {
            IMediaDevice device = deviceType == "Tv" ? new TvDevice() : new PhoneDevice();
            
            MediaFile file;
            if (mediaType == "Video")
                file = new VideoFile(device, "prezentare.mp4");
            else
                file = new AudioFile(device, "interviu.mp3");

            var vm = new BridgeViewModel
            {
                MediaType = mediaType,
                DeviceType = deviceType,
                Result = file.Play()
            };

            return View(vm);
        }

        // ─── Proxy ───────────────────────────────────────────────────────────────
        public IActionResult Proxy(string role = "Guest")
        {
            var session = new UserSession("testUser", role);
            ISensitiveResource proxy = new ResourceProxy(session);

            string readResult;
            string writeResult;

            try { readResult = proxy.GetCustomerData(42); } 
            catch (Exception ex) { readResult = "Eroare: " + ex.Message; }

            try { writeResult = proxy.UpdateFinancialRecords(1001, 2500m); }
            catch (Exception ex) { writeResult = "Eroare: " + ex.Message; }

            var vm = new ProxyViewModel
            {
                Role = role,
                ReadResult = readResult,
                WriteResult = writeResult
            };

            return View(vm);
        }
    }

    public class FlyweightViewModel
    {
        public int TotalDocumentCharacters { get; set; }
        public int TotalFactoryInstances { get; set; }
    }

    public class DecoratorViewModel
    {
        public bool UseSms { get; set; }
        public bool UsePush { get; set; }
        public string Result { get; set; } = string.Empty;
    }

    public class BridgeViewModel
    {
        public string MediaType { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
    }

    public class ProxyViewModel
    {
        public string Role { get; set; } = string.Empty;
        public string ReadResult { get; set; } = string.Empty;
        public string WriteResult { get; set; } = string.Empty;
    }
}
