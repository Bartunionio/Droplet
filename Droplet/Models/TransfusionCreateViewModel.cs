using System.ComponentModel.DataAnnotations;

namespace Droplet.Models.ViewModels
{
    public class TransfusionCreateViewModel
    {
        [Required(ErrorMessage = "Please select a recipient")]
        public int SelectedRecipientId { get; set; }

        [Required(ErrorMessage = "Please enter the blood quantity")]
        public int BloodQuantity { get; set; }

        [Required(ErrorMessage = "Please select a hospital")]
        public int SelectedHospitalId { get; set; }

        [Required(ErrorMessage = "Please select a doctor")]
        public int SelectedDoctorId { get; set; }
    }
}
