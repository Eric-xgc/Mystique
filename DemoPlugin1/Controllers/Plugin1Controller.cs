﻿using DemoPlugin1.Models;
using DemoReferenceLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Mystique.Core.Attributes;
using Mystique.Core.Contracts;
using Mystique.Core.Mvc.Infrastructure;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DemoPlugin1.Controllers
{
    [Area(ModuleDefiniation.MODULE_NAME)]
    public class Plugin1Controller : Controller
    {
        private readonly INotificationRegister _notificationRegister;
        private readonly IDataStore _dataStore;

        public Plugin1Controller(INotificationRegister notificationRegister, IDataStore dataStore)
        {
            _notificationRegister = notificationRegister;
            _dataStore = dataStore;
        }

        [Page("Plugin One")]
        [HttpGet]
        public IActionResult HelloWorld()
        {
            string content = new Demo().SayHello();
            ViewBag.Content = content + "; Plugin2 triggered";

            TestClass testClass = new TestClass
            {
                Message = "Hello World"
            };

            _notificationRegister.Publish("LoadHelloWorldEvent", JsonConvert.SerializeObject(new LoadHelloWorldEvent() { Str = "Hello World" }));

            ViewBag.Books = JsonConvert.DeserializeObject<List<BookViewModel>>(_dataStore.Query("BookInventory", "Available_Books", string.Empty, source: ModuleDefiniation.MODULE_NAME));

            return View(testClass);
        }

        [HttpGet]
        public IActionResult Register()
        {
            MystiqueStartup.Services.AddScoped<IHandler, MyHandler>();
            return Content("OK");
        }

        [HttpGet]
        public IActionResult Show()
        {
            ServiceProvider provider = MystiqueStartup.Services.BuildServiceProvider();
            using (IServiceScope scope = provider.CreateScope())
            {
                IHandler handler = scope.ServiceProvider.GetService<IHandler>();
                return Content(handler.Work());
            }

        }
    }

    public interface IHandler
    {
        string Work();
    }

    public class MyHandler : IHandler
    {
        public string Work()
        {
            return "My Handler Work";
        }
    }

    public class LoadHelloWorldEvent
    {
        public string Str { get; set; }
    }
}
