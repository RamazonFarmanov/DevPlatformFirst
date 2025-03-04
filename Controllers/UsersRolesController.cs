using DevPlatform.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevPlatform.Controllers
{
    public class UsersRolesController : Controller
    {
        private UserManager<IdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        public UsersRolesController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        [HttpGet]
        public IActionResult Roles()
        {
            return View(roleManager.Roles.ToList());
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            return RedirectToAction("Roles");
        }
        [HttpGet]
        public async Task<IActionResult> DeleteRole(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            if (role != null) 
            {
                await roleManager.DeleteAsync(role);
            }
            else
            {
                return NotFound();
            }
            return RedirectToAction("Roles");
        }
        [HttpGet]
        public async Task<IActionResult> EditUserRole(string id)
        {
            IdentityUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                var allRoles = roleManager.Roles.ToList();
                var userRole = await userManager.GetRolesAsync(user);
                UserRoleViewModel model = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserName = user.UserName,
                    AllRoles = allRoles,
                    UserRoles = userRole
                };
                return View(model);
            }
            return RedirectToAction("Users");
        }
        [HttpPost]
        public async Task<IActionResult> EditUserRole(string userId, List<string> roles)
        {
            IdentityUser user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {

                var userRoles = await userManager.GetRolesAsync(user);

                var allRoles = roleManager.Roles.ToList();

                var addedRoles = roles.Except(userRoles);

                var removedRoles = userRoles.Except(roles);

                await userManager.AddToRolesAsync(user, addedRoles);

                await userManager.RemoveFromRolesAsync(user, removedRoles);

                return RedirectToAction("Users");
            }

            return NotFound();
        }
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            UserRoleListViewModel users = new UserRoleListViewModel();
            List<UserRoleViewModel> users_list = new List<UserRoleViewModel>();
            foreach (var user in userManager.Users.ToList())
            {
                var roles = await userManager.GetRolesAsync(user);
                users_list.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserEmail = user.Email,
                    UserRoles = roles,
                    AllRoles = roleManager.Roles.ToList()
                });
            }
            users.Users = users_list;
            return View(users);
        }
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserCreationViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    user = new IdentityUser { Email = model.Email, UserName = model.Name };
                    var result = await userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Users");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", $"This email is already used by {user.UserName}");
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            IdentityUser user = await userManager.FindByIdAsync(id);
            if(user != null)
            {
                return View(new EditUserViewModel { Id = user.Id, Email = user.Email, Name = user.UserName });
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Name;
                    var result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        ModelState.AddModelError("", "User has been changed successfully!");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    var passwordValidator = HttpContext.RequestServices.GetService(typeof(IPasswordValidator<IdentityUser>)) as IPasswordValidator<IdentityUser>;
                    var passwordHasher = HttpContext.RequestServices.GetService(typeof(IPasswordHasher<IdentityUser>)) as IPasswordHasher<IdentityUser>;

                    IdentityResult result = await passwordValidator.ValidateAsync(userManager, user, model.Password);
                    if (result.Succeeded)
                    {
                        user.PasswordHash = passwordHasher.HashPassword(user, model.Password);
                        await userManager.UpdateAsync(user);
                        ModelState.AddModelError("", "Password has been changed successfully");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("Password", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User is not found");
                }
            }
            return View("EditUser", model);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUsers(UserRoleListViewModel model)
        {
            var toDelete = model.Users.Where(u => u.Selected).ToList();
            foreach (var user in toDelete)
            {
                var exists = await userManager.FindByEmailAsync(user.UserEmail);
                if (exists != null)
                {
                    await userManager.DeleteAsync(exists);
                }
            }
            return RedirectToAction("Users");
        } 
    }
}
