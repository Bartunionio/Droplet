using Droplet.Models.Entities;

namespace Droplet.Models
{
    public class HospitalViewModel
    {
        public Hospital Hospital { get; set; }
        public ICollection<Doctor> Personnel { get; set; }
    }
}
