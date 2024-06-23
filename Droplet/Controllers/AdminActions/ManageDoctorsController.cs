using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Droplet.Data;
using Droplet.Models.Entities;
using Droplet.Helpers;
using System.Drawing;
using Droplet.Models;

namespace Droplet.Controllers.AdminActions
{
    public class ManageDoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManageDoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ManageDoctors
        [Route("/AdminActions/ManageDoctors", Name = "doctorlist")]
        public async Task<IActionResult> Index()
        {
            return View("~/Views/AdminActions/ManageDoctors/Index.cshtml", await _context.Doctors.ToListAsync());
        }

        // GET: ManageDoctors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            var viewModel = new DoctorViewModel
            {
                Doctor = doctor,
                Hospitals = _context.Hospitals.ToList(),
                SelectedHospitalIds = doctor.Hospitals.Select(h => h.Id).ToList()
            };

            return View("~/Views/AdminActions/ManageDoctors/Details.cshtml", viewModel);
        }

        // GET: ManageDoctors/Create
        public IActionResult Create()
        {
            var viewModel = new DoctorViewModel
            {
                Hospitals = _context.Hospitals.ToList()
            };
            return View("~/Views/AdminActions/ManageDoctors/Create.cshtml", viewModel);
        }

        // POST: ManageDoctors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (PESELHelper.IsValidPESEL(viewModel.Doctor.PESEL))
                {
                    var existingDoctor = await _context.Doctors.FirstOrDefaultAsync(d => d.PESEL == viewModel.Doctor.PESEL);

                    if (existingDoctor == null)
                    {
                        if (viewModel.SelectedHospitalIds != null)
                        {
                            foreach (var hospitalId in viewModel.SelectedHospitalIds)
                            {
                                var hospital = await _context.Hospitals.FindAsync(hospitalId);
                                if (hospital != null)
                                {
                                    viewModel.Doctor.Hospitals.Add(hospital);
                                }
                            }
                        }

                        _context.Add(viewModel.Doctor);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("Doctor.PESEL", "A doctor with this PESEL already exists.");
                    }
                }
                else
                {
                    ModelState.AddModelError("Doctor.PESEL", "Invalid PESEL.");
                }
            }

            viewModel.Hospitals = _context.Hospitals.ToList();
            return View("~/Views/AdminActions/ManageDoctors/Create.cshtml", viewModel);
        }

        // GET: ManageDoctors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Hospitals)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            var viewModel = new DoctorViewModel
            {
                Doctor = doctor,
                Hospitals = _context.Hospitals.ToList(),
                SelectedHospitalIds = doctor.Hospitals.Select(h => h.Id).ToList()
            };

            return View("~/Views/AdminActions/ManageDoctors/Edit.cshtml", viewModel);
        }

        // POST: ManageDoctors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DoctorViewModel viewModel)
        {
            if (id != viewModel.Doctor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (PESELHelper.IsValidPESEL(viewModel.Doctor.PESEL))
                {
                    var existingDoctor = await _context.Doctors
                                                       .FirstOrDefaultAsync(d => d.PESEL == viewModel.Doctor.PESEL && d.Id != viewModel.Doctor.Id);

                    if (existingDoctor != null)
                    {
                        ModelState.AddModelError("Doctor.PESEL", "Another doctor with this PESEL already exists.");
                        viewModel.Hospitals = _context.Hospitals.ToList();
                        return View("~/Views/AdminActions/ManageDoctors/Edit.cshtml", viewModel);
                    }

                    try
                    {
                        var doctorToUpdate = await _context.Doctors
                                                           .Include(d => d.Hospitals)
                                                           .FirstOrDefaultAsync(d => d.Id == id);

                        if (doctorToUpdate != null)
                        {
                            doctorToUpdate.FirstName = viewModel.Doctor.FirstName;
                            doctorToUpdate.LastName = viewModel.Doctor.LastName;
                            doctorToUpdate.PESEL = viewModel.Doctor.PESEL;

                            // Update hospitals
                            doctorToUpdate.Hospitals.Clear();
                            if (viewModel.SelectedHospitalIds != null)
                            {
                                foreach (var hospitalId in viewModel.SelectedHospitalIds)
                                {
                                    var hospital = await _context.Hospitals.FindAsync(hospitalId);
                                    if (hospital != null)
                                    {
                                        doctorToUpdate.Hospitals.Add(hospital);
                                    }
                                }
                            }

                            _context.Update(doctorToUpdate);
                            await _context.SaveChangesAsync();
                        }
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!DoctorExists(viewModel.Doctor.Id))
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
                else
                {
                    ModelState.AddModelError("Doctor.PESEL", "Invalid PESEL.");
                }
            }

            viewModel.Hospitals = _context.Hospitals.ToList();
            return View("~/Views/AdminActions/ManageDoctors/Edit.cshtml", viewModel);
        }

        // GET: ManageDoctors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            var viewModel = new DoctorViewModel
            {
                Doctor = doctor,
                Hospitals = _context.Hospitals.ToList(),
                SelectedHospitalIds = doctor.Hospitals.Select(h => h.Id).ToList()
            };

            return View("~/Views/AdminActions/ManageDoctors/Delete.cshtml", viewModel);
        }

        // POST: ManageDoctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.Id == id);
        }
    }
}
