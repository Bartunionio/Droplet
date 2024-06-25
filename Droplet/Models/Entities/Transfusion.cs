using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Droplet.Models.Entities
{
    public class Transfusion
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Recipient")]
        public int IdRecipient { get; set; }
        public Recipient Recipient { get; set; }

        [ForeignKey("Hospital")]
        public int IdHospital { get; set; }
        public Hospital Hospital { get; set; }

        [ForeignKey("Doctor")]
        public int IdDoctor { get; set; }
        public Doctor Doctor { get; set; }

        public DateTime Date { get; set; }


        public ICollection<Bank> BloodUsed { get; set; } = new List<Bank>();
    }
}
