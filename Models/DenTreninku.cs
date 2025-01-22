using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace FitnessWebApp.Models
{
    public class DenTreninku
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné.")]
        [Display(Name = "Datum tréninku")]
        public DateTime DatumTreninku { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné.")]
        public string TypTreninku { get; set; }

        public int TPId { get; set; }

        [ForeignKey("TPId")]
        public TP TP { get; set; }

        [NotMapped]
        public List<Cvik> Cviky { get; set; }

        [Column("Cviky")]
        public string CvikySerialized
        {
            get => JsonConvert.SerializeObject(Cviky);
            set => Cviky = JsonConvert.DeserializeObject<List<Cvik>>(value);
        }

    }
}
