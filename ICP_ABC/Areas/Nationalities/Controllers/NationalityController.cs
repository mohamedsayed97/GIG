
using ICP_ABC.Areas.Nationalities.Models;
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
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ICP_ABC.Areas.Nationalities.Controllers
{
    public class NationalityController : Controller
    {

        private ApplicationDbContext dbContext = new ApplicationDbContext();
        UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(new ApplicationDbContext());
        static string[] IDs;
        //static string[] IDs;

        public NationalityController() { }
        //GetLastCode
        public string GetLastCode()
        {
            var querey = "SELECT MAX(CAST(Code AS INT)) as MaxCode FROM Nationality";
            var appSettings = ConfigurationManager.ConnectionStrings;
            var SqlCon = appSettings["ICPRO"];
            var ReturnedData = SqlHelper.ExecuteDataset(SqlCon.ToString(), CommandType.Text, querey);
            var LastCode = ReturnedData.Tables[0].Rows[0][0].ToString();
            return LastCode;
        }

        // GET: Group

        public ActionResult Index()
        {
            return View();
        }
        [AuthorizedRights(Screen = "Nationality", Right = "Create")]
        public ActionResult Create()
        {
            //var lastcode = dbContext.Nationalities.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault();
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
        [AuthorizedRights(Screen = "Nationality", Right = "Create")]
        public ActionResult Create(CreateViewModel model)
        {
            //var lastcode = dbContext.Nationalities.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault();
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

                    Nationality Nationality = new Nationality
                    {
                        Code = lastcode,
                        Name = model.Name,
                        DeletFlag = DeleteFlag.NotDeleted,
                        Maker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        Checker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        Chk = true, //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        UserID = User.Identity.GetUserId()// dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                    };
                    dbContext.Nationalities.Add(Nationality);
                    try
                    {
                        dbContext.SaveChanges();
                        return RedirectToAction("Details", new { Code = lastcode });
                        //return RedirectToAction("Search");
                    }
                    catch (DbEntityValidationException e)
                    {

                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }


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

                    Nationality Nationality = new Nationality
                    {
                        Code = lastcode,
                        Name = model.Name,
                        DeletFlag = DeleteFlag.NotDeleted,
                        Maker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        UserID = User.Identity.GetUserId()// dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                    };
                    dbContext.Nationalities.Add(Nationality);
                    try
                    {
                        dbContext.SaveChanges();
                        return RedirectToAction("Details", new { Code = lastcode });
                        //return RedirectToAction("Search");
                    }
                    catch (DbEntityValidationException e)
                    {

                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }


                }
                else
                {
                    return View(model);
                }
            }


          

        }

        [AuthorizedRights(Screen = "Nationality", Right = "Update")]
        public ActionResult Edit(string Code)
        {
            var currentNationality = dbContext.Nationalities
                   .Where(g => g.Code == Code)
                   .FirstOrDefault();
            EditViewModel model = new EditViewModel
            {
                Code = currentNationality.Code,
                Name = currentNationality.Name
               
            };
            return View(model);
        }
        [HttpPost]
        [AuthorizedRights(Screen = "Nationality", Right = "Update")]
        public ActionResult Edit(EditViewModel model)
        {
            var currentNationality = dbContext.Nationalities.Where(g => g.Code == model.Code).FirstOrDefault();
            currentNationality.Name = model.Name;
            currentNationality.SysDate = DateTime.Now;
            currentNationality.EditFlag = true;
            currentNationality.UserID = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = model.Code });
            //return RedirectToAction("Search");
        }

        [AuthorizedRights(Screen = "Nationality", Right = "Read")]
        public ViewResult Search(string sortOrder, string currentFilter, string searchString, int? page, string CodeString, string RadioCHeck)
        {
            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;

            if (page==null && sortOrder == null && currentFilter == null && searchString == null  && CodeString == null && RadioCHeck == null )
            {
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                var Nationalities = dbContext.Nationalities.Where(c => c.DeletFlag == DeleteFlag.NotDeleted).Take(0);

                return View(Nationalities.ToPagedList(pageNumber, pageSize));

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

                var Nationalities = dbContext.Nationalities.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);
                if (!String.IsNullOrEmpty(searchString))
                {
                    Nationalities = Nationalities.Where(s => s.Name.StartsWith(searchString));
                    ViewData["searchString"] = searchString;
                }

                //checkRadio 
                if (!String.IsNullOrEmpty(RadioCHeck))
                {
                    //Case Auth
                    if (RadioCHeck == "1")
                    {
                        Nationalities = Nationalities.Where(s => s.Auth == true);

                    }
                    //Case Checker
                    if (RadioCHeck == "2")
                    {
                        Nationalities = Nationalities.Where(s => s.Chk == true && s.Auth == false);

                    }
                    //Case maker
                    if (RadioCHeck == "3")
                    {
                        Nationalities = Nationalities.Where(s => s.Auth == false && s.Chk == false);

                    }
                    //Case All
                    if (RadioCHeck == "4")
                    {
                        Nationalities = Nationalities.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);

                    }


                }
                //

                if (!String.IsNullOrEmpty(CodeString))
                {
                    Nationalities = Nationalities.Where(s => s.Code.StartsWith(CodeString));
                    ViewData["Code"] = CodeString;
                }

                TempData["NaTionForExc"] = Nationalities.Select(x => x.Code).ToList();




                switch (sortOrder)
                {
                    case "name_desc":
                        Nationalities = Nationalities.OrderByDescending(s => s.Name);
                        break;
                    case "Code":
                        Nationalities = Nationalities.OrderBy(s => s.Code);
                        break;
                    case "code_desc":
                        Nationalities = Nationalities.OrderByDescending(s => s.Code);
                        break;
                    default:  // Name ascending 
                        Nationalities = Nationalities.OrderBy(s => s.NationalityID);
                        break;
                }
                //int count = Nationalities.ToList().Count();
                //IDs = new string[count];
                //IDs = Nationalities.Select(s => s.Code).ToArray();

                int pageSize = 4;
                int pageNumber = (page ?? 1);
                ViewData["radioCHeck"] = RadioCHeck;
                int count = Nationalities.ToList().Count();
                IDs = new string[count];
                IDs = Nationalities.Select(s => s.Code).ToArray();
                int[] myInts = Array.ConvertAll(IDs, s => int.Parse(s));
                Array.Sort(myInts);
                IDs = myInts.Select(x => x.ToString()).ToArray();
                return View(Nationalities.ToPagedList(pageNumber, pageSize));
            }
        }
        [AuthorizedRights(Screen = "Nationality", Right = "Read")]
        public ViewResult Details(string Code)
        {
            var Model = dbContext.Nationalities.Where(c => c.Code == Code).FirstOrDefault();

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


            DetailsViewModel CurrentNationality = new DetailsViewModel
            {
                Code = Model.Code,
                Name = Model.Name,
                Auth = Model.Auth,
                Check = Model.Chk,
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

            return View(CurrentNationality);

        }
        [AuthorizedRights(Screen = "Nationality", Right = "Read")]
        public ActionResult Next(string id)
        {
            var arrlenth = IDs.Count();
            var LastObj = dbContext.Nationalities.OrderBy(s => s.Code).ToList().LastOrDefault(s => s.Code == IDs[arrlenth - 1]);
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
                var Code = dbContext.Nationalities.Where(u => u.Code == id).Select(s => s.Code).FirstOrDefault();
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
        [AuthorizedRights(Screen = "Nationality", Right = "Read")]
        public ActionResult Previous(string id)
        {
            var arrindex = IDs[0];
            var FirstObj = dbContext.Nationalities.OrderBy(s => s.Code).FirstOrDefault(s => s.Code == arrindex);
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
                var Code = dbContext.Nationalities.Where(u => u.Code == id).Select(s => s.Code).FirstOrDefault();
                return RedirectToAction("Details", new { Code = Code });
            }

        }
        [AuthorizedRights(Screen = "Nationality", Right = "Read")]
        public ActionResult ExportToExcel()
        {
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var gv = new GridView();


            var List = (List<string>)TempData["NaTionForExc"];
            gv.DataSource = dbContext.Nationalities.Where(Del => List.Contains(Del.Code) && Del.DeletFlag == DeleteFlag.NotDeleted).Select(x => new {NationalityCode= x.Code, NationalityName =x.Name}).ToList();
            //gv.DataSource = (from s in dbContext.Nationalities
            //                 select new
            //                 {
            //                     s.Code,
            //                     s.Name


            //                 })
            //                     .ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Nationality" + NowTime + ".xls");
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
        [AuthorizedRights(Screen = "Nationality", Right = "Read")]
        public ActionResult ExportToPDF()
        {
            var Nationalities = dbContext.Nationalities.Select(s => s).Where(s => IDs.Contains(s.Code)).OrderBy(u => u.Name).ToList();
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var report = new PartialViewAsPdf("~/Areas/Nationalities/Views/Nationality/ExportToPDF.cshtml", Nationalities);
            return report;

        }
        [AuthorizedRights(Screen = "Nationality", Right = "Authorized")]
        public ActionResult AuthorizeNationality(string Code)
        {
            var Nationality = dbContext.Nationalities.Where(f => f.Code == Code).FirstOrDefault();
            Nationality.Auth = true;
            Nationality.Auther = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = Code });

        }
        [AuthorizedRights(Screen = "Nationality", Right = "Check")]
        public ActionResult CheckNationality(string Code)
        {
            var Nationality = dbContext.Nationalities.Where(f => f.Code == Code).FirstOrDefault();
            Nationality.Chk = true;
            Nationality.Checker = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = Code });

        }

        [AuthorizedRights(Screen = "Nationality", Right = "Delete")]
        public ActionResult Delete(string Code)
        {
            var Nationality = dbContext.Nationalities.Where(f => f.Code == Code).FirstOrDefault();
            Nationality.DeletFlag = DeleteFlag.Deleted;
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}