using CollectiveKnowledgePlatform.Data;
using CollectiveKnowledgePlatform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CollectiveKnowledgePlatform.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public IActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);

            if(comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Moderator") || User.IsInRole("Administrator"))
            {
                db.Comments.Remove(comm);
                db.SaveChanges();
                return Redirect("/Topics/Show/" + comm.TopicId);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti acest comentariu";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Topics/Show/" + comm.TopicId);
            }
        }

        [Authorize(Roles = "User,Moderator,Administrator")]
        public IActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);

            if(comm.UserId == _userManager.GetUserId(User))
            {
                return View(comm);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati comentariul";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Topics/Show/" + comm.TopicId);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public IActionResult Edit(int id, Comment requestComment)
        {
            Comment comm = db.Comments.Find(id);

            if(comm.UserId ==  _userManager.GetUserId(User))
            {
                if (ModelState.IsValid)
                {
                    comm.Continut = requestComment.Continut;
                    //comm.Date = requestComment.Date;
                    comm.Date = DateTime.Now;
                    db.SaveChanges();
                    return Redirect("/Topics/Show/" + comm.TopicId);
                }
                else
                {
                    Console.WriteLine("++++++++++++++++++");
                    return View(requestComment);
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati acest comentariu";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Topics/Show/" + comm.TopicId);
            }
        }
    }
}
