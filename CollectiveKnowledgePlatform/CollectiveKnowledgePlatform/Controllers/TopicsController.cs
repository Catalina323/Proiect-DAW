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
using System.Text.RegularExpressions;

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
            
            var topics = from topic in db.Topics//.Include("Category")
                               .Where(t => t.CategoryId == id)
                               .OrderBy(t => t.Id)
                            select topic;

            ViewBag.CatId = id;

            //******* inceput MOTOR DE CAUTARE ***********
            var search = "";
            
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {

                // eliminam spatiile libere
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();

                // Cautare in articol (Title si Content)
                List<int> topicIds = db.Topics.Where
                (t => t.Title.Contains(search) || t.Text.Contains(search))
                .Select(a => a.Id).ToList();

                // Cautare in comentarii (Content)

                List<int> topicIdsOfCommentsWithSearchString =
                db.Comments.Where(c => c.Continut.Contains(search))
                .Select(c => (int)c.TopicId).ToList();

                // Se formeaza o singura lista formata din toate id-urile
                //selectate anterior
                List<int> mergedIds =
                topicIds.Union(topicIdsOfCommentsWithSearchString).ToList();
                // Lista articolelor care contin cuvantul cautat
                // fie in articol -> Title si Content
                // fie in comentarii -> Content
                topics = db.Topics.Where(topic =>
                mergedIds.Contains(topic.Id) && topic.CategoryId == id)
                .Include("Category")
                .Include("User")
                .OrderBy(a => a.Id);
            }
            ViewBag.SearchString = search;
            // AFISARE PAGINATA
            //{ ... implementarea se afla in sectiunea anterioara }


            //********* inceput AFISARE PAGINATA ***********

            int _perPage = 3;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"];
            }

            int totalItems = topics.Count();
            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            var paginatedTopics = topics.Skip(offset).Take(_perPage);

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
            ViewBag.Topics = paginatedTopics;

            //********* sfarsit AFISARE PAGINATA ********



            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Topics/Index/" + id + "?search="
                + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Topics/Index/" + id + "?page";
            }


            //********** sfarsit MOTOR DE CAUTARE ***********

            

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
        public IActionResult New(int? id)
        {
            ViewBag.CatId = id;
            Topic topic = new Topic();

            topic.CategoryId = id;

            return View(topic);
        }

     
        [HttpPost]
        [Authorize(Roles = "User, Moderator, Administrator")]
        public IActionResult New(int? CatId, Topic topic)
        {

            // preluam id-ul utiliz care posteaza topicul
            topic.UserId = _userManager.GetUserId(User);
            //topic.CategoryId = ViewBag.CatId;
            //Console.WriteLine("ajunge pe new cu post");
            topic.CategoryId = CatId;

            Topic topic2 = new Topic();
            topic2.UserId = topic.UserId;
            topic2.Text = topic.Text;
            topic2.CategoryId = topic.CategoryId;
            topic2.Title = topic.Title;

            if (ModelState.IsValid)
            {

                db.Topics.Add(topic2);
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
            var selectList = new List<SelectListItem>();

            var categories = from cat in db.Categories
                             select cat;

            foreach (var category in categories)
            {
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
