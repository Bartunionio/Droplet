using Droplet.Data;
using Droplet.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Droplet.Controllers.MenagersActions
{
    public class DonorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Donor
        [Route("/ManagerActions/MenageDonors", Name = "menage_donors")]
        public async Task<IActionResult> Index()
        {
            var donors = await _context.Donors.ToListAsync();
            return View("~/Views/ManagerActions/MenageDonors/Index.cshtml", donors);
        }
        // GET: Donor/Add
        public IActionResult Add()
        {
            return View("~/Views/ManagerActions/MenageDonors/Add.cshtml");
        }

        // POST: Donor/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("FirstName,LastName,PESEL,BloodType")] Donor donor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(donor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/ManagerActions/MenageDonors/Add.cshtml", donor);
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
            return View("~/Views/ManagerActions/MenageDonors/Edit.cshtml", donor);
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
            return View("~/Views/ManagerActions/MenageDonors/Edit.cshtml", donor);
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

            return View("~/Views/ManagerActions/MenageDonors/Delete.cshtml", donor);
        }

        // POST: Donor/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        [Route("/ManagerActions/MenageDonors/DeleteConfirmed", Name = "donordeleteconfirmed")]
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
