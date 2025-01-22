using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FitnessWebApp.Models
{
    public class TreninkoveData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string UzivatelId { get; set; }
        public double VahaUzivatele { get; set; }
        public DateTime Datum { get; set; }
        public int CvikId { get; set; }
        public int CisloSerie { get; set; }
        public int PocetOpakovani { get; set; }
        public double CvicenaVaha { get; set; }
        public int TpId { get; set; }

       [ForeignKey("CvikId")]
        public virtual Cvik Cvik { get; set; }
    }
}
