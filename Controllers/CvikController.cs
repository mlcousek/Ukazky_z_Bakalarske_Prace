using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessWebApp.Data;
using FitnessWebApp.Models;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using static FitnessWebApp.Controllers.TPController;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
//POKUD CHCEME TESTOVAT U SEBE TAK NUTNO UPRAVIT METODY: Create 2433, UlozitPridaneData 2686, UlozitUpraveneData 2755
namespace FitnessWebApp.Controllers
{
    [Authorize]
    public class CvikController : Controller
    {
        private readonly ApplicationDbContext _context;


        public CvikController(ApplicationDbContext context)
        {
            _context = context;
        }


        // Akce pro přidání dat do databáze
        public async Task<IActionResult> PridejData()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {

                ///////////!!!!!!!!
                ///
                ///
                    // TO DO: NEJAK NORMALNE PRIDAT DATA!!!
                ///
                ///
                ///////////////!!!!!!!!


                //string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "cviky.json");

                // var json = await System.IO.File.ReadAllTextAsync(filePath);

                // Deserializovat JSON data na seznam cvičení
                //var cviky1 = JsonConvert.DeserializeObject<List<Cvik>>(json);
                //await _context.Cvik.AddRangeAsync(cviky1);

                //1zakomentovani

                foreach (var cvik in cviky)
                {
                    //Console.WriteLine("{");
                    //Console.WriteLine($"\"Nazev\": \"{cvik.Nazev}\",");
                    //Console.WriteLine($"\"PopisCviku\": \"{cvik.PopisCviku}\",");
                    //Console.WriteLine($"\"Partie\": \"{cvik.Partie}\",");
                    //Console.WriteLine("\"UzivatelId\": \"userId\",");
                    //Console.WriteLine($"\"TypyTreninku\": [ \"{string.Join("\" , \"", cvik.TypyTreninku)}\" ],");
                    //Console.WriteLine($"\"PoctySerii\": [ {string.Join(" , ", cvik.PočtySérií)} ],");
                    //Console.WriteLine($"\"PoctyOpakovani\": [ \"{string.Join("\" , \"", cvik.PočtyOpakování)}\" ],");
                    //Console.WriteLine($"\"PauzyMeziSeriemi\": [ {string.Join(", ", cvik.PauzyMeziSériemi)} ]");
                    //Console.WriteLine("},");

                    _context.Cvik.Add(cvik);
                }

                var uzivatel = await _context.Users
                                .Where(dt => dt.Id == userId)
                                .ToListAsync();
                uzivatel[0].PridaneData = true;
                _context.SaveChanges();

            }
            return RedirectToAction("Index"); 
        }

