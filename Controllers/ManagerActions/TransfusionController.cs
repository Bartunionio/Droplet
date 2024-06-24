using Droplet.Data;
using Droplet.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Droplet.Controllers.ManagerActions
{
    public class TransfusionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransfusionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transfusion/Create
        [HttpGet]
        [Route("/ManagerActions/Transfusion/Create", Name = "transfusion_create")]
        public async Task<IActionResult> Create()
        {
            // Get recipients
            var recipients = await _context.Recipients
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.FullName
                })
                .ToListAsync();

            // Get hospitals
            var hospitals = await _context.Hospitals
                .Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = h.Name
                })
                .ToListAsync();

            // Get doctors (Implement logic to fetch doctors associated with hospitals)
            var doctors = new List<SelectListItem>(); // Implement logic based on your schema

            ViewBag.Recipients = new SelectList(recipients, "Value", "Text");
            ViewBag.Hospitals = new SelectList(hospitals, "Value", "Text");
            ViewBag.Doctors = new SelectList(doctors, "Value", "Text");

            return View("~/Views/ManagerActions/Transfusion/Create.cshtml", new Transfusion());
        }

        // GET: Transfusion/GetEligibleBlood
        [HttpGet]
        [Route("/ManagerActions/Transfusion/GetEligibleBlood")]
        public async Task<IActionResult> GetEligibleBlood(int recipientId)
        {
            var recipient = await _context.Recipients.FindAsync(recipientId);
            if (recipient == null)
            {
                return Json(new { success = false, message = "Recipient not found" });
            }

            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var cutOffDate = currentDate.AddDays(-35);

            var eligibleBlood = await _context.Donations
                .Where(d => d.IdTransfusion == null && d.Date >= cutOffDate && d.Donor.BloodType == recipient.BloodType)
                .Select(d => new
                {
                    Id = d.Id,
                    Description = $"{d.Donor.FullName} - Blood Type: {d.Donor.BloodType}"
                })
                .ToListAsync();

            return Json(new { success = true, data = eligibleBlood });
        }

        // POST: Transfusion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/ManagerActions/Transfusion/Create", Name = "transfusion_create")]
        public async Task<IActionResult> Create(Transfusion model, int bloodId)
        {
            if (ModelState.IsValid)
            {
                // Find the selected blood donation
                var bloodDonation = await _context.Donations.FindAsync(bloodId);

                if (bloodDonation != null)
                {
                    model.BloodUsed = new List<Bank> { bloodDonation };
                    _context.Transfusions.Add(model);

                    // Update the blood donation with the transfusion ID
                    bloodDonation.IdTransfusion = model.Id;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), "Home"); // Redirect to Home or Blood_status view
                }
            }

            // If model state is not valid or blood donation was not found, reload the page with options
            var recipients = await _context.Recipients
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.FullName
                })
                .ToListAsync();

            var hospitals = await _context.Hospitals
                .Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = h.Name
                })
                .ToListAsync();

            var doctors = new List<SelectListItem>(); // You need to implement this logic based on your schema

            ViewBag.Recipients = new SelectList(recipients, "Value", "Text");
            ViewBag.Hospitals = new SelectList(hospitals, "Value", "Text");
            ViewBag.Doctors = new SelectList(doctors, "Value", "Text");
            ViewBag.BloodOptions = new SelectList(new List<object>()); // Empty list for failed scenario

            return View("~/Views/ManagerActions/Transfusion/Create.cshtml", model);
        }
    }
}
