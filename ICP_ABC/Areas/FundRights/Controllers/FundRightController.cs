using ICP_ABC.Areas.FundRights.Models;
using ICP_ABC.Areas.GroupsRights.Models;
using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using PagedList;
using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Extentions;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Validation;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using Rotativa;
using ICP_ABC.Areas.UsersSecurity.Models;

namespace ICP_ABC.Areas.FundRights.Controllers
{
    [Authorize]
    public class FundRightController : Controller
    {
        
        private ApplicationDbContext dbContext = new ApplicationDbContext();
        static int[] IDs;
        // GET: FundRights/FundRight
        public ActionResult Index()
        {
            return View();
        }
        [AuthorizedRights(Screen = "fundright", Right = "Create")]
        public ActionResult Create()
        {

            ViewData["Groups"] = new SelectList(dbContext.UserGroups.Where(g => g.HasFundRight == false).ToList(), "Code", "Name");
            var funds = dbContext.Funds.ToList();
            ViewData["Funds"] = funds;
            return View();
        }
        [HttpPost]
        [AuthorizedRights(Screen = "fundright", Right = "Create")]
        public ActionResult Create(string Rights)
        {
            var JsonRightsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FundRightsViewModel>>(Rights);

            List<FundRight> rights = new List<FundRight>();

            var Level = dbContext.UserSecurities.Select(Val => Val.Levels).FirstOrDefault();
            if (Level == Levels.TwoLevels)
            {
                foreach (var item in JsonRightsList)
                {
                    FundRight model = new FundRight
                    {
                        Code = item.Code,
                        GroupID = int.Parse(item.GroupID),
                        FundID = int.Parse(item.FundId),
                        Auth = false,
                        Chk = true,
                        DeletFlag = DeleteFlag.NotDeleted,
                        Maker = User.Identity.GetUserId(),
                        Checker = User.Identity.GetUserId(),
                        EditFlag = false,
                        UserID = User.Identity.GetUserId(),
                        FundRightAuth = true

                    };
                    rights.Add(model);

                }

                //set the usergroup preoperty has rights to true
                int CurrentGroupId = int.Parse(rights[0].Code);
                var userGroup = dbContext.UserGroups.Where(g => g.GroupID == CurrentGroupId).FirstOrDefault();
                userGroup.HasFundRight = true;
                dbContext.FundRights.AddRange(rights);
                dbContext.SaveChanges();
                return Json(new { success = true, responseText = "Added" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                foreach (var item in JsonRightsList)
                {
                    FundRight model = new FundRight
                    {
                        Code = item.Code,
                        GroupID = int.Parse(item.GroupID),
                        FundID = int.Parse(item.FundId),
                        Auth = false,
                        Chk = false,
                        DeletFlag = DeleteFlag.NotDeleted,
                        Maker = User.Identity.GetUserId(),
                        EditFlag = false,
                        UserID = User.Identity.GetUserId(),
                        FundRightAuth = true

                    };
                    rights.Add(model);

                }

                //set the usergroup preoperty has rights to true
                int CurrentGroupId = rights[0].GroupID;
                var userGroup = dbContext.UserGroups.Where(g => g.GroupID == CurrentGroupId).FirstOrDefault();
                userGroup.HasFundRight = true;
                dbContext.FundRights.AddRange(rights);
                dbContext.SaveChanges();
                return Json(new { success = true, responseText = "Added" }, JsonRequestBehavior.AllowGet);
            }

           
        }
        [AuthorizedRights(Screen = "fundright", Right = "Read")]
        public ViewResult Search(string sortOrder, string currentFilter, string searchString, int? page,string Code,string RadioCHeck)
        {

            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;
            if (page == null && sortOrder == null && currentFilter == null && searchString == null  && Code == null  && RadioCHeck == null )
            {
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                var groups = dbContext.UserGroups.Include(g => g.FundRights).Where(g => g.HasFundRight == true).Take(0);
               
                return View(groups.ToPagedList(pageNumber, pageSize));

            }
            else
            {


                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

                //if (searchString != null)
                //{
                //    page = 1;
                //}
                //else
                //{
                //    searchString = currentFilter;
                //}

                ViewBag.CurrentFilter = searchString;

                var groups = dbContext.UserGroups.Include(g => g.FundRights).Where(g => g.HasFundRight == true);
                if (!String.IsNullOrEmpty(searchString))
                {
                    groups = groups.Where(s => s.Name.Contains(searchString));
                    ViewData["searchString"] = searchString;
                }

                //checkRadio 
                if (!String.IsNullOrEmpty(RadioCHeck))
                {
                    //Case Auth
                    if (RadioCHeck == "1")
                    {
                        groups = groups.Where(s => s.Auth == true);

                    }
                    //Case Checker
                    if (RadioCHeck == "2")
                    {
                        groups = groups.Where(s => s.Chk == true && s.Auth == false);

                    }
                    //Case maker
                    if (RadioCHeck == "3")
                    {
                        groups = groups.Where(s => s.Auth == false && s.Chk == false);

                    }
                    //Case All
                    if (RadioCHeck == "4")
                    {
                        groups = groups.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);

                    }


                }
                //

                if (!String.IsNullOrEmpty(Code))
                {
                    groups = groups.Where(s => s.Code.Contains(Code));
                    ViewData["Code"] = Code;
                }

                TempData["FundrightForExc"] = groups.Select(x => x.GroupID).ToList();
                switch (sortOrder)
                {
                    case "name_desc":
                        groups = groups.OrderByDescending(s => s.Name);
                        break;
                    case "date_desc":
                        groups = groups.OrderByDescending(s => s.HasGroupRight);
                        break;
                    default:  // Name ascending 
                        //groups = groups.OrderBy(s => s.Name);
                        groups = groups.OrderBy(s => s.GroupID);
                        break;
                }

                int pageSize = 4;
                int pageNumber = (page ?? 1);
                ViewData["radioCHeck"] = RadioCHeck;
                int count = groups.ToList().Count();
                IDs = new int[count];
                IDs = groups.Select(s => s.GroupID).ToArray();
                //int[] myInts = Array.ConvertAll(IDs, s => int.Parse(s));
                var v = groups.Count();
                Array.Sort(IDs);
                //IDs = myInts.Select(x => x.ToString()).ToArray();
                return View(groups.ToPagedList(pageNumber, pageSize));
            }
        }
        [AuthorizedRights(Screen = "fundright", Right = "Update")]
        public ActionResult Edit(int Id)
        {
           
            ViewData["Groups"] = new SelectList(dbContext.UserGroups
                                .Where(g => g.GroupID == Id)
                                .ToList(), "GroupID", "Name", Id);

            ViewData["Code"] = dbContext.UserGroups
                                .Where(g => g.GroupID == Id).Select(x=>x.Code).FirstOrDefault();
            //ViewData["AllFundsExceptChosen"] = dbContext.Funds.Where(f=>f.FundID != ).ToList();

            var rights = dbContext.FundRights.Where(fr => fr.GroupID == Id).ToList();
            var funds = dbContext.Funds.ToList();
            List<Fund> chosenFunds = new List<Fund>();
            List<Fund> UnchoosedFunds = new List<Fund>();
            
            for (int x = 0; x < rights.Count; x++)
            {
                for (int i = 0; i < funds.Count; i++)
                {
                    if (rights[x].FundID==funds[i].FundID)
                    {
                        chosenFunds.Add(funds[i]);                          
                    }
                }
            }

            UnchoosedFunds = funds.Except(chosenFunds).ToList();
            ViewData["chosenFunds"] = chosenFunds;
            ViewData["UnchoosedFunds"] = UnchoosedFunds;
            return View(rights);
        }

        [HttpPost]
        [AuthorizedRights(Screen = "fundright", Right = "Update")]
        public ActionResult Edit(string Rights, string Code)
        {
            var JsonRightsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FundRightsViewModel>>(Rights);
            //string RightsCode = JsonRightsList[0].Code;
            var OldRights = dbContext.FundRights.Where(gr => gr.Code == Code).ToList();
            dbContext.FundRights.RemoveRange(OldRights);
            List<FundRight> rights = new List<FundRight>();
            foreach (var item in JsonRightsList)
            {
                FundRight model = new FundRight
                {
                    Code = item.Code,
                    GroupID = int.Parse(item.GroupID),
                    FundID = int.Parse(item.FundId),
                    FundRightAuth = true,
                    Maker = User.Identity.GetUserId(),
                    Auth = false,
                    Chk = false,
                    DeletFlag = DeleteFlag.NotDeleted,
                    EditFlag = true,
                    UserID = User.Identity.GetUserId()

                };
                rights.Add(model);

            }
            
            //set the usergroup preoperty has rights to true
            if (rights.Count>0)
            {
                int CurrentGroupId = rights[0].GroupID;
                var userGroup = dbContext.UserGroups.Where(g => g.GroupID == CurrentGroupId).FirstOrDefault();
                userGroup.HasFundRight = true;
                dbContext.FundRights.AddRange(rights);
               
            }
            else
            {
                int GroupCode = int.Parse(Code);
                var userGroup = dbContext.UserGroups.Where(g => g.GroupID == GroupCode).FirstOrDefault();
                userGroup.HasFundRight = false;
            }
            try
            {
                dbContext.SaveChanges();
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

            return Json(new { success = true, responseText = "Added" }, JsonRequestBehavior.AllowGet);
        }
        [AuthorizedRights(Screen = "fundright", Right = "Read")]
        public ActionResult Details(int Id)
        {

            ViewData["Groups"] = new SelectList(dbContext.UserGroups
                                .Where(g => g.GroupID == Id)
                                .ToList(), "GroupID", "Name", Id);
             var group= dbContext.UserGroups.Where(u=>u.GroupID==Id).FirstOrDefault();
            ViewData["Code"] = dbContext.UserGroups.Where(u => u.GroupID == Id).Select(x=>x.Code).FirstOrDefault();
            var rights = dbContext.FundRights.Where(g => g.GroupID == Id).ToList();

            ViewBag.Actualy_auth = rights.FirstOrDefault().Auth;
            var ThisUser = User.Identity.GetUserId();

            if (rights.FirstOrDefault().Maker == ThisUser)
            {
                if (rights.FirstOrDefault().Chk == true && rights.FirstOrDefault().Auth == false)
                {
                    rights.FirstOrDefault().Chk = true;
                    rights.FirstOrDefault().Auth = false;
                }
                if (rights.FirstOrDefault().Chk == false && rights.FirstOrDefault().Auth == false)
                {
                    rights.FirstOrDefault().Chk = true;
                    rights.FirstOrDefault().Auth = true;

                }
            }
            else
            {
                if (rights.FirstOrDefault().Chk == true && rights.FirstOrDefault().Auth == true)
                {
                    rights.FirstOrDefault().Chk = true;
                    rights.FirstOrDefault().Auth = true;

                }

                else if (rights.FirstOrDefault().Chk == true && rights.FirstOrDefault().Auth == false)
                {
                    if (rights.FirstOrDefault().Checker != ThisUser)
                    {
                        rights.FirstOrDefault().Chk = true;
                        rights.FirstOrDefault().Auth = false;

                    }
                    else
                    {
                        rights.FirstOrDefault().Chk = true;
                        rights.FirstOrDefault().Auth = true;

                    }
                }
                else if (rights.FirstOrDefault().Chk == false && rights.FirstOrDefault().Auth == false)
                {
                    rights.FirstOrDefault().Chk = false;
                    rights.FirstOrDefault().Auth = true;

                }

            }


            var funds = dbContext.Funds.ToList();
            List<Fund> chosenFunds = new List<Fund>();
            for (int x = 0; x < rights.Count; x++)
            {
                for (int i = 0; i < funds.Count; i++)
                {
                    if (rights[x].FundID == funds[i].FundID)
                    {
                        chosenFunds.Add(funds[i]);
                    }
                }
            }
            ViewData["Funds"] = chosenFunds;
            if (IDs != null)
            {
                if (group.GroupID == IDs.Last())
                {
                    TempData["Last"] = "Last";
                }
                if (group.GroupID == IDs.First())
                {
                    TempData["First"] = "First";
                }
            }
            else
            {
                TempData["Last"] = "Last";
                TempData["First"] = "First";
            }


            return View(rights);
        }

        [AuthorizedRights(Screen = "fundright", Right = "Read")]
        public ActionResult Next(int Code)
        {
            var arrlenth = IDs.Count();
            var LastObj = dbContext.UserGroups.OrderBy(s => s.Code).ToList().LastOrDefault(s => s.GroupID == IDs[arrlenth - 1]);
            if (Code == LastObj.GroupID)
            {
                TempData["Last"] = "Last";
                return RedirectToAction("Details", new { Id = LastObj.GroupID });
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
                var code = dbContext.UserGroups.Where(u => u.GroupID == Code).Select(s => s.GroupID).FirstOrDefault();

                if (Code == LastObj.GroupID)
                {
                    TempData["Last"] = "Last";
                }
                else
                {
                    TempData["Last"] = "NotLast";
                }


                return RedirectToAction("Details", new { Id = code });
            }

        }

        [AuthorizedRights(Screen = "fundright", Right = "Read")]
        public ActionResult Previous(int Code)
        {
            var arrindex = IDs[0];
            var FirstObj = dbContext.UserGroups.OrderBy(s => s.Code).FirstOrDefault(s => s.GroupID == arrindex);
            if (Code == FirstObj.GroupID)
            {
                TempData["First"] = "First";
                return RedirectToAction("Details", new { Id = FirstObj.GroupID });
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

                if (Code == FirstObj.GroupID)
                {
                    TempData["First"] = "First";
                }
                else
                {
                    TempData["First"] = "NotFirst";
                }
                var code = dbContext.UserGroups.Where(u => u.GroupID == Code).Select(s => s.GroupID).FirstOrDefault();
                return RedirectToAction("Details", new { Id = code });
            }

        }

        [AuthorizedRights(Screen = "fundright", Right = "Read")]
        public ActionResult ExportToExcel()
        {
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var gv = new GridView();

            var List = (List<int>)TempData["FundrightForExc"];
            gv.DataSource = dbContext.FundRights.Where(Del => List.Contains(Del.GroupID) && Del.DeletFlag == DeleteFlag.NotDeleted).Select(x => new { Code =x.Code, GroupName=x.UserGroup.Name,x.Fund.Name }).OrderBy(u => u.Code).ToList();
            //gv.DataSource = dbContext.FundRights.Select(s => s).Where(s => List.Contains(s.GroupID)).OrderBy(u => u.Code).ToList();
            //gv.DataSource = (from s in dbContext.FundRights
            //                 select new
            //                 {
            //                     s.Code,
            //                   fundname =  s.Fund.Name,
            //                    GreoupName = s.UserGroup



            //                 })
            //                     .ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=FundRights" + NowTime + ".xls");
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
            ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "BranchID", "BName");
            return RedirectToAction("Search");
        }

        [AuthorizedRights(Screen = "fundright", Right = "Read")]
        public ActionResult ExportToPDF()
        {
            var List = (List<int>)TempData["FundrightForExc"];
            //var Rights = dbContext.FundRights.Where(Del => List.Contains(Del.GroupID) && Del.DeletFlag == DeleteFlag.NotDeleted).ToList();
            var Rights = dbContext.FundRights.Select(s => s).Where(s => List.Contains(s.GroupID)).Include(G=>G.UserGroup).Include(F=>F.Fund).ToList();


            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");

            var report = new PartialViewAsPdf("~/Areas/FundRights/Views/FundRight/ExportToPDF.cshtml", Rights);
            return report;

        }

        [AuthorizedRights(Screen = "fundright", Right = "Authorized")]
        public ActionResult AuthorizeFund(int Id)
        {
            //add true value to auth 
            //var UserGroup = dbContext.UserGroups.Where(f => f.GroupID == Id).FirstOrDefault();
            //UserGroup.Auth = true;
            var funds = dbContext.FundRights.Where(f => f.GroupID==Id).ToList();
            foreach (var item in funds)
            {
                item.Auther = User.Identity.GetUserId();
                item.Auth = true;
            }
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Id = Id });
        }
        [AuthorizedRights(Screen = "fundright", Right = "Check")]
        public ActionResult CheckFund(int Id)
        {
            //add true value to auth 
            //var UserGroup = dbContext.UserGroups.Where(f => f.GroupID == Id).FirstOrDefault();
            //UserGroup.Chk = true;
            var FundRights = dbContext.FundRights.Where(f => f.GroupID == Id).ToList();
            foreach (var item in FundRights)
            {
                item.Checker = User.Identity.GetUserId();
                item.Chk = true;
            }
            dbContext.SaveChanges();
            return RedirectToAction("Details",new { Id = Id });
        }
        [AuthorizedRights(Screen = "fundright", Right = "Delete")]
        public ActionResult Delete(int Id)
        {
            var Rights = dbContext.FundRights.Where(fr => fr.GroupID == Id).ToList();

            foreach (var item in Rights)
            {
                dbContext.FundRights.Remove(item);
            }
            var group = dbContext.UserGroups.Where(u => u.GroupID == Id).FirstOrDefault();
            group.HasFundRight = false;
            dbContext.SaveChanges();
            return RedirectToAction("Index");
            
            
        }

    }
}