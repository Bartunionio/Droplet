using Droplet.Data;
using Droplet.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Droplet.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Droplet.Data.Enum;
using Microsoft.AspNetCore.Authorization;

namespace Droplet.Controllers.MenagersActions
{
    [Authorize(Roles = "Admin,Manager")]
    public class DonorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Donor
        [Route("/ManagerActions/ManageDonors", Name = "managedonors")]
        public async Task<IActionResult> Index()
        {
            var donors = await _context.Donors.ToListAsync();
            return View("~/Views/ManagerActions/ManageDonors/Index.cshtml", donors);
        }
        // GET: Donor/Add
        public IActionResult Add()
        {
            ViewBag.BloodType = BloodTypeEnum.O_Positive.ToSelectList();
            return View("~/Views/ManagerActions/ManageDonors/Add.cshtml");
        }

        // POST: Donor/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("FirstName,LastName,PESEL,BloodType")] Donor donor)
        {
            if (ModelState.IsValid)
            {
                if (PESELHelper.IsValidPESEL(donor.PESEL))
                {
                    var existingDonor = await _context.Donors.FirstOrDefaultAsync(d => d.PESEL == donor.PESEL);

                    if (existingDonor == null)
                    {
                        _context.Add(donor);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("PESEL", "A donor with this PESEL already exists.");
                    }
                    
                }
                else
                {
                    ModelState.AddModelError("PESEL", "Invalid PESEL.");
                }
            }
            ViewBag.BloodType = BloodTypeEnum.O_Positive.ToSelectList();
            return View("~/Views/ManagerActions/ManageDonors/Add.cshtml", donor);
        }



        // GET: Donor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donor = await _context.Donors.FindAsync(id);
            if (donor == null)
            {
                return NotFound();
            }
            ViewBag.BloodType = BloodTypeEnum.O_Positive.ToSelectList();
            return View("~/Views/ManagerActions/ManageDonors/Edit.cshtml", donor);
        }

        // POST: Donor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,PESEL,BloodType")] Donor donor)
        {
            if (id != donor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (PESELHelper.IsValidPESEL(donor.PESEL))
                {
                    var otherDonor = await _context.Donors
                                               .FirstOrDefaultAsync(d => d.PESEL == donor.PESEL && d.Id != donor.Id);

                    if (otherDonor != null)
                    {
                        ModelState.AddModelError("PESEL", "Another donor with this PESEL already exists.");
                        ViewBag.BloodType = BloodTypeEnum.O_Positive.ToSelectList();
                        return View("~/Views/ManagerActions/ManageDonors/Edit.cshtml", donor);
                    }

                    try
                    {
                        _context.Update(donor);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!DonorExists(donor.Id))
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
                    ModelState.AddModelError("PESEL", "Invalid PESEL.");
                }

            }
            ViewBag.BloodType = BloodTypeEnum.O_Positive.ToSelectList();
            return View("~/Views/ManagerActions/ManageDonors/Edit.cshtml", donor);
        }

        // GET: Donor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donor = await _context.Donors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (donor == null)
            {
                return NotFound();
            }
            return View("~/Views/ManagerActions/ManageDonors/Delete.cshtml", donor);
        }

        // POST: Donor/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        [Route("/ManagerActions/ManageDonors/DeleteConfirmed", Name = "donordeleteconfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var donor = await _context.Donors.FindAsync(id);
            if (donor == null)
            {
                return NotFound();
            }

            _context.Donors.Remove(donor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool DonorExists(int id)
        {
            return _context.Donors.Any(e => e.Id == id);
        }
    }

}
