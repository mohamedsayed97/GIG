using ICP_ABC.Areas.Blocks.Models;
using ICP_ABC.Areas.UsersSecurity.Models;
using ICP_ABC.Extentions;
using ICP_ABC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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


namespace ICP_ABC.Areas.Blocks.Controllers
{
    [Authorize]
    public class BlockController : Controller
    {

        private ApplicationDbContext dbContext = new ApplicationDbContext();
        UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(new ApplicationDbContext());
        static int[] IDs;

        // GET: Group
        [AuthorizedRights(Screen = "Block", Right = "Read")]
        public ActionResult Index()
        {
            var CMB = dbContext.blocktable.ToList();
            ViewData["CMB"] = new SelectList(CMB, "name", "code");
            return View();
        }
        [AuthorizedRights(Screen = "Block", Right = "Create")]
        public ActionResult Create()
        {
            //var CMB = dbContext.blocktable.ToList();
            //ViewData["CMB"] = new SelectList(CMB, "name","code");

            var CMB = dbContext.blocktable.ToList();
            ViewData["CMB"] = CMB;
            //var CMB = new List<SelectListItem>()
            //{
            //new SelectListItem() {Text="Block", Value="0"},
            //new SelectListItem() { Text="UnBlock", Value="1"}
            //};

            var LastCode = dbContext.Block.OrderByDescending(s => s.code).Select(s => s.code).FirstOrDefault();
            //var TryingToParse = int.TryParse(LastCode, out int Code);
            if (LastCode == 0)
            {
                ViewData["LastCode"] =  1;
            }
            else
            {
                ViewData["LastCode"] = LastCode+ 1;
            }
            var UserId = User.Identity.GetUserId();
            var user = dbContext.Users.Where(u => u.Id == UserId).FirstOrDefault();
            var UserBranchId = dbContext.Users.Where(b => b.Id == UserId).Select(b => b.BranchId).FirstOrDefault();
            var UserBranchName = dbContext.Branches.Where(b => b.BranchID == UserBranchId).Select(b => b.BName).FirstOrDefault();
            //ViewData["Branches"] = UserBranchName;
            ViewData["Branches"] = new SelectList(dbContext.Branches.Where(b => b.BranchID == UserBranchId).ToList(), "BName", "BranchID");
            //ViewData["Funds"] = new SelectList( User.Identity.UserFunds(),"FundID","Name");
            ViewData["Funds"] = new SelectList(  dbContext.Funds.Where(f => dbContext.FundRights
                                                  .Where(fr => fr.GroupID == dbContext.UserGroups
                                                  .Where(g => g.GroupID == user.GroupId).FirstOrDefault().GroupID)
                                                  .Select(h => h.FundID).ToList().Contains(f.FundID)).ToList() ,"FundID","Name");

            return View();
        }

        [HttpPost]
        [AuthorizedRights(Screen = "Block", Right = "Create")]
        public ActionResult Create( BlockViewModel model)
        {
            var customer = dbContext.Customers.Where(c => c.CustomerID == model.cust_id).FirstOrDefault();
            var Level = dbContext.UserSecurities.Select(Val => Val.Levels).FirstOrDefault();
            
            if (customer ==null)
            {
                return RedirectToAction("Create");
            }
            var LastCode = dbContext.Block.OrderByDescending(s => s.code).Select(s => s.code).FirstOrDefault();
            var CMBBlock = "";
            if (model.BlockCmb == 0)
            {
                CMBBlock = "Block";
            }
            else
            {
                CMBBlock = "UnBlock";
            }
            if (!GetQty(model.qty_block.ToString(),model.cust_id,model.fund_id.ToString(), CMBBlock))
            {
                return RedirectToAction("Create");

            }

            if (Level == Levels.TwoLevels) {


                if (ModelState.IsValid)
                {
                    //var cmb = model.BlockCmb == "Block" ? 0 : 1;
                    Block block = new Block
                    {
                        code = LastCode + 1,
                        cust_id = model.cust_id ,
                        fund_id = model.fund_id,// User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        branch_id = customer.Branch.BranchID,
                        BlockCmb = model.BlockCmb,
                        block_date = DateTime.Now,// dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        qty_block = model.qty_block,
                        block_reson = model.block_reson,
                        DeletFlag = DeleteFlag.NotDeleted,
                        Chk = true,
                        Maker = User.Identity.GetUserId(),
                        Checker = User.Identity.GetUserId()



                    };
                    dbContext.Block.Add(block);
                    dbContext.SaveChanges();
                    //return RedirectToAction("Details", new { Code = block.code });
                    return RedirectToAction("Details", new { Code = block.code });
                    //return RedirectToAction("Search");
                }
                else
                {
                    return View(model);
                }
            }
            else {
                if (ModelState.IsValid)
                {
                    //var cmb = model.BlockCmb == "Block" ? 0 : 1;
                    Block block = new Block
                    {
                        code = LastCode + 1,
                        cust_id = model.cust_id,
                        fund_id = model.fund_id,// User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        branch_id = customer.Branch.BranchID,
                        BlockCmb = model.BlockCmb,
                        block_date = DateTime.Now,// dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        qty_block = model.qty_block,
                        block_reson = model.block_reson,
                        DeletFlag = DeleteFlag.NotDeleted,
                        Chk = false,
                        Maker = User.Identity.GetUserId()



                    };
                    dbContext.Block.Add(block);
                    dbContext.SaveChanges();
                    //return RedirectToAction("Details", new { Code = block.code });
                    return RedirectToAction("Details", new { Code = block.code });
                    //return RedirectToAction("Search");
                }
                else
                {
                    return View(model);
                }

            }

        }

