using Droplet.Data;
using Droplet.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Droplet.Models;

namespace Droplet.Controllers
{
    [Route("/Home", Name = "blood_status")]
    public class BloodStatusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BloodStatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var bloodBank = _context.Donations
                .Include(b => b.Donor)
                .ToList();

            var viewModel = bloodBank.Select(donation => new BloodStatusViewModel
            {
                Id = donation.Id,
                Date = donation.Date.ToString("d"),
                DonorName = donation.Donor.FirstName
            }).ToList();

            return View("~/Views/Home/Blood_status.cshtml", viewModel);
        }
    }
}
