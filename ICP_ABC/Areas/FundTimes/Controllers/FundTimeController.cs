using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Areas.FundTimes.Models;
using ICP_ABC.Areas.UsersSecurity.Models;
using ICP_ABC.Extentions;
using ICP_ABC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using Rotativa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ICP_ABC.Areas.FundTimes.Controllers
{
    [Authorize]
    public class FundTimeController : Controller
    {
        private ApplicationDbContext dbContext = new ApplicationDbContext();

        static int[] IDs;
        // GET: FundTimes/FundTime
        
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [AuthorizedRights(Screen = "Fund Time", Right = "Create")]
        public ActionResult Create()
        {
            
            var funds = User.Identity.UserFunds();
            //var x = dbContext.Funds.Where(f => funds.Select(s=>s.FundID).Contains(f.FundID))
            //ViewData["Funds"] = new SelectList(funds, "FundID","Name");
            ViewData["Funds"] = new SelectList(dbContext.Funds.Where(x=>x.HasFundTime == false).ToList(), "FundID", "Name");

            
            return View();
        }
        [HttpPost]
        [AuthorizedRights(Screen = "Fund Time", Right = "Create")]
        public ActionResult Create(CreateViewModel model)
        {
            // var FundTimes = dbContext.FundTimes.Where(f=>f.FundId);
            //var hasTime = dbContext.Funds.Where(f => f.FundID == model.FundId).FirstOrDefault().HasFundTime;

            //if (hasTime)
            //{
            //    SignInStatus result = new SignInStatus();
            //    result = SignInStatus.LockedOut;
            //    string[] errors = new string[] { "Time Is already Assigned to this fund" };
            //    IdentityResult LoginResult = new IdentityResult(errors);
            //    AddErrors(LoginResult);
            //    var funds = User.Identity.UserFunds();
            //    //var x = dbContext.Funds.Where(f => funds.Select(s=>s.FundID).Contains(f.FundID))
            //    ViewData["Funds"] = new SelectList(funds, "FundID", "Name");
            //    return View();
            //}
            var Level = dbContext.UserSecurities.Select(Val => Val.Levels).FirstOrDefault();
            if (Level == Levels.TwoLevels)
            {
                var time = Convert.ToDateTime(model.Time);
                FundTime fundTime = new FundTime
                {
                    FundId = model.FundId,
                    Time = time.TimeOfDay,
                    Maker = User.Identity.GetUserId(),
                    Checker = User.Identity.GetUserId(),
                    EditFlag = false,
                    Chk = true,
                    Auth = false,
                    DeletFlag = DeleteFlag.NotDeleted
                };
                var fund = dbContext.Funds.Where(f => f.FundID == model.FundId).FirstOrDefault();
               
                fund.HasFundTime = true;
                dbContext.FundTimes.Add(fundTime);
                dbContext.SaveChanges();
                HttpContext.Session.Add("Funds", null);
               
                return RedirectToAction("Details", new { Id = fundTime.FundTimeID });
            }
            else
            {
                var time = Convert.ToDateTime(model.Time);
                FundTime fundTime = new FundTime
                {
                    FundId = model.FundId,
                    Time = time.TimeOfDay,
                    Maker = User.Identity.GetUserId(),
                    EditFlag = false,
                    Chk = false,
                    Auth = false,
                    DeletFlag = DeleteFlag.NotDeleted
                };
                var fund = dbContext.Funds.Where(f => f.FundID == model.FundId).FirstOrDefault();
                fund.HasFundTime = true;
                dbContext.FundTimes.Add(fundTime);
                dbContext.SaveChanges();
          
                HttpContext.Session.Add("Funds", null);
                return RedirectToAction("Details", new { Id = fundTime.FundTimeID });
            }
            //var time = Convert.ToDateTime(model.Time);
            //FundTime fundTime = new FundTime
            //{
            //    FundId = model.FundId,
            //    Time =time.TimeOfDay,
            //    Maker = User.Identity.GetUserId(),
            //    EditFlag = false,
            //    Chk = false,
            //    Auth = false,
            //    DeletFlag= DeleteFlag.NotDeleted
            //};
            //var fund = dbContext.Funds.Where(f => f.FundID == model.FundId).FirstOrDefault();
            //fund.HasFundTime = true;
            //dbContext.FundTimes.Add(fundTime);
            //dbContext.SaveChanges();
            //HttpContext.Session.Add("Funds", null);
            //return RedirectToAction("Details", new { Id = model.FundTimeID });
           
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [AuthorizedRights(Screen = "Fund Time", Right = "Update")]
        public ActionResult Edit(int Id)
        {
            var CurrentFundTime = dbContext.FundTimes
                   .Where(g => g.FundTimeID == Id)
                   .FirstOrDefault();
            CreateViewModel model = new CreateViewModel
            {
                FundTimeID =CurrentFundTime.FundTimeID,
                FundId = CurrentFundTime.FundId,
                Time = CurrentFundTime.Time.ToString()
            };
            ViewData["Funds"] = new SelectList(dbContext.Funds.Where(f=>f.FundID==CurrentFundTime.FundId).ToList(), "FundID", "Name");
            ViewData["Time"] = CurrentFundTime.Time.ToString();
            return View(model);
        }
        [HttpPost]
        [AuthorizedRights(Screen = "Fund Time", Right = "Update")]
        public ActionResult Edit(CreateViewModel model)
        {
            var time = Convert.ToDateTime(model.Time);
            var CurrentFundTime = dbContext.FundTimes.Where(g => g.FundId == model.FundId).FirstOrDefault();
            CurrentFundTime.Time = time.TimeOfDay;
            
            CurrentFundTime.EditFlag = true;
            CurrentFundTime.UserID = User.Identity.GetUserId();
            dbContext.SaveChanges();
            //return RedirectToAction("Search");
            return RedirectToAction("Details", new { Id = model.FundTimeID });
        }

        [AuthorizedRights(Screen = "Fund Time", Right = "Read")]
        public ViewResult Search(string sortOrder, string currentFilter, string searchString, int? page, string Code, string RadioCHeck)
        {
            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;

            if (page == null && sortOrder == null && currentFilter == null && searchString == null && Code == null && RadioCHeck == null)
            {
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                var FundTimes = dbContext.FundTimes.Where(c => c.DeletFlag == DeleteFlag.NotDeleted).Take(0);

                return View(FundTimes.ToPagedList(pageNumber, pageSize));

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

                var FundTimes = dbContext.FundTimes.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);
                if (!String.IsNullOrEmpty(Code))
                {
                    FundTimes = FundTimes.Where(s => s.Fund.Code.Contains(Code));
                    ViewData["Code"] = Code;

                }
                //checkRadio 
                if (!String.IsNullOrEmpty(RadioCHeck))
                {
                    //Case Auth
                    if (RadioCHeck == "1")
                    {
                        FundTimes = FundTimes.Where(s => s.Auth == true);

                    }
                    //Case Checker
                    if (RadioCHeck == "2")
                    {
                        FundTimes = FundTimes.Where(s => s.Chk == true && s.Auth == false);

                    }
                    //Case maker
                    if (RadioCHeck == "3")
                    {
                        FundTimes = FundTimes.Where(s => s.Auth == false && s.Chk == false);

                    }
                    //Case All
                    if (RadioCHeck == "4")
                    {
                        FundTimes = FundTimes.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);

                    }


                }
                //

                if (!String.IsNullOrEmpty(searchString))
                {
                    FundTimes = FundTimes.Where(s => s.Fund.Name.Contains(searchString));
                    ViewData["searchString"] = searchString;
                }

                TempData["FundTimeForExc"] = FundTimes.Select(x => x.FundTimeID).ToList();
                switch (sortOrder)
                {
                    case "name_desc":
                        FundTimes = FundTimes.OrderByDescending(s => s.Fund.Name);
                        break;

                    case "code_desc":
                        FundTimes = FundTimes.OrderByDescending(s => s.Time);
                        break;
                    default:  // Name ascending 
                        //FundTimes = FundTimes.OrderBy(s => s.Time);
                        FundTimes = FundTimes.OrderBy(s => s.FundTimeID);
                        break;
                }
                //int count = FundTimes.ToList().Count();
                //IDs = new string[count];
                //IDs = FundTimes.Select(s => s.Code).ToArray();

                int pageSize = 4;
                int pageNumber = (page ?? 1);
                ViewData["radioCHeck"] = RadioCHeck;
                int count = FundTimes.ToList().Count();
                IDs = new int[count];
                IDs = FundTimes.Select(s => s.FundTimeID).ToArray();
                Array.Sort(IDs);
                return View(FundTimes.ToPagedList(pageNumber, pageSize)); 
            }
        }

        [AuthorizedRights(Screen = "FundSetup", Right = "Read")]
        public ViewResult Details(int Id)
        {
            ViewData["Funds"] = new SelectList(dbContext.Funds.ToList(), "FundID", "Name");
            var Model = dbContext.FundTimes.Where(c => c.FundTimeID == Id).FirstOrDefault();

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
            CreateViewModel CurrentFundTime = new CreateViewModel();
            CurrentFundTime.FundTimeID = Model.FundTimeID;
            CurrentFundTime.FundId = Model.FundId;
            CurrentFundTime.Auth = Model.Auth;
            CurrentFundTime.Check = Model.Chk;
            CurrentFundTime.AuthForEditAndDelete = Actualy_auth;
            CurrentFundTime.Time = Model.Time.ToString();
            if (IDs != null)
            {
                if (Model.FundTimeID == IDs.Last())
                {
                    TempData["Last"] = "Last";
                }
                if (Model.FundTimeID == IDs.First())
                {
                    TempData["First"] = "First";
                }
            }
            else
            {
                TempData["Last"] = "Last";
                TempData["First"] = "First";
            }


            return View(CurrentFundTime);

        }

        [AuthorizedRights(Screen = "FundSetup", Right = "Read")]
        public ActionResult Next(int id)
        {
            var arrlenth = IDs.Count();
            var LastObj = dbContext.FundTimes.OrderBy(s => s.FundTimeID).ToList().LastOrDefault(s => s.FundTimeID == IDs[arrlenth - 1]);
            if (id == LastObj.FundTimeID)
            {
                TempData["Last"] = "Last";
                return RedirectToAction("Details", new { Id = LastObj.FundTimeID });
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
                var Code = dbContext.FundTimes.Where(u => u.FundTimeID == id).Select(s => s.FundTimeID).FirstOrDefault();
                if (Code == LastObj.FundTimeID)
                {
                    TempData["Last"] = "Last";
                }
                else
                {
                    TempData["Last"] = "NotLast";
                }


                return RedirectToAction("Details", new { Id = Code });
            }

        }
        [AuthorizedRights(Screen = "FundSetup", Right = "Read")]
        public ActionResult Previous(int id)
        {
            var arrindex = IDs[0];
            var FirstObj = dbContext.FundTimes.OrderBy(s => s.FundTimeID).FirstOrDefault(s => s.FundTimeID == arrindex);
            if (id == FirstObj.FundTimeID)
            {
                TempData["First"] = "First";
                return RedirectToAction("Details", new { Id = FirstObj.FundTimeID });
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

                if (id == FirstObj.FundTimeID)
                {
                    TempData["First"] = "First";
                }
                else
                {
                    TempData["First"] = "NotFirst";
                }
                var Code = dbContext.FundTimes.Where(u => u.FundTimeID == id).Select(s => s.FundTimeID).FirstOrDefault();
                return RedirectToAction("Details", new { Id = Code });
            }

        }
        [AuthorizedRights(Screen = "Fund Time", Right = "Read")]
        public ActionResult ExportToExcel()
        {
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var gv = new GridView();


            var List = (List<int>)TempData["FundTimeForExc"];
            gv.DataSource = dbContext.FundTimes.Where(Del => List.Contains(Del.FundTimeID) && Del.DeletFlag == DeleteFlag.NotDeleted).OrderBy(or=>or.FundTimeID).Select(x => new {FundName = x.Fund.Name, x.Time }). ToList();



            //gv.DataSource = (from s in dbContext.FundTimes
            //                 select new
            //                 {
            //                     s.Fund.Name,
            //                     s.Time


            //                 })
            //                     .ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=FundTime" + NowTime + ".xls");
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
        [AuthorizedRights(Screen = "Fund Time", Right = "Read")]
        public ActionResult ExportToPDF()
        {
            var FundTimes = dbContext.FundTimes.Select(s => s).Where(s => IDs.Contains(s.FundTimeID)).OrderBy(u => u.Time).ToList();
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");

            var report = new PartialViewAsPdf("~/Areas/FundTimes/Views/FundTime/ExportToPDF.cshtml", FundTimes);
            return report;

        }
        [AuthorizedRights(Screen = "Fund Time", Right = "Authorized")]
        public ActionResult AuthorizeFundTime(int Id)
        {
            var Fund = dbContext.FundTimes.Where(f => f.FundTimeID == Id).FirstOrDefault();
            Fund.Auth = true;
            Fund.Auther = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details",new { Id = Id });
        }
        [AuthorizedRights(Screen = "Fund Time", Right = "Check")]
        public ActionResult CheckFundTime(int Id)
        {
            var Fund = dbContext.FundTimes.Where(f => f.FundTimeID == Id).FirstOrDefault();
            Fund.Chk = true;
            Fund.Checker = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Id = Id });

        }


        public ActionResult Delete(int Code)
        {
            var fund = dbContext.FundTimes.Where(f => f.FundTimeID == Code).FirstOrDefault();
            fund.DeletFlag = DeleteFlag.Deleted;
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}