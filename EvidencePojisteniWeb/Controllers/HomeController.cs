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
                new() { Name = "HTML5/CSS3", Img = "html_css.png", Description = "HTML5 a CSS3 spolupracuj� na tvorb� modern�ch webov�ch str�nek. HTML5 poskytuje strukturu a obsah, zat�mco CSS3 se star� o vizu�ln� prezentaci a rozlo�en� obsahu. Spole�n� umo��uj� vytv��et responzivn�, poutav� a u�ivatelsky p��v�tiv� webov� str�nky." },
                new() { Name = "C#", Img = "csharp.png", Description = "C# je modern�, objektov� orientovan� programovac� jazyk, vyvinut� spole�nost� Microsoft. Je sou��st� platformy .NET a je navr�en jako jazyk jednoduch�, modern� a typov� bezpe�n�. C# kombinuje principy a funkce z jin�ch jazyk�, jako jsou C++, Java a Pascal." },
                new() { Name = "JavaScript", Img = "javascript.png", Description = "JavaScript (JS) je skriptovac� jazyk pou��van� p�edev��m pro tvorbu interaktivn�ch webov�ch str�nek a aplikac�. Umo��uje dynamick� chov�n� str�nek, jako jsou animace, reakce na akce u�ivatele, aktualizace obsahu bez nutnosti obnoven� str�nky a dal��." },
                new() { Name = "ASP.NET Core MVC", Img = "asp.net-core-mvc.webp", Description = "ASP.NET Core MVC je opensourcov�, multiplatformn� a testovateln� prezenta�n� framework. Stru�n� �e�eno, ASP.NET Core MVC je zp�sob, jak efektivn� stav�t webov� str�nky a webov� API v prost�ed� ASP.NET Core, kter� klade d�raz na modularitu, testovatelnost a modern� webov� standardy." },
                new() { Name = "Bootstrap", Img = "bootstrap.png", Description = "Bootstrap je front-endov� framework, kter� usnad�uje v�voj responzivn�ch webov�ch str�nek a aplikac�. Obsahuje p�edem p�ipraven� CSS a JavaScriptov� komponenty a syst�m m��ky, d�ky �emu� se web automaticky p�izp�sob� r�zn�m velikostem obrazovky." },
                new() { Name = "MS-SQL Datab�ze", Img = "t-sql_databaze.webp", Description = "MS SQL datab�ze, nebo p�esn�ji Microsoft SQL Server, je rela�n� datab�zov� syst�m vyvinut� spole�nost� Microsoft. Slou�� k ukl�d�n�, spr�v� a vyhled�v�n� dat v tabulkov�m form�tu a je �iroce pou��v�n pro r�zn� aplikace, v�etn� datov�ch sklad�, analytick�ch syst�m� a webov�ch aplikac�." }
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
