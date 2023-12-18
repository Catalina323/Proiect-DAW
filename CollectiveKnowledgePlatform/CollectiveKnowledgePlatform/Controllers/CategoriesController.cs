using CollectiveKnowledgePlatform.Data;
using CollectiveKnowledgePlatform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CollectiveKnowledgePlatform.Controllers
{
    public class CategoriesController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CategoriesController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            var categories = from category in db.Categories
                             orderby category.Name
                             select category;
            ViewBag.Categories = categories;
            SetAccessRights();
            return View();
        }

        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult Show(int id)
        {
            Category category = db.Categories.Find(id);
            SetAccessRights();
            return View(category);
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Moderator,Administrator")]
        public ActionResult New(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                TempData["message"] = "Categoria a fost adaugata cu succes";
                Console.WriteLine("categoria a fost adaugata in baza de date");
                return RedirectToAction("Index");
            }
            else
            {
                Console.WriteLine("categoria a esuat");
                return View(category);
            }
        }

        [Authorize(Roles = "Moderator,Administrator")]
        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
            return View(category);
        }

        [HttpPost]
        [Authorize(Roles = "Moderator,Administrator")]
        public ActionResult Edit(int id, Category requestCategory) 
        {
            Category category = db.Categories.Find(id);
            if(ModelState.IsValid)
            {
                category.Name = requestCategory.Name;
                category.Description = requestCategory.Description;
                db.SaveChanges();
                TempData["Message"] = "Categoria a fost modificata!";
                return RedirectToAction("Index");
            }
            else
            {
                return View(requestCategory);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Moderator,Administrator")]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            TempData["message"] = "Categoria a fost stearsa";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("Moderator") || User.IsInRole("Administrator"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.EsteAdmin = User.IsInRole("Administrator");
            ViewBag.EsteModerator = User.IsInRole("Moderator");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

    }
}
