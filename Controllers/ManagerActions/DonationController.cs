using Droplet.Data;
using Droplet.Models.Entities;
using Droplet.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Droplet.Controllers.ManagerActions
{
    public class DonationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonationController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult BloodStatus()
        {
            return View("~/Views/Home/Blood_status.cshtml");
        }

        // GET: Donation/Create
        [HttpGet]
        [Route("/ManagerActions/Donation", Name = "donation")]
        public async Task<IActionResult> Create()
        {
            var donors = await _context.Donors
                .Select(d => new
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    PESEL = d.PESEL
                })
                .ToListAsync();

            List<SelectListItem> donorList = donors.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.FullName} - PESEL: {d.PESEL}"
            }).ToList();

            ViewBag.Donors = new SelectList(donorList, "Value", "Text");
            return View("~/Views/ManagerActions/Donation/Create.cshtml", new DonationCreateViewModel());
        }

        // POST: Donation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/ManagerActions/Donation", Name = "donation")]
        public async Task<IActionResult> Create(DonationCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var donation = new Bank
                {
                    IdDonor = model.DonorId,
                    Date = DateOnly.FromDateTime(DateTime.Now)
                };

                _context.Donations.Add(donation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(BloodStatus), "Home");
            }

            var donors = await _context.Donors
                .Select(d => new
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    PESEL = d.PESEL
                })
                .ToListAsync();

            List<SelectListItem> donorList = donors.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.FullName} - PESEL: {d.PESEL}"
            }).ToList();

            ViewBag.Donors = new SelectList(donorList, "Value", "Text", model.DonorId);
            return View("~/Views/ManagerActions/Donation/Create.cshtml", model);
        }
    }
}
