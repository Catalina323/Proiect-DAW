using CollectiveKnowledgePlatform.Data;
using CollectiveKnowledgePlatform.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

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

        //********** METODA  INDEX ********
        
        [Authorize(Roles = "User,Moderator,Administrator")]
        [AllowAnonymous]
        public IActionResult Index(int? id)
        {

            if(id == null)
                return NotFound();
            //var topics = db.Topics.Include("Topics").Include("User");

            //int? cat = ViewBag.CatId;
            //Console.WriteLine(cat.ToString());
            
            var topics = from topic in db.Topics//.Include("Category")
                               .Where(t => t.CategoryId == id)//CategoryId) 
                            select topic;

            ViewBag.Topics = topics;
            ViewBag.CatId = id;

            //Console.WriteLine(CategoryId.ToString());

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            SetAccessRights();
            return View();
        }

        //********* METODELE  SHOW **********

        [Authorize(Roles = "User,Moderator,Administrator")]
        [AllowAnonymous]
        public IActionResult Show(int id)
        {
            Topic topic = db.Topics
                                   .Include("User")
                                   .Include("Comments")
                                   .Include("Comments.User")
                                   .Where(t => t.Id == id)
                                   .First();

            SetAccessRights();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            SetAccessRights();
            return View(topic);
        }

        //********** METODE  NEW ***********

        [Authorize(Roles = "User, Moderator, Administrator")]
        public IActionResult New()
        {
            //ViewBag.CatId = cat;
            //Topic topic = new Topic();

            //topic.CategoryId = cat;

            return View();
        }

     
        [Authorize(Roles = "User, Moderator, Administrator")]
        [HttpPost]
        public IActionResult New(Topic topic)
        {

            // preluam id-ul utiliz care posteaza topicul
            topic.UserId = _userManager.GetUserId(User);
            //topic.CategoryId = ViewBag.CatId;
            //Console.WriteLine("ajunge pe new cu post");
            
            if (ModelState.IsValid)
            {
                db.Topics.Add(topic);
                db.SaveChanges();
                TempData["message"] = "Topicul a fost adaugat";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index", new { id = topic.CategoryId });
            }
            else
            {
                //topic.Categ = GetAllCategories();
                SetAccessRights();
                return View(topic);
            }
        }

        //********* METODELE  EDIT ******************

        [Authorize(Roles = "User, Moderator, Administrator")]
        public IActionResult Edit(int id)
        {

            Topic topic = db.Topics//.Include("Category")
                                        .Where(t => t.Id == id)
                                        .First();
            

            if (topic.UserId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
            {
                if(User.IsInRole("Administrator") || User.IsInRole("Moderator"))
                {
                    //daca e admin/moderator mai pun si o lista de categorii din care poate schimba adminul
                    var categorii = GetCategory();
                    ViewBag.Categories = categorii;
                    ViewBag.CurrentCategory = topic.Category;
                }

                SetAccessRights();
                return View(topic);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui topic care nu va apartine";
                TempData["messageType"] = "alert-danger";
                SetAccessRights();
                return RedirectToAction("Index", new { id = topic.CategoryId });
            }

        }


        [HttpPost]
        [Authorize(Roles = "User,Moderator, Administrator")]
        public IActionResult Edit(int id, Topic requestTopic)
        {
            Topic topic = db.Topics.Find(id);

            if (ModelState.IsValid)
            {
                if (topic.UserId == _userManager.GetUserId(User) || User.IsInRole("Administrator") || User.IsInRole("Moderator"))
                {
                    topic.Title = requestTopic.Title;
                    topic.Text = requestTopic.Text;
                    if(User.IsInRole("Administrator") || User.IsInRole("Moderator"))
                    {
                        topic.CategoryId = requestTopic.CategoryId;
                    }
                    //topic.CategoryId = requestTopic.CategoryId;
                    TempData["message"] = "Topicul a fost modificat";
                    TempData["messageType"] = "alert-success";
                    db.SaveChanges();
                    return RedirectToAction("Index", new {id = topic.CategoryId});
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", new {id = topic.CategoryId});
                }
            }
            else
            {
                SetAccessRights();
                return View(requestTopic);
            }
        }

        
        //********* METODA  DELETE ***************

        [HttpPost]
        [Authorize(Roles = "User, Moderator, Administrator")]
        public ActionResult Delete(int id)
        {
            //ASTA POATE PENTRU CAND O SA AVEM COMENTARII
            Topic topic = db.Topics.Include("Comments")
                                         .Where(art => art.Id == id)
                                        .First();

            //Topic topic = db.Topics.Find(id);

            if (topic.UserId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
            {
                db.Topics.Remove(topic);
                db.SaveChanges();
                TempData["message"] = "Topicul a fost sters";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index", new { id = topic.CategoryId });
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un topic care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", new { id = topic.CategoryId });
            }
        }

        //ADAUGARE COMENTARII LA UN TOPIC
        [HttpPost]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public IActionResult Show([FromForm] Comment comment)
        {

            comment.Date = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return Redirect("/Topics/Show/" + comment.TopicId);
            }
            else
            {
                Topic topic = db.Topics.Include("User")
                                        .Include("Comments").Include("Comments.User")
                                        .Where(t => t.Id == comment.TopicId).First();

                SetAccessRights();
                return View(topic);
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

        [NonAction]
        public IEnumerable<SelectListItem> GetCategory()
        {
            // generam o lista de tipul SelectListItem fara elemente
            var selectList = new List<SelectListItem>();

            // extragem toate categoriile din baza de date
            var categories = from cat in db.Categories
                             select cat;

            // iteram prin categorii
            foreach (var category in categories)
            {
                // adaugam in lista elementele necesare pentru dropdown
                // id-ul categoriei si denumirea acesteia
                selectList.Add(new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.Name.ToString()
                });
            }
            
            return selectList;
        }

    }
}
