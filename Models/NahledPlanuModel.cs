using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FitnessWebApp.Models
{
    public class NahledPlanuModel
    {
        public List<DenTreninku> Treninky { get; set; }
        public TP TP { get; set; }
    }
}
