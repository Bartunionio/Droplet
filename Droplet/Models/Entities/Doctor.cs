using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Droplet.Models.Entities
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PESEL { get; set; }

        public ICollection<Hospital> Hospitals { get; set; } = new List<Hospital>();
        public ICollection<Transfusion> Transfusions { get; set; } = new List<Transfusion>();

    }
}
