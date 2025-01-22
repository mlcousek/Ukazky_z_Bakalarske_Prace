using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessWebApp.Data;
using System.Security.Claims;
using FitnessWebApp.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace FitnessWebApp.Controllers
{

    public static class PridaneTestovaciDataGlobalni
    {
        public static int PridaneTestovaciDataDoAplikace { get; set; } = 0;
    }

    [Authorize]
    public class TPController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TPController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class GeneratorTréninkovýchDat
        {
            public static List<DateTime> VytvářeníDatumůTréninku(int tréninkyZaTýden, int týdny, DayOfWeek[] dnyTréninku)
            {
                if (dnyTréninku.Length == 0 || tréninkyZaTýden <= 0 || týdny <= 0)
                {
                    throw new ArgumentException("Invalid input parameters");
                }

                List<DateTime> dataTréninků = new List<DateTime>();

                DateTime startovacíDatum = DateTime.Now.Date; 
                DayOfWeek dnes = startovacíDatum.DayOfWeek;
                int dnyDoPondeli = (7 + (int)DayOfWeek.Monday - (int)dnes) % 7;
                startovacíDatum = startovacíDatum.AddDays(dnyDoPondeli);

                for (int týden = 0; týden < týdny; týden++)
                {
                    foreach (var denTréninku in dnyTréninku)
                    {
                        if((int)denTréninku == 0)
                        {
                            DateTime datumTréninku = startovacíDatum.AddDays((týden * 7) + (int)denTréninku + 6);
                            dataTréninků.Add(datumTréninku);
                        }
                        else
                        {
                            DateTime datumTréninku = startovacíDatum.AddDays((týden * 7) + (int)denTréninku -1);
                            dataTréninků.Add(datumTréninku);
                        }
                    }
                }
                return dataTréninků;
            }

            public static List<DateTime> VytvářeníDatumůTréninkuTestovaci(int tréninkyZaTýden, int týdny, DayOfWeek[] dnyTréninku)
            {
                if (dnyTréninku.Length == 0 || tréninkyZaTýden <= 0 || týdny <= 0)
                {
                    throw new ArgumentException("Invalid input parameters");
                }

                List<DateTime> dataTréninků = new List<DateTime>();

                DateTime startovacíDatum;
                DateTime tedka = DateTime.Now.Date;
                if(tedka.DayOfWeek == DayOfWeek.Monday)
                {
                    startovacíDatum = DateTime.Now.Date.AddDays(-21);
                }
                else
                {
                    startovacíDatum = DateTime.Now.Date.AddDays(-28);
                }

                DayOfWeek dnes = startovacíDatum.DayOfWeek;
                int dnyDoPondeli = (7 + (int)DayOfWeek.Monday - (int)dnes) % 7;
                startovacíDatum = startovacíDatum.AddDays(dnyDoPondeli);

                for (int týden = 0; týden < týdny; týden++)
                {
                    foreach (var denTréninku in dnyTréninku)
                    {
                        if ((int)denTréninku == 0)
                        {
                            DateTime datumTréninku = startovacíDatum.AddDays((týden * 7) + (int)denTréninku + 6);
                            dataTréninků.Add(datumTréninku);
                        }
                        else
                        {
                            DateTime datumTréninku = startovacíDatum.AddDays((týden * 7) + (int)denTréninku - 1);
                            dataTréninků.Add(datumTréninku);
                        }
                    }
                }
                return dataTréninků;
            }
        }

        public static class TypTreninkuHelper
        {
            public static string GetTypTreninkuZkratka(TP TP, string typTreninku)
            {
                string suffix = typTreninku switch
                {
                    "Nohy" => "Nohy",
                    "Ramena + biceps" => "RamBic",
                    "Záda" => "Zada",
                    "Hrudník + triceps" => "HrTric",
                    "Push" => "Push",
                    "Pull" => "Pull",
                    "Legs" => "Legs",
                    "Kruhový trénink 1" => "1",
                    "Kruhový trénink 2" => "2",
                    "Kruhový trénink 3" => "3",
                    _ => ""
                };

                return TP.DruhTP + TP.StylTP + suffix;
            }
        }

        private string GetTypTreninkuVM(int cislodne)
        {
            string[] treninky = {
                "Hrudník + triceps", // 0
                "Nohy",              // 1
                "Ramena + biceps",   // 2
                "Záda"               // 3
            };

            if (cislodne >= 0 && cislodne < treninky.Length)
            {
                return treninky[cislodne];
            }

            return "Chyba";
        }
        private string GetTypTreninkuPPL(int cislodne)
        {
            string[] treninky = {
                "Push",  // 0
                "Pull",  // 1
                "Legs"   // 2
            };

            if (cislodne >= 0 && cislodne < treninky.Length)
            {
                return treninky[cislodne];
            }

            return "Chyba";
        }


        private string GetTypTreninkuKR(int cislodne)
        {
            string[] treninky = {
                "Kruhový trénink 1", // 0
                "Kruhový trénink 2", // 1
                "Kruhový trénink 3"  // 2
            };

            if (cislodne >= 0 && cislodne < treninky.Length)
            {
                return treninky[cislodne];
            }

            return "Chyba";
        }



        // GET: TP
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TP.Include(t => t.User);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var uzivatel = _context.Users.Where(t => t.Id == userId).ToList();
            var uzivatelIdZaznam = await _context.TP.SingleOrDefaultAsync(tp => tp.Id == uzivatel[0].TPId);

            var jakCastoDny = 0;
            foreach (var user in uzivatel)
            {
                jakCastoDny = user.JakCastoAktualizovatVahu;

            }

            if (uzivatelIdZaznam != null)
            {
                DateTime tedka = DateTime.Now;
                DateTime datumPoslednihoUlozeni = uzivatelIdZaznam.DatumPoslednihoUlozeniVahy;
                TimeSpan rozdil = tedka - datumPoslednihoUlozeni;

                if (rozdil.TotalDays >= jakCastoDny)              /////NASTAVENI JAK CASTO BUDE KONTROLA VAHY, DAM CO DEN KVULI TESTOVANI
                {
                    uzivatelIdZaznam.AktualniVaha = false;
                    await _context.SaveChangesAsync();
                }

                var ZkontrolovaneDny = uzivatelIdZaznam.ZkontrolovaneDny;
                if (ZkontrolovaneDny == false)
                {
                    var denVTydnuRecords = await _context.TP
                            .Where(tp => tp.Id == uzivatelIdZaznam.Id)
                            .SelectMany(tp => tp.DnyVTydnu)
                            .ToListAsync();
                    var i = 1;
                    foreach (var denVTydnuRecord in denVTydnuRecords)
                    {
                        denVTydnuRecord.Den = (DayOfWeek)i;

                        i++;
                        if (i == 7)
                        {
                            i = 0;
                        }
                    }
                        uzivatelIdZaznam.ZkontrolovaneDny = true;
                }
                
                await _context.SaveChangesAsync();
            }

            if(uzivatelIdZaznam != null && uzivatelIdZaznam.DruhTP != "CUS")
            {
                var denVTydnuRecords = await _context.TP
                            .Where(tp => tp.Id == uzivatelIdZaznam.Id)
                            .SelectMany(tp => tp.DnyVTydnu)
                            .ToListAsync();
                DayOfWeek[] dnyTréninku = new DayOfWeek[uzivatelIdZaznam.PocetTreninkuZaTyden];
                int i = 0;
                foreach (var denVTydnuRecord in denVTydnuRecords)
                {
                    if(denVTydnuRecord.DenTréninku == true)
                    {
                        dnyTréninku[i] = denVTydnuRecord.Den; 
                        i++;
                    }
                }

                List<DateTime> dataTréninkovýchDnů = GeneratorTréninkovýchDat.VytvářeníDatumůTréninku(uzivatelIdZaznam.PocetTreninkuZaTyden, uzivatelIdZaznam.Délka * 4, dnyTréninku);
                var typTreninku = "";
                var typTreninkuZkratka = "";

                var typTreninkuCislo = 0;
                if (uzivatelIdZaznam.UlozenaDataDnu == false)
                {
                    try
                    {
                        foreach (var datumTreninkovehoDne in dataTréninkovýchDnů)
                        {
                            if(uzivatelIdZaznam.StylTP == "VM")
                            {
                                if(typTreninkuCislo == 4)
                                {
                                    typTreninkuCislo = 0;
                                }

                                typTreninku = GetTypTreninkuVM(typTreninkuCislo);
                                typTreninkuZkratka = TypTreninkuHelper.GetTypTreninkuZkratka(uzivatelIdZaznam, typTreninku);


                                var poradiCviku = GetPoradiCviku(typTreninkuZkratka);
                                string[] poradiCvikuArray = poradiCviku.Split(',');


                                var cviky = _context.Cvik
                                            .Where(c => c.UzivatelId == userId)
                                            .AsEnumerable()
                                            .Where(c => c.TypyTreninku.Contains(typTreninkuZkratka))
                                            .ToList();

                                List<Cvik> noveCviky = new List<Cvik>();
                                for (int j = 0; j < cviky.Count; j++)
                                {
                                    noveCviky.Add(cviky[j]);
                                }
                                for (int j = 0; j < cviky.Count; j++)
                                {
                                    int index = Array.IndexOf(poradiCvikuArray, cviky[j].Nazev);
                                    noveCviky.RemoveAt(index);
                                    noveCviky.Insert(index, cviky[j]);
                                }

                                var treninkoveDataEntita = new DenTreninku { DatumTreninku = datumTreninkovehoDne, TPId = uzivatelIdZaznam.Id, TypTreninku = typTreninku, Cviky = noveCviky };
                                _context.DenTreninku.Add(treninkoveDataEntita);
                                typTreninkuCislo++;

                            } else if (uzivatelIdZaznam.StylTP == "PPL")
                            {
                                if (typTreninkuCislo == 3)
                                {
                                    typTreninkuCislo = 0;
                                }

                                typTreninku = GetTypTreninkuPPL(typTreninkuCislo);
                                typTreninkuZkratka = TypTreninkuHelper.GetTypTreninkuZkratka(uzivatelIdZaznam, typTreninku);


                                var poradiCviku = GetPoradiCviku(typTreninkuZkratka);
                                string[] poradiCvikuArray = poradiCviku.Split(',');


                                var cviky = _context.Cvik
                                            .Where(c => c.UzivatelId == userId) 
                                            .AsEnumerable() 
                                            .Where(c => c.TypyTreninku != null) 
                                            .Where(c => c.TypyTreninku.Contains(typTreninkuZkratka)) 
                                            .ToList(); 

                                List<Cvik> noveCviky = new List<Cvik>();
                                for (int j = 0; j < cviky.Count; j++)
                                {
                                    noveCviky.Add(cviky[j]);
                                }
                                for (int j = 0; j < cviky.Count; j++)
                                {
                                    int index = Array.IndexOf(poradiCvikuArray, cviky[j].Nazev);
                                    noveCviky.RemoveAt(index);
                                    noveCviky.Insert(index, cviky[j]);
                                }

                                var treninkoveDataEntita = new DenTreninku { DatumTreninku = datumTreninkovehoDne, TPId = uzivatelIdZaznam.Id, TypTreninku = typTreninku, Cviky = noveCviky };
                                _context.DenTreninku.Add(treninkoveDataEntita);
                                typTreninkuCislo++;

                            }
                            else if (uzivatelIdZaznam.StylTP == "KR")
                            {
                                if (typTreninkuCislo == 3)
                                {
                                    typTreninkuCislo = 0;
                                }
                                typTreninku = GetTypTreninkuKR(typTreninkuCislo);
                                typTreninkuZkratka = TypTreninkuHelper.GetTypTreninkuZkratka(uzivatelIdZaznam, typTreninku);

                                var poradiCviku = GetPoradiCviku(typTreninkuZkratka);
                                string[] poradiCvikuArray = poradiCviku.Split(',');


                                var cviky = _context.Cvik
                                            .Where(c => c.UzivatelId == userId)
                                            .AsEnumerable()
                                            .Where(c => c.TypyTreninku.Contains(typTreninkuZkratka))
                                            .ToList();

                                List<Cvik> noveCviky = new List<Cvik>();
                                for (int j = 0; j < cviky.Count; j++)
                                {
                                    noveCviky.Add(cviky[j]);
                                }
                                for (int j = 0; j < cviky.Count; j++)
                                {
                                    int index = Array.IndexOf(poradiCvikuArray, cviky[j].Nazev);
                                    noveCviky.RemoveAt(index);
                                    noveCviky.Insert(index, cviky[j]);
                                }

                                var treninkoveDataEntita = new DenTreninku { DatumTreninku = datumTreninkovehoDne, TPId = uzivatelIdZaznam.Id, TypTreninku = typTreninku, Cviky = noveCviky };
                                _context.DenTreninku.Add(treninkoveDataEntita);
                                typTreninkuCislo++;
                            }
                        }

                        await _context.SaveChangesAsync();
                        uzivatelIdZaznam.UlozenaDataDnu = true;
                        await _context.SaveChangesAsync();

                        Ok("Data úspěšně uložena.");

                    }
                    catch (Exception ex)
                    {
                         BadRequest($"Chyba při ukládání dat: {ex.Message}");
                    }
                }
                var treninkoveData = await _context.DenTreninku
                                .Where(dt => dt.TPId == uzivatel[0].TPId)
                                .ToListAsync();

                var tpInfo = await _context.TP
                                .Where(dt => dt.Id == uzivatelIdZaznam.Id)
                                .ToListAsync();
                ViewBag.DenTreninku = treninkoveData;
                ViewBag.TP = tpInfo;
            }
            if (uzivatelIdZaznam != null && uzivatelIdZaznam.DruhTP == "CUS")
            {
                var tpInfo = await _context.TP
                             .Where(dt => dt.Id == uzivatelIdZaznam.Id)
                             .ToListAsync();
                var treninkoveData = await _context.DenTreninku
                                .Where(dt => dt.TPId == uzivatel[0].TPId)
                                .ToListAsync();
                ViewBag.DenTreninku = treninkoveData;
                ViewBag.TP = tpInfo;
            }
                var datacviku = _context.TreninkoveData
                            .Where(id => id.UzivatelId == userId)
                            .ToList();

            ViewBag.Uzivatel = uzivatel;
            ViewBag.Datacviku = datacviku;

            if (uzivatel[0].TPId != null)
            {
                var TPP = _context.TP
                                .FirstOrDefault(dt => dt.Id == uzivatel[0].TPId);
                ViewBag.TPP = TPP;

                var treninkoveDny = await _context.DenTreninku
                                .Where(dt => dt.TPId == uzivatel[0].TPId)
                                .ToListAsync();
                ViewBag.treninkoveDny = treninkoveDny;
            }


            return View(await applicationDbContext.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PridatTrenink(string typTreninku, DateTime datumTreninku, string? customTrenink = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var uzivatel = _context.Users.FirstOrDefault(t => t.Id == userId);

            var ideckoPlanu = uzivatel.TPId;
            if(ideckoPlanu != null)
            {
                if (customTrenink != null)
                {
                    var novyTrenink = new DenTreninku
                    {
                        TypTreninku = customTrenink,
                        DatumTreninku = datumTreninku,
                        TPId = (int)ideckoPlanu,
                        Cviky = new List<Cvik>(),
                    };
                    _context.DenTreninku.Add(novyTrenink);
                }
                else
                {
                    var novyTrenink = new DenTreninku
                    {
                        TypTreninku = typTreninku,
                        DatumTreninku = datumTreninku,
                        TPId = (int)ideckoPlanu,
                        Cviky = new List<Cvik>(),
                    };
                    _context.DenTreninku.Add(novyTrenink);
                }


                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return BadRequest("Uživatel nebo plán není platný.");
        }


        //funkce pro vytvoreni testovacich dat
        public async Task<IActionResult> PridatTestovaciData()
        {
            var currentUser = _context.Users.FirstOrDefault(t => t.UserName == "test@test.cz");

            var tP = new TP
            {
                Délka = 3,
                DruhTP = "BSH",
                StylTP = "VM",
                PocetTreninkuZaTyden = 4,
                UzivatelID = currentUser.Id,
                ZkontrolovaneDny = false,
                UlozenaDataDnu = false,
            };
            PridaneTestovaciDataGlobalni.PridaneTestovaciDataDoAplikace = 3;
            _context.TP.Add(tP);
            await _context.SaveChangesAsync();

            /////////////////////////!!!!!!!!!!!!!!!
            ///
            /// VYTVARENI TESTOVACIHO UCTU JAKO U UZIVATELE
            ///
            ////////////////////////!!!!!!!!!!!!!!!

            var dnes = DateTime.Now;
            var denDnes = dnes.DayOfWeek;
            var denDnes1 = (int)denDnes;
            var dnyTreninkuNaZapsani = _context.DenTreninku.ToList();


              /////////////////////////!!!!!!!!!!!!!!!
              ///
              /// NORMALNI NACTENI TRENINKOVYCH DAT
              ///
              ////////////////////////!!!!!!!!!!!!!!!
            

            foreach (var ser in serie)
            {
                _context.TreninkoveData.Add(ser);
            }

            var novyCvik = new Cvik { Nazev = "Dřep na jedné noze", PopisCviku = "Dřep na jedné noze", Partie = "Nohy", UzivatelId = userId, cvikVytvorenUzivatelem = true };
            if (novyCvik.TypyTreninku == null)
            {
                novyCvik.TypyTreninku = new List<string>();
                novyCvik.PočtySérií = new List<int>();
                novyCvik.PočtyOpakování = new List<string>();
                novyCvik.PauzyMeziSériemi = new List<int>();
            }
            _context.Cvik.Add(novyCvik);
            await _context.SaveChangesAsync();

            PridaneTestovaciDataGlobalni.PridaneTestovaciDataDoAplikace = 8;

            return RedirectToAction("Index", "Home");
        }

        // GET: TP/Create
        public IActionResult Create()
        {
            ViewData["UzivatelID"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: TP/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Délka,DruhTP,StylTP,PocetTreninkuZaTyden,DnyVTydnu,UzivatelID,ZkontorlovaneDny,UlozenaDataDnu,AktualniVaha,DatumPoslednihoUlozeniVahy")] TP tP)
        {
            if (ModelState.ContainsKey("User"))
            {
                ModelState.Remove("User");
            }

            if (ModelState.IsValid)
            {
                _context.Add(tP);
                await _context.SaveChangesAsync();

                // Aktualizace záznamu v tabulce AspNetUsers
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
                var currentUser = await _context.Users.FindAsync(userId); 
                currentUser.TPId = tP.Id;
                if(currentUser.TreninkovePlany == null)
                {
                    currentUser.TreninkovePlany = new List<int>();
                }
                currentUser.TreninkovePlany.Add(tP.Id);

                tP.User = currentUser;
                tP.AktualniVaha = true;
                
                tP.DatumPoslednihoUlozeniVahy = DateTime.Now;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["UzivatelID"] = new SelectList(_context.Users, "Id", "Id", tP.UzivatelID);
            return View(tP);
        }

        //vytvoreni vlastniho planu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VytvoritVlastniPlan([Bind("Id,UzivatelID,ZkontorlovaneDny,UlozenaDataDnu,AktualniVaha,DatumPoslednihoUlozeniVahy")] TP tP)
        {
            if (ModelState.ContainsKey("User"))
            {
                ModelState.Remove("User");
                ModelState.Remove("DruhTP");
                ModelState.Remove("StylTP");
            }



            if (ModelState.IsValid)
            {
                tP.Délka = -1;
                tP.DruhTP = "CUS";
                tP.StylTP = "CUS";
                tP.PocetTreninkuZaTyden = -1;
                tP.ZkontrolovaneDny = true;

                _context.Add(tP);
                await _context.SaveChangesAsync();

                // Aktualizace záznamu v tabulce AspNetUsers
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var currentUser = await _context.Users.FindAsync(userId);
                currentUser.TPId = tP.Id;
                if (currentUser.TreninkovePlany == null)
                {
                    currentUser.TreninkovePlany = new List<int>();
                }
                currentUser.TreninkovePlany.Add(tP.Id);

                tP.User = currentUser;
                tP.AktualniVaha = true;

                tP.DatumPoslednihoUlozeniVahy = DateTime.Now;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["UzivatelID"] = new SelectList(_context.Users, "Id", "Id", tP.UzivatelID);
            return View(tP);
        }

        // GET: TP/Delete/5
        public async Task<IActionResult> Delete1(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tP = await _context.TP
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tP == null)
            {
                return NotFound();
            }

            return View(tP);
        }

        // POST: TP/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tP = await _context.TP.FindAsync(id);
            
           
            if (tP != null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var uzivatelIdZaznam = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);

                uzivatelIdZaznam.TPId = null;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TPExists(int id)
        {
            return _context.TP.Any(e => e.Id == id);
        }
        
        [HttpPost]
        public async Task<IActionResult> AktualizaceVahy(VahaZFrontendu vaha)
        {
            if (vaha != null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var uzivatelIdZaznam = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);
                double cislo;

                if (double.TryParse(vaha.Váha, NumberStyles.Number, CultureInfo.InvariantCulture, out cislo))
                {
                    uzivatelIdZaznam.Vaha = cislo;

                    var uzivatelTP = await _context.TP.SingleOrDefaultAsync(u => u.UzivatelID == userId);
                    if (uzivatelTP != null)
                    {
                        uzivatelTP.AktualniVaha = true;
                        uzivatelTP.DatumPoslednihoUlozeniVahy = DateTime.Now;
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return BadRequest("Nepodařilo se převést váhu na desetinné číslo.");
                }
            }

            return View(vaha);
        }

        [HttpPost]
        public async Task<IActionResult> AktualizaceUrovne()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var uzivatelIdZaznam = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);
               
            uzivatelIdZaznam.Uroven = 2;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult OfflineNahled()
        {
            return View();
        }

        public async Task<IActionResult> NahledPlanu()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var uzivatelIdZaznam = await _context.TP.SingleOrDefaultAsync(tp => tp.UzivatelID == userId);

            var treninky = await _context.DenTreninku
                                .Where(d => d.TPId == uzivatelIdZaznam.Id)  
                                .ToListAsync();

            var model = new NahledPlanuModel
            {
                Treninky = treninky,
                TP = uzivatelIdZaznam,
            };

   

            return View(model);
        }

        public async Task<IActionResult> NahledPlanuDny()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var uzivatel = _context.Users.FirstOrDefault(t => t.Id == userId);

            var uzivatelIdZaznam = await _context.TP.SingleOrDefaultAsync(tp => tp.Id == uzivatel.TPId);

            var treninky = await _context.DenTreninku
                                .Where(d => d.TPId == uzivatelIdZaznam.Id)
                                .OrderBy(t => t.DatumTreninku)
                                .ToListAsync();

            var model = new NahledPlanuModel
            {
                Treninky = treninky,
                TP = uzivatelIdZaznam,
            };

            return View(model);
        }

        public async Task<IActionResult> NahledPlanuTreninky()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var uzivatel = _context.Users.FirstOrDefault(t => t.Id == userId);

            var uzivatelIdZaznam = await _context.TP.SingleOrDefaultAsync(tp => tp.Id == uzivatel.TPId);

            var treninky = await _context.DenTreninku
                                .Where(d => d.TPId == uzivatelIdZaznam.Id)
                                .ToListAsync();

            var model = new NahledPlanuModel
            {
                Treninky = treninky,
                TP = uzivatelIdZaznam,
            };

            return View(model);
        }

        private string GetPoradiCviku(string typTreninkuZkratka)
        {

            var workoutPlans = new Dictionary<string, string>
            {
                { "BSHKR1", "Dřepy,Legpress,Lýtka ve stoje,Předkopy,Zákopy,Mrtvý tah,Stahování tyče na stroji před hlavu - vertikálně,Benchpress,Tlaky na hrudník na nakloněné lavici,Tlaky na ramena s jednoručnou činkou,Upažování s jednoručnou činkou,Bicepsové přítahy jednoruček,Bicepsové přítahy obouručky,Tricepsové stahování kladky,Tricepsové stahování kladky za hlavu" },
                { "BSHKR2", "Dřepy,Legpress,Rumunský mrtvý tah,Mrtvý tah,Shyby nadhmatem,Stahování tyče na stroji před hlavu - vertikálně,Přitahování tyče na stroji - horizontálně,Benchpress,Tlaky na hrudník na nakloněné lavici,Tlaky na ramena s jednoručnou činkou,Upažování s jednoručnou činkou,Bicepsové přítahy jednoruček,Bicepsové přítahy obouručky,Tricepsové stahování kladky,Tricepsové stahování kladky za hlavu" },
                { "BSHKR3", "Dřepy,Legpress,Zákopy,Mrtvý tah,Stahování tyče na stroji před hlavu - vertikálně,Benchpress,Tlaky na hrudník na nakloněné lavici,Pec deck,Stahování kladek na hrudník,Tlaky na ramena s jednoručnou činkou,Upažování s jednoručnou činkou,Bicepsové přítahy jednoruček,Bicepsové přítahy obouručky,Tricepsové stahování kladky,Tricepsové stahování kladky za hlavu" },
                { "BSHVMNohy", "Dřepy s vlastní vahou,Dřepy,Legpress,Zákopy,Předkopy,Bulharský dřep,Rumunský mrtvý tah,Hiptrusty,Lýtka ve stoje" },
                { "BSHVMRamBic", "Tlaky na ramena - rozcvička,Tlaky na ramena s jednoručnou činkou,Upažování s jednoručnou činkou,Stroj na zadky ramen,Upažování na kladce,Tlaky na stroji,Bicepsové přítahy jednoruček,Bicepsové přítahy obouručky,Bicepsové přítahy na stroji" },
                { "BSHVMHrTric", "Kliky,Benchpress,Tlaky na hrudník na nakloněné lavici,Pec deck,Stahování kladek na hrudník,Dipy na bradle,Tricepsové stahování kladky,Tricepsové stahování kladky za hlavu" },
                { "BSHVMZada", "Mrtvý tah bez závaží,Mrtvý tah,Shyby nadhmatem,Stahování tyče na stroji před hlavu - vertikálně,Přitahování tyče na stroji - horizontálně,Přitahování na stroji,Přitahování tyče ve stoje,Sklapovačky,Přitahování noh na bradlech" },
                { "BSHPPLLegs", "Dřepy s vlastní vahou,Dřepy,Legpress,Zákopy,Předkopy,Bulharský dřep,Rumunský mrtvý tah,Hiptrusty,Lýtka ve stoje" },
                { "BSHPPLPush", "Kliky,Benchpress,Tlaky na ramena s jednoručnou činkou,Tlaky na stroji,Tlaky na hrudník na nakloněné lavici,Stahování kladek na hrudník,Upažování s jednoručnou činkou,Stroj na zadky ramen,Dipy na bradle,Tricepsové stahování kladky" },
                { "BSHPPLPull", "Mrtvý tah bez závaží,Mrtvý tah,Shyby nadhmatem,Stahování tyče na stroji před hlavu - vertikálně,Přitahování tyče na stroji - horizontálně,Přitahování tyče ve stoje,Bicepsové přítahy jednoruček,Bicepsové přítahy obouručky,Bicepsové přítahy na stroji" },
                { "RVKR1", "Dřepy,Legpress,Lýtka ve stoje,Předkopy,Zákopy,Mrtvý tah,Stahování tyče na stroji před hlavu - vertikálně,Benchpress,Tlaky na hrudník na nakloněné lavici,Tlaky na ramena s jednoručnou činkou,Upažování s jednoručnou činkou,Bicepsové přítahy jednoruček,Bicepsové přítahy obouručky,Tricepsové stahování kladky,Tricepsové stahování kladky za hlavu" },
                { "RVKR2", "Dřepy,Legpress,Rumunský mrtvý tah,Mrtvý tah,Shyby nadhmatem,Stahování tyče na stroji před hlavu - vertikálně,Přitahování tyče na stroji - horizontálně,Benchpress,Tlaky na hrudník na nakloněné lavici,Tlaky na ramena s jednoručnou činkou,Upažování s jednoručnou činkou,Bicepsové přítahy jednoruček,Bicepsové přítahy obouručky,Tricepsové stahování kladky,Tricepsové stahování kladky za hlavu" },
                { "RVKR3", "Dřepy,Legpress,Zákopy,Mrtvý tah,Stahování tyče na stroji před hlavu - vertikálně,Benchpress,Tlaky na hrudník na nakloněné lavici,Pec deck,Stahování kladek na hrudník,Tlaky na ramena s jednoručnou činkou,Upažování s jednoručnou činkou,Bicepsové přítahy jednoruček,Bicepsové přítahy obouručky,Tricepsové stahování kladky,Tricepsové stahování kladky za hlavu" },
                { "RVVMNohy", "Dřepy s vlastní vahou,Dřepy,Legpress,Zákopy,Předkopy,Bulharský dřep,Rumunský mrtvý tah,Hiptrusty,Lýtka ve stoje" },
                { "RVVMRamBic", "Tlaky na ramena - rozcvička,Tlaky na ramena s jednoručnou činkou,Upažování s jednoručnou činkou,Stroj na zadky ramen,Upažování na kladce,Tlaky na stroji,Bicepsové přítahy jednoruček,Bicepsové přítahy obouručky,Bicepsové přítahy na stroji" },
                { "RVVMHrTric", "Kliky,Benchpress,Tlaky na hrudník na nakloněné lavici,Pec deck,Stahování kladek na hrudník,Dipy na bradle,Tricepsové stahování kladky,Tricepsové stahování kladky za hlavu" },
                { "RVVMZada", "Mrtvý tah bez závaží,Mrtvý tah,Shyby nadhmatem,Stahování tyče na stroji před hlavu - vertikálně,Přitahování tyče na stroji - horizontálně,Přitahování na stroji,Přitahování tyče ve stoje,Sklapovačky,Přitahování noh na bradlech" },
                { "RVPPLLegs", "Dřepy s vlastní vahou,Dřepy,Legpress,Zákopy,Předkopy,Bulharský dřep,Rumunský mrtvý tah,Hiptrusty,Lýtka ve stoje" },
                { "RVPPLPush", "Kliky,Benchpress,Tlaky na ramena s jednoručnou činkou,Tlaky na stroji,Tlaky na hrudník na nakloněné lavici,Stahování kladek na hrudník,Upažování s jednoručnou činkou,Stroj na zadky ramen,Dipy na bradle,Tricepsové stahování kladky" },
                { "RVPPLPull", "Mrtvý tah bez závaží,Mrtvý tah,Shyby nadhmatem,Stahování tyče na stroji před hlavu - vertikálně,Přitahování tyče na stroji - horizontálně,Přitahování tyče ve stoje,Bicepsové přítahy jednoruček,Bicepsové přítahy obouručky,Bicepsové přítahy na stroji" },
                { "SRKR1", "Dřepy,Legpress,Lýtka ve stoje,Předkopy,Zákopy,Mrtvý tah,Stahování tyče na stroji před hlavu - vertikálně,Benchpress,Tlaky na hrudník na nakloněné lavici,Tlaky na ramena s jednoručnou činkou,Upažování s jednoručnou činkou,Bicepsové přítahy jednoruček,Bicepsové přítahy obouručky,Tricepsové stahování kladky,Tricepsové stahování kladky za hlavu" },
                { "SRKR2", "Dřepy,Legpress,Rumunský mrtvý tah,Mrtvý tah,Shyby nadhmatem,Stahování tyče na stroji před hlavu - vertikálně,Přitahování tyče na stroji - horizontálně,Benchpress,Tlaky na hrudník na nakloněné lavici,Tlaky na ramena s jednoručnou činkou,Upažování s jednoručnou činkou,Bicepsové přítahy jednoruček,Bicepsové přítahy obouručky,Tricepsové stahování kladky,Tricepsové stahování kladky za hlavu" },
                { "SRKR3", "Dřepy,Legpress,Zákopy,Mrtvý tah,Stahování tyče na stroji před hlavu - vertikálně,Benchpress,Tlaky na hrudník na nakloněné lavici,Pec deck,Stahování kladek na hrudník,Tlaky na ramena s jednoručnou činkou,Upažování s jednoručnou činkou,Bicepsové přítahy jednoruček,Bicepsové přítahy obouručky,Tricepsové stahování kladky,Tricepsové stahování kladky za hlavu" },
                { "SRVMNohy", "Dřepy s vlastní vahou,Dřepy,Legpress,Zákopy,Předkopy,Bulharský dřep,Rumunský mrtvý tah,Hiptrusty,Lýtka ve stoje" },
                { "SRVMRamBic", "Tlaky na ramena - rozcvička,Tlaky na ramena s jednoručnou činkou,Upažování s jednoručnou činkou,Stroj na zadky ramen,Upažování na kladce,Tlaky na stroji,Bicepsové přítahy jednoruček,Bicepsové přítahy obouručky,Bicepsové přítahy na stroji" },
                { "SRVMHrTric", "Kliky,Benchpress,Tlaky na hrudník na nakloněné lavici,Pec deck,Stahování kladek na hrudník,Dipy na bradle,Tricepsové stahování kladky,Tricepsové stahování kladky za hlavu" },
                { "SRVMZada", "Mrtvý tah bez závaží,Mrtvý tah,Shyby nadhmatem,Stahování tyče na stroji před hlavu - vertikálně,Přitahování tyče na stroji - horizontálně,Přitahování na stroji,Přitahování tyče ve stoje,Sklapovačky,Přitahování noh na bradlech" },
                { "SRPPLLegs", "Dřepy s vlastní vahou,Dřepy,Legpress,Zákopy,Předkopy,Bulharský dřep,Rumunský mrtvý tah,Hiptrusty,Lýtka ve stoje" },
                { "SRPPLPush", "Kliky,Benchpress,Tlaky na ramena s jednoručnou činkou,Tlaky na stroji,Tlaky na hrudník na nakloněné lavici,Stahování kladek na hrudník,Upažování s jednoručnou činkou,Stroj na zadky ramen,Dipy na bradle,Tricepsové stahování kladky" },
                { "SRPPLPull", "Mrtvý tah bez závaží,Mrtvý tah,Shyby nadhmatem,Stahování tyče na stroji před hlavu - vertikálně,Přitahování tyče na stroji - horizontálně,Přitahování tyče ve stoje,Bicepsové přítahy jednoruček,Bicepsové přítahy obouručky,Bicepsové přítahy na stroji" }

            };

            return workoutPlans.TryGetValue(typTreninkuZkratka, out var cviceni) ? cviceni : "Trénink nebyl nalezen";  // Nebo jiná výchozí hodnota pro neexistující klíče
                
        }

    }
}
