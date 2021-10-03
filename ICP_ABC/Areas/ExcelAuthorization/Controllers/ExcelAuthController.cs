using ICP_ABC.Areas.ExcelAuthorization.Models;
using ICP_ABC.Areas.Policies.Models;
using ICP_ABC.Extentions;
using ICP_ABC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormCollection = Microsoft.AspNetCore.Http.FormCollection;

namespace ICP_ABC.Areas.ExcelAuthorization.Controllers
{

    public class ExcelAuthController : Controller
    {


        private readonly ApplicationDbContext dbContext;

        public ExcelAuthController()
        {
            dbContext = new ApplicationDbContext();
        }
        // GET: ExcelAuthorization/ExcelAuth
        public ActionResult Index()
        {
            return View();
        }

        // GET: ExcelAuthorization/ExcelAuth/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public String getscreenname(int type)
        {
            String Screen = "";
            switch (type)
            {
                case 1:
                    Screen = "Addition";
                    break;
                case 2:
                    Screen = "Contribution";
                    break;
                case 3:
                    Screen = "Withdrawal";
                    break;
                case 4:
                    Screen = "Surrender";
                    break;
                case 5:
                    Screen = "Modification";
                    break;
            }
            return Screen;
        }

        public Boolean CheckRole(int type, string action)
        {
            if (type == 0 || type == null)
                return false;
            var user = HttpContext.User;
            String Screen = getscreenname(type);
            if (Screen == "" || Screen == null)
                return false;
            if (user.Identity.HasTheRights(Screen, "Read"))
                return true;

            return false;
        }



        [Authorize]
        public ViewResult Search(int type, string currentFilter, DateTime? date, string searchString, int? page, string Code, string RadioCHeck)
        {
            ViewData["Type"] = type;
            ViewData["Typename"] = getscreenname(type);
            if (CheckRole(type, "Read"))
            {
                var TransData = dbContext.ExcelDetails
                                         .Where(t => t.Screen == type);

                ViewBag.CurrentFilter = searchString;
                if (!String.IsNullOrEmpty(searchString))
                {
                    TransData = TransData.Where(s => s.Name.StartsWith(searchString));
                    ViewData["searchString"] = searchString;

                }

                //checkRadio 
                if (!String.IsNullOrEmpty(RadioCHeck))
                {
                    //change to status pending deleted authed
                    //Case Auth
                    if (RadioCHeck == "1")
                    {
                        TransData = TransData.Where(s => s.Auth == true);
                    }
                    //Case Checker
                    if (RadioCHeck == "2")
                    {
                        byte pending = (byte)ExcelStatus.Pending;
                        TransData = TransData.Where(s => s.Status == pending);
                    }
                    //Case maker
                    if (RadioCHeck == "3")
                    {
                        byte delete = (byte)ExcelStatus.Deleted;
                        TransData = TransData.Where(s => s.Status == delete);
                        var r = TransData.ToList();
                    }
                    //Case All
                    if (RadioCHeck == "4")
                    {
                        TransData = dbContext.ExcelDetails.Where(t => t.Screen == type);
                    }
                }
                //

                if (!String.IsNullOrEmpty(Code))
                {
                    TransData = TransData.Where(s => s.Id.ToString().StartsWith(Code));
                    ViewData["Code"] = Code;
                }
                if (date != null)
                {
                    //TransData = TransData.Where(s => s.uploadDate.Day == date.Value.Day && s.uploadDate.Month == date.Value.Month && s.uploadDate.Year == date.Value.Year);
                    var data = date.Value.Date;
                    TransData = TransData.Where(s => DbFunctions.TruncateTime(s.uploadDate) <= data);
                    ViewData["date"] = date;
                }
                //TempData["TransForExc"] = TransData.Select(x => x.Id).ToList();

                int pageSize = 4;
                int pageNumber = (page ?? 1);

                int count = TransData.Count();
                ViewData["radioCHeck"] = RadioCHeck;
                return View(TransData.OrderBy(p => p.Id).ToPagedList(pageNumber, pageSize));

                 }
                 //return RedirectToAction("Index", "Home", new { area = "Home" });
                 return View("~/Areas/Home/Views/Home/Index.cshtml");
            }

        [Authorize]
        public Boolean AuthorizeExcel(int Code, int type)
        {
            if (CheckRole(type, "Authorized"))
            {
                var Excel = dbContext.ExcelDetails.Where(f => f.Id == Code).FirstOrDefault();
                Excel.Auth = true;
                Excel.Auther = User.Identity.GetUserId();
                Excel.Status = (byte)ExcelStatus.Approved;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        [Authorize]
        public Boolean UnAuthorizeExcel(int Code, int type)
        {
            if (CheckRole(type, "Authorized"))
            {
                var Excel = dbContext.ExcelDetails.Where(f => f.Id == Code).FirstOrDefault();
                if(Excel==null)
                    return false;
                Excel.Auth = true;
                Excel.Auther = User.Identity.GetUserId();
                Excel.Status = (byte)ExcelStatus.Pending;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public ActionResult DownloadExcel(int id)
        {

            byte[] file = dbContext.ExcelDetails.FirstOrDefault(F => F.Id == id).FileContent;
            if (file == null || file.Length < 0)
                return HttpNotFound();
            var data = dbContext.ExcelDetails.FirstOrDefault(F => F.Id == id);
            Response.ContentType = data.ContentType;
            Response.AddHeader("content-disposition", "attachment;filename=Excel.xlsx");
            Response.BinaryWrite(file);
            Response.End();
            return RedirectToAction("Search");

        }


        public ActionResult Delete(int id, int type)
        {
            if (CheckRole(type, "Delete"))
            {
                var data = dbContext.ExcelDetails.FirstOrDefault(x => x.Id == id);
                if (data == null)
                    return HttpNotFound();
                if (data.Auth == true)
                    return RedirectToAction("search", "ExcelAuth", new { area = "ExcelAuthorization", Type = type });
                data.Status = (byte)ExcelStatus.Deleted;
                dbContext.SaveChanges();
                return RedirectToAction("search", "ExcelAuth", new { area = "ExcelAuthorization", Type = type });
            }
            return RedirectToAction("Index", "Home", new { area = "Home" });
        }

    }
}
