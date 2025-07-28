using System.Diagnostics;
using EvidencePojisteniWeb.Models;
using EvidencePojisteniWeb.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace EvidencePojisteniWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            var technologie = new List<TechnologieViewModel>
            {
                new() { Name = "HTML5/CSS3", Img = "html_css.png", Description = "HTML5 a CSS3 spolupracují na tvorbì moderních webových stránek. HTML5 poskytuje strukturu a obsah, zatímco CSS3 se stará o vizuální prezentaci a rozložení obsahu. Spoleènì umožòují vytváøet responzivní, poutavé a uživatelsky pøívìtivé webové stránky." },
                new() { Name = "C#", Img = "csharp.png", Description = "C# je moderní, objektovì orientovaný programovací jazyk, vyvinutý spoleèností Microsoft. Je souèástí platformy .NET a je navržen jako jazyk jednoduchý, moderní a typovì bezpeèný. C# kombinuje principy a funkce z jiných jazykù, jako jsou C++, Java a Pascal." },
                new() { Name = "JavaScript", Img = "javascript.png", Description = "JavaScript (JS) je skriptovací jazyk používaný pøedevším pro tvorbu interaktivních webových stránek a aplikací. Umožòuje dynamické chování stránek, jako jsou animace, reakce na akce uživatele, aktualizace obsahu bez nutnosti obnovení stránky a další." },
                new() { Name = "ASP.NET Core MVC", Img = "asp.net-core-mvc.webp", Description = "ASP.NET Core MVC je opensourcový, multiplatformní a testovatelný prezentaèní framework. Struènì øeèeno, ASP.NET Core MVC je zpùsob, jak efektivnì stavìt webové stránky a webová API v prostøedí ASP.NET Core, který klade dùraz na modularitu, testovatelnost a moderní webové standardy." },
                new() { Name = "Bootstrap", Img = "bootstrap.png", Description = "Bootstrap je front-endový framework, který usnadòuje vývoj responzivních webových stránek a aplikací. Obsahuje pøedem pøipravené CSS a JavaScriptové komponenty a systém møížky, díky èemuž se web automaticky pøizpùsobí rùzným velikostem obrazovky." },
                new() { Name = "MS-SQL Databáze", Img = "t-sql_databaze.webp", Description = "MS SQL databáze, nebo pøesnìji Microsoft SQL Server, je relaèní databázový systém vyvinutý spoleèností Microsoft. Slouží k ukládání, správì a vyhledávání dat v tabulkovém formátu a je široce používán pro rùzné aplikace, vèetnì datových skladù, analytických systémù a webových aplikací." }
            };
            return View(technologie);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
