using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Droplet.Models.Entities
{
    public class Transfusion
    {
        [Key]
        public int Id { get; set; }
        public DateOnly Date { get; set; }

        [ForeignKey("Recipient")]
        public int IdRecipient { get; set; } = default!;
        public Recipient Recipient { get; set; } = default!;

        [ForeignKey("Hospital")]
        public int IdHospital { get; set; } = default!;
        public Hospital Hospital { get; set; } = default!;

        [ForeignKey("Doctor")]
        public int IdDoctor { get; set; } = default!;
        public Doctor Doctor { get; set; } = default!;

        public ICollection<Bank> BloodUsed { get; set; } = new List<Bank>();
    }
}
