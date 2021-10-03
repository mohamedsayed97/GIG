using ICP_ABC.Areas.GroupsRights.Models;
using ICP_ABC.Models;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;
using ICP_ABC.Extentions;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Validation;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using Rotativa;
using System.Configuration;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using ICP_ABC.Areas.UsersSecurity.Models;

namespace ICP_ABC.Areas.GroupsRights.Controllers
{
    [Authorize]
    public class GroupRightController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext dbContext = new ApplicationDbContext();
        static int[] IDs;
        //public string GetLastCode()
        //{
        //    var querey = "SELECT MAX(CAST(branchcode AS INT)) as MaxCode FROM GroupRights";
        //    var appSettings = ConfigurationManager.ConnectionStrings;
        //    var SqlCon = appSettings["ICPRO"];
        //    var ReturnedData = SqlHelper.ExecuteDataset(SqlCon.ToString(), CommandType.Text, querey);
        //    var LastCode = ReturnedData.Tables[0].Rows[0][0].ToString();
        //    return LastCode;
        //}
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: GroupsRights/GroupRight
        public ActionResult Index()
        {
            return View();
        }
        // GET: GroupsRights/GroupRight/Create
        //[AuthorizedRights(Screen = "Grouprightes", Right = "Create")]
        [AuthorizedRights(Screen = "Grouprightes", Right = "Create")]
        public ActionResult Create()
        {
            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;
            ViewData["Groups"] = new SelectList(dbContext.UserGroups.Where(g => g.HasGroupRight == false ).Where(x=>x.DeletFlag == DeleteFlag.NotDeleted).ToList(), "Code", "Name");
            var screens = dbContext.Screens.ToList();
            ViewData["Screens"] = screens;
            return View();
        }

