using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Droplet.Data;
using Droplet.Models.Entities;
using Droplet.Models;
using System.Numerics;
using Microsoft.AspNetCore.Authorization;

namespace Droplet.Controllers.AdminActions
{
    [Authorize(Roles ="Admin")]
    public class ManageHospitalsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManageHospitalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ManageHospitals
        [Route("/AdminActions/ManageHospitals", Name = "hospitallist")]
        public async Task<IActionResult> Index()
        {
            return View("~/Views/AdminActions/ManageHospitals/Index.cshtml", await _context.Hospitals.ToListAsync());
        }

        // GET: ManageHospitals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hospital = await _context.Hospitals
                .Include(h => h.Doctors)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hospital == null)
            {
                return NotFound();
            }

            var viewModel = new HospitalViewModel
            {
                Hospital = hospital,
                Personnel = hospital.Doctors.ToList()
            };

            return View("~/Views/AdminActions/ManageHospitals/Details.cshtml", viewModel);
        }

        // GET: ManageHospitals/Create
        public IActionResult Create()
        {
            return View("~/Views/AdminActions/ManageHospitals/Create.cshtml");
        }

        // POST: ManageHospitals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Street,PostalCode")] Hospital hospital)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hospital);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/AdminActions/ManageHospitals/Create.cshtml", hospital);
        }

        // GET: ManageHospitals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hospital = await _context.Hospitals.FindAsync(id);
            if (hospital == null)
            {
                return NotFound();
            }
            return View("~/Views/AdminActions/ManageHospitals/Edit.cshtml", hospital);
        }

        // POST: ManageHospitals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Street,PostalCode")] Hospital hospital)
        {
            if (id != hospital.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hospital);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HospitalExists(hospital.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/AdminActions/ManageHospitals/Edit.cshtml", hospital);
        }

        // GET: ManageHospitals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hospital = await _context.Hospitals
                .Include(h => h.Transfusions)
                .Include(h => h.Doctors)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hospital == null)
            {
                return NotFound();
            }

            return View("~/Views/AdminActions/ManageHospitals/Delete.cshtml", hospital);
        }

        // POST: ManageHospitals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await _context.Transfusions.FirstOrDefaultAsync(h => h.IdHospital == id) != null)
            {
                return RedirectToAction(nameof(Delete));
            }

            var hospital = await _context.Hospitals
                .Include(h => h.Doctors)
                .FirstOrDefaultAsync(h => h.Id == id);
       
            if (hospital != null)
            {
                if (hospital.Doctors.Any())
                {
                    return RedirectToAction(nameof(Delete));
                }
                _context.Hospitals.Remove(hospital);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HospitalExists(int id)
        {
            return _context.Hospitals.Any(e => e.Id == id);
        }
    }
}
