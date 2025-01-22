using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FitnessWebApp.Models
{
    public class TP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Délka { get; set; }
        public string DruhTP { get; set; }
        public string StylTP { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné.")]
        [Display(Name = "Počet tréninků za týden")]
        public int PocetTreninkuZaTyden { get; set; }

        [JsonIgnore]
        [Required(ErrorMessage = "Toto pole je povinné.")]
        [Display(Name = "Dny v týdnu")]
        public List<DenVTydnu> DnyVTydnu { get; set; }

        [ForeignKey("UzivatelID")]
        public Uzivatel User { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné.")]
        public string UzivatelID { get; set; }

        public bool ZkontrolovaneDny {  get; set; }

        public bool UlozenaDataDnu { get; set; }
        public bool AktualniVaha {  get; set; }
        public DateTime DatumPoslednihoUlozeniVahy {  get; set; }


        public TP() {
            DnyVTydnu = new List<DenVTydnu>();
        }

    }
}
