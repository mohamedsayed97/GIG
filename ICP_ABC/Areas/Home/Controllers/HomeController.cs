using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ICP_ABC.Areas.Home.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
       // private ApplicationDbContext dbContext = new ApplicationDbContext();
        //static bool Mode;

        public ActionResult Index()
        {
            ViewBag.Message = "";
            //    var currentuser = User.Identity.Name;
            //    Mode = dbContext.Users.Where(s => s.UserName == currentuser).Select(s => s.DarkMode).FirstOrDefault();
            //    ViewData["color"] = true;
            return View();
        }
        public ActionResult IndexFromCalender()
        {
            ViewBag.Message = "This Day Is Vacation Day.";
            //    var currentuser = User.Identity.Name;
            //    Mode = dbContext.Users.Where(s => s.UserName == currentuser).Select(s => s.DarkMode).FirstOrDefault();
            //    ViewData["color"] = true;
            return View("Index");
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}