        // GET: Cvik
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.uzivatelId = userId;
            return View(await _context.Cvik.ToListAsync());
        }

        // GET: VolbaVah
        public IActionResult VolbaVah()
        {
            return View();
        }

        // GET: Playlisty
        public IActionResult Playlisty()
        {
            return View();
        }

        // GET: Cvik/Create
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vsechnyCviky = _context.Cvik
                      .Where(id => id.UzivatelId == userId)
                      .ToList();

            ViewBag.vsechnyCviky = vsechnyCviky;

            return View();
        }
        // GET: Cvik/Create1
        public IActionResult Create1(string datum)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vsechnyCviky = _context.Cvik
                      .Where(id => id.UzivatelId == userId)
                      .ToList();

            ViewBag.vsechnyCviky = vsechnyCviky;
            ViewBag.datum = datum;
            return View();
        }

        // POST: Cvik/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CvikId,Nazev,PočtyOpakování,PočtySérií,PauzyMeziSériemi,Partie,PopisCviku")] Cvik cvik, string? datum = null)
        {
            if (ModelState.ContainsKey("UzivatelId"))
            {
                ModelState.Remove("UzivatelId");
            }
            if (ModelState.ContainsKey("Uzivatel"))
            {
                ModelState.Remove("Uzivatel");
            }
            if (ModelState.ContainsKey("PočtySérií"))
            {
                ModelState.Remove("PočtySérií");
            }
            if (ModelState.ContainsKey("PauzyMeziSériemi"))
            {
                ModelState.Remove("PauzyMeziSériemi");
            }
            if (ModelState.IsValid)
            {
                
                if(datum == null)
                {
                    var uzivatelId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    cvik.UzivatelId = uzivatelId;
                    cvik.TypyTreninku = null;
                    cvik.PauzyMeziSériemi = null;
                    cvik.PočtyOpakování = null;
                    cvik.PočtySérií = null;
                    cvik.cvikVytvorenUzivatelem = true;
                    _context.Add(cvik);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Cvik");
                }
                else
                {
                    cvik.cvikVytvorenUzivatelem = true;
                    cvik.TypyTreninku = new List<string>();
                    var uzivatelId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    cvik.UzivatelId = uzivatelId;
                    datum = datum.Substring(0, 10); /////////////////////TADYYYYY

                    // UPRAVIT PODLE TOHO KDE TO FICI

                    string[] casti = datum.Split('/');  //NASAZENI NA SERVERU     //JAKOBY 0 na 1
                    //NASAZENI NA SERVERU musi byt tyto ify ana local to zakomentovat
                    if (casti[0].Length == 1)
                    {
                        casti[0] = "0" + casti[0];
                    }
                    if (casti[1].Length == 1)
                    {
                        casti[1] = "0" + casti[1];
                    }
                    string prevedeneDatum = $"{casti[1]}-{casti[0]}-{casti[2]}"; //NASAZENI NA SERVERU


                    //string[] casti = datum.Split('.');  //U SEBE TEST
                    //string prevedeneDatum = $"{casti[0]}-{casti[1]}-{casti[2]}"; //U SEBE TEST

                    

                    DateTime parsedDate = DateTime.ParseExact(prevedeneDatum, "dd-MM-yyyy", CultureInfo.InvariantCulture);



                    var uzivatelTPZaznam = await _context.TP.SingleOrDefaultAsync(tp => tp.UzivatelID == uzivatelId);
                    var denTreninku = await _context.DenTreninku.SingleOrDefaultAsync(tp => tp.TPId == uzivatelTPZaznam.Id && tp.DatumTreninku == parsedDate);

                    var zkratkaTreninku = TypTreninkuHelper.GetTypTreninkuZkratka(uzivatelTPZaznam, denTreninku.TypTreninku);
                    cvik.TypyTreninku.Add(zkratkaTreninku);

                    _context.Add(cvik);
                    await _context.SaveChangesAsync();
                    denTreninku.Cviky.Add(cvik);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction("Index", "TP");
                    // string prevedeneDatum2 = $"{casti[2]}-{casti[1]}-{casti[0]}"; //U SEBE TEST
                    string prevedeneDatum2 = $"{casti[2]}-{casti[0]}-{casti[1]}";  //NASAZENI NA SERVERU
                    return RedirectToAction("Trenink", "Trenink", new { date = prevedeneDatum2 });
                }
            }
            return View(cvik);
        }

        // GET: Cvik/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vsechnyCviky = _context.Cvik
                      .Where(id => id.UzivatelId == userId)
                      .ToList();

            ViewBag.vsechnyCviky = vsechnyCviky;

            var cvik = await _context.Cvik.FindAsync(id);
            ViewBag.cvikNaEdit = cvik;
            if (cvik == null)
            {
                return NotFound();
            }
            return View(cvik);
        }

        // POST: Cvik/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CvikId,Nazev,PopisCviku,Partie")] Cvik cvik)
        {
            if (id != cvik.CvikId)
            {
                return NotFound();
            }

            var uzivatelId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cvikNaUpravu = _context.Cvik.Where(t => t.CvikId == id).Where(t => t.UzivatelId == uzivatelId).ToList();
            var treninkoveDnyKdeJeCvik = _context.DenTreninku.ToList().Where(den => den.Cviky.Any(c => c.CvikId == cvik.CvikId)).ToList();

            for( var i = 0; i < treninkoveDnyKdeJeCvik.Count(); i++)
            {
                var indexCviku = treninkoveDnyKdeJeCvik[i].Cviky.FindIndex(c => c.CvikId == id);
                var cvikCoMenim = treninkoveDnyKdeJeCvik[i].Cviky[indexCviku];
                cvikCoMenim.Nazev = cvik.Nazev;
                cvikCoMenim.PopisCviku = cvik.PopisCviku;
                cvikCoMenim.Partie = cvik.Partie;
                await _context.SaveChangesAsync();
            }

            if (cvikNaUpravu[0] != null)
            {

                cvikNaUpravu[0].Nazev = cvik.Nazev;
                cvikNaUpravu[0].Partie = cvik.Partie;
                cvikNaUpravu[0].PopisCviku = cvik.PopisCviku;
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Cvik");
            }
            
            return View(cvik);
        }

        // GET: Cvik/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cvik = await _context.Cvik
                .FirstOrDefaultAsync(m => m.CvikId == id);
            if (cvik == null)
            {
                return NotFound();
            }

            return View(cvik);
        }

        // POST: Cvik/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uzivatelId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var uzivatel = await _context.Users.FirstOrDefaultAsync(t => t.Id == uzivatelId);
            var zaznamyKSmazani = await _context.TreninkoveData
                        .Where(t => t.CvikId == id && t.UzivatelId == uzivatelId)
                        .ToListAsync();
            var dnyVTreninkuObsahuji = await _context.DenTreninku 
                                        .Where(t => t.TPId == uzivatel.TPId).ToListAsync();

            var vyfiltrovaneZaznamy = dnyVTreninkuObsahuji.Where(zaznam =>
                    zaznam.Cviky.Any(cvik => cvik.CvikId == id)
                ).ToList();

            foreach (var zaznam in vyfiltrovaneZaznamy)
            {
                zaznam.Cviky.RemoveAll(c => c.CvikId == id);
            }

            if (zaznamyKSmazani.Any())
            {
                _context.TreninkoveData.RemoveRange(zaznamyKSmazani);
                await _context.SaveChangesAsync();
            }

            var cvik = await _context.Cvik.FindAsync(id);
            if (cvik != null)
            {
                _context.Cvik.Remove(cvik);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CvikExists(int id)
        {
            return _context.Cvik.Any(e => e.CvikId == id);
        }


        // GET: Cvik/PridatDoPlanu
        public async Task<IActionResult> PridatDoPlanu(DateTime date)
        {
            var cvikyKtereZustanou = new List<Cvik>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.uzivatelId = userId;
            ViewBag.datum = date;

            var uzivatel = await _context.Users
                    .FirstOrDefaultAsync(dt => dt.Id == userId);
            if (uzivatel != null)
            {
                var dnyTreninku = await _context.DenTreninku
                                       .Where(dt => dt.TPId == uzivatel.TPId)
                                       .ToListAsync();

                var treninkKamPridavam = dnyTreninku.FirstOrDefault(dt => dt.DatumTreninku.ToString("yyyy-MM-dd") == date.ToString("yyyy-MM-dd"));
                var vsechnyCviky = await _context.Cvik
                                       .Where(dt => dt.UzivatelId == uzivatel.Id)
                                       .ToListAsync();

                cvikyKtereZustanou = vsechnyCviky.Where(cv => !treninkKamPridavam.Cviky.Any(treninkovyCvik => cv.CvikId == treninkovyCvik.CvikId)).ToList();
            }

            return View(cvikyKtereZustanou);
        }




        // GET: Cvik/NastavitData
        public IActionResult NastavitData(int id, string date)
        {
            var cvik = _context.Cvik
                       .Where(dt => dt.CvikId == id)
                       .ToList();


            //ViewBag.datum = date.Substring(0,10); //Podle toho kde to fici U SEBE / NA SERVERU
            ViewBag.datum = date.Substring(0,10); //Podle toho kde to fici U SEBE / NA SERVERU
            return View(cvik);
        }

        // GET: Cvik/UpravitCvik
        public IActionResult UpravitCvik(int id, string date)
        {
            var cvik = _context.Cvik
                       .Where(dt => dt.CvikId == id)
                       .ToList();


            ViewBag.datum = date.Substring(0, 10); //stejne jako nad timto
            return View(cvik);
        }

        // POST: Cvik/UlozitPridaneData
        [HttpPost]
        public IActionResult UlozitPridaneData(int cvikId, int pocetSerií, string opakování, int pauza, string datum)
        {
            var cvik = _context.Cvik.FirstOrDefault(cvik => cvik.CvikId == cvikId);
            if (cvik != null)
            {
                var idUzivatele = cvik.UzivatelId;
                var uzivatel = _context.Users.FirstOrDefault(x => x.Id == idUzivatele);
                var treninkovyPlan = _context.TP.Where(dt => dt.UzivatelID == idUzivatele).Where(dt => dt.Id == uzivatel.TPId).ToList();


                var idTP = treninkovyPlan[0].Id;

                // UPRAVIT PODLE TOHO KDE TO FICI


                string[] casti = datum.Split('/');  //NASAZENI  NA SERVERU


                //NASAZENI NA SERVERU musi byt tyto ify ana local to zakomentovat
                if (casti[0].Length == 1)
                {
                    casti[0] = "0" + casti[0];
                }
                if (casti[1].Length == 1)
                {
                    casti[1] = "0" + casti[1];
                }

                string prevedeneDatum = $"{casti[1]}-{casti[0]}-{casti[2]}"; //NASAZENI NA SERVERU

                //string[] casti = datum.Split('.');  //U SEBE TEST
                //string prevedeneDatum = $"{casti[0]}-{casti[1]}-{casti[2]}"; //U SEBE TEST


                throw new Exception($"Debug: Datum = {prevedeneDatum}");
                DateTime parsedDate = DateTime.ParseExact(prevedeneDatum, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                var DenTrenikuSTPaDnem = _context.DenTreninku.FirstOrDefault(dt => dt.DatumTreninku == parsedDate && dt.TPId == idTP);
                var TypTreninkuKratke = TypTreninkuHelper.GetTypTreninkuZkratka(treninkovyPlan[0], DenTrenikuSTPaDnem.TypTreninku);

                if (cvik.PočtySérií == null)
                {
                    cvik.PočtySérií = new List<int>();
                    cvik.PočtyOpakování = new List<string>();
                    cvik.PauzyMeziSériemi = new List<int>();
                    cvik.TypyTreninku = new List<string>();
                }

                if (!cvik.TypyTreninku.Any(t => t == TypTreninkuKratke))
                {
                    // Pokud neexistuje, přidejte ho
                    cvik.TypyTreninku.Add(TypTreninkuKratke);
                    cvik.PočtySérií.Add(pocetSerií);
                    cvik.PočtyOpakování.Add(opakování);
                    cvik.PauzyMeziSériemi.Add(pauza);
                }


                DenTrenikuSTPaDnem.Cviky.Add(cvik);

                _context.SaveChanges();

                int index = cvik.TypyTreninku.IndexOf(TypTreninkuKratke);

                cvik.PočtySérií[index] = pocetSerií;
                cvik.PočtyOpakování[index] = opakování;
                cvik.PauzyMeziSériemi[index] = pauza;

                int index1 = DenTrenikuSTPaDnem.Cviky.FindIndex(c => c.CvikId == cvik.CvikId);
                DenTrenikuSTPaDnem.Cviky[index1].PočtySérií[index] = pocetSerií;
                DenTrenikuSTPaDnem.Cviky[index1].PočtyOpakování[index] = opakování;
                DenTrenikuSTPaDnem.Cviky[index1].PauzyMeziSériemi[index] = pauza;
                _context.SaveChanges();


                // string prevedeneDatum2 = $"{casti[2]}-{casti[1]}-{casti[0]}"; //U SEBE TEST
                string prevedeneDatum2 = $"{casti[2]}-{casti[0]}-{casti[1]}";  //NASAZENI NA SERVERU

                //return RedirectToAction("Index", "TP");
                return RedirectToAction("Trenink", "Trenink", new { date = prevedeneDatum2 });
            }
            else
            {
                return NotFound(); 
            }
        }

        // POST: Cvik/UlozitUpraveneData
        [HttpPost]
        public IActionResult UlozitUpraveneData(int cvikId, int pocetSerií, string opakování, int pauza, string datum)
        {
            var cvik = _context.Cvik.FirstOrDefault(cvik => cvik.CvikId == cvikId);
            if (cvik != null)
            {
                var idUzivatele = cvik.UzivatelId;
                var uzivatel = _context.Users.FirstOrDefault(t => t.Id == idUzivatele);
                var treninkovyPlan = _context.TP.Where(dt => dt.Id == uzivatel.TPId).ToList();
                var idTP = uzivatel.TPId;

                // UPRAVIT PODLE TOHO KDE TO FICI MUSIM TESTNOUT ALE ASI TO TADY JE NA OBOU STEJNE

                string[] casti = datum.Split('-');  //NASAZENI NA SERVERU
                //NASAZENI NA SERVERU musi byt tyto ify ana local to zakomentovat
                if (casti[0].Length == 1)
                {
                    casti[0] = "0" + casti[0];
                }
                if (casti[1].Length == 1)
                {
                    casti[1] = "0" + casti[1];
                }
                string prevedeneDatum = $"{casti[0]}-{casti[1]}-{casti[2]}"; //NASAZENI NA SERVERU


                //string[] casti = datum.Split('-');  //U SEBE TEST
                //string prevedeneDatum = $"{casti[0]}-{casti[1]}-{casti[2]}"; //U SEBE TEST

                DateTime parsedDate = DateTime.ParseExact(prevedeneDatum, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var DenTrenikuSTPaDnem = _context.DenTreninku.FirstOrDefault(dt => dt.DatumTreninku == parsedDate && dt.TPId == idTP);
                var TypTreninkuKratke = TypTreninkuHelper.GetTypTreninkuZkratka(treninkovyPlan[0], DenTrenikuSTPaDnem.TypTreninku);

                int index = cvik.TypyTreninku.IndexOf(TypTreninkuKratke);

                cvik.PočtySérií[index] = pocetSerií;
                cvik.PočtyOpakování[index] = opakování;
                cvik.PauzyMeziSériemi[index] = pauza;

                int index1 = DenTrenikuSTPaDnem.Cviky.FindIndex(c => c.CvikId == cvik.CvikId);
                DenTrenikuSTPaDnem.Cviky[index1].PočtySérií[index] = pocetSerií;
                DenTrenikuSTPaDnem.Cviky[index1].PočtyOpakování[index] = opakování;
                DenTrenikuSTPaDnem.Cviky[index1].PauzyMeziSériemi[index] = pauza;
                _context.SaveChanges();

                string prevedeneDatum2 = $"{casti[0]}-{casti[1]}-{casti[2]}";

                //return RedirectToAction("Index", "TP");
                return RedirectToAction("Trenink", "Trenink", new { date = prevedeneDatum2 });

                //return RedirectToAction("Index", "TP");
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPost]
        public async Task<IActionResult> SmazatDataCvikuZDatabazeIDenTreninku(DataZFrontendu data)
        {
            if (data != null)
            {
                var cvikId = data.CvikId;
                var uzivatelId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                DateTime datum;
                if (DateTime.TryParse(data.Datum, out datum))
                {
                    var cvikCoJePotrebaUpravit = await _context.Cvik
                        .Where(t => t.CvikId == cvikId && t.UzivatelId == uzivatelId)
                        .ToListAsync();
                    var uzivatel = await _context.Users.Where(u => u.Id == uzivatelId).ToListAsync();
                    DateTime novyDatum = new DateTime(datum.Year, datum.Month, datum.Day, 0, 0, 0);
                    var denTreninkuSCvikem = await _context.DenTreninku
                        .Where(t => t.DatumTreninku == novyDatum)
                        .Where(t => t.TPId == uzivatel[0].TPId)
                        .ToListAsync();
                    var TP = await _context.TP
                        .Where(t => t.UzivatelID == uzivatel[0].Id)
                        .ToListAsync();
                    denTreninkuSCvikem[0].Cviky.RemoveAll(c => c.CvikId == cvikCoJePotrebaUpravit[0].CvikId);

                    await _context.SaveChangesAsync();
                    var denTreninkuSCvikem1 = await _context.DenTreninku
                       .Where(t => t.DatumTreninku == novyDatum)
                       .Where(t => t.TPId == uzivatel[0].TPId)
                       .ToListAsync();

                    var vsechnyDnyTreninku = await _context.DenTreninku
                       .Where(t => t.TPId == uzivatel[0].TPId)
                       .ToListAsync();

                    bool existujeCvik = denTreninkuSCvikem1[0].Cviky.Any();
                    if(existujeCvik == false)
                    {
                        _context.DenTreninku.Remove(denTreninkuSCvikem1[0]);

                        // Uložte změny do databáze
                        _context.SaveChanges();

                        return Json(new { wasDeleted = true });

                    }

                    //}

                    return Json(new { wasDeleted = false });
                }
                else
                {
                    return Json(new { wasDeleted = false });
                }
            }

            return Json(new { wasDeleted = false });
        }

        [HttpPost]
        public async Task<IActionResult> ZmenitPoradi(DataZFrontendu data)
        {

            if (data != null)
            {
                var cvikId = data.CvikId;
                var uzivatelId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                DateTime datum;
                if (DateTime.TryParse(data.Datum, out datum))
                {
                    var uzivatel = await _context.Users.Where(u => u.Id == uzivatelId).ToListAsync();
                    DateTime novyDatum = new DateTime(datum.Year, datum.Month, datum.Day, 0, 0, 0);
                    var denTreninkuSCvikem = await _context.DenTreninku
                        .Where(t => t.DatumTreninku == novyDatum)
                        .Where(t => t.TPId == uzivatel[0].TPId)
                        .ToListAsync();
                    
                    var staryIndex = denTreninkuSCvikem[0].Cviky.FindIndex(cvik => cvik.CvikId == cvikId);
                    var novyIndex = data.CisloSerie - 1;

                    if(staryIndex > novyIndex)
                    {
                        var cvik = denTreninkuSCvikem[0].Cviky[staryIndex];
                        for (int i = staryIndex - 1; i >= novyIndex; i--)
                        {
                            denTreninkuSCvikem[0].Cviky[i + 1] = denTreninkuSCvikem[0].Cviky[i];
                        }
                        denTreninkuSCvikem[0].Cviky[novyIndex] = cvik;
                    } 
                    else if(staryIndex < novyIndex)
                    {
                        var cvik = denTreninkuSCvikem[0].Cviky[staryIndex];
                        for (int i = staryIndex + 1; i <= novyIndex; i++)
                        {
                            denTreninkuSCvikem[0].Cviky[i - 1] = denTreninkuSCvikem[0].Cviky[i];
                        }
                        denTreninkuSCvikem[0].Cviky[novyIndex] = cvik;
                    }

                        await _context.SaveChangesAsync();
                    

                    return RedirectToAction("Index", "TP");
                }
                else
                {
                    return RedirectToAction("Index", "TP");
                }
            }
            return View(data);
        }
    }
}

