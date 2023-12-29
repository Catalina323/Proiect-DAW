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

            int _perPage = 3;
            ViewBag.CatId = id;

            //********* inceput afisare paginata ***********

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"];
            }

            // Fiind un numar variabil de articole, verificam de
            // fiecare data utilizand metoda Count()
            int totalItems = topics.Count();

            // Se preia pagina curenta din View-ul asociat
            // Numarul paginii este valoarea parametrului page din ruta
            // /Articles/Index?page=valoare
            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

           
            // Pentru prima pagina offsetul o sa fie zero
            // Pentru pagina 2 o sa fie 3
            // Asadar offsetul este egal cu numarul de articole
            //care au fost deja afisate pe paginile anterioare
            var offset = 0;
            // Se calculeaza offsetul in functie de numarul paginii la care suntem
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            // Se preiau articolele corespunzatoare pentru fiecare pagina
            // la care ne aflam in functie de offset
            var paginatedTopics = topics.Skip(offset).Take(_perPage);

            // Preluam numarul ultimei pagini
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
            // Trimitem articolele cu ajutorul unui ViewBag
            //catre View-ul corespunzator
            ViewBag.Topics = paginatedTopics;

            //********* sf afisare paginata ********


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
