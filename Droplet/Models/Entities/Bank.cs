using Droplet.Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Droplet.Models.Entities
{
    public class Bank
    {
        [Key]
        public int Id { get; set; }
        public DateOnly Date { get; set; } = default!;

        [ForeignKey("Donor")]
        public int IdDonor { get; set; }
        public Donor Donor { get; set; } = default!;

        [ForeignKey("Transfusion")]
        public int? IdTransfusion { get; set; } = null;
        public Transfusion Transfusion { get; set; }
        

    }
}
