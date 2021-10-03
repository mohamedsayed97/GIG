using ICP_ABC.Areas.Calendars.Models;
using ICP_ABC.Areas.UsersSecurity.Models;
using ICP_ABC.Extentions;
using ICP_ABC.Models;
//using Microsoft.Ajax.Utilities;
using Microsoft.Ajax.Utilities;
using Microsoft.ApplicationBlocks.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Calendar = ICP_ABC.Areas.Calendars.Models.Calendar;

namespace ICP_ABC.Areas.Calendars.Controllers
{
    [Authorize]
    
    public class CalendarController : Controller
    {


        private ApplicationDbContext dbContext = new ApplicationDbContext();
        UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(new ApplicationDbContext());
        static string[] IDs;

        public CalendarController() { }

        //GetLastCode
        public string GetLastCode()
        {
            var querey = "SELECT MAX(CAST(Code AS INT)) as MaxCode FROM Calendar";
            var appSettings = ConfigurationManager.ConnectionStrings;
            var SqlCon = appSettings["ICPRO"];
            var ReturnedData = SqlHelper.ExecuteDataset(SqlCon.ToString(), CommandType.Text, querey);
            var LastCode = ReturnedData.Tables[0].Rows[0][0].ToString();
            return LastCode;
        }

        //Calendar c = new Calendar();

        // GET: Group
        [AuthorizedRights(Screen = "Calendar", Right = "Read")]
        public ActionResult Index()
        {
            return View();
        }
        [AuthorizedRights(Screen = "Calendar", Right = "Create")]
        public ActionResult Create()
        {
            //var LastCode = dbContext.Calendars.OrderByDescending(s => s.CalendarID).Select(s => s.Code).FirstOrDefault();
            var LastCode = GetLastCode();
            var TryingToParse = int.TryParse(LastCode, out int Code);
            if (TryingToParse)
            {
                ViewData["LastCode"] = Code + 1;
            }
            else
            {
                ViewData["LastCode"] = 1;
            }
            return View();
        }

