using FitnessWebApp.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessWebApp.Models
{
    public class Uzivatel : IdentityUser
    {
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public int Vek { get; set; }
        public int Vyska { get; set; }
        public double Vaha { get; set; }
        public int Uroven { get; set; }
        public int Pohlavi { get; set; }
        public int JakCastoAktualizovatVahu { get; set; }
        public bool PridaneData { get; set; }
        public DateTime PomocneDatum { get; set; }
        public int? TPId { get; set; }
        [NotMapped]
        public List<int>? TreninkovePlany { get; set; }

        [Column("TreninkovyPlany")]
        public string? TreninkovyPlanySerialized
        {
            get => TreninkovePlany != null ? string.Join(",", TreninkovePlany) : null;
            set
            {
                if (value != null && value != "")
                {
                    TreninkovePlany = value?.Split(',').Select(int.Parse).ToList();
                }
            }
        }

    }
}
