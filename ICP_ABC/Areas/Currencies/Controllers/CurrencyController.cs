using ICP_ABC.Areas.Currencies.Models;
using ICP_ABC.Areas.UsersSecurity.Models;
using ICP_ABC.Extentions;
using ICP_ABC.Models;
using Microsoft.ApplicationBlocks.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ICP_ABC.Areas.Currencies.Controllers
{
    [Authorize]
    public class CurrencyController : Controller
    {
        private ApplicationDbContext dbContext = new ApplicationDbContext();
        UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(new ApplicationDbContext());
        static string[] IDs;

        //GetLastCode
        public string GetLastCode()
        {
            var querey = "SELECT MAX(CAST(Code AS INT)) as MaxCode FROM Currency";
            var appSettings = ConfigurationManager.ConnectionStrings;
            var SqlCon = appSettings["ICPRO"];
            var ReturnedData = SqlHelper.ExecuteDataset(SqlCon.ToString(), CommandType.Text, querey);
            var LastCode = ReturnedData.Tables[0].Rows[0][0].ToString();
            return LastCode;
        }
        [AuthorizedRights(Screen = "Currency", Right = "Read")]
        public ActionResult Index()
        {
            return View();
        }
        [AuthorizedRights(Screen = "Currency", Right = "Create")]
        public ActionResult Create()
        {
            //var lastcode = dbContext.Currencies.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault();
            var lastcode = GetLastCode();

            var trieng = int.TryParse(lastcode, out int Code);
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
        [AuthorizedRights(Screen = "Currency", Right = "Create")]
        public ActionResult Create(CreateViewModel model)
        {
            var lastcode = GetLastCode();
            //var lastcode = dbContext.Currencies.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault();
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

            var Level = dbContext.UserSecurities.Select(Val => Val.Levels).FirstOrDefault();
            if (Level == Levels.TwoLevels)
            {
                if (ModelState.IsValid)
                {


                    Currency Currency = new Currency
                    {
                        Code = lastcode,
                        Name = model.Name,
                        ShortName = model.ShortName,
                        Maker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        Chk = true, //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        Checker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        UserID = User.Identity.GetUserId()// dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                    };
                    dbContext.Currencies.Add(Currency);
                    dbContext.SaveChanges();
                    return RedirectToAction("Details", new { Code = lastcode });
                    //return RedirectToAction("Search");

                }
                else
                {
                    return View(model);
                }
            }
            else
            {
                if (ModelState.IsValid)
                {


                    Currency Currency = new Currency
                    {
                        Code = lastcode,
                        Name = model.Name,
                        ShortName = model.ShortName,
                        Maker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        UserID = User.Identity.GetUserId()// dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                    };
                    dbContext.Currencies.Add(Currency);
                    dbContext.SaveChanges();
                    return RedirectToAction("Details", new { Code = lastcode });
                    //return RedirectToAction("Search");

                }
                else
                {
                    return View(model);
                }
            }

          
           
        }

        [AuthorizedRights(Screen = "Currency", Right = "Update")]
        public ActionResult Edit(string Code)
        {
            var currentCurrency = dbContext.Currencies
                   .Where(g => g.Code == Code)
                   .FirstOrDefault();
            EditViewModel model = new EditViewModel
            {
                Code = currentCurrency.Code,
                Name = currentCurrency.Name,
                ShortName= currentCurrency.ShortName
            };
            return View(model);
        }
        [HttpPost]
        [AuthorizedRights(Screen = "Currency", Right = "Update")]
        public ActionResult Edit(EditViewModel model)
        {
            var currentCurrency = dbContext.Currencies.Where(g => g.Code == model.Code).FirstOrDefault();
            currentCurrency.Name = model.Name;
            currentCurrency.SysDate = DateTime.Now;
            currentCurrency.ShortName = model.ShortName;
            currentCurrency.EditFlag = true;
            currentCurrency.UserID = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = model.Code });


            //return RedirectToAction("Search");
        }

        [AuthorizedRights(Screen = "Currency", Right = "Read")]
        public ViewResult Search(string sortOrder, string currentFilter, string searchString, int? page, string Code,string ShortName,string RadioCHeck)
        {
            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;


            if (page == null && sortOrder == null && currentFilter == null && searchString == null  && Code == null && RadioCHeck == null )
            {
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                var Currencies = dbContext.Currencies.Where(c => c.DeletFlag == DeleteFlag.NotDeleted).Take(0);

                return View(Currencies.ToPagedList(pageNumber, pageSize));

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

                var Currencies = dbContext.Currencies.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);
                if (!String.IsNullOrEmpty(searchString))
                {
                    Currencies = Currencies.Where(s => s.Name.StartsWith(searchString));
                    ViewData["searchString"] = searchString;


                }
                //checkRadio 
                if (!String.IsNullOrEmpty(RadioCHeck))
                {
                    //Case Auth
                    if (RadioCHeck == "1")
                    {
                        Currencies = Currencies.Where(s => s.Auth == true);

                    }
                    //Case Checker
                    if (RadioCHeck == "2")
                    {
                        Currencies = Currencies.Where(s => s.Chk == true && s.Auth == false);

                    }
                    //Case maker
                    if (RadioCHeck == "3")
                    {
                        Currencies = Currencies.Where(s => s.Auth == false && s.Chk == false);

                    }
                    //Case All
                    if (RadioCHeck == "4")
                    {
                        Currencies = Currencies.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);

                    }


                }
                //

                if (!String.IsNullOrEmpty(Code))
                {
                    Currencies = Currencies.Where(s => s.Code.StartsWith(Code));
                    ViewData["Code"] = Code;


                }
                if (!String.IsNullOrEmpty(ShortName))
                {
                    Currencies = Currencies.Where(s => s.ShortName.StartsWith(ShortName));
                    ViewData["ShortName"] = ShortName;


                }

                TempData["CurrencyForExc"] = Currencies.Select(x => x.Code).ToList(); 
                switch (sortOrder)
                {
                    case "name_desc":
                        Currencies = Currencies.OrderByDescending(s => s.Name);
                        break;
                    case "Code":
                        Currencies = Currencies.OrderBy(s => s.Code);
                        break;
                    case "code_desc":
                        Currencies = Currencies.OrderByDescending(s => s.Code);
                        break;
                    default:  // Name ascending 
                        Currencies = Currencies.OrderBy(s => s.CurrencyID);
                        break;
                }
                //int count = Currencies.ToList().Count();
                //IDs = new string[count];
                //IDs = Currencies.Select(s => s.Code).ToArray();

                int pageSize = 4;
                int pageNumber = (page ?? 1);
                ViewData["radioCHeck"] = RadioCHeck;
                int count = Currencies.ToList().Count();
                IDs = new string[count];
                IDs = Currencies.Select(s => s.Code).ToArray();
                int[] myInts = Array.ConvertAll(IDs, s => int.Parse(s));
                Array.Sort(myInts);
                IDs = myInts.Select(x => x.ToString()).ToArray();
                return View(Currencies.ToPagedList(pageNumber, pageSize));
            }
        }

        [AuthorizedRights(Screen = "Currency", Right = "Read")]
        public ViewResult Details(string Code)
        {
            var Model = dbContext.Currencies.Where(c => c.Code == Code).FirstOrDefault();

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


            DetailsViewModel CurrentCurrency = new DetailsViewModel
            {
                Code = Model.Code,
                Name = Model.Name,
                ShortName= Model.ShortName,
                Check=Model.Chk,
                Auth = Model.Auth,
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


            return View(CurrentCurrency);

        }

        [AuthorizedRights(Screen = "Currency", Right = "Read")]
        public ActionResult Next(string id)
        {
            var arrlenth = IDs.Count();
            var LastObj = dbContext.Currencies.OrderBy(s => s.Code).ToList().LastOrDefault(s => s.Code == IDs[arrlenth - 1]);
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
                var Code = dbContext.Currencies.Where(u => u.Code == id).Select(s => s.Code).FirstOrDefault();
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
        [AuthorizedRights(Screen = "Currency", Right = "Read")]
        public ActionResult Previous(string id)
        {
            var arrindex = IDs[0];
            var FirstObj = dbContext.Currencies.OrderBy(s => s.Code).FirstOrDefault(s => s.Code == arrindex);
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
                var Code = dbContext.Currencies.Where(u => u.Code == id).Select(s => s.Code).FirstOrDefault();
                return RedirectToAction("Details", new { Code = Code });
            }

        }

        [AuthorizedRights(Screen = "Currency", Right = "Read")]
        public ActionResult ExportToExcel()
        {
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var gv = new GridView();

            var List = (List<string>)TempData["CurrencyForExc"];

            gv.DataSource = dbContext.Currencies.Where(Del => List.Contains(Del.Code) && Del.DeletFlag == DeleteFlag.NotDeleted).Select(x => new { x.Code, x.Name, x.ShortName }).ToList();
            //gv.DataSource = (from s in dbContext.Currencies
            //                 select new
            //                 {
            //                     s.Code,
            //                     s.Name,
            //                     s.ShortName


            //                 })
            //                     .ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Currency" + NowTime + ".xls");
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
            ViewData["Groups"] = new SelectList(dbContext.UserGroups.ToList(), "GroupID", "Name");
            ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "branchcode", "BName");
            return RedirectToAction("Search");
        }

        [AuthorizedRights(Screen = "Currency", Right = "Read")]
        public ActionResult ExportToPDF()
        {
            var Currencies = dbContext.Currencies.Select(s => s).Where(s => IDs.Contains(s.Code)).OrderBy(u => u.Name).ToList();
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");

            var report = new PartialViewAsPdf("~/Areas/Currencies/Views/Currency/ExportToPDF.cshtml", Currencies);
            return report;

        }

        [AuthorizedRights(Screen = "Currency", Right = "Authorized")]
        public ActionResult AuthorizeCurrency(string Code)
        {
            var Currency = dbContext.Currencies.Where(f => f.Code == Code).FirstOrDefault();
            Currency.Auth = true;
            Currency.Auther = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = Code });

        }

        [AuthorizedRights(Screen = "Currency", Right = "Check")]
        public ActionResult CheckCurrency(string Code)
        {
            var Currency = dbContext.Currencies.Where(f => f.Code == Code).FirstOrDefault();
            Currency.Chk = true;
            Currency.Checker = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = Code });

        }

        [AuthorizedRights(Screen = "Currency", Right = "Delete")]
        public ActionResult Delete(string Code)
        {
            var Currency = dbContext.Currencies.Where(f => f.Code == Code).FirstOrDefault();
            Currency.DeletFlag = DeleteFlag.Deleted;
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}