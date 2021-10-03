using ICP_ABC.Areas.Group.Models;
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

namespace ICP_ABC.Areas.Group.Controllers
{
    [Authorize]
    public class GroupController : Controller
    {
        private ApplicationDbContext dbContext = new ApplicationDbContext();
        UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(new ApplicationDbContext());
        static string[] IDs;


        //GetLastCode
        public string GetLastCode()
        {
            var querey = "SELECT MAX(CAST(Code AS INT)) as MaxCode FROM UserGroup";
            var appSettings = ConfigurationManager.ConnectionStrings;
            var SqlCon = appSettings["ICPRO"];
            var ReturnedData = SqlHelper.ExecuteDataset(SqlCon.ToString(), CommandType.Text, querey);
            var LastCode = ReturnedData.Tables[0].Rows[0][0].ToString();
            return LastCode;
        }
        public ActionResult Index()
        {
            return View();
        }
        [AuthorizedRights(Screen = "Group", Right = "Create")]
        public ActionResult Create()
        {
            //var lastcode = dbContext.UserGroups.OrderByDescending(s => s.GroupID).Select(s => s.Code).FirstOrDefault();
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
        [AuthorizedRights(Screen = "Group", Right = "Create")]
        public ActionResult Create(CreateViewModel model)
        {
            var lastcode = GetLastCode();
            //var lastcode = dbContext.UserGroups.OrderByDescending(s => s.GroupID).Select(s => s.Code).FirstOrDefault();
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

                    UserGroup Group = new UserGroup
                    {
                        Code = lastcode,
                        Chk = true,
                        Name = model.Name,
                        Maker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        Checker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        UserID = User.Identity.GetUserId()// dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                    };
                    dbContext.UserGroups.Add(Group);
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

                    UserGroup Group = new UserGroup
                    {
                        Code = lastcode,
                        Name = model.Name,
                        Maker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        UserID = User.Identity.GetUserId()// dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                    };
                    dbContext.UserGroups.Add(Group);
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

        [AuthorizedRights(Screen = "Group", Right = "Update")]
        public ActionResult Edit(string Code)
        {
            var currentGroup = dbContext.UserGroups
                   .Where(g => g.Code == Code)
                   .FirstOrDefault();
            EditViewModel model = new EditViewModel
            {
                Code = currentGroup.Code,
                Name = currentGroup.Name
            };
            return View(model);
        }
        [HttpPost]
        [AuthorizedRights(Screen = "Group", Right = "Update")]
        public ActionResult Edit(EditViewModel model)
        {
            var currentGroup = dbContext.UserGroups.Where(g => g.Code == model.Code).FirstOrDefault();
            currentGroup.Code = model.Code;
            currentGroup.Name = model.Name;
            currentGroup.SysDate = DateTime.Now;
            currentGroup.EditFlag = true;
            currentGroup.UserID = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details",new { Code = model.Code });
        }

        [AuthorizedRights(Screen = "Group", Right = "Read")]
        public ViewResult Search(string sortOrder, string currentFilter, string searchString, int? page, string Code, string RadioCHeck)
        {
            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;

            if (page == null && sortOrder == null && currentFilter == null && searchString == null  && Code == null  && RadioCHeck == null )
            {
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                //var Users = dbContext.Users.ToList().Take(0);
                var userGroups = dbContext.UserGroups.ToList().Take(0);
                //ViewData["Groups"] = new SelectList(dbContext.UserGroups.ToList(), "GroupID", "Name");
                //ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "BranchID", "Name");
                return View(userGroups.ToPagedList(pageNumber, pageSize));

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

                var userGroups = dbContext.UserGroups.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);
                if (!String.IsNullOrEmpty(searchString))
                {
                    userGroups = userGroups.Where(s => s.Name.Contains(searchString));
                    ViewData["searchString"] = searchString;
                }


                //checkRadio 
                if (!String.IsNullOrEmpty(RadioCHeck))
                {
                    //Case Auth
                    if (RadioCHeck == "1")
                    {
                        userGroups = userGroups.Where(s => s.Auth == true);

                    }
                    //Case Checker
                    if (RadioCHeck == "2")
                    {
                        userGroups = userGroups.Where(s => s.Chk == true && s.Auth == false);

                    }
                    //Case maker
                    if (RadioCHeck == "3")
                    {
                        userGroups = userGroups.Where(s => s.Auth == false && s.Chk == false);

                    }
                    //Case All
                    if (RadioCHeck == "4")
                    {
                        userGroups = userGroups.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);

                    }


                }
                //

                if (!String.IsNullOrEmpty(Code))
                {
                    userGroups = userGroups.Where(s => s.Code.Contains(Code));
                    ViewData["Code"] = Code;

                }

                TempData["GroupForExc"] = userGroups.Select(x => x.GroupID).ToList();

                switch (sortOrder)
                {
                    case "name_desc":
                        userGroups = userGroups.OrderByDescending(s => s.Name);
                        break;
                    case "Code":
                        userGroups = userGroups.OrderBy(s => s.Code);
                        break;
                    case "code_desc":
                        userGroups = userGroups.OrderByDescending(s => s.Code);
                        break;
                    default:  // Name ascending 
                        userGroups = userGroups.OrderBy(s => s.GroupID);
                        break;
                }
                //int count = userGroups.ToList().Count();
                //IDs = new string[count];
                //IDs = userGroups.Select(s => s.Code).ToArray();

                int pageSize = 4;
                int pageNumber = (page ?? 1);
                ViewData["radioCHeck"] = RadioCHeck;

                int count = userGroups.ToList().Count();
                IDs = new string[count];
                IDs = userGroups.Select(s => s.Code).ToArray();
                int[] myInts = Array.ConvertAll(IDs, s => int.Parse(s));
                Array.Sort(myInts);
                IDs = myInts.Select(x => x.ToString()).ToArray();

                return View(userGroups.ToPagedList(pageNumber, pageSize));
            }
        }

        [AuthorizedRights(Screen = "Group", Right = "Read")]
        public ViewResult Details(string Code)
        {
            var Model = dbContext.UserGroups.Where(c => c.Code == Code).FirstOrDefault();
            var Actualy_auth = Model.Auth;
            var ThisUser = User.Identity.GetUserId();
            if (Model.Maker == ThisUser)
            {
                if (Model.Chk == true && Model.Auth == false) { 
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
                if (Model.Chk == true && Model.Auth == true) {
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


            //-----------------------------------------------------
            DetailsViewModel CurrentGroup = new DetailsViewModel
            {
                Code = Model.Code,
                Name = Model.Name,
                Check = Model.Chk,
                Auth = Model.Auth,
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

            return View(CurrentGroup);

        }

        [AuthorizedRights(Screen = "Group", Right = "Read")]
        public ActionResult Next(string id)
        {
            var arrlenth = IDs.Count();
            var LastObj = dbContext.UserGroups.OrderBy(s => s.Code).ToList().LastOrDefault(s => s.Code == IDs[arrlenth - 1]);
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
                var Code = dbContext.UserGroups.Where(u => u.Code == id).Select(s => s.Code).FirstOrDefault();

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
        [AuthorizedRights(Screen = "Group", Right = "Read")]
        public ActionResult Previous(string id)
        {
            var arrindex = IDs[0];
            var FirstObj = dbContext.UserGroups.OrderBy(s => s.Code).FirstOrDefault(s => s.Code == arrindex);
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
                var Code = dbContext.UserGroups.Where(u => u.Code == id).Select(s => s.Code).FirstOrDefault();
                return RedirectToAction("Details", new { Code = Code });
            }

        }
        [AuthorizedRights(Screen = "Group", Right = "Read")]
        public ActionResult ExportToExcel()
        {
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var gv = new GridView();
            var List = (List<int>)TempData["GroupForExc"];
            //var UserGroups = dbContext.UserGroups.Select(s => s).Where(s => List.Contains(s.GroupID)).OrderBy(u => u.Name).ToList();
            gv.DataSource = dbContext.UserGroups.Where(Del => List.Contains(Del.GroupID) && Del.DeletFlag == DeleteFlag.NotDeleted).Select(x => new { x.Code,GroupName= x.Name }).OrderBy(u => u.GroupName).ToList();

            //gv.DataSource = (from s in dbContext.UserGroups
            //                 select new
            //                 {
            //                     s.Code,
            //                     s.Name


            //                 })
            //                     .ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Group" + NowTime + ".xls");
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
        [AuthorizedRights(Screen = "Group", Right = "Read")]
        public ActionResult ExportToPDF()
        {
            var UserGroups = dbContext.UserGroups.Select(s => s).Where(s => IDs.Contains(s.Code)).OrderBy(u => u.Name).ToList();
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");

            var report = new PartialViewAsPdf("~/Areas/Group/Views/Group/ExportToPDF.cshtml", UserGroups);
            return report;

        }

        [AuthorizedRights(Screen = "Group", Right = "Authorized")]
        public ActionResult AuthorizeGroup(string Code)
        {
            var Group = dbContext.UserGroups.Where(f => f.Code == Code).FirstOrDefault();
            Group.Auth = true;
            Group.Auther = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = Code });
        }
        [AuthorizedRights(Screen = "Group", Right = "Check")]
        public ActionResult CheckGroup(string Code)
        {
           

            //var Group = dbContext.UserGroups.Where(f => f.Code == Code).FirstOrDefault();
            //var Maker = dbContext.UserGroups.Where(f => f.Code == Code).Select(s => s.Maker);
            //var Checker = dbContext.UserGroups.Where(f => f.Code == Code).Select(s => s.Checker);
            //var ThisUser = User.Identity.GetUserId();
            //if (Maker.ToString() == ThisUser)
            //{
            //    Group.Chk = false;
            //}
            //else if(Group.Chk != true && Group.Checker != ThisUser)
            //{
            //    Group.Chk = true;
            //}
            //--------------------
            var Group = dbContext.UserGroups.Where(f => f.Code == Code).FirstOrDefault();
            Group.Chk = true;
            Group.Checker = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = Code });
        }

        [AuthorizedRights(Screen = "Group", Right = "Delete")]
        public ActionResult Delete(string Code)
        {
            var Group = dbContext.UserGroups.Where(f => f.Code == Code).FirstOrDefault();
            Group.DeletFlag = DeleteFlag.Deleted;
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}