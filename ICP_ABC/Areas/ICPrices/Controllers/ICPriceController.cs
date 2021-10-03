using ICP_ABC.Extentions;
using ICP_ABC.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Areas.ICPrices.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using ICP_ABC.Areas.Funds.Models;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using Rotativa;

using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.Ajax.Utilities;
using System.Configuration;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using ICP_ABC.Areas.UsersSecurity.Models;

namespace ICP_ABC.Areas.ICPrices.Controllers
{
    public class ICPriceController : Controller
    {

           public ICPriceController()
            {
           // var x = User.Identity.UserFunds();
            //var Funds = User.Identity.UserFunds().Where(f => dbContext.ICPrices.Where(i => i.Date < DateTime.Today).Select(i => i.FundId).ToList().Contains(f.FundID)).ToList();
            //foreach (var item in Funds)
            //{
            //    item.HasICPrice = false;
            //}
            //dbContext.SaveChanges();
        }
       
            private ApplicationDbContext dbContext = new ApplicationDbContext();
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            static string[] IDs;
            //List<Fund> Funds = new List<Fund>(); 

            public ActionResult Index()
            {


            ViewData["Funds"] = new SelectList(dbContext.Funds.Where(f=>f.HasICPrice==false).ToList(), "FundID", "Name");
            return View();
            }
        public string GetLastCode()
        {
            var querey = "SELECT MAX(CAST(Code AS INT)) as MaxCode FROM ICPrice";
            var appSettings = ConfigurationManager.ConnectionStrings;
            var SqlCon = appSettings["ICPRO"];
            var ReturnedData = SqlHelper.ExecuteDataset(SqlCon.ToString(), CommandType.Text, querey);
            var LastCode = ReturnedData.Tables[0].Rows[0][0].ToString();
            return LastCode;
        }
        [AuthorizedRights(Screen = "ICPrice", Right = "Create")]
            public ActionResult Create()
             {
            //var lastcode = dbContext.ICPrices.OrderByDescending(s => s.Code ).Select(s => s.Code).FirstOrDefault();
            //var MaxCode = dbContext.ICPrices.Last();
            //var lastcode = dbContext.ICPrices.Where(M => M.DeletFlag == DeleteFlag.NotDeleted).OrderByDescending(s => s.ICPriceID).Select(s => s.Code).FirstOrDefault();
            var lastcode = GetLastCode();
       
            if (lastcode == null)
            {
                lastcode = "0";

            }
            //var lastcode = MaxCode.Code;

            var trieng = int.TryParse(lastcode.ToString(), out int Code);
            //    var Fundsasda = User.Identity.UserFunds();
            //var ThisUser = User.Identity.GetUserId();
            //ViewData["Funds"] = new SelectList(User.Identity.UserFunds().Where(d => d.HasFundTime== true).Where(x=>x.DeletFlag== DeleteFlag.NotDeleted).ToList(), "FundID", "Name");
             var currentFunds = new List<Fund>();
             var UserId = User.Identity.GetUserId();
             var user = dbContext.Users.Where(u => u.Id == UserId).FirstOrDefault();

             currentFunds = dbContext.Funds.Where(f => dbContext.FundRights
                  .Where(fr => fr.GroupID == dbContext.UserGroups
                  .Where(g => g.GroupID == user.GroupId).FirstOrDefault().GroupID)
                  .Select(h => h.FundID).ToList().Contains(f.FundID)).ToList();

            //currentFunds = dbContext.Funds.Where(f => dbContext.FundRights
            //    .Where(fr => fr.GroupID == dbContext.UserGroups
            //    .Where(g => g.GroupID == user.GroupId).FirstOrDefault().GroupID)
            //    .Select(h => h.FundID).ToList().Contains(f.FundID)).Where(d => d.HasFundTime == true).Where(x => x.DeletFlag == DeleteFlag.NotDeleted).ToList();
            //   ViewData["Funds"] = new SelectList(currentFunds, "FundID", "Name");
            ViewData["Funds"] = new SelectList(currentFunds.Where(x=>x.DeletFlag== DeleteFlag.NotDeleted).ToList(), "FundID", "Name");
            //ViewData["Funds"] = new SelectList(User.Identity.UserFunds().Where(d => d.HasFundTime== true).Where(x=>x.DeletFlag== DeleteFlag.NotDeleted).ToList(), "FundID", "Name");
                if (trieng)
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
            [AuthorizedRights(Screen = "ICPrice", Right = "Create")]
            public ActionResult Create(CreateViewModel model)
            {
                    //var lastcode = dbContext.ICPrices.Where(M=>M.DeletFlag == DeleteFlag.NotDeleted).OrderByDescending(s => s.ICPriceID).Select(s => s.Code).FirstOrDefault();
                var lastcode = GetLastCode();
                    //var lastcode1 = dbContext.ICPrices.Max(u => u.ICPriceID);
                     //var lastcode = dbContext.ICPrices.Where(u => u.ICPriceID == lastcode1).Select(X => X.Code).FirstOrDefault();
                      var trieng = int.TryParse(lastcode, out int Code);
                    if (trieng)
                    {   
                        lastcode = (Code + 1).ToString();
                        ViewData["LastCode"] = Code + 1;
                    }
                    else
                    {
                        lastcode = 1.ToString();
                        ViewData["LastCode"] = 1;
                    }
                   
                    //var x = dbContext.ICPrices.Where(ic => ic.FundId == model.FundId).OrderByDescending(i => i.Date).FirstOrDefault();
                    //var y = x.Date.Day;
                     var today = DateTime.Today.Date;
                    var LastDate = dbContext.ICPrices.Where(ic => ic.FundId == model.FundId && ic.Date >= today && ic.DeletFlag == DeleteFlag.NotDeleted).OrderByDescending(i => i.Date).FirstOrDefault();
                    if (LastDate != null)
                    {
                        var Fundsasda = User.Identity.UserFunds();
                        ViewData["Funds"] = new SelectList(User.Identity.UserFunds(), "FundID", "Name");
                        string[] errors = new string[] { "Price Already Made" };
                        IdentityResult result = new IdentityResult(errors);
                        AddErrors(result);
                        return View();
                    }
                   
            var Level = dbContext.UserSecurities.Select(Val => Val.Levels).FirstOrDefault();
            if (Level == Levels.TwoLevels)
            {


       
                ICPrice iCPrice = new ICPrice
                {
                    Code = lastcode,
                    FundId = model.FundId,
                    Date = model.Date,
                    ProcessingDate = model.ProcessingDate,
                    Chk = true,
                    Auth = false,
                    EditFlag = false,
                    DeletFlag = DeleteFlag.NotDeleted,
                    Price = model.Price,
                    SysDate = DateTime.Now,
                    Maker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                    Checker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                    Auther = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                    UserID = User.Identity.GetUserId()// dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                };
                var fund = dbContext.Funds.Where(f => f.FundID == model.FundId && f.DeletFlag == DeleteFlag.NotDeleted).FirstOrDefault();
                fund.HasICPrice = true;
                dbContext.ICPrices.Add(iCPrice);
                try
                {
                    dbContext.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                           raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }
                return RedirectToAction("Details", new { Code = model.Code });


            }
            else
            {
                        var Date = DateTime.Now;

                        ICPrice iCPrice = new ICPrice
                        {
                            Code = lastcode,
                            FundId = model.FundId,
                            Date = model.Date,
                            ProcessingDate = model.ProcessingDate,
                            Chk = false,
                            Auth = false,
                            EditFlag = false,
                            DeletFlag = DeleteFlag.NotDeleted,
                            Price = model.Price,
                            SysDate = DateTime.Now,
                            Maker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                            Auther = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                            UserID = User.Identity.GetUserId()// dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        };
                        dbContext.ICPrices.Add(iCPrice);
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                        {
                            Exception raise = dbEx;
                            foreach (var validationErrors in dbEx.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    string message = string.Format("{0}:{1}",
                                        validationErrors.Entry.Entity.ToString(),
                                        validationError.ErrorMessage);
                                    // raise a new exception nesting
                                    // the current instance as InnerException
                                    raise = new InvalidOperationException(message, raise);
                                }
                            }
                            throw raise;
                        }
                        return RedirectToAction("Details", new { Code = model.Code });


             

            }
            
        }

            //[AuthorizedRights(Screen = "ICPrice", Right = "Update")]
            public ActionResult Edit(string Code)
            {
            var currentICPrice = dbContext.ICPrices.Where(g => g.Code == Code && g.DeletFlag== DeleteFlag.NotDeleted).FirstOrDefault();
            var fundtime = dbContext.FundTimes.Where(f => f.FundId == currentICPrice.FundId && f.DeletFlag == DeleteFlag.NotDeleted).FirstOrDefault();
            ViewData["Funds"] = new SelectList(dbContext.Funds.Where(f => f.FundID == currentICPrice.FundId && f.DeletFlag == DeleteFlag.NotDeleted).ToList(), "FundID", "Name");
            if (fundtime.Time > DateTime.Now.TimeOfDay)
            {
                var Date = DateTime.Now;
                CreateViewModel model = new CreateViewModel
                {
                    Code = currentICPrice.Code,
                    FundId = currentICPrice.FundId,
                    Date = Date,
                    ProcessingDate = currentICPrice.ProcessingDate,
                    Price = currentICPrice.Price
                };

                return View(model);

            }
            else
            {
                var Date = currentICPrice.Date;
                    
                    //var DateAfterCheck = ChekCalendar(Date.AddDays(1));
                    //var Date = DateTime.Now.AddDays(1);
                    CreateViewModel model = new CreateViewModel
                    {
                        Code = currentICPrice.Code,
                        FundId = currentICPrice.FundId,
                        Date = Date,
                        ProcessingDate = currentICPrice.ProcessingDate,
                        Price = currentICPrice.Price
                    };
                    return View(model);
                
                
              
            }
        }
            [HttpPost]
            [AuthorizedRights(Screen = "ICPrice", Right = "Update")]
            public ActionResult Edit(EditViewModel model)
            {
            var lastcode = dbContext.ICPrices.Where(x=>x.DeletFlag == DeleteFlag.NotDeleted).OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault();

            var currentICPrice = dbContext.ICPrices.Where(g => g.Code == model.Code && g.DeletFlag == DeleteFlag.NotDeleted).FirstOrDefault();
           

                var CHeckDate = model.Date.AddDays(1);
                var DateAfterCheck = ChekCalendar(CHeckDate);

                currentICPrice.Price = model.Price;
                currentICPrice.Date = model.Date;
                currentICPrice.ProcessingDate = model.Date;
            
                currentICPrice.SysDate = DateTime.Now;
                currentICPrice.EditFlag = true;
                dbContext.SaveChanges();
                

                
            return RedirectToAction("Details", new { Code = model.Code });
        }

            [AuthorizedRights(Screen = "ICPrice", Right = "Read")]
            public ViewResult Search(string sortOrder, string currentFilter, string searchString,string Funds, int? page, string Code, string RadioCHeck)
            {
            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            
            ViewBag.SecuLevel = seculevel;
            if (page ==null&&sortOrder == null && currentFilter == null && searchString == null && Funds == null && Code == null && RadioCHeck == null)
            {
                ViewData["Funds"] = new SelectList(dbContext.Funds.Where(F => F.DeletFlag == DeleteFlag.NotDeleted && F.HasICPrice == true).ToList(), "FundID", "Name");
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                //var Users = dbContext.Users.ToList().Take(0);
                var icprice = dbContext.ICPrices.Where(X=>X.DeletFlag == DeleteFlag.NotDeleted).ToList().Take(0);
                //ViewData["Groups"] = new SelectList(dbContext.UserGroups.ToList(), "GroupID", "Name");
                //ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "BranchID", "Name");
                return View(icprice.ToPagedList(pageNumber, pageSize));

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
                var ICPrices = dbContext.ICPrices.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);
            if (!String.IsNullOrEmpty(Funds))
                {
                    var Fundid = int.Parse(Funds);
                    ICPrices = ICPrices.Where(s => s.Fund.FundID == Fundid);

                //ViewData["searchString"] = searchString;
                //ViewData["Funds"] = Funds;
            }

                //checkRadio 
                if (!String.IsNullOrEmpty(RadioCHeck))
                {
                    //Case Auth
                    if (RadioCHeck == "1")
                    {
                        ICPrices = ICPrices.Where(s => s.Auth == true);

                    }
                    //Case Checker
                    if (RadioCHeck == "2")
                    {
                        ICPrices = ICPrices.Where(s => s.Chk == true && s.Auth == false);

                    }
                    //Case maker
                    if (RadioCHeck == "3")
                    {
                        ICPrices = ICPrices.Where(s => s.Auth == false && s.Chk == false);

                    }
                    //Case All
                    if (RadioCHeck == "4")
                    {
                        ICPrices = ICPrices.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);

                    }


                }
                //

                if (!String.IsNullOrEmpty(Code))
                {
                    ICPrices = ICPrices.Where(s => s.Code.Contains(Code));
                 ViewData["Code"] = Code;
            }
                TempData["ICpriceForExc"] = ICPrices.Select(x => x.ICPriceID).ToList();

                switch (sortOrder)
                {
                    case "name_desc":
                        ICPrices = ICPrices.OrderByDescending(s => s.Fund.Name);
                        break;
                    case "Code":
                        ICPrices = ICPrices.OrderBy(s => s.Code);
                        break;
                    case "code_desc":
                        ICPrices = ICPrices.OrderByDescending(s => s.Code);
                        break;
                    default:  // Name ascending 
                        //ICPrices = ICPrices.OrderBy(s => s.Fund.Name);
                        ICPrices = ICPrices.OrderBy(s => s.ICPriceID);
                        break;
                }
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                int count = dbContext.ICPrices.Where(x=>x.DeletFlag == DeleteFlag.NotDeleted).ToList().Count();
                IDs = new string[count];
                ViewData["radioCHeck"] = RadioCHeck;
                //IDs = ICPrices.Select(s => s.Code).ToArray();
                IDs = dbContext.ICPrices.Where(x => x.DeletFlag == DeleteFlag.NotDeleted).ToList().Select(s => s.Code).ToArray();
                int[] myInts = Array.ConvertAll(IDs, s => int.Parse(s));
                Array.Sort(myInts);
                IDs = myInts.Select(x => x.ToString()).ToArray();
                ViewData["Funds"] = new SelectList(dbContext.Funds.Where(F => F.DeletFlag == DeleteFlag.NotDeleted && F.HasICPrice == true).ToList(), "FundID", "Name"); 
                return View(ICPrices.ToPagedList(pageNumber, pageSize));
                }
            }

