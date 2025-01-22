using FitnessWebApp.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using FitnessWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using static FitnessWebApp.Controllers.TPController;


namespace FitnessWebApp.Controllers
{
    [Authorize]
    public class TreninkController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TreninkController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Trenink()
        {
            var requestUrl = _httpContextAccessor.HttpContext.Request.GetDisplayUrl();

            var last10Characters = requestUrl.Substring(Math.Max(0, requestUrl.Length - 10));

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var uzivatel = _context.Users
                           .Where(id => id.Id == userId)
                           .ToList();

            DateTime inputDate = DateTime.ParseExact(last10Characters, "yyyy-MM-dd", null);

            var denTreninku = _context.DenTreninku
                              .Where(x => x.TPId == uzivatel[0].TPId)
                              .Where(d => d.DatumTreninku == inputDate)
                              .ToList();

            var cviky = denTreninku[0].Cviky;

            var datacviku = _context.TreninkoveData
                        .Where(id => id.UzivatelId == userId)
                        .ToList();

            var TPUzivatele = _context.TP
                              .Where(x => x.Id == uzivatel[0].TPId)
                               .ToList();

            var typTreninkuZkratka = TypTreninkuHelper.GetTypTreninkuZkratka(TPUzivatele[0], denTreninku[0].TypTreninku);

            ViewBag.typTreninkuZkratka = typTreninkuZkratka;
            ViewBag.typTreninku = denTreninku[0].TypTreninku;
            ViewBag.cviky = cviky;
            ViewBag.datacviku = datacviku;
            ViewBag.uzivatel = uzivatel;

            return View();
        }
    }
}


