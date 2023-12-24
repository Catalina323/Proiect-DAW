using CollectiveKnowledgePlatform.Data;
using CollectiveKnowledgePlatform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CollectiveKnowledgePlatform.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Index()
        {


            var users = from user in db.Users
                        orderby user.UserName
                        select user;

            ViewBag.UsersList = users;
            return View();
        }


        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Edit(string id)
        {
            ApplicationUser user = db.Users.Find(id);

            var roluri = new List<SelectListItem>();

            var roles = from role in db.Roles
                        select role;

            foreach(var role in roles)
            {
                roluri.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }

            Console.WriteLine(roluri);

            //user.AllRoles = GetAllRoles();

            var roleNames = await _userManager.GetRolesAsync(user); 

            var currentUserRole = _roleManager.Roles
                                              .Where(r => roleNames.Contains(r.Name))
                                              .Select(r => r.Id)
                                              .First();
            ViewBag.UserRole = currentUserRole;
            ViewBag.RoleNames = roluri;

            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Edit(string id, ApplicationUser newData, [FromForm] string newRole)
        {
            ApplicationUser user = db.Users.Find(id);

            if (ModelState.IsValid)
            {
                user.UserName = newData.UserName;
                user.Email = newData.Email;

                var roles = db.Roles.ToList();

                foreach (var role in roles)
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name); //scot user din roluri anterioare
                }
                var roleName = await _roleManager.FindByIdAsync(newRole);//adaugam noul rol
                await _userManager.AddToRoleAsync(user, roleName.ToString());

                db.SaveChanges();

            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Show(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            var role = await _userManager.GetRolesAsync(user);

            ViewBag.Role = role;

            return View(user);
        }

        ///***********************DELETE*********************\\\
        ///LA DELETE ESTE MAI GREU, TREBUIE STERS IN CASCADA TOT\\\
        ///***********************DELETE*********************\\\


    }
}