            [AuthorizedRights(Screen = "ICPrice", Right = "Read")]
            public ViewResult Details(string Code)
            {
                var Model = dbContext.ICPrices.Where(c => c.Code == Code).FirstOrDefault();
                var CheckSub = dbContext.Subscriptions.Where(c => c.NAV == Model.Price && c.ProcessingDate==Model.ProcessingDate && c.fund_id==Model.FundId && c.nav_date==Model.Date && c.delreason == DeleteFlag.NotDeleted).FirstOrDefault();
                var CheckRed = dbContext.Redemptions.Where(c => c.NAV == Model.Price && c.ProcessingDate==Model.ProcessingDate &&  c.fund_id==Model.FundId && c.nav_date==Model.Date && c.delreason == DeleteFlag.NotDeleted).FirstOrDefault();
            var CheckTrans = dbContext.Trans.Where(c => c.unit_price == Model.Price && c.ProcessingDate==Model.ProcessingDate && c.fund_id == Model.FundId && c.value_date==Model.Date).FirstOrDefault();
            var Actualy_auth = Model.Auth;
            if (Actualy_auth == true) { 
                    
            }
            else
            {
                if (CheckRed != null || CheckSub != null && CheckTrans != null)
                {
                    Actualy_auth = true;
                }
            }
            //var CheckRed = dbContext.ICPrices.Where(c => c.Code == Code).FirstOrDefault();

            
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


            DetailsViewModel CurrentICPrice = new DetailsViewModel
                {
                    Code = Model.Code,
                    FundId = Model.FundId,
                    Date = Model.Date,
                    ProcessingDate = Model.ProcessingDate,
                    Price = Model.Price, 
                    Check = Model.Chk,
                    Auth= Model.Auth,
                    AuthForEditAndDelete=Actualy_auth

                };
            if (IDs != null)
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

            ViewData["Funds"] = new SelectList(dbContext.Funds.Where(f=>f.FundID==Model.FundId).ToList(), "FundID", "Name");
                return View(CurrentICPrice);
            }

