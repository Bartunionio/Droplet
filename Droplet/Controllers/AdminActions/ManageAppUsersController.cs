using Droplet.Data;
using Droplet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Droplet.Controllers
{
    public class ManageAppUsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ManageAppUsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: ManageAppUsersController
        [Route("/AdminActions/ManageAppUsers", Name ="appuserlist")]
        public async Task<IActionResult> Index()
        {
            var usersWithRoles = new List<UserViewModel>();

            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault(); // Assuming each user has exactly one role

                usersWithRoles.Add(new UserViewModel
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Role = role ?? "No role assigned",
                });
            }

            usersWithRoles = usersWithRoles.OrderBy(u => u.Role).ThenBy(u => u.Id).ToList();

            return View("~/Views/AdminActions/ManageAppUsers/Index.cshtml", usersWithRoles);
        }

        // GET: ManageAppUsersController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Role = role ?? "No role assigned",
                EmailConfirmed = user.EmailConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                PhoneNumber = user.PhoneNumber
            };

            return View("~/Views/AdminActions/ManageAppUsers/Details.cshtml", userViewModel);
        }


        // GET: ManageAppUsersController/Edit/5
        [HttpGet]
        [Route("/AdminActions/ManageAppUsers/Edit/{id}", Name = "appuseredit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var currentRole = roles.FirstOrDefault();

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Role = currentRole ?? "No role assigned",
            };

            return View("~/Views/AdminActions/ManageAppUsers/Edit.cshtml", userViewModel);
        }

        // POST: ManageAppUsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/AdminActions/ManageAppUsers/Edit/{id}", Name = "appusereditpost")]
        public async Task<ActionResult> Edit(string id, UserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Get current roles
            var roles = await _userManager.GetRolesAsync(user);
            var currentRole = roles.FirstOrDefault();

            // Remove current role if exists
            if (!string.IsNullOrEmpty(currentRole))
            {
                var removeResult = await _userManager.RemoveFromRoleAsync(user, currentRole);
                if (!removeResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Failed to remove current role.");
                    return View("~/Views/AdminActions/ManageAppUsers/Edit.cshtml", model);
                }
            }

            // Assign new role if selected
            if (!string.IsNullOrEmpty(model.Role) && model.Role != "No role assigned")
            {
                var addResult = await _userManager.AddToRoleAsync(user, model.Role);
                if (!addResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Failed to add new role.");
                    return View("~/Views/AdminActions/ManageAppUsers/Edit.cshtml", model);
                }
            }



                Console.WriteLine("asd");
            return RedirectToAction(nameof(Index));
        }

        // GET: ManageAppUsersController/Delete/5
        [Route("/AdminActions/ManageAppUsers/Delete.cshtml", Name = "appuserdelete")]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Role = role ?? "No role assigned",
            };

            return View("~/Views/AdminActions/ManageAppUsers/Delete.cshtml", userViewModel);
        }

        // POST: ManageAppUsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/AdminActions/ManageAppUsers/DeleteConfirmed", Name = "appuserdeleteconfirmed")]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Console.WriteLine("asdas");
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("~/Views/AdminActions/ManageAppusers/Delete.cshtml", new UserViewModel { Id = id });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
