using FitnessWebApp.Data;
using FitnessWebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Encodings.Web;
using System.Text;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using static FitnessWebApp.Controllers.TPController;


namespace FitnessWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Uzivatel> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<Uzivatel> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var uzivatel = _context.Users
                               .Where(dt => dt.Id == userId)
                               .ToList();
            var testovaciData = PridaneTestovaciDataGlobalni.PridaneTestovaciDataDoAplikace;



            // var tpInfo = _context.TP.ToList();
            ViewBag.Uzivatel = uzivatel;
            ViewBag.TestovaciData = testovaciData;

            return View();
        }

        public async Task<IActionResult> ZmenitVek(DataZFrontendu data)
        {
            if (data != null)
            {
                DateTime datum;
                if (DateTime.TryParse(data.Datum, out datum))
                {
                    DateTime novyDatum = new DateTime(datum.Year, datum.Month, datum.Day, 0, 0, 0);
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var uzivatel = _context.Users
                                   .Where(dt => dt.Id == userId)
                                   .ToList();

                    foreach (var user in uzivatel)
                    {
                        user.Vek = user.Vek + 1;
                        user.PomocneDatum = novyDatum;
                    }

                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Home");
                }

            }

            return View(data);
        }


        public async Task<IActionResult> ZmenitJakCastoSeAktualizujeVaha(DataZFrontendu data)
        {
            if (data != null)
            {

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var uzivatel = _context.Users
                               .Where(dt => dt.Id == userId)
                               .ToList();

                foreach (var user in uzivatel)
                {
                    user.JakCastoAktualizovatVahu = data.CvikId;

                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");

            }

            return View(data);
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

        [Route("/StatusCodeError/{statusCode}")]
        public IActionResult Errors(int statusCode)
        {
            if (statusCode == 404)
            {
                ViewBag.ErrorMessage = "404 Strátka nebyla nalezena";
            }

            return View();
        }

        //kod dolu slouzi pouze pro vytvoreni testovacich dat !!! kopiruju kod z TP Controller, protoze nechci menit tam ty tridy na static
        public async Task<IActionResult> PridatTestovaciData1()
        {
            var user = new Uzivatel
            {
                Jmeno = "Test",
                Prijmeni = "Test",
                Vaha = 75.5,
                Vyska = 170,
                Vek = 25,
                Uroven = 2,
                Pohlavi = 1,
                PridaneData = false,
                PomocneDatum = DateTime.Today,
                UserName = "test@test.cz",
                Email = "test@test.cz",
                JakCastoAktualizovatVahu = 1,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, "Test123");

            await _context.SaveChangesAsync();
            if (result.Succeeded)
            {
                PridaneTestovaciDataGlobalni.PridaneTestovaciDataDoAplikace = 1;
                return RedirectToAction("Index");
            }
            else
            {
                return BadRequest("Chyba pøi vytváøení testovacího uživatele.");
            }
        }
    }  
}
