using CollectiveKnowledgePlatform.Data;
using CollectiveKnowledgePlatform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollectiveKnowledgePlatform.Controllers
{
    public class TopicsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TopicsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
    
        }

        //********** METODA  INDEX *********

        [Authorize(Roles = "User,Moderator,Administrator")]
        public IActionResult Index(int CatId)
        {
            //var topics = db.Topics.Include("Topics").Include("User");

            var topics = from topic in db.Topics.Include("Category")
                               .Where(t => t.CategoryId == CatId) 
                            select topic;

            ViewBag.Topics = topics;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View();
        }

        //********* METODELE  SHOW **********

        [Authorize(Roles = "User,Moderator,Administrator")]
        public IActionResult Show(int id)
        {
            Topic topic = db.Topics.Include("Category")
                                         .Include("User")
                                         .Include("Comment")
                                         .Include("Comment.User")
                                         .Where(t => t.Id == id)
                                         .First();

            SetAccessRights();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View(topic);
        }

        //********** METODE  NEW ***********

        [Authorize(Roles = "User, Moderator, Administrator")]
        public IActionResult New()
        {
            //Topic topic = new Topic();
            // Se preia lista de categorii cu ajutorul metodei GetAllCategories()
            //topic.Categ = GetAllCategories();
            
            //pt CODUL COMENTAT MAI SUS idee preluata dar vreau sa modific
            //revin la ea daca nu merge cum vreau eu

            return View();
        }

     
        [Authorize(Roles = "User, Moderator, Administrator")]
        [HttpPost]
        public IActionResult New(Topic topic)
        {

            // preluam id-ul utiliz care posteaza topicul
            topic.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Topics.Add(topic);
                db.SaveChanges();
                TempData["message"] = "Topicul a fost adaugat";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                //topic.Categ = GetAllCategories();
                return View(topic);
            }
        }

        //********* METODELE  EDIT ******************

        [Authorize(Roles = "User, Moderator, Administrator")]
        public IActionResult Edit(int id)
        {

            Topic topic = db.Topics.Include("Category")
                                        .Where(t => t.Id == id)
                                        .First();

            //topic.Categ = GetAllCategories();

            if (topic.UserId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
            {
                return View(topic);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui topic care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

        }


        [HttpPost]
        [Authorize(Roles = "User,Moderator, Administrator")]
        public IActionResult Edit(int id, Topic requestTopic)
        {
            Topic topic = db.Topics.Find(id);


            if (ModelState.IsValid)
            {
                if (topic.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    topic.Title = requestTopic.Title;
                    topic.Text = requestTopic.Text;
                    //topic.CategoryId = requestTopic.CategoryId;
                    TempData["message"] = "Articolul a fost modificat";
                    TempData["messageType"] = "alert-success";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                //requestTopic.Categ = GetAllCategories();
                return View(requestTopic);
            }
        }

        
        //********* METODA  DELETE ***************

        [HttpPost]
        [Authorize(Roles = "User, Moderator, Administrator")]
        public ActionResult Delete(int id)
        {
            //ASTA POATE PENTRU CAND O SA AVEM COMENTARII
            //Topic topic = db.Topics.Include("Comments")
            //                             .Where(art => art.Id == id)
            //                             .First();

            Topic topic = db.Topics.Find(id);

            if (topic.UserId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
            {
                db.Topics.Remove(topic);
                db.SaveChanges();
                TempData["message"] = "Articolul a fost sters";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un topic care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }


        //**********************************************
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