        [HttpPost]
        [AuthorizedRights(Screen = "Calendar", Right = "Create")]
        public ActionResult Create(CreateViewModel model)
        {
            
            var isValid = dbContext.Calendars.Where(f => f.Code == model.Code).FirstOrDefault();
            if (isValid != null)
            {
                string[] errors = new string[] { "Code Is Already Exist" };
                IdentityResult result = new IdentityResult(errors);
                AddErrors(result);
                return View(model);
            }

            var Level = dbContext.UserSecurities.Select(Val => Val.Levels).FirstOrDefault();
            
            if (Level == Levels.TwoLevels)
            {
                if (ModelState.IsValid)
                {

                    var calendar = new Calendar
                    {

                        Code = model.Code,
                        Vacation_Name = model.Vacation_Name,
                        Vacation_date = model.Vacation_date,
                        Maker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        Checker = User.Identity.GetUserId(),
                        Chk = true,//dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        UserID = User.Identity.GetUserId()// dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                    };
                    dbContext.Calendars.Add(calendar);
                    dbContext.SaveChanges();
                }
                else
                {
                    return View(model);
                }
                // return View();.
                return RedirectToAction("Details", new { Code = model.Code.ToString() });
                //return RedirectToAction("Search");
            }
            else
            {
                if (ModelState.IsValid)
                {

                    var calendar = new Calendar
                    {

                        Code = model.Code,
                        Vacation_Name = model.Vacation_Name,
                        Vacation_date = model.Vacation_date,
                        Maker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        UserID = User.Identity.GetUserId()// dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                    };
                    dbContext.Calendars.Add(calendar);
                    dbContext.SaveChanges();
                }
                else
                {
                    return View(model);
                }
                // return View();.
                return RedirectToAction("Details", new { Code = model.Code.ToString() });
                //return RedirectToAction("Search");
            }


            if (ModelState.IsValid)
            {

                var calendar = new Calendar
                {
                    
                    Code = model.Code,
                    Vacation_Name = model.Vacation_Name,
                    Vacation_date = model.Vacation_date,
                    Maker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                    UserID = User.Identity.GetUserId()// dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                };
                dbContext.Calendars.Add(calendar);
                dbContext.SaveChanges();
            }
            else
            {
                return View(model);
            }
            // return View();.
            return RedirectToAction("Details", new { Code = model.Code.ToString() });
            //return RedirectToAction("Search");
        }

        [AuthorizedRights(Screen = "Calendar", Right = "Update")]
        public ActionResult Edit(string Code)
        {
            var currentCalendar = dbContext.Calendars
                   .Where(g => g.Code == Code)
                   .FirstOrDefault();
            EditViewModel model = new EditViewModel
            {
                Code = currentCalendar.Code,
                Vacation_Name = currentCalendar.Vacation_Name,
                Vacation_date = currentCalendar.Vacation_date
            };
            return View(model);
        }
        [HttpPost]
        [AuthorizedRights(Screen = "Calendar", Right = "Update")]
        public ActionResult Edit(EditViewModel model)
        {
            var currentCalendar = dbContext.Calendars.Where(g => g.Code == model.Code).FirstOrDefault();
            currentCalendar.Vacation_Name = model.Vacation_Name;
            currentCalendar.Vacation_date = model.Vacation_date;
            currentCalendar.SysDate = DateTime.Now;
            currentCalendar.EditFlag = true;
            currentCalendar.UserID = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = model.Code.ToString() });
        }

        [AuthorizedRights(Screen = "Calendar", Right = "Read")]
        public ViewResult Search(string sortOrder, string currentFilter, string searchString, int? page, string Code,string searchDate, string RadioCHeck)
        {
            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;
            if (page == null && sortOrder == null && currentFilter == null && searchString == null && Code == null && searchDate == null && RadioCHeck == null)
            {
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                var Calendar = dbContext.Calendars.Where(c => c.DeletFlag == DeleteFlag.NotDeleted).Take(0);

                return View(Calendar.ToPagedList(pageNumber, pageSize));

            }
            else
            {



                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.DateSortParm = sortOrder == "Code" ? "code_desc" : "Code";

                //if (searchString != null)
                //{
                //    page = 1;
                //}
                //else
                //{
                //    searchString = currentFilter;
                //}

                ViewBag.CurrentFilter = searchString;

                var Calendar = dbContext.Calendars.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);
                if (!String.IsNullOrEmpty(searchString))
                {
                    Calendar = Calendar.Where(s => s.Vacation_Name.StartsWith(searchString));
                    ViewData["searchString"] = searchString;
                }

                //checkRadio 
                if (!String.IsNullOrEmpty(RadioCHeck))
                {
                    //Case Auth
                    if (RadioCHeck == "1")
                    {
                        Calendar = Calendar.Where(s => s.Auth == true);

                    }
                    //Case Checker
                    if (RadioCHeck == "2")
                    {
                        Calendar = Calendar.Where(s => s.Chk == true && s.Auth == false);

                    }
                    //Case maker
                    if (RadioCHeck == "3")
                    {
                        Calendar = Calendar.Where(s => s.Auth == false && s.Chk == false);

                    }
                    //Case All
                    if (RadioCHeck == "4")
                    {
                        Calendar = Calendar.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);

                    }


                }
                //

                if (!String.IsNullOrEmpty(searchDate))
                {
                    var changedatatype = Convert.ToDateTime(searchDate);
                    Calendar = Calendar.Where(s => s.Vacation_date == changedatatype);
                    ViewData["searchDate"] = searchDate;
                }

                if (!String.IsNullOrEmpty(Code))
                {
                    Calendar = Calendar.Where(s => s.Code.StartsWith(Code));
                    ViewData["Code"] = Code;
                }

                TempData["CalendarForExc"] = Calendar.Select(x => x.Code).ToList();

                switch (sortOrder)
                {
                    case "name_desc":
                        Calendar = Calendar.OrderByDescending(s => s.Vacation_Name);
                        break;
                    case "Code":
                        Calendar = Calendar.OrderBy(s => s.Code);
                        break;
                    case "code_desc":
                        Calendar = Calendar.OrderByDescending(s => s.Code);
                        break;
                    default:  // Name ascending 
                        Calendar = Calendar.OrderBy(s => s.CalendarID);
                        break;
                }
                //int count = Calendar.ToList().Count();
                //IDs = new string[count];
                //IDs = Calendar.Select(s => s.Code).ToArray();

                int pageSize = 4;
                int pageNumber = (page ?? 1);
                ViewData["radioCHeck"] = RadioCHeck;
                int count = Calendar.ToList().Count();
                IDs = new string[count];
                IDs = Calendar.Select(s => s.Code).ToArray();

                int[] myInts = Array.ConvertAll(IDs, s => int.Parse(s));
                Array.Sort(myInts);
                IDs = myInts.Select(x => x.ToString()).ToArray();

                return View(Calendar.ToPagedList(pageNumber, pageSize));
            }
        }

