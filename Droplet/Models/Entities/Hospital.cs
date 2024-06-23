using System.ComponentModel.DataAnnotations;

namespace Droplet.Models.Entities
{
    public class Hospital
    {
        [Key]
        public int Id { get; set; }
        public string Address { get; set; } = default!;
        public string Street { get; set; } = default!;
        public string PostalCode { get; set; } = default!;

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
        public ICollection<Transfusion> Transfusions { get; set; } = new List<Transfusion>();
    }
}
