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

        int sortOrder = 0;

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
        public IActionResult Index(int? id, int? sortOrder = 0)
        {

            if(id == null)
                return NotFound();

            /*var topics = from topic in db.Topics//.Include("Category")
                               .Where(t => t.CategoryId == id)
                               .OrderBy(t => t.Id)
                            select topic;*/
            var topics = GetSortedTopics(id, sortOrder);

            ViewBag.CatId = id;

            //******* inceput MOTOR DE CAUTARE ***********
            var search = "";
            
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {

                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                
                List<int> topicIds = db.Topics.Where
                (t => t.Title.Contains(search) || t.Text.Contains(search))
                .Select(a => a.Id).ToList();

                List<int> topicIdsOfCommentsWithSearchString =
                db.Comments.Where(c => c.Continut.Contains(search))
                .Select(c => (int)c.TopicId).ToList();

                
                List<int> mergedIds =
                topicIds.Union(topicIdsOfCommentsWithSearchString).ToList();
                
                topics = db.Topics.Where(topic =>
                mergedIds.Contains(topic.Id) && topic.CategoryId == id)
                .Include("Category")
                .Include("User")
                .OrderBy(a => a.Id);
            }
            ViewBag.SearchString = search;
            

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

            //adaugare tipuri de sortari
            string[] sortari = {"Id", "Reactii", "Alfabetic" };
            ViewBag.Sortari = sortari;
            ViewBag.SortareSelectata = sortari[(int)sortOrder];

            ViewBag.CurrentUser = _userManager.GetUserId(User);
            SetAccessRights();
            return View();
        }


        //************SETARE INAINTE DE METODA INDEX*********\\



        //********* METODA  SHOW **********

        [Authorize(Roles = "User,Moderator,Administrator")]
        [AllowAnonymous]
        public IActionResult Show(int id)
        {
            Topic topic = db.Topics
                                   .Include("User")
                                   .Include("Comments")
                                   .Include("Comments.User")
                                   .Include("TopicLikes")
                                   .Where(t => t.Id == id)
                                   .First();

            SetAccessRights();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            int likes = 0;
            foreach (var like in topic.TopicLikes)
            {
                likes = likes + like.Type;
            }

            ViewBag.Likes = likes;

            SetLike(topic);
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
                    if((User.IsInRole("Administrator") || User.IsInRole("Moderator") && requestTopic.CategoryId != null))
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

        //**** LIKE ***** DISLIKE *******

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public IActionResult Like(int? id, string L)
        {
            
            if(L == "Like")
            {
                //cazul in care DAM  LIKE
                TopicLike like = new TopicLike();

                like.UserId = _userManager.GetUserId(User);
                like.TopicId = id;
                like.Type = 1;

                
                if (ModelState.IsValid)
                {
                    
                    db.TopicLikes.Add(like);
                    db.SaveChanges();
                    return Redirect("/Topics/Show/" + id);

                }
                else
                {
                     return Redirect("/Topics/Show/" + id);
                }

            }

            
            if(L == "Liked")
            {
                //cazul in care STERGEM  LIKE UL

                Topic topic = db.Topics.Include("TopicLikes")
                                .Where(t => t.Id == id)
                                .First();

                foreach(TopicLike like in topic.TopicLikes)
                {
                    if(like.UserId == _userManager.GetUserId(User) && like.Type == 1)
                    {
                        db.Remove(like);
                        db.SaveChanges();

                        TempData["message"] = "Like ul a fost sters";
                        TempData["messageType"] = "alert-success";
                        return Redirect("/Topics/Show/" + id);
                    }

                }
            }

            //pe ultimul return nu cred ca intra dar da eroare daca nu e pus
            //deoarece crede ca exista cazul in care nu returneaza nimic

            return Redirect("/Topics/Show/" + id);

        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public IActionResult Dislike(int? id, string L)
        {
            if (L == "Dislike")
            {
                //cazul in care DAM  DISLIKE
                TopicLike like = new TopicLike();

                like.UserId = _userManager.GetUserId(User);
                like.TopicId = id;
                like.Type = -1;

                if (ModelState.IsValid)
                {
                    db.TopicLikes.Add(like);
                    db.SaveChanges();
                    return Redirect("/Topics/Show/" + id);

                }
                else
                {
                    return Redirect("/Topics/Show/" + id);
                }

            }


            if (L == "Disliked")
            {
                //cazul in care STERGEM  DISLIKE UL

                Topic topic = db.Topics.Include("TopicLikes")
                                .Where(t => t.Id == id)
                                .First();

                foreach (TopicLike like in topic.TopicLikes)
                {
                    if (like.UserId == _userManager.GetUserId(User) && like.Type == -1)
                    {
                        db.Remove(like);
                        db.SaveChanges();

                        TempData["message"] = "Dislike ul a fost sters";
                        TempData["messageType"] = "alert-success";
                        return Redirect("/Topics/Show/" + id);
                    }
                }
            }

            //pe ultimul return nu cred ca intra dar da eroare daca nu e pus
            //deoarece crede ca exista cazul in care nu returneaza nimic

            return Redirect("/Topics/Show/" + id);
        }

        public IQueryable<Topic> GetSortedTopics(int? id, int? sortOrder)
        {
            if (sortOrder == 0)
            {
                //sortate dupa id
                var topics = from topic in db.Topics
                   .Where(t => t.CategoryId == id)
                   .OrderBy(t => t.Id)
                             select topic;
                return topics;
            }
            else if(sortOrder == 1)
            {
                //sortare dupa numarul de reactii
                var topics = from topic in db.Topics
                                .Include("TopicLikes")
                                .Where(t => t.CategoryId == id)
                                .OrderByDescending(t => t.TopicLikes.Count)
                                .ThenBy(t => t.Id) 
                                select topic;
                return topics;
            }
            else if(sortOrder == 2)
            {
                //sortate alfabetic dupa titlu
                var topics = from topic in db.Topics
                             .Where(t => t.CategoryId == id)
                             .OrderBy(t => t.Title)
                             .ThenBy(t => t.Id) select topic;
                return topics;
            }
            return from topic in db.Topics
                        .Where(t => t.CategoryId == id)
                         .OrderBy(t => t.Id)
                   select topic;

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

        private void SetLike(Topic topic)
        {
            //vrem sa existe 3 stari ale unui topic pentru like:
            // 1 - utiliz a dat like
            // 2 - utiliz a dat dislike
            // 3 - utiliz nu a reactionat

            ViewBag.Like = "Like";
            ViewBag.Dislike = "Dislike";
            ViewBag.Reaction = false;

            var UserId = _userManager.GetUserId(User);

            foreach( var l in topic.TopicLikes) 
            {
                if(l.UserId == UserId)
                {
                    if (l.Type == 1)
                    {
                        ViewBag.Like = "Liked";
                        ViewBag.Reaction = true;
                    }
                    else if (l.Type == -1)
                    {
                        ViewBag.Dislike = "Disliked";
                        ViewBag.Reaction = true;
                    }   
                }
            }
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