        [AuthorizedRights(Screen = "Calendar", Right = "Read")]
        public ViewResult Details(string Code)
        {


            var Model = dbContext.Calendars.Where(c => c.Code == Code).FirstOrDefault();
            var Actualy_auth = Model.Auth;
            var ThisUser = User.Identity.GetUserId();

            if (Model.Maker == ThisUser)
            {
                if (Model.Chk == true && Model.Auth == false)
                {
                    Model.Chk = true;
                    Model.Auth = false;
                }
                if (Model.Chk == false && Model.Auth == false)
                {
                    Model.Chk = true;
                    Model.Auth = true;

                }
            }
            else
            {
                if (Model.Chk == true && Model.Auth == true)
                {
                    Model.Chk = true;
                    Model.Auth = true;

                }

                else if (Model.Chk == true && Model.Auth == false)
                {
                    if (Model.Checker != ThisUser)
                    {
                        Model.Chk = true;
                        Model.Auth = false;

                    }
                    else
                    {
                        Model.Chk = true;
                        Model.Auth = true;

                    }
                }
                else if (Model.Chk == false && Model.Auth == false)
                {
                    Model.Chk = false;
                    Model.Auth = true;

                }

            }



            DetailsViewModel CurrentCalendar = new DetailsViewModel
            {
                Code = Model.Code,
                Vacation_Name = Model.Vacation_Name,
                Vacation_date = Model.Vacation_date,
                Auth = Model.Auth,
                Check = Model.Chk,
                AuthForEditAndDelete= Actualy_auth


            };
            if (IDs != null )
            {
                if (Model.Code == IDs.Last())
                {
                    TempData["Last"] = "Last";
                }
                if (Model.Code == IDs.First())
                {
                    TempData["First"] = "First";
                }
            }
            else
            {
                TempData["Last"] = "Last";
                TempData["First"] = "First";
            }
            return View(CurrentCalendar);

        }

        [AuthorizedRights(Screen = "Calendar", Right = "Read")]
        public ActionResult Next(string id)
        {
            var arrlenth = IDs.Count();
            var LastObj = dbContext.Calendars.OrderBy(s => s.Code).ToList().LastOrDefault(s => s.Code == IDs[arrlenth - 1]);
            if (id == LastObj.Code)
            {
                TempData["Last"] = "Last";
                return RedirectToAction("Details", new { Code = LastObj.Code });
            }
            else
            {
                int counter = 0;
                int next = 0;
                foreach (var item in IDs)
                {

                    if (item == id)
                    {
                        next = counter + 1;

                    }
                    counter++;
                }
                id = IDs[next];
                var Code = dbContext.Calendars.Where(u => u.Code == id).Select(s => s.Code).FirstOrDefault();
                if (Code == LastObj.Code)
                {
                    TempData["Last"] = "Last";
                }
                else
                {
                    TempData["Last"] = "NotLast";
                }
                return RedirectToAction("Details", new { Code = Code });
            }

        }
        [AuthorizedRights(Screen = "Calendar", Right = "Read")]
        public ActionResult Previous(string id)
        {
            var arrindex = IDs[0];
            var FirstObj = dbContext.Calendars.OrderBy(s => s.Code).FirstOrDefault(s => s.Code == arrindex);
            if (id == FirstObj.Code)
            {
                TempData["First"] = "First";
                return RedirectToAction("Details", new { Code = FirstObj.Code });
            }
            else
            {
                int counter = 0;
                int previous = 0;
                foreach (var item in IDs)
                {

                    if (item == id)
                    {
                        previous = counter - 1;

                    }
                    counter++;
                }
                id = IDs[previous];
                if (id == FirstObj.Code)
                {
                    TempData["First"] = "First";
                }
                else
                {
                    TempData["First"] = "NotFirst";
                }
                var Code = dbContext.Calendars.Where(u => u.Code == id).Select(s => s.Code).FirstOrDefault();
                return RedirectToAction("Details", new { Code = Code });
            }

        }

