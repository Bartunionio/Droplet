using Droplet.Data;
using Droplet.Models;
using Droplet.Models.Entities;
using Droplet.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Droplet.Controllers.ManagerActions
{
    [Authorize(Roles = "Admin,Manager")]
    public class TransfusionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransfusionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Transfusion_table()
        {
            var transfusions = await _context.Transfusions
                .Include(t => t.Recipient)
                .Include(t => t.Hospital)
                .Include(t => t.Doctor)
                .ToListAsync();

            return View("~/Views/Home/Transfusion_view.cshtml", transfusions);
        }

        // GET: Transfusion/Create
        [HttpGet]
        [Route("/ManagerActions/Transfusion/Create", Name = "transfusion_create")]
        public async Task<IActionResult> Create()
        {
            var model = new TransfusionCreateViewModel();
            await LoadCreateForm(model);
            return View("~/Views/ManagerActions/Transfusion/Create.cshtml", model);
        }

        // POST: Transfusion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/ManagerActions/Transfusion/Create", Name = "transfusion_create_post")]
        public async Task<IActionResult> Create(TransfusionCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var recipient = await _context.Recipients.FindAsync(model.SelectedRecipientId);
                var hospital = await _context.Hospitals.FindAsync(model.SelectedHospitalId);
                var doctor = await _context.Doctors.FindAsync(model.SelectedDoctorId);

                if (recipient == null || hospital == null || doctor == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid recipient, hospital, or doctor selection.");
                    await LoadCreateForm(model);
                    return View("~/Views/ManagerActions/Transfusion/Create.cshtml", model);
                }

                var totalBloodAvailable = await _context.Donations
                    .Where(d => d.IdTransfusion == null && d.Donor.BloodType == recipient.BloodType)
                    .CountAsync();

                if (totalBloodAvailable < model.BloodQuantity)
                {
                    ModelState.AddModelError(string.Empty, $"There are not enough eligible blood donations available for transfusion. Available: {totalBloodAvailable}, Required: {model.BloodQuantity}");
                    await LoadCreateForm(model);
                    return View("~/Views/ManagerActions/Transfusion/Create.cshtml", model);
                }

                var transfusion = new Transfusion
                {
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    IdRecipient = model.SelectedRecipientId,
                    IdHospital = model.SelectedHospitalId,
                    IdDoctor = model.SelectedDoctorId,
                };

                _context.Transfusions.Add(transfusion);
                await _context.SaveChangesAsync();

                var bloodDonations = await _context.Donations
                    .Include(d => d.Donor)
                    .Where(d => d.IdTransfusion == null && d.Donor.BloodType == recipient.BloodType)
                    .OrderBy(d => d.Date)
                    .Take(model.BloodQuantity)
                    .ToListAsync();

                if (bloodDonations.Count < model.BloodQuantity)
                {
                    ModelState.AddModelError(string.Empty, $"There are not enough eligible blood donations available for transfusion. Available: {bloodDonations.Count}, Required: {model.BloodQuantity}");
                    _context.Transfusions.Remove(transfusion);
                    await _context.SaveChangesAsync();
                    await LoadCreateForm(model);
                    return View("~/Views/ManagerActions/Transfusion/Create.cshtml", model);
                }

                foreach (var donation in bloodDonations)
                {
                    donation.IdTransfusion = transfusion.Id;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Transfusion_table));
            }

            await LoadCreateForm(model);
            return View("~/Views/ManagerActions/Transfusion/Create.cshtml", model);
        }

        // GET: Transfusion/GetDoctorsByHospital
        [HttpGet]
        [Route("/ManagerActions/Transfusion/GetDoctorsByHospital")]
        public async Task<IActionResult> GetDoctorsByHospital(int hospitalId)
        {
            var doctors = await _context.Doctors
                .Where(d => d.Hospitals.Any(h => h.Id == hospitalId))
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.FullName
                })
                .ToListAsync();

            return Json(new { success = true, data = doctors });
        }

        // Helper method to load create form with updated data
        private async Task LoadCreateForm(TransfusionCreateViewModel model)
        {
            var recipients = await _context.Recipients
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.FullName
                })
                .ToListAsync();

            var hospitals = await _context.Hospitals
                .Include(h => h.Doctors)
                .Select(h => new HospitalViewModel
                {
                    Hospital = h,
                    Personnel = h.Doctors.ToList()
                })
                .ToListAsync();

            ViewBag.Recipients = new SelectList(recipients, "Value", "Text");
            ViewBag.Hospitals = new SelectList(hospitals, "Hospital.Id", "Hospital.Name");

            if (model.SelectedHospitalId != null)
            {
                var doctors = await _context.Doctors
                    .Where(d => d.Hospitals.Any(h => h.Id == model.SelectedHospitalId))
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.FullName
                    })
                    .ToListAsync();

                ViewBag.Doctors = new SelectList(doctors, "Value", "Text", model.SelectedDoctorId);
            }
            else
            {
                ViewBag.Doctors = new SelectList(new List<SelectListItem>(), "Value", "Text");
            }
        }
    }
}
