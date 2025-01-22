using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Newtonsoft.Json;

namespace FitnessWebApp.Models
{
    public class DenVTydnu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Toto pole je povinné.")]
        public DayOfWeek Den { get; set; }
        public Boolean DenTréninku { get; set; }
    }
    public static class PomocnikSDaty
    {
        public static string ZjistitJmeno(DayOfWeek dayOfWeek)
        {
            CultureInfo cultureInfo = new CultureInfo("cs-CZ"); // Čeština
            return cultureInfo.DateTimeFormat.GetDayName(dayOfWeek);
        }
    }
}
