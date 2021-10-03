using ICP_ABC.Areas.Cities.Models;
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

namespace ICP_ABC.Areas.Cities.Controllers
{
    [Authorize]
    public class CityController : Controller
    {
        private ApplicationDbContext dbContext = new ApplicationDbContext();
        UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(new ApplicationDbContext());
        static string[] IDs;


        [AuthorizedRights(Screen = "City", Right = "Read")]
        public ActionResult Index()
        {
            return View();
        }


        //GetLastCode
        public string GetLastCode()
        {
            var querey = "SELECT MAX(CAST(Code AS INT)) as MaxCode FROM City";
            var appSettings = ConfigurationManager.ConnectionStrings;
            var SqlCon = appSettings["ICPRO"];
            var ReturnedData = SqlHelper.ExecuteDataset(SqlCon.ToString(), CommandType.Text, querey);
            var LastCode = ReturnedData.Tables[0].Rows[0][0].ToString();
            return LastCode;
        }
        [AuthorizedRights(Screen = "City", Right = "Create")]
        public ActionResult Create()
        {
            //var lastcode = dbContext.Cities.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault();
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
        [AuthorizedRights(Screen = "City", Right = "Create")]
        public ActionResult Create(CreateViewModel model)
        {
            //var lastcode = dbContext.Cities.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault();
            var lastcode = GetLastCode();
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

                    City City = new City
                    {
                        Code = lastcode,
                        Name = model.Name,
                        Maker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        Checker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        Chk = true, //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        UserID = User.Identity.GetUserId()// dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                    };
                    dbContext.Cities.Add(City);
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

                    City City = new City
                    {
                        Code = lastcode,
                        Name = model.Name,
                        Maker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        UserID = User.Identity.GetUserId()// dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                    };
                    dbContext.Cities.Add(City);
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

        [AuthorizedRights(Screen = "City", Right = "Update")]
        public ActionResult Edit(string Code)
        {
           var currentCity= dbContext.Cities
                  .Where(g => g.Code == Code)
                  .FirstOrDefault();
            EditViewModel model = new EditViewModel
            {
                Code = currentCity.Code,
                Name = currentCity.Name
            };
            return View(model);
        }
        [HttpPost]
        [AuthorizedRights(Screen = "City", Right = "Update")]
        public ActionResult Edit(EditViewModel model)
        {
            var currentCity = dbContext.Cities.Where(g => g.Code == model.Code).FirstOrDefault();
            currentCity.Name = model.Name;
            currentCity.SysDate = DateTime.Now;
            currentCity.EditFlag = true;
            currentCity.UserID = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = model.Code });
            //return RedirectToAction("Search");
        }

        [AuthorizedRights(Screen = "City", Right = "Read")]
        public ViewResult Search(string sortOrder, string currentFilter, string searchString, int? page,string Code,string RadioCHeck)
        {
            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;

            if (page == null &&sortOrder == null && currentFilter == null && searchString == null  && Code == null && RadioCHeck == null )
            {
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                var cities = dbContext.Cities.Where(c => c.DeletFlag == DeleteFlag.NotDeleted).Take(0);

                return View(cities.ToPagedList(pageNumber, pageSize));

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

                var cities = dbContext.Cities.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);
                if (!String.IsNullOrEmpty(searchString))
                {
                    cities = cities.Where(s => s.Name.StartsWith(searchString));
                    ViewData["searchString"] = searchString;

                }

                //checkRadio 
                if (!String.IsNullOrEmpty(RadioCHeck))
                {
                    //Case Auth
                    if (RadioCHeck == "1")
                    {
                        cities = cities.Where(s => s.Auth == true);

                    }
                    //Case Checker
                    if (RadioCHeck == "2")
                    {
                        cities = cities.Where(s => s.Chk == true && s.Auth == false);

                    }
                    //Case maker
                    if (RadioCHeck == "3")
                    {
                        cities = cities.Where(s => s.Auth == false && s.Chk == false);

                    }
                    //Case All
                    if (RadioCHeck == "4")
                    {
                        cities = cities.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);

                    }


                }
                //

                if (!String.IsNullOrEmpty(Code))
                {
                    cities = cities.Where(s => s.Code.StartsWith(Code));
                    ViewData["Code"] = Code;
                }
                TempData["CityForExc"] = cities.Select(x => x.CityID).ToList();

                switch (sortOrder)
                {
                    case "name_desc":
                        cities = cities.OrderByDescending(s => s.Name);
                        break;
                    case "Code":
                        cities = cities.OrderBy(s => s.Code);
                        break;
                    case "code_desc":
                        cities = cities.OrderByDescending(s => s.Code);
                        break;
                    default:  // Name ascending 
                        cities = cities.OrderBy(s => s.CityID);
                        break;
                }
                //int count = cities.ToList().Count();
                //IDs = new string[count];
                //IDs = cities.Select(s => s.Code).ToArray();

                int pageSize = 4;
                int pageNumber = (page ?? 1);

                int count = cities.ToList().Count();
                IDs = new string[count];
                IDs = cities.Select(s => s.Code).ToArray();
                ViewData["radioCHeck"] = RadioCHeck;
                int[] myInts = Array.ConvertAll(IDs, s => int.Parse(s));
                Array.Sort(myInts);
                IDs = myInts.Select(x => x.ToString()).ToArray();

                return View(cities.ToPagedList(pageNumber, pageSize));
            }
        }