        [HttpPost]
        [AuthorizedRights(Screen = "Grouprightes", Right = "Create")]
        public ActionResult Create(string Rights)
        {
            var JsonRightsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RightsViewModel>>(Rights);

            List<GroupRight> rights = new List<GroupRight>();

            var Level = dbContext.UserSecurities.Select(Val => Val.Levels).FirstOrDefault();
            if (Level == Levels.TwoLevels)
            {
                foreach (var item in JsonRightsList)
                {
                    GroupRight model = new GroupRight
                    {
                        Code = item.Code,
                        Create = item.hasCreate,
                        Read = item.hasRead,
                        Update = item.hasUpdate,
                        Delete = item.hasDelete,
                        None = item.noneOfAll,
                        Check = item.Check,
                        Authorized = item.Authorize,
                        FormID = int.Parse(item.FormID),
                        EditFlag = false,
                        GroupId = int.Parse(item.GroupID),

                        Checker = User.Identity.GetUserId(),
                        Chk = true,

                        Maker = User.Identity.GetUserId()
                    };

                    if (model.None && (model.Create || model.Update || model.Delete) || model.Read && (model.Create || model.Update || model.Delete))
                    {
                        throw new System.InvalidOperationException("unLogical Rights");
                    }
                    rights.Add(model);

                }

                //set the usergroup preoperty has rights to true
                int CurrentGroupId = int.Parse(rights[0].Code);
                var userGroup = dbContext.UserGroups.Where(g => g.GroupID == CurrentGroupId).FirstOrDefault();
                userGroup.HasGroupRight = true;
                dbContext.GroupRights.AddRange(rights);

                dbContext.SaveChanges();
                return Json(new { success = true, responseText = "Added" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                foreach (var item in JsonRightsList)
                {
                    GroupRight model = new GroupRight
                    {
                        Code = item.Code,
                        Create = item.hasCreate,
                        Read = item.hasRead,
                        Update = item.hasUpdate,
                        Delete = item.hasDelete,
                        None = item.noneOfAll,
                        Check = item.Check,
                        Authorized = item.Authorize,
                        FormID = int.Parse(item.FormID),
                        EditFlag = false,
                        GroupId = int.Parse(item.GroupID),
                        Maker = User.Identity.GetUserId()
                    };

                    if (model.None && (model.Create || model.Update || model.Delete) || model.Read && (model.Create || model.Update || model.Delete))
                    {
                        throw new System.InvalidOperationException("unLogical Rights");
                    }
                    rights.Add(model);

                }

                //set the usergroup preoperty has rights to true
                //int CurrentGroupId = int.Parse(rights[0].Code);
                int CurrentGroupId = rights[0].GroupId;
                //var userGroup = dbContext.UserGroups.Where(g => g.GroupID == rights[0].GroupId).FirstOrDefault();
                var userGroup = dbContext.UserGroups.Where(g => g.GroupID == CurrentGroupId).FirstOrDefault();
                userGroup.HasGroupRight = true;
                dbContext.GroupRights.AddRange(rights);

                dbContext.SaveChanges();
                return Json(new { success = true, responseText = "Added" }, JsonRequestBehavior.AllowGet);
            }

           
        }

        [AuthorizedRights(Screen = "Grouprightes", Right = "Read")]
        public ViewResult Search(string sortOrder, string currentFilter, string searchString, int? page,string Code,string RadioCHeck)
        {
            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;

            if (page == null && sortOrder == null && currentFilter == null && searchString == null && Code == null&& RadioCHeck == null )
            {
                int pageSize = 3;
                int pageNumber = (page ?? 1);
                var groups = dbContext.UserGroups.Include(g => g.groupRights).Where(g => g.HasGroupRight == true).Take(0);
            
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

                var groups = dbContext.UserGroups.Include(g => g.groupRights).Where(g => g.HasGroupRight == true);
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
                TempData["GrouprightForExc"] = groups.Select(x => x.GroupID).ToList();
                switch (sortOrder)
                {
                    case "name_desc":
                        groups = groups.OrderByDescending(s => s.Name);
                        break;
                    case "date_desc":
                        groups = groups.OrderByDescending(s => s.HasGroupRight);
                        break;
                    default:  // Name ascending 
                         
                        groups = groups.OrderBy(s => s.GroupID);
                        break;
                }

                int pageSize = 3;
                int pageNumber = (page ?? 1);
                ViewData["radioCHeck"] = RadioCHeck;
                int count = groups.ToList().Count();
                IDs = new int[count];
                IDs = groups.Select(s => s.GroupID).ToArray();
                Array.Sort(IDs);
                return View(groups.ToPagedList(pageNumber, pageSize));
            }
        }

        [AuthorizedRights(Screen = "Grouprightes", Right = "Update")]
        public ActionResult Edit(int Id)
        {
            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;
            ViewData["Groups"] = new SelectList(dbContext.UserGroups
                                .Where(g => g.GroupID == Id)
                                .ToList(), "GroupID", "Name", Id);

            //ViewData["Code"] = Id;
            ViewData["Code"] = dbContext.UserGroups.Where(g => g.GroupID == Id).Select(x=>x.Code).FirstOrDefault();
            //ViewBag.GrpRightsScreens = dbContext.GroupRights
            //                                .Include(g => g.Screen)
            //                                 .Where(g => g.Code == Id.ToString()).ToList();
            var ListOfR = dbContext.GroupRights
                                            .Include(g => g.Screen)
                                             .Where(g => g.GroupId == Id).ToList();
            EditViewModel Model = new EditViewModel
            {
               GroupRight = ListOfR,Code= Id.ToString(),
                GId = Id
            };
            return View(Model);
        }

        [HttpPost]
        [AuthorizedRights(Screen = "Grouprightes", Right = "Update")]
        public ActionResult Edit(string Rights)
        {
            var JsonRightsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RightsViewModel>>(Rights);
            string RightsCode = JsonRightsList[0].Code;
            var OldRights = dbContext.GroupRights.Where(gr => gr.Code == RightsCode).ToList();
            
            List<GroupRight> rights = new List<GroupRight>();
            foreach (var item in JsonRightsList)
            {
                GroupRight model = new GroupRight
                {
                    Code = item.Code,
                    Create = item.hasCreate,
                    Read = item.hasRead,
                    Update = item.hasUpdate,
                    Delete = item.hasDelete,
                    None = item.noneOfAll,
                    Check = item.Check,
                    Authorized = item.Authorize,
                    FormID = int.Parse(item.FormID),
                    EditFlag = true,
                    GroupId = int.Parse(item.GroupID),
                    UserId = User.Identity.GetUserId(),
                    Maker= OldRights.First().Maker
                };

                if (model.None && (model.Create || model.Update || model.Delete) || model.Read && (model.Create || model.Update || model.Delete))
                {
                    throw new System.InvalidOperationException("unLogical Rights");
                }
                rights.Add(model);

            }
            
            //set the usergroup preoperty has rights to true
            int GrpID = rights[0].GroupId;
            var userGroup = dbContext.UserGroups.Where(g => g.GroupID == GrpID).FirstOrDefault();
            userGroup.HasGroupRight = true;

            
            try
            {
                dbContext.GroupRights.RemoveRange(OldRights);
                dbContext.GroupRights.AddRange(rights);
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

        [AuthorizedRights(Screen = "Grouprightes", Right = "Read")]
        public ActionResult Details(int Id)

        {
          
            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;
            ViewData["Groups"] = new SelectList(dbContext.UserGroups
                                .Where(g => g.GroupID == Id)
                                .ToList(), "GroupID", "Name", Id);

            ViewData["Code"] = dbContext.UserGroups.Where(g => g.GroupID == Id).Select(x => x.Code).FirstOrDefault();
            ViewData["GrpRightsScreens"] = dbContext.GroupRights
                                            .Include(g => g.Screen)
                                             .Where(g => g.GroupId == Id)
                                             .ToList();

            var Model = dbContext.GroupRights
                                            .Include(g => g.Screen)
                                             .Where(g => g.GroupId == Id)
                                             .ToList();
            var Actualy_auth = Model.FirstOrDefault().Auth;
            ViewBag.Actualy_auth = Actualy_auth;
            var ThisUser = User.Identity.GetUserId();
            if (Model.FirstOrDefault().Maker == ThisUser)
            {
                if (Model.FirstOrDefault().Chk == true && Model.FirstOrDefault().Auth == false)
                {
                    Model.FirstOrDefault().Chk = true;
                    Model.FirstOrDefault().Auth = false;
                }
                if (Model.FirstOrDefault().Chk == false && Model.FirstOrDefault().Auth == false)
                {
                    Model.FirstOrDefault().Chk = true;
                    Model.FirstOrDefault().Auth = true;

                }
            }
            else
            {
                if (Model.FirstOrDefault().Chk == true && Model.FirstOrDefault().Auth == true)
                {
                    Model.FirstOrDefault().Chk = true;
                    Model.FirstOrDefault().Auth = true;

                }

                else if (Model.FirstOrDefault().Chk == true && Model.FirstOrDefault().Auth == false)
                {
                    if (Model.FirstOrDefault().Checker != ThisUser)
                    {
                        Model.FirstOrDefault().Chk = true;
                        Model.FirstOrDefault().Auth = false;

                    }
                    else
                    {
                        Model.FirstOrDefault().Chk = true;
                        Model.FirstOrDefault().Auth = true;

                    }
                }
                else if (Model.FirstOrDefault().Chk == false && Model.FirstOrDefault().Auth == false)
                {
                    Model.FirstOrDefault().Chk = false;
                    Model.FirstOrDefault().Auth = true;

                }

            }

            if (IDs != null)
            {
                if (Id == IDs.Last())
                {
                    TempData["Last"] = "Last";
                }
                if (Id== IDs.First())
                {
                    TempData["First"] = "First";
                }
            }
            else
            {
                TempData["Last"] = "Last";
                TempData["First"] = "First";
            }



            return View(Model);
        }
        [AuthorizedRights(Screen = "Grouprightes", Right = "Authorized")]
        public ActionResult AuthorizeRights(int ID)
        {
            var groupRights = dbContext.GroupRights.Where(f => f.GroupId == ID).ToList();
            foreach (var item in groupRights)
            {
                item.Auther = User.Identity.GetUserId();
                item.Auth = true;
            }
            dbContext.SaveChanges();
            return RedirectToAction("Details",new {Id = ID });
        }
        [AuthorizedRights(Screen = "Grouprightes", Right = "Check")]
        public ActionResult CheckRights(int ID)
        {
            
            var groupRights = dbContext.GroupRights.Where(f => f.GroupId == ID).ToList();
            foreach (var item in groupRights)
            {
                item.Checker = User.Identity.GetUserId();
                item.Chk = true;
            }
            dbContext.SaveChanges();
            return RedirectToAction("Details",new {Id = ID });

        }
        [AuthorizedRights(Screen = "Grouprightes", Right = "Delete")]
        public ActionResult Delete(int Id)
        {
            SignInStatus result = new SignInStatus();
            var Rights = dbContext.GroupRights.Where(g => g.GroupId == Id).ToList();
            if (!Rights[0].Auth)
            {
                //foreach (var item in Rights)
                //{
                //    item.DeletFlag = DeleteFlag.Deleted;
                //}
                dbContext.GroupRights.RemoveRange(Rights);
                var group = dbContext.UserGroups.Where(u => u.GroupID == Id).FirstOrDefault();
                group.HasGroupRight = false;
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            result = SignInStatus.LockedOut;
            string[] errors = new string[] { "UnAuth This rirth then delete it" };
            IdentityResult LoginResult = new IdentityResult(errors);
            AddErrors(LoginResult);
            return RedirectToAction("Detailsd", Id);
        }

        [AuthorizedRights(Screen = "Grouprightes", Right = "Read")]
        public ActionResult Next(int id)
        {
            var arrlenth = IDs.Count();
            var G_Code = IDs[arrlenth - 1];
            //var LastObj = dbContext.GroupRights.OrderBy(s => s.Code).ToList().LastOrDefault(s => s.Code == IDs[arrlenth - 1]);
            var LastObj = dbContext.GroupRights.Include(g => g.Screen).Where(g => g.GroupId == G_Code).ToList();
            if (id == LastObj.FirstOrDefault().GroupId)
            {
                TempData["Last"] = "=Last";
                return RedirectToAction("Details", new { Id = LastObj.FirstOrDefault().GroupId });
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
                var ID = dbContext.GroupRights.Where(u => u.GroupId == id).Select(s => s.GroupId).FirstOrDefault();
                if (id == LastObj.FirstOrDefault().GroupId)
                {
                    TempData["Last"] = "Last";
                }
                else
                {
                    TempData["Last"] = "NotLast";
                }

                return RedirectToAction("Details", new { Id = ID });
            }

        }
        [AuthorizedRights(Screen = "Grouprightes", Right = "Read")]
        public ActionResult Previous(int id)
        {
            var arrindex = IDs[0];
            //var FirstObj = dbContext.GroupRights.OrderBy(s => s.Code).FirstOrDefault(s => s.UserGroups.Code == arrindex);
            var FirstObj = dbContext.GroupRights.Include(g => g.Screen).Where(g => g.GroupId == arrindex).ToList();
            if (id == FirstObj.FirstOrDefault().GroupId)
            {
                TempData["First"] = "=First";
                return RedirectToAction("Details", new { Id = FirstObj.FirstOrDefault().GroupId });
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

                if (id == FirstObj.FirstOrDefault().GroupId)
                {
                    TempData["First"] = "First";
                }
                else
                {
                    TempData["First"] = "NotFirst";
                }
                var ID = dbContext.GroupRights.Where(u => u.GroupId == id).Select(s => s.GroupId).FirstOrDefault();
                return RedirectToAction("Details", new { Id = ID });
            }

        }
        [AuthorizedRights(Screen = "Grouprightes", Right = "Read")]
        public ActionResult ExportToExcel()
        {
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var gv = new GridView();
            var List = (List<int>)TempData["GrouprightForExc"];
            gv.DataSource = dbContext.GroupRights.Where(Del => List.Contains(Del.GroupId) && Del.DeletFlag == DeleteFlag.NotDeleted).Select(x => new { x.Code, x.Screen.Name, Create=x.Create,Read=x.Read,Update=x.Update,Delete=x.Delete }).OrderBy(u => u.Code).ToList();
            var GroupRights = dbContext.GroupRights.Select(s => s).Where(s => List.Contains(s.GroupId)).OrderBy(u => u.GroupId).ToList();
            //gv.DataSource = (from s in dbContext.GroupRights
            //                 select new
            //                 {
            //                     s.Code,
            //                     s.UserGroups.Name


            //                 })
            //                     .ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=GroupRights" + NowTime + ".xls");
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
        [AuthorizedRights(Screen = "Grouprightes", Right = "Read")]
        public ActionResult ExportToPDF()
        {
            var GroupRights = dbContext.GroupRights.Select(s => s).Where(s => IDs.Contains(s.GroupId)).OrderBy(u => u.GroupId).ToList();
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");

            var report = new PartialViewAsPdf("~/Areas/GroupsRights/Views/GroupRight/ExportToPDF.cshtml", GroupRights);
            return report;

        }
         public ActionResult GetGroupID(string Code)
        {
            var GroupID = dbContext.UserGroups.Where(s => s.Code == Code).Select(u => u.GroupID).FirstOrDefault();

            return Json(new {Gid=GroupID }, JsonRequestBehavior.AllowGet);
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