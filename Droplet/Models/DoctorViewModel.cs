using Droplet.Models.Entities;

namespace Droplet.Models
{
    public class DoctorViewModel
    {
        public Doctor Doctor { get; set; }
        public List<int> SelectedHospitalIds { get; set; } = new List<int>();
        public List<Hospital> Hospitals { get; set; } = new List<Hospital>();
    }
}
