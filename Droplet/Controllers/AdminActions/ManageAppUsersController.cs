using Droplet.Data;
using Droplet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Droplet.Controllers
{
    [Authorize(Roles = "Admin")]
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
        [Route("/AdminActions/ManageAppUsers", Name = "appuserlist")]
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

            usersWithRoles = usersWithRoles.OrderBy(u => u.Role).ThenBy(u => u.Username).ToList();

            return View("~/Views/AdminActions/ManageAppUsers/Index.cshtml", usersWithRoles);
        }

        // GET: ManageAppUsersController/Details/5
        public async Task<ActionResult> Details(string username)
        {
            if (username == null)
            {
                return NotFound();
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username);
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
        [Route("/AdminActions/ManageAppUsers/Edit/{username}", Name = "appuseredit")]
        public async Task<ActionResult> Edit(string username)
        {
            if (username == null)
            {
                return NotFound();
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return NotFound();
            }
            // Forbid editing current user
            if (user == _userManager.GetUserAsync(User).Result)
            {
                return RedirectToAction(nameof(Index));
            }

            var roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Admin", Text = "Admin" },
                new SelectListItem { Value = "Manager", Text = "Manager" },
                new SelectListItem { Value = "User", Text = "User" }
            };

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "No role assigned",
                Roles = roles
            };

            return View("~/Views/AdminActions/ManageAppUsers/Edit.cshtml", userViewModel);
        }

        // POST: ManageAppUsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/AdminActions/ManageAppUsers/Edit/{username}", Name = "appusereditpost")]
        public async Task<ActionResult> Edit(string username, UserViewModel model)
        {
            if (username != model.Username)
            {
                return NotFound();
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return NotFound();
            }
            // Forbid editing current user
            if (user == _userManager.GetUserAsync(User).Result)
            {
                return RedirectToAction(nameof(Index));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var currentRole = roles.FirstOrDefault();

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
            return RedirectToAction(nameof(Index));
        }

        // GET: ManageAppUsersController/Delete/5
        [Route("/AdminActions/ManageAppUsers/Delete.cshtml", Name = "appuserdelete")]
        public async Task<ActionResult> Delete(string username)
        {
            if (username == null)
            {
                return NotFound();
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return NotFound();
            }

            // Forbid deleting current user
            if (user == _userManager.GetUserAsync(User).Result)
            {
                return RedirectToAction(nameof(Index));
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

            return View("~/Views/AdminActions/ManageAppUsers/Delete.cshtml", userViewModel);
        }

        // POST: ManageAppUsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/AdminActions/ManageAppUsers/DeleteConfirmed", Name = "appuserdeleteconfirmed")]
        public async Task<ActionResult> DeleteConfirmed(string username)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return NotFound();
            }
            // Forbid deleting current user
            if (user == _userManager.GetUserAsync(User).Result)
            {
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("~/Views/AdminActions/ManageAppusers/Delete.cshtml", new UserViewModel { Username = username });
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