        [AuthorizedRights(Screen = "Block", Right = "Update")]
        public ActionResult Edit(int Code)

        {
            
            var model = dbContext.Block
                   .Where(g => g.code == Code)
                   .FirstOrDefault();
            
            var CMB = dbContext.blocktable.ToList();
            //ViewData["CMB"] = new SelectList(CMB, "code", "name",model.BlockCmb);

            //var CMB = dbContext.blocktable.ToList();
            ViewData["CMB"] = CMB;
            BlockViewModel block = new BlockViewModel
            {
              code = model.code,
              block_date= model.block_date.ToString("dd-MM-yyyy"),
              cust_id= model.cust_id.ToString(),
              CustomerName = dbContext.Customers.Where(c => c.CustomerID == model.cust_id.ToString()).Select(c => c.EnName).FirstOrDefault(),
              BlockCmb = model.BlockCmb,
              qty_block = model.qty_block,
              branch_id = model.branch_id,
              fund_id= model.fund_id,
              block_reson = model.block_reson
              

        };
            var UserId = User.Identity.GetUserId();
            var user = dbContext.Users.Where(u => u.Id == UserId).FirstOrDefault();
          var x=  dbContext.Funds.Where(f => dbContext.FundRights
                    .Where(fr => fr.GroupID == dbContext.UserGroups
                    .Where(g => g.GroupID == user.GroupId).FirstOrDefault().GroupID)
                    .Select(h => h.FundID).ToList().Contains(f.FundID)).ToList();
            //ViewData["Funds"] = new SelectList(User.Identity.UserFunds().Where(f=>f.FundID==model.fund_id).ToList(), "FundID", "Name");
            ViewData["Funds"] = new SelectList(dbContext.Funds.Where(f => dbContext.FundRights
                    .Where(fr => fr.GroupID == dbContext.UserGroups
                    .Where(g => g.GroupID == user.GroupId).FirstOrDefault().GroupID)
                    .Select(h => h.FundID).ToList().Contains(f.FundID)).ToList().Where(f=>f.FundID==model.fund_id).ToList(), "FundID", "Name");
            ViewData["Branch"] = model.Branch.BName;

            return View(block);
        }
        [HttpPost]
        [AuthorizedRights(Screen = "Block", Right = "Update")]
        public ActionResult Edit(BlockViewModel model)
        {
            var currentBlock = dbContext.Block.Where(g => g.code == model.code).FirstOrDefault();

                //currentBlock.code = model.c,
                //currentBlock.cust_id = model.cust_id;
                // User.Identity.GetUserId(); //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault();
                //currentBlock.branch_id = model.branch_id;
               currentBlock.BlockCmb = model.BlockCmb;
               currentBlock.fund_id = model.fund_id;
               currentBlock.block_date = DateTime.Now;// dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault();
               currentBlock.qty_block = model.qty_block;
               currentBlock.block_reson = model.block_reson;


           
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = model.code });
            //return RedirectToAction("Search");
        }

        [AuthorizedRights(Screen = "Block", Right = "Read")]
        public ViewResult Search(string sortOrder,string RadioCHeck, string currentFilter, int? page, string BranchId, string CustId,string FundId, string CMB,string AuthType)
        {
            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;
            if (page == null&&sortOrder == null && RadioCHeck == null && currentFilter == null && BranchId == null && CustId == null && FundId == null && CMB == null && AuthType == null )
            {
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                var Blocks = dbContext.Block.Select(s => s).Where(c => c.DeletFlag == DeleteFlag.NotDeleted).Take(0);
           
                var BlockType = dbContext.blocktable.ToList();
                ViewData["BlockType"] = new SelectList(BlockType, "code", "name");
                var authType = new List<SelectListItem>()
            {
            new SelectListItem() { Text="All", Value=""},
            new SelectListItem() {Text="UaAuth", Value="0"},
            new SelectListItem() { Text="Auth", Value="1"}

            };
              
                ViewData["Funds"] = new SelectList(User.Identity.UserFunds(), "FundID", "Name");
                ViewData["authType"] = new SelectList(authType, "Value", "Text", "");

                return View(Blocks.ToPagedList(pageNumber, pageSize));

            }
            else
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.DateSortParm = sortOrder == "Code" ? "code_desc" : "Code";

                //if (CustId != null)
                //{
                //    page = 1;
                //}
                //else
                //{
                //    CustId = currentFilter;
                //}

                ViewBag.CurrentFilter = CustId;

                var Blocks = dbContext.Block.Select(s => s).Where(c => c.DeletFlag == DeleteFlag.NotDeleted);

                if (!String.IsNullOrEmpty(CustId))
                {
                    //int CustID = int.Parse(CustId);
                    Blocks = Blocks.Where(s => s.cust_id == CustId);
                    ViewData["CustId"] = CustId;
                }

                if (!String.IsNullOrEmpty(BranchId))
                {
                    //int BranchID = int.Parse(BranchId);
                    Blocks = Blocks.Where(s => s.Branch.BName.Contains(BranchId));
                    ViewData["BranchId"] = BranchId;
                }
                if (!String.IsNullOrEmpty(FundId))
                {
                    int FundID = int.Parse(FundId);
                    Blocks = Blocks.Where(s => s.fund_id == FundID);
                    ViewData["FundId"] = FundId;
                }

                if (!String.IsNullOrEmpty(CMB))
                {
                    int BlockCmb = int.Parse(CMB);
                    Blocks = Blocks.Where(s => s.BlockCmb == BlockCmb);
                    ViewData["CMB"] = CMB;
                }

                if (!String.IsNullOrEmpty(AuthType))
                {
                    bool AType = AuthType == "0" ? false : true;
                    Blocks = Blocks.Where(s => s.blockauth == AType);
                    //bool x =Convert.ToBoolean(5);
                    //switch (AuthType)
                    //{
                    //    case "Auth":
                    //        Blocks = Blocks.Where(s => s.blockauth == true);
                    //        ViewData["AuthType"] = AuthType;
                    //        break;
                    //    case "UnAuth":
                    //        Blocks = Blocks.Where(s => s.blockauth == false);
                    //        ViewData["AuthType"] = AuthType;
                    //        break;
                    //}

                }

                //checkRadio 
                if (!String.IsNullOrEmpty(RadioCHeck))
                {
                    //Case Auth
                    if (RadioCHeck == "1")
                    {
                        Blocks = Blocks.Where(s => s.blockauth == true);

                    }
                    //Case Checker
                    if (RadioCHeck == "2")
                    {
                        Blocks = Blocks.Where(s => s.Chk == true && s.blockauth == false);

                    }
                    //Case maker
                    if (RadioCHeck == "3")
                    {
                        Blocks = Blocks.Where(s => s.blockauth == false && s.Chk == false);

                    }
                    //Case All
                    if (RadioCHeck == "4")
                    {
                        Blocks = Blocks.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);

                    }


                }
                //

                TempData["BlockForExc"] = Blocks.Select(x => x.code).ToList();
                switch (sortOrder)
                {
                    case "name_desc":
                        Blocks = Blocks.OrderByDescending(s => s.Customer.EnName);
                        break;
                    case "Code":
                        Blocks = Blocks.OrderBy(s => s.code);
                        break;
                    case "code_desc":
                        Blocks = Blocks.OrderByDescending(s => s.code);
                        break;
                    default:  // Name ascending 
                        //Blocks = Blocks.OrderBy(s => s.Customer.AccountNO);
                        Blocks = Blocks.OrderBy(s => s.code);
                        break;
                }
                //int count = Blocks.ToList().Count();
                //IDs = new string[count];
                //IDs = Blocks.Select(s => s.Code).ToArray();


                var BlockType = dbContext.blocktable.ToList();
                ViewData["BlockType"] = new SelectList(BlockType, "code", "name");
                var authType = new List<SelectListItem>()
            {
            new SelectListItem() { Text="All", Value=""},
            new SelectListItem() {Text="UaAuth", Value="0"},
            new SelectListItem() { Text="Auth", Value="1"}

            };
                //ViewData["BlockType"] = new SelectList(BlockType, "Value", "Text");
                ViewData["Funds"] = new SelectList(User.Identity.UserFunds(), "FundID", "Name");
                ViewData["authType"] = new SelectList(authType, "Value", "Text", "");


                int pageSize = 4;
                int pageNumber = (page ?? 1);
                ViewData["radioCHeck"] = RadioCHeck;
                int count = Blocks.ToList().Count();
                IDs = new int[count];
                IDs = Blocks.Select(s => s.code).ToArray();
                Array.Sort(IDs);
                //int[] myInts = Array.ConvertAll(IDs, s => int.Parse(s));



                return View(Blocks.ToPagedList(pageNumber, pageSize));
            }
        }

        [AuthorizedRights(Screen = "Block", Right = "Read")]
        public ViewResult Details(int Code)
        {
            var Model = dbContext.Block.Where(c => c.code == Code).FirstOrDefault();
            var Actualy_auth = Model.blockauth;
            var ThisUser = User.Identity.GetUserId();
            if (Model.Maker == ThisUser)
            {
                if (Model.Chk == true && Model.blockauth == false)
                {
                    Model.Chk = true;
                    Model.blockauth = false;
                }
                if (Model.Chk == false && Model.blockauth == false)
                {
                    Model.Chk = true;
                    Model.blockauth = true;

                }
            }
            else
            {
                if (Model.Chk == true && Model.blockauth == true)
                {
                    Model.Chk = true;
                    Model.blockauth = true;

                }

                else if (Model.Chk == true && Model.blockauth == false)
                {
                    if (Model.Checker != ThisUser)
                    {
                        Model.Chk = true;
                        Model.blockauth = false;

                    }
                    else
                    {
                        Model.Chk = true;
                        Model.blockauth = true;

                    }
                }
                else if (Model.Chk == false && Model.blockauth == false)
                {
                    Model.Chk = false;
                    Model.blockauth = true;

                }

            }

            BlockViewModel CurrentBlock = new BlockViewModel
            {
                
                code = Model.code,
                cust_id = Model.cust_id.ToString(),
                fund_id = Model.fund_id,// User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                branch_id = Model.branch_id,
                BlockCmb = Model.BlockCmb ,
                block_date = Model.block_date.ToString("dd-MM-yyyy"),// dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                qty_block = Model.qty_block,
                block_reson = Model.block_reson,
                CustomerName = dbContext.Customers.Where(c => c.CustomerID == Model.cust_id.ToString()).Select(c => c.EnName).FirstOrDefault(),
                Auth = Model.blockauth,
                Check = Model.Chk,
                AuthForEditAndDelete=Actualy_auth

            };
            //ViewData["Branch"] = model.Branch.Name; //dbContext.Branches.Where(b => b.BranchID == CurrentBlock.branch_id).FirstOrDefault().Name;
            // ViewData["CMB"] = CurrentBlock.BlockCmb;
            // ViewData["Funds"] = model.Fund.Name; //dbContext.Funds.Where(f => f.FundID == CurrentBlock.fund_id).FirstOrDefault().Name;
            //var CMB = new List<SelectListItem>()
            //{
            //new SelectListItem() {Text="Block", Value="0"},
            //new SelectListItem() { Text="UnBlock", Value="1"}
            //};

            var CMB = dbContext.blocktable.ToList();

            //var UserId = User.Identity.GetUserId();
            //var UserId = User.Identity.GetUserId();
            var Barachname = dbContext.Branches.Where(b => b.BranchID == Model.branch_id).Select(b => b.BName).FirstOrDefault();

            //var UserBranchId = dbContext.Users.Where(b => b.Id == UserId).Select(b => b.BranchId).FirstOrDefault();
            //var UserBranchName = dbContext.Branches.Where(b => b.BranchID == UserBranchId).Select(b => b.BName).FirstOrDefault();
            var customer = dbContext.Customers.Where(c => c.CustomerID == Model.cust_id.ToString()).FirstOrDefault();

            ViewData["Branches"] = customer.Branch.BName;
            //ViewData["Branches"] = Barachname;
           // ViewData["Branches"] = Model.Branch.BName; //new SelectList(dbContext.Branches.Where(b => b.UserID == UserId).ToList(), "BranchID", "Name", model.branch_id);
            ViewData["Funds"] = Model.Fund.Name; //new SelectList(User.Identity.UserFunds(), "FundID", "Name");
            ViewData["CMB"] = new SelectList(CMB, "code", "name", Model.BlockCmb);

            //if (IDs != null)
            //{
            //    if (Model.code == IDs.Last())
            //    {
            //        TempData["Last"] = "Last";
            //    }
            //    if (Model.code == IDs.First())
            //    {
            //        TempData["First"] = "First";
            //    }
            //}
            //else
            //{
            //    TempData["Last"] = "Last";
            //    TempData["First"] = "First";
            //}
            return View(CurrentBlock);

        }

        [AuthorizedRights(Screen = "Block", Right = "Read")]
        public ActionResult Next(int id)
        {
            var arrlenth = IDs.Count();
            var LastObj = dbContext.Block.OrderBy(s => s.code).ToList().LastOrDefault(s => s.code == IDs[arrlenth - 1]);
            if (id == LastObj.code)
            {
                TempData["Last"] = "Last";
                return RedirectToAction("Details", new { Code = LastObj.code });
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
                var Code = dbContext.Block.Where(u => u.code == id).Select(s => s.code).FirstOrDefault();
                if (Code == LastObj.code)
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
        [AuthorizedRights(Screen = "Block", Right = "Read")]
        public ActionResult Previous(int id)
        {
            var arrindex = IDs[0];
            var FirstObj = dbContext.Block.OrderBy(s => s.code).FirstOrDefault(s => s.code == arrindex);
            if (id == FirstObj.code)
            {
                TempData["First"] = "First";
                return RedirectToAction("Details", new { Code = FirstObj.code });
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
                if (id == FirstObj.code)
                {
                    TempData["First"] = "First";
                }
                else
                {
                    TempData["First"] = "NotFirst";
                }
                var Code = dbContext.Block.Where(u => u.code == id).Select(s => s.code).FirstOrDefault();
                return RedirectToAction("Details", new { Code = Code });
            }

        }

        [AuthorizedRights(Screen = "Block", Right = "Read")]
        public ActionResult ExportToExcel()
        {
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var gv = new GridView();

            var List = (List<int>)TempData["BlockForExc"];
            var data = dbContext.Block.Where(Del => List.Contains(Del.code) && Del.DeletFlag == DeleteFlag.NotDeleted).Select(x => new { x.code, CustomerName=x.Customer.EnName,Branch = x.Branch.BName,QTY=x.qty_block,BlockAndUnblock= x.BlockCmb== 0 ?"Block":"Unblock" ,Auth=x.blockauth }).ToList()/*.ForEach(i=> { if (i.BlockAndUnblock == "0") { i=>i.BlockAndUnblock="Block" } })*/;
            
            gv.DataSource = data;
          
            //foreach(var item in data)
            //{
            //    if (item.BlockCmb == 0)
            //    {
            //        item.BlockCmb = "Bolck";
            //    }
            //    else
            //    {
            //        item.BlockCmb = "UnBlock";
            //    }
            //}
            //gv.DataSource = (from s in dbContext.Block
            //                 select new
            //                 {
            //                     s.code,
            //                     s.Customer.EnName


            //                 })
            //                     .ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Block" + NowTime + ".xls");
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
        [AuthorizedRights(Screen = "Block", Right = "Read")]
        public ActionResult ExportToPDF()
        {
            var branches = dbContext.Block.Select(s => s).Where(s => IDs.Contains(s.code)).OrderBy(u => u.code).ToList();
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");

            var report = new PartialViewAsPdf("~/Areas/Blocks/Views/Block/ExportToPDF.cshtml", branches);
            return report;

        }

        [AuthorizedRights(Screen = "Block", Right = "Authorized")]
        public ActionResult AuthorizeBlock(int Code)
        {
            var block = dbContext.Block.Where(f => f.code == Code).FirstOrDefault();
            block.blockauth = true;
            block.Auther = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details",new { Code = Code});
        }

        [AuthorizedRights(Screen = "Block", Right = "Check")]
        public ActionResult CheckBlock(int Code)
        {
            var Block = dbContext.Block.Where(f => f.code == Code).FirstOrDefault();
            Block.Chk = true;
            Block.Checker = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = Code });
        }

        [AuthorizedRights(Screen = "Block", Right = "Delete")]
        public ActionResult Delete(int Code)
        {
            var Block = dbContext.Block.Where(f => f.code == Code).FirstOrDefault();
            Block.DeletFlag = DeleteFlag.Deleted;
            dbContext.SaveChanges();
            //return RedirectToAction("Details", new { Code = model.code });
            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult GetUserInfo(string CustId)
        {

            var customer = dbContext.Customers.Where(c => c.CustomerID == CustId).FirstOrDefault();
            if (customer == null)
            {
                //customer.EnName = "No Customer Found";
                return Json(new { customerName = "No Customer Found" });
            }
            return Json  (new { customerName = customer.EnName });
        }

        [HttpPost]
        public JsonResult GetQtyRange(string Count, string CustomerID, string FundID,string CMBBlock)
        {

            if (GetQty(Count, CustomerID, FundID, CMBBlock))
            {
                return Json(new { state = "Match" }, JsonRequestBehavior.AllowGet);
            }
            int avilableQTY = ViewBag.AvQty;
           // avilableQTY = 10; //for Test
                return Json(new { status = "Not Match", Max = avilableQTY }, JsonRequestBehavior.AllowGet);


        }

        bool GetQty(string Count, string CustomerID, string FundID, string CMBBlock)
        {
            //var custID = int.Parse(CustomerID);
            var Customer = dbContext.Customers
                .Where(c => c.CustomerID == CustomerID)
                .FirstOrDefault();
            var fundID = int.Parse(FundID);
            var Fund = dbContext.Funds.Where(f => f.FundID == fundID).FirstOrDefault();
          

            int avilableQTY = 0;
            if (CMBBlock == "Block")
            {
                var subscriptions = dbContext.Subscriptions.Where(s => s.cust_id == CustomerID && s.fund_id == fundID && s.auth == 1).ToList();
                decimal? subs = subscriptions.Sum(s => s.units);
                var redemptions = dbContext.Redemptions.Where(r => r.cust_id == CustomerID && r.fund_id == fundID ).ToList();
                decimal? reds = redemptions.Sum(s => s.units); ;

                var blockedList = dbContext.Block.Where(b => b.cust_id ==CustomerID  && b.fund_id == fundID && b.BlockCmb == 0).ToList();
                decimal block = blockedList.Sum(s => s.qty_block); ;

                var UnBlocked = dbContext.Block.Where(b => b.cust_id == CustomerID && b.fund_id == fundID && b.BlockCmb == 1 && b.blockauth == true).ToList();
                decimal unblock = UnBlocked.Sum(s => s.qty_block);
                var x = (subs - reds) - (block - unblock);
                avilableQTY = Convert.ToInt32(x);
                ViewBag.AvQty = (avilableQTY < 0) ? 0 : avilableQTY;
                if (int.Parse(Count) > avilableQTY)
                {
                    return false;
                }
               

            }
            else if (CMBBlock == "UnBlock")
            {
                var blockedList = dbContext.Block.Where(b => b.cust_id == CustomerID && b.fund_id == fundID && b.BlockCmb == 0 && b.blockauth == true).ToList();
                decimal block = blockedList.Sum(s => s.qty_block); ;
                
                avilableQTY = Convert.ToInt32(block);
                ViewBag.AvQty =  (avilableQTY < 0) ? 0 : avilableQTY;
                if (int.Parse(Count) > avilableQTY)
                {
                    return false;
                }
            }
           
            //avilableQTY = 10;//for test
            
            //if (int.Parse(Count) > avilableQTY || int.Parse(Count) < avilableQTY)
            //{
            //    return false;
            //}
            //if (int.Parse(Count) > avilableQTY) {
            //    return false;
            //}
            //else if(int.Parse(Count) <= avilableQTY )
            //{
            //    return true;
            //}
            return true;
            
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [HttpPost]
        [Route("CheckUserBlockRight")]
        public JsonResult GetUserBlockRight()
        {
           var ThisUser= User.Identity.GetUserId();
            var Check = dbContext.Users.Where(c => c.Id == ThisUser && c.UnBlockRight == true).FirstOrDefault();
            if (Check != null)
            {
                //customer.EnName = "No Customer Found";
                return Json(new { VisibleAll = "True" });
            }
            else
            {
                return Json(new { VisibleAll = "False" });
            }
           
        }


    }
}