        [AuthorizedRights(Screen = "City", Right = "Read")]
        public ViewResult Details(string Code)
        {
            var Model = dbContext.Cities.Where(c => c.Code == Code).FirstOrDefault();
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



            DetailsViewModel CurrentCity = new DetailsViewModel
            {
                Code = Model.Code,
                Name = Model.Name,
                Auth= Model.Auth,
                Check= Model.Chk,
                AuthForEditAndDelete = Actualy_auth

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
            return View(CurrentCity);

        }

        [AuthorizedRights(Screen = "City", Right = "Read")]
        public ActionResult Next(string id)
        {
            var arrlenth = IDs.Count();
            var LastObj = dbContext.Cities.OrderBy(s => s.Code).ToList().LastOrDefault(s => s.Code == IDs[arrlenth - 1]);
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
                var Code = dbContext.Cities.Where(u => u.Code == id).Select(s => s.Code).FirstOrDefault();
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
        [AuthorizedRights(Screen = "City", Right = "Read")]
        public ActionResult Previous(string id)
        {
            var arrindex = IDs[0];
            var FirstObj = dbContext.Cities.OrderBy(s => s.Code).FirstOrDefault(s => s.Code == arrindex);
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
                var Code = dbContext.Cities.Where(u => u.Code == id).Select(s => s.Code).FirstOrDefault();
                return RedirectToAction("Details", new { Code = Code });
            }

        }

        [AuthorizedRights(Screen = "City", Right = "Read")]
        public ActionResult ExportToExcel()
        {
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var gv = new GridView();
            var List = (List<int>)TempData["CityForExc"];

            gv.DataSource = dbContext.Cities.Where(Del => List.Contains(Del.CityID) && Del.DeletFlag == DeleteFlag.NotDeleted).Select(x => new { Code = x.Code,CityName= x.Name }).ToList();
            
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=City" + NowTime + ".xls");
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

        [AuthorizedRights(Screen = "City", Right = "Read")]
        public ActionResult ExportToPDF()
        {
            var Cities = dbContext.Cities.Select(s => s).Where(s => IDs.Contains(s.Code)).OrderBy(u => u.Name).ToList();
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");

            var report = new PartialViewAsPdf("~/Areas/Cities/Views/City/ExportToPDF.cshtml", Cities);
            return report;

        }

        [AuthorizedRights(Screen = "City", Right = "Authorized")]
        public ActionResult AuthorizeCity(string Code)
        {
            var City = dbContext.Cities.Where(f => f.Code == Code).FirstOrDefault();
            City.Auth = true;
            City.Auther = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = Code });

        }

        [AuthorizedRights(Screen = "City", Right = "Check")]
        public ActionResult CheckCity(string Code)
        {
            var city = dbContext.Cities.Where(f => f.Code == Code).FirstOrDefault();
            city.Chk = true;
            city.Checker = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = Code });

        }

        [AuthorizedRights(Screen = "City", Right = "Delete")]
        public ActionResult Delete(string Code)
        {
            var City = dbContext.Cities.Where(f => f.Code == Code).FirstOrDefault();
            City.DeletFlag = DeleteFlag.Deleted;
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}