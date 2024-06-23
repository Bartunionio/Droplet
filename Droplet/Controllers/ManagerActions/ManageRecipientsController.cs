using Droplet.Data;
using Droplet.Helpers;
using Droplet.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Droplet.Controllers.ManagersActions
{
    public class RecipientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecipientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Recipient
        [Route("/ManagerActions/ManageRecipients", Name = "manage_recipients")]
        public async Task<IActionResult> Index()
        {
            var recipients = await _context.Recipients.ToListAsync();
            return View("~/Views/ManagerActions/ManageRecipients/Index.cshtml", recipients);
        }

        // GET: Recipient/Add
        public IActionResult Add()
        {
            return View("~/Views/ManagerActions/ManageRecipients/Add.cshtml");
        }

        // POST: Recipient/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("FirstName,LastName,PESEL,BloodType")] Recipient recipient)
        {
            if (ModelState.IsValid)
            {
                if (PESELHelper.IsValidPESEL(recipient.PESEL))
                {
                    var existingDonor = await _context.Recipients.FirstOrDefaultAsync(r => r.PESEL == recipient.PESEL);

                    if (existingDonor == null)
                    {
                        _context.Add(recipient);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("PESEL", "A recipient with this PESEL already exists.");
                    }

                }
                else
                {
                    ModelState.AddModelError("PESEL", "Invalid PESEL.");
                }
            }
            return View("~/Views/ManagerActions/ManageRecipients/Add.cshtml", recipient);
        }

        // GET: Recipient/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipient = await _context.Recipients.FindAsync(id);
            if (recipient == null)
            {
                return NotFound();
            }
            return View("~/Views/ManagerActions/ManageRecipients/Edit.cshtml", recipient);
        }

        // POST: Recipient/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,PESEL,BloodType")] Recipient recipient)
        {
            if (id != recipient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (PESELHelper.IsValidPESEL(recipient.PESEL))
                {

                    var otherRecipient = await _context.Recipients
                                                    .FirstOrDefaultAsync(r => r.PESEL == recipient.PESEL && r.Id != recipient.Id);

                    if (otherRecipient != null)
                    {
                        ModelState.AddModelError("PESEL", "Another recipient with this PESEL already exists.");
                        return View("~/Views/ManagerActions/ManageRecipients/Edit.cshtml", recipient);
                    }

                    try
                    {
                        _context.Update(recipient);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!RecipientExists(recipient.Id))
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
            return View("~/Views/ManagerActions/ManageRecipients/Edit.cshtml", recipient);
        }

        // GET: Recipient/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipient = await _context.Recipients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipient == null)
            {
                return NotFound();
            }

            return View("~/Views/ManagerActions/ManageRecipients/Delete.cshtml", recipient);
        }

        // POST: Recipient/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        [Route("/ManagerActions/ManageRecipients/DeleteConfirmed", Name = "recipient_delete_confirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipient = await _context.Recipients.FindAsync(id);
            if (recipient == null)
            {
                return NotFound();
            }

            _context.Recipients.Remove(recipient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipientExists(int id)
        {
            return _context.Recipients.Any(e => e.Id == id);
        }
    }
}