            [AuthorizedRights(Screen = "ICPrice", Right = "Read")]
            public ActionResult Next(string Code)
            {
                var arrlenth = IDs.Count();
                var LastObj = dbContext.ICPrices.OrderBy(s => s.Code).ToList().LastOrDefault(s => s.Code == IDs[arrlenth - 1]);
                if (Code == LastObj.Code)
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

                        if (item == Code)
                        {
                            next = counter + 1;

                        }
                        counter++;
                    }
                    Code = IDs[next];
                    var Id = dbContext.ICPrices.Where(u => u.Code == Code).Select(s => s.Code).FirstOrDefault();
                if (Code == LastObj.Code)
                {
                    TempData["Last"] = "Last";
                }
                else
                {
                    TempData["Last"] = "NotLast";
                }


                return RedirectToAction("Details", new { Code = Id });
                }

            }
            [AuthorizedRights(Screen = "ICPrice", Right = "Read")]
            public ActionResult Previous(string Code)
            {
                var arrindex = IDs[0];
                var FirstObj = dbContext.ICPrices.OrderBy(s => s.Code).FirstOrDefault(s => s.Code == arrindex);
                if (Code == FirstObj.Code)
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

                        if (item == Code)
                        {
                            previous = counter - 1;

                        }
                        counter++;
                    }
                    Code = IDs[previous];

                if (Code == FirstObj.Code)
                {
                    TempData["First"] = "First";
                }
                else
                {
                    TempData["First"] = "NotFirst";
                }
                var Id = dbContext.ICPrices.Where(u => u.Code == Code).Select(s => s.Code).FirstOrDefault();
                    return RedirectToAction("Details", new { Code = Id });
                }
            }

        [AuthorizedRights(Screen = "ICPrice", Right = "Read")]
        public ActionResult ExportToExcel()
        {
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var gv = new GridView();
            
                   var List = (List<int>)TempData["ICpriceForExc"];
            gv.DataSource = dbContext.ICPrices.Where(Del => List.Contains(Del.ICPriceID) && Del.DeletFlag == DeleteFlag.NotDeleted).Select(x => new { x.Code, FundName= x.Fund.Name, x.Price }).OrderBy(u => u.Code).ToList();


            //gv.DataSource = (from s in dbContext.ICPrices
            //                 select new
            //                 {
            //                     s.Code,
            //                     s.Price


            //                 })
            //                     .ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=ICprice" + NowTime + ".xls");
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

        [AuthorizedRights(Screen = "ICPrice", Right = "Read")]
        public ActionResult ExportToPDF()
        {
            var branches = dbContext.ICPrices.Select(s => s).Where(s => IDs.Contains(s.Code)).OrderBy(u => u.ICPriceID).ToList();
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");

            var report = new PartialViewAsPdf("~/Areas/ICPrices/Views/ICPrice/ExportToPDF.cshtml", branches);
            return report;

        }
        [AuthorizedRights(Screen = "ICPrice", Right = "Authorized")]
        public ActionResult AuthorizeICPrice(string Code)
            {
                var ICPrice = dbContext.ICPrices.Where(f => f.Code == Code).ToList();
                foreach (var item in ICPrice)
                {
                    item.Auther = User.Identity.GetUserId();
                    item.Auth = true;
                }
                dbContext.SaveChanges();
                return RedirectToAction("Details",new { Code = Code });
            }
        [AuthorizedRights(Screen = "ICPrice", Right = "Check")]
        public ActionResult CheckICPrice(string Code)
            {
            var ICPrice = dbContext.ICPrices.Where(f => f.Code == Code).ToList();
            foreach (var item in ICPrice)
            {
                item.Checker = User.Identity.GetUserId();
                item.Chk = true;
            }
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = Code });

        }

        [AuthorizedRights(Screen = "ICPrice", Right = "Delete")]
        public ActionResult Delete(string Code)
        {
            var ICPrice = dbContext.ICPrices.Where(f => f.Code == Code && f.Auth ==false).ToList();
            foreach (var item in ICPrice)
            {
                    item.DeletFlag = DeleteFlag.Deleted;
               
            }
                dbContext.SaveChanges();
                return RedirectToAction("Index");
        }

           

            public bool ChecKThuresday()
            {
            var thisDay = DateTime.Now;
            
            if(thisDay.DayOfWeek == DayOfWeek.Thursday)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
            public bool ChecKFridayAndSaturday(DateTime Date)
            {

                if (Date.DayOfWeek == DayOfWeek.Friday || Date.DayOfWeek == DayOfWeek.Saturday)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            public bool ChecKThuresday(DateTime Date)
            {

                if (Date.DayOfWeek == DayOfWeek.Thursday)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            private void AddErrors(IdentityResult result)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

        [Route("ChekDayINCalendar")]
        [HttpPost]
        public DateTime ChekCalendar(DateTime Date)
        {
            var Calendar = dbContext.Calendars.Where(x => x.DeletFlag != DeleteFlag.Deleted).ToList();
            foreach (var Item in Calendar)
            {
               if(!ChecKFridayAndSaturday(Date)) 
                { 
                    if (Item.Vacation_date == Date)
                    {
                        Date.AddDays(1);
                    }
                }
            }
            return Date;
          

        }
       
        [Route("CheckVacation")]
        [HttpPost]
        public int CheckVacation(DateTime Day)
        {

              var Calendar = dbContext.Calendars.Where(x => x.Vacation_date == Day.Date &&  x.DeletFlag == DeleteFlag.NotDeleted).FirstOrDefault();
            //&& x.Auth == true
                if (Calendar != null)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            

        }
        private object GetDayWillBeInserted(DateTime? lastDay)
        {
            if (lastDay == null)
            {
                if (DateTime.Now.Day == DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
                return new { Success = true , Date = DateTime.Now };
               
                else
                 return new { Success = true , Date = (new DateTime(DateTime.Now.Year,DateTime.Now.Month-1, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month-1))) };
            }
           else
            {
                if(lastDay.Value.Month < DateTime.Now.Month - 1)
                  return new { Success = true, Date = (new DateTime(lastDay.Value.Year, lastDay.Value.Month + 1, DateTime.DaysInMonth(lastDay.Value.Year, lastDay.Value.Month +1 ))) };
                else
                  return new { Success = false, Message="You can not insert before this : "+ (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))).ToString() };
        }
        }
        public ActionResult CheckDate(int FundId)
        {
        
            var LastInsertrdDay = dbContext.ICPrices.Where(ic => ic.FundId == FundId && ic.DeletFlag == DeleteFlag.NotDeleted).OrderByDescending(i => i.Date).FirstOrDefault();
            
            dynamic result =(LastInsertrdDay != null) ? GetDayWillBeInserted(LastInsertrdDay.Date) : GetDayWillBeInserted(null);

            if(result.Success)
                return Json(new { success = true, ICDate = result.Date.ToString(), ProcessDate = result.Date.ToString(), responseText = "Added" }, JsonRequestBehavior.AllowGet);

            return Json(new { success = false, ICDate = "", ProcessDate = "", responseText = result.Message }, JsonRequestBehavior.AllowGet);

         

        }


        public DateTime CheckAuthDay(DateTime Day, int FundId)
        {
            var DayId = dbContext.Day.Where(x => x.DayName == Day.DayOfWeek.ToString()).Select(thisID => thisID.Day_Id).FirstOrDefault();
            //var DayId2 = dbContext.Day.Where(x => x.DayName == Day.ToString("dddd")).Select(thisID => thisID.Day_Id).FirstOrDefault();
            var CheckINAuthDayOrNot = dbContext.FundAuthDay.Where(x => x.FundId == FundId && x.Day_Id == DayId).FirstOrDefault();
            
            if (CheckINAuthDayOrNot != null)
            {
                    var CheckDay =CheckVacation(Day);
                    var ChecKFridayorSaturday = ChecKFridayAndSaturday(Day);
                        if (CheckDay == 1 || ChecKFridayorSaturday == true)
                        {
                            Day = Day.AddDays(1);
                            var CheCkAuthAgain = CheckAuthDay(Day, FundId);
                            return CheCkAuthAgain;
                           
                        }
                         else
                         {
                             return Day;   
                         }
                    
            }
            else
            {
                Day = Day.AddDays(1);
                var DayAfterCheckVacation = CheckAuthDay(Day, FundId);
                return DayAfterCheckVacation;
                        
            }
                  
        }

        public DateTime CheckFundWorkingDay(DateTime Day, int FundId)
        {
            var DayId = dbContext.Day.Where(x => x.DayName == Day.DayOfWeek.ToString()).Select(thisID => thisID.Day_Id).FirstOrDefault();
            //var DayId2 = dbContext.Day.Where(x => x.DayName == Day.ToString("dddd")).Select(thisID => thisID.Day_Id).FirstOrDefault();
          
                
                var CheckDay = CheckVacation(Day);
                var ChecKFridayorSaturday = ChecKFridayAndSaturday(Day);
                if (CheckDay == 1 || ChecKFridayorSaturday == true)
                {
                    Day = Day.AddDays(1);
                    var CheCkAuthAgain = CheckFundWorkingDay(Day, FundId);
                    return CheCkAuthAgain;

                }
                else
                {
                    return Day;
                }

            
          

        }
        [Route("CheckPrice")]
        [HttpPost]
        public ActionResult CheckICprice(decimal Price, int FundID)
        {
            string decimal_places = "";
            var ICpriceFOrThisFund = dbContext.Funds.Where(x => x.FundID == FundID && x.DeletFlag == DeleteFlag.NotDeleted).Select(m => m.ICprice).FirstOrDefault();
            var LastPrice = dbContext.ICPrices.Where(x => x.FundId == FundID && x.DeletFlag == DeleteFlag.NotDeleted).OrderByDescending(Date => Date.Date).Select(m => m.Price).FirstOrDefault();
            var PriceTol = dbContext.Funds.Where(x => x.FundID == FundID && x.DeletFlag == DeleteFlag.NotDeleted).Select(m => m.PriceTol).FirstOrDefault();
            var MaxPriceAfterCalcTol = LastPrice + LastPrice * PriceTol;
            var MinPriceAfterCalcTol = LastPrice - LastPrice * PriceTol;
            if (LastPrice > 0)
            {
                if (PriceTol != 0) {
                    if (Price <= MaxPriceAfterCalcTol && Price >= MinPriceAfterCalcTol)
                    {

                        string input_decimal_number = Convert.ToString(Price);
                        var regex = new System.Text.RegularExpressions.Regex("(?<=[\\.])[0-9]+");
                        if (regex.IsMatch(input_decimal_number))
                        {
                            decimal_places = regex.Match(input_decimal_number).Value;
                        }
                        var length = decimal_places.Length;
                        if (ICpriceFOrThisFund >=0)
                        {
                            if (ICpriceFOrThisFund == length)
                            {
                                return Json(new { status = 1 });
                            }
                            else
                            {
                                return Json(new { status = 2, ICpriceFund = ICpriceFOrThisFund });
                            }

                        }
                        else
                        {
                            return Json(new { status = 0});
                        }


                    }
                    else
                    {
                        return Json(new { status = 3 , Max = MaxPriceAfterCalcTol, Min = MinPriceAfterCalcTol });
                    }
                }
                else
                {
                    return Json(new { status = 4 });
                }
                //else
                //{
                //    if (Price >= MaxPriceAfterCalcTol && Price < LastPrice)
                //    {

                //        string input_decimal_number = Convert.ToString(Price);
                //        var regex = new System.Text.RegularExpressions.Regex("(?<=[\\.])[0-9]+");
                //        if (regex.IsMatch(input_decimal_number))
                //        {
                //            decimal_places = regex.Match(input_decimal_number).Value;
                //        }
                //        var length = decimal_places.Length;
                //        if (ICpriceFOrThisFund != null)
                //        {
                //            if (ICpriceFOrThisFund == length)
                //            {
                //                return Json(new { status = 1 });
                //            }
                //            else
                //            {
                //                return Json(new { status = 0 });
                //            }

                //        }
                //        else
                //        {
                //            return Json(new { status = 0 });
                //        }


                //    }
                //    else
                //    {
                //        return Json(new { status = 2 });
                //    }
                //}
                   
            }
            else
            {
                string input_decimal_number = Convert.ToString(Price);
                var regex = new System.Text.RegularExpressions.Regex("(?<=[\\.])[0-9]+");
                if (regex.IsMatch(input_decimal_number))
                {
                    decimal_places = regex.Match(input_decimal_number).Value;
                }
                var length = decimal_places.Length;
                if (ICpriceFOrThisFund != null)
                {
                    if (ICpriceFOrThisFund == length)
                    {
                        return Json(new { status = 1 });
                    }
                    else
                    {
                        //return Json(new { status = 2 , Max = MaxPriceAfterCalcTol,Min = MinPriceAfterCalcTol });
                        return Json(new { status = 2, ICpriceFund = ICpriceFOrThisFund });
                    }

                }
                else
                {
                    return Json(new { status = 0 });
                }
                //return Json(new { status = 1 });
            }

        }

    }

}