using ICP_ABC.Areas.Branches.Models;
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
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace ICP_ABC.Areas.Branches.Controllers
{
    [Authorize]
    public class BranchController : Controller
    {
        string servername = WebConfigurationManager.AppSettings["servername_URL"];
        private ApplicationDbContext dbContext = new ApplicationDbContext();
        UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(new ApplicationDbContext());
        static string[] IDs;

        public BranchController() { }


        //GetLastCode
        public string GetLastCode()
        {
            var querey = "SELECT MAX(CAST(BranchID AS INT)) as MaxCode FROM Branch";
            var appSettings = ConfigurationManager.ConnectionStrings;
            var SqlCon = appSettings["ICPRO"];
            var ReturnedData = SqlHelper.ExecuteDataset(SqlCon.ToString(), CommandType.Text, querey);
            var LastCode = ReturnedData.Tables[0].Rows[0][0].ToString();
            return LastCode;
        }
        // GET: Group
        [AuthorizedRights(Screen = "Branch", Right = "Read")]
        public ActionResult Index()
        {
            return View();
        }
        [AuthorizedRights(Screen = "Branch", Right = "Create")]
        public ActionResult Create()
        {
            //var LastCode = dbContext.Branches.OrderByDescending(s => s.branchcode).Select(s => s.branchcode).FirstOrDefault();
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
        [AuthorizedRights(Screen = "Branch", Right = "Create")]
        public ActionResult Create(CreateViewModel model)
        {
            var isValid = dbContext.Branches.Where(f => f.branchcode == model.branchcode).FirstOrDefault();
            if (isValid != null)
            {
                string[] errors = new string[] { "Code Is Already Exist" };
                IdentityResult result = new IdentityResult(errors);
                AddErrors(result);
                return View(model);
            }

            var Level = dbContext.UserSecurities.Select(Val=>Val.Levels).FirstOrDefault();
           

            if (Level == Levels.TwoLevels)
            {
                if (ModelState.IsValid)
                {
                    Branch Branch = new Branch
                    {
                        branchcode = model.branchcode,

                        BName = model.BName,
                        Maker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        Checker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        Chk = true, //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        UserID = User.Identity.GetUserId()// dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                    };
                    dbContext.Branches.Add(Branch);
                    dbContext.SaveChanges();
                }
                else
                {
                    return View(model);
                }
                return RedirectToAction("Details", new { Code = model.BranchID.ToString() });
            }
            else
            {
                if (ModelState.IsValid)
                {
                    Branch Branch = new Branch
                    {
                        branchcode = model.branchcode,

                        BName = model.BName,
                        Maker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        UserID = User.Identity.GetUserId()// dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                    };
                    dbContext.Branches.Add(Branch);
                    dbContext.SaveChanges();
                }
                else
                {
                    return View(model);
                }
                return RedirectToAction("Details", new { Code = model.BranchID.ToString() });
            }

           
            //return RedirectToAction("Search");
        }

        [AuthorizedRights(Screen = "Branch", Right = "Update")]
        public ActionResult Edit(string Code)
        {
            var Id = Convert.ToInt32(Code);
            var currentBranch = dbContext.Branches
                   .Where(g => g.BranchID == Id)
                   .FirstOrDefault();
            EditViewModel model = new EditViewModel
            {
                BranchID=Id,
                branchcode = currentBranch.branchcode,
                BName = currentBranch.BName
            };
            return View(model);
        }
        [HttpPost]
        [AuthorizedRights(Screen = "Branch", Right = "Update")]
        public ActionResult Edit(EditViewModel model)
        {
            var currentBranch = dbContext.Branches.Where(g => g.BranchID == model.BranchID).FirstOrDefault();
            currentBranch.BName = model.BName;
            currentBranch.SysDate = DateTime.Now;
            currentBranch.EditFlag = true;
            currentBranch.UserID = User.Identity.GetUserId();
            dbContext.SaveChanges();
            //return RedirectToAction("Search");
            return RedirectToAction("Details", new { Code = model.BranchID.ToString() });
        }

        [AuthorizedRights(Screen = "Branch", Right = "Read")]
        public ViewResult Search(string sortOrder, string currentFilter, string searchString, int? page, string Code,string RadioCHeck)
        {

            //ViewBag.SecuLevel = dbContext.UserSecurities.Select(Val => Val.Levels).FirstOrDefault();
            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;
            if (page == null && sortOrder == null && currentFilter == null && searchString == null  && Code == null && RadioCHeck == null )
            {
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                var Branches = dbContext.Branches.Where(c => c.DeletFlag == DeleteFlag.NotDeleted).Take(0);

                return View(Branches.ToPagedList(pageNumber, pageSize));

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

                var Branches = dbContext.Branches.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);
                if (!String.IsNullOrEmpty(searchString))
                {
                    Branches = Branches.Where(s => s.BName.Contains(searchString));
                    ViewData["searchString"] = searchString;
                }

                //checkRadio 
                if (!String.IsNullOrEmpty(RadioCHeck))
                {
                    //Case Auth
                    if (RadioCHeck == "1")
                    {
                        Branches = Branches.Where(s => s.Auth == true);
                        
                    }
                    //Case Checker
                    if (RadioCHeck == "2")
                    {
                        Branches = Branches.Where(s => s.Chk == true && s.Auth == false);

                    }
                    //Case maker
                    if (RadioCHeck == "3")
                    {
                        Branches = Branches.Where(s => s.Auth == false && s.Chk == false);

                    }
                    //Case All
                    if (RadioCHeck == "4")
                    {
                        Branches = Branches.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);

                    }

               
                }
                //

                TempData["BranchForExc"] = Branches.Select(x => x.BranchID).ToList();
                if (!String.IsNullOrEmpty(Code))
                {
                    Branches = Branches.Where(s => s.branchcode.Contains(Code));
                    ViewData["Code"] = Code;
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        Branches = Branches.OrderByDescending(s => s.BName);
                        break;
                    case "Code":
                        Branches = Branches.OrderBy(s => s.branchcode);
                        break;
                    case "code_desc":
                        Branches = Branches.OrderByDescending(s => s.branchcode);
                        break;
                    default:  // Name ascending 
                        Branches = Branches.OrderBy(s => s.BranchID);
                        break;
                }
                //int count = Branches.ToList().Count();
                //IDs = new string[count];
                //IDs = Branches.Select(s => s.Code).ToArray();

                int pageSize = 4;
                int pageNumber = (page ?? 1);
                ViewData["radioCHeck"] = RadioCHeck;
                int count = Branches.ToList().Count();
                IDs = new string[count];
                IDs = Branches.Select(s => s.BranchID.ToString()).ToArray();

                //int[] intarray = { 1, 2, 3, 4, 5 };
                //string[] result = intarray.Select(x => x.ToString()).ToArray();


                int[] myInts = Array.ConvertAll(IDs, s => int.Parse(s));
                Array.Sort(myInts);
                IDs = myInts.Select(x => x.ToString()).ToArray();

                return View(Branches.ToPagedList(pageNumber, pageSize));
            }
        }

        [AuthorizedRights(Screen = "Branch", Right = "Read")]
        public ViewResult Details(string Code)
        {

            var CodeCheck = Convert.ToInt32(Code);
            var Model = dbContext.Branches.Where(c => c.BranchID == CodeCheck).FirstOrDefault();
           
           
            if(Model.Auth != null) {
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



            DetailsViewModel CurrentBranch = new DetailsViewModel
            {
                branchcode = Model.branchcode,
                BName = Model.BName,
                Auth = Model.Auth,
                Check= Model.Chk,
                BranchID= Model.BranchID,
                AuthForEditAndDelete =Actualy_auth


            };
            if (IDs != null)
            {
                if (Model.BranchID.ToString() == IDs.Last())
                {
                    TempData["Last"] = "Last";
                }
                if (Model.BranchID.ToString() == IDs.First())
                {
                    TempData["First"] = "First";
                }
            }
            else
            {
                TempData["Last"] = "Last";
                TempData["First"] = "First";
            }
        
            return View(CurrentBranch);
            }
            else
            {
                return View();
            }
        }

        [AuthorizedRights(Screen = "Branch", Right = "Read")]
        public ActionResult Next(string id)
        {
            var arrlenth = IDs.Count();
            var LastObj = dbContext.Branches.OrderBy(s => s.BranchID).ToList().LastOrDefault(s => s.BranchID.ToString() == IDs[arrlenth - 1]);
            if (id == LastObj.BranchID.ToString())
            {
                TempData["Last"] = "Last";
                return RedirectToAction("Details", new { Code = LastObj.BranchID.ToString() });
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
                var Code = dbContext.Branches.Where(u => u.BranchID.ToString() == id).Select(s => s.BranchID).FirstOrDefault();
                if (Code == LastObj.BranchID)
                {
                    TempData["Last"] = "Last";
                }
                else
                {
                    TempData["Last"] = "NotLast";
                }
                return RedirectToAction("Details", new { Code = Code.ToString() });
            }

        }
        [AuthorizedRights(Screen = "Branch", Right = "Read")]
        public ActionResult Previous(string id)
        {
            var arrindex = IDs[0];
            var FirstObj = dbContext.Branches.OrderBy(s => s.BranchID).FirstOrDefault(s => s.BranchID.ToString() == arrindex);
            if (id == FirstObj.BranchID.ToString())
            {
                TempData["First"] = "First";
                return RedirectToAction("Details", new { Code = FirstObj.BranchID.ToString() });
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
                if (id == FirstObj.BranchID.ToString())
                {
                    TempData["First"] = "First";
                }
                else
                {
                    TempData["First"] = "NotFirst";
                }
                var Code = dbContext.Branches.Where(u => u.BranchID.ToString() == id).Select(s => s.BranchID).FirstOrDefault();
                return RedirectToAction("Details", new { Code = Code.ToString() });
            }

        }

        [AuthorizedRights(Screen = "Branch", Right = "Read")]
        public ActionResult ExportToExcel()
        {
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var gv = new GridView();
            var List = (List<int>)TempData["BranchForExc"];
            gv.DataSource = dbContext.Branches.Where(Del => List.Contains(Del.BranchID) && Del.DeletFlag == DeleteFlag.NotDeleted).Select(x => new { Code = x.BranchID, x.branchcode, x.BName }).ToList();
            //gv.DataSource = (from s in dbContext.Branches
            //                 select new
            //                 {
            //                     s.branchcode,
            //                     s.BName


            //                 })
            //                     .ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Branch" + NowTime + ".xls");
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
        [AuthorizedRights(Screen = "Branch", Right = "Read")]
        public ActionResult ExportToPDF()
        {
            var branches = dbContext.Branches.Select(s => s).Where(s => IDs.Contains(s.BranchID.ToString())).OrderBy(u => u.BName).ToList();
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");

            var report = new PartialViewAsPdf("~/Areas/Branches/Views/Branch/ExportToPDF.cshtml", branches);
            return report;

        }

        [AuthorizedRights(Screen = "Branch", Right = "Authorized")]
        public ActionResult AuthorizeBranch(string Code)
        {
            var Branch = dbContext.Branches.Where(f => f.BranchID.ToString() == Code).FirstOrDefault();
            Branch.Auth = true;
            Branch.Auther = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details",new { Code = Code });
        }

        [AuthorizedRights(Screen = "Branch", Right = "Check")]
        public ActionResult CheckBranch(string Code)
        {
            var Branch = dbContext.Branches.Where(f => f.BranchID.ToString() == Code).FirstOrDefault();
            Branch.Chk = true;
            Branch.Checker = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = Code });

        }

        [AuthorizedRights(Screen = "Branch", Right = "Delete")]
        public ActionResult Delete(string Code)
        {
            var Id = Convert.ToInt32(Code);
            var Branch = dbContext.Branches.Where(f => f.BranchID == Id).FirstOrDefault();
           Branch.DeletFlag = DeleteFlag.Deleted;
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


    }
}