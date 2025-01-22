using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessWebApp.Models
{
    public class Cvik
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CvikId { get; set; }
        [Required(ErrorMessage = "Toto pole je povinné.")]
        public string Nazev { get; set; }
        [NotMapped]
        public List<string>? PočtyOpakování { get; set; }

        [Column("PočetOpakování")]
        public string? PočetOpakováníSerialized
        {
            get => PočtyOpakování != null ? string.Join(".", PočtyOpakování) : null;
            set => PočtyOpakování = value?.Split('.').ToList();
        }
        [NotMapped]
        public List<int>? PočtySérií { get; set; }

        [Column("PočetSérií")]
        public string? PočetSériíSerialized
        {
            get => PočtySérií != null ? string.Join(",", PočtySérií) : null;
            set
            {
                if (value != null && value != "")
                {
                    PočtySérií = value?.Split(',').Select(int.Parse).ToList();
                }
            }
        }
        [NotMapped]
        public List<int>? PauzyMeziSériemi { get; set; }

        [Column("PauzaMeziSériemi")]
        public string? PauzaMeziSériemiSerialized
        {
            get => PauzyMeziSériemi != null ? string.Join(",", PauzyMeziSériemi) : null;
            set
            {
                if (value != null && value != "")
                {
                    PauzyMeziSériemi = value?.Split(',').Select(int.Parse).ToList();
                }
            }
        }

        [Required(ErrorMessage = "Toto pole je povinné.")]
        public string Partie {  get; set; }

        public string? PopisCviku { get; set; }
        [NotMapped]
        public List<string>? TypyTreninku { get; set; }

        [Column("TypTreninku")]
        public string? TypTreninkuSerialized
        {
            get => TypyTreninku != null ? string.Join(",", TypyTreninku) : null;
            set => TypyTreninku = value?.Split(',').ToList();
        }
        public string UzivatelId { get; set; }

        [ForeignKey("UzivatelId")]
        public virtual Uzivatel Uzivatel { get; set; }
        public bool cvikVytvorenUzivatelem { get; set; }
    }
}
