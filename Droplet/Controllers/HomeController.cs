using Droplet.Data;
using Droplet.Models;
using Droplet.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Droplet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // GET: Blood Status
        [Route("Views/Home/Blood_status.cshtml", Name = "blood_status")]
        public async Task<IActionResult> BloodStatus()
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var cutOffDate = currentDate.AddDays(-35);

            var bloodStatus = await _context.Donations
                .Include(d => d.Donor)
                .Where(b => b.IdTransfusion == null && b.Date > cutOffDate)
                .GroupBy(b => b.Donor.BloodType)
                .Select(g => new
                {
                    BloodType = g.Key,
                    Quantity = g.Count()
                })
                .ToListAsync();

            return View("Views/Home/Blood_status.cshtml", bloodStatus);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