        [AuthorizedRights(Screen = "Calendar", Right = "Read")]
        public ActionResult ExportToExcel()
        {
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var gv = new GridView();

            var List = (List<string>)TempData["CalendarForExc"];
            gv.DataSource = dbContext.Calendars.Where(Del => List.Contains(Del.Code) && Del.DeletFlag == DeleteFlag.NotDeleted).Select(x => new { x.Code, x.Vacation_Name, x.Vacation_date }).ToList();
            //gv.DataSource = (from s in dbContext.Calendars
            //                 select new
            //                 {
            //                     s.Code,
            //                     s.Vacation_Name


            //                 })
            //                     .ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Calendar" + NowTime + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.ContentEncoding = System.Text.Encoding.Unicode;
            Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

            Response.Charset = "UTF-8";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("Search");
        }
        [AuthorizedRights(Screen = "Calendar", Right = "Read")]
        public ActionResult ExportToPDF()
        {
            var Calendar = dbContext.Calendars.Select(s => s).Where(s => IDs.Contains(s.Code)).OrderBy(u => u.Vacation_Name).ToList();
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");

            var report = new PartialViewAsPdf("~/Areas/Calendars/Views/Calendar/ExportToPDF.cshtml", Calendar);
            return report;

        }

        [AuthorizedRights(Screen = "Calendar", Right = "Authorized")]
        public ActionResult AuthorizeCalendar(string Code)
        {
            var Calendar = dbContext.Calendars.Where(f => f.Code == Code).FirstOrDefault();
            Calendar.Auth = true;
            Calendar.Auther = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = Code });
        }

        [AuthorizedRights(Screen = "Calendar", Right = "Check")]
        public ActionResult CheckCalendar(string Code)
        {
            var Calendar = dbContext.Calendars.Where(f => f.Code == Code).FirstOrDefault();
            Calendar.Chk = true;
            Calendar.Checker = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = Code });

        }

        [AuthorizedRights(Screen = "Calendar", Right = "Delete")]
        public ActionResult Delete(string Code)
        {
            var Calendar = dbContext.Calendars.Where(f => f.Code == Code).FirstOrDefault();
            Calendar.DeletFlag = DeleteFlag.Deleted;
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        [Route("ChekCalendar")]
        [HttpPost]
        public ActionResult ChekCalendar()
        {
            var ThisDay = DateTime.Today;
            var x = ThisDay.Day;
            var Calendar = dbContext.Calendars.Where(f => f.Vacation_date == ThisDay && f.DeletFlag== DeleteFlag.NotDeleted).ToList();
            if (Calendar.Count() > 0)
            {
                var CheckMaker = User.Identity.GetUserId();
                var VacationTransaction = dbContext.UserSecurities.Where(f => f.UserID == CheckMaker).FirstOrDefault();
                if(VacationTransaction != null)
                {
                    if(VacationTransaction.ViewTransaction == false && VacationTransaction.CreateTransaction == false)
                    {
                        return Json(new { result = 0 });
                    }else if (VacationTransaction.ViewTransaction == true && VacationTransaction.CreateTransaction == false)
                    {
                        return Json(new { result = 1 });
                    }
                    else if (VacationTransaction.ViewTransaction == false && VacationTransaction.CreateTransaction == true)
                    {
                        return Json(new { result = 2 });
                    }
                    else if (VacationTransaction.ViewTransaction == true && VacationTransaction.CreateTransaction == true)
                    {
                        return Json(new { result = 2 });
                    }
                }
                return Json(new { result = 1 });
                
            }

            return Json(new { NOtInCalendar= 1 });

        }

        
    }
}