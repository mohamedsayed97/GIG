using ICP_ABC.Extentions;
using ICP_ABC.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.Script.Serialization;
using ICP_ABC.Areas.ICPrices.Models;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity.Validation;
using Rotativa;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using PagedList;
using ICP_ABC.Areas.Redemptions.Models;
using ICP_ABC.Areas.Subscriptions.Models;
using System.Configuration;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using ICP_ABC.Areas.UsersSecurity.Models;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using ICP_ABC.Areas.VestingRules.Models;
using ICP_ABC.Areas.Policies.Models;

namespace ICP_ABC.Areas.Redemptions.Controllers
{
    
    [Authorize]
    public class RedemptionController : Controller
    {
      

        private ApplicationDbContext dbContext = new ApplicationDbContext();
        static int[] IDs;

        // GET: Subscriptions/Subscriptions
        public ActionResult Index()
        {

            return View();
        }
        //public string GetLastCode()
        //{
        //    var querey = "SELECT MAX(Code) as MaxCode FROM Redemption";
        //    var appSettings = ConfigurationManager.ConnectionStrings;
        //    var SqlCon = appSettings["ICPRO"];
        //    var ReturnedData = SqlHelper.ExecuteDataset(SqlCon.ToString(), CommandType.Text, querey);
        //    var LastCode = ReturnedData.Tables[0].Rows[0][0].ToString();
        //    return LastCode;
        //}
        public ActionResult Create()
        {
           var lastcode = dbContext.LastCodes.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault();
           ViewData["LastCode"] = lastcode + 1;
           ViewData["Funds"] = new SelectList(dbContext.Funds.ToList(), "FundID", "Name"); //new SelectList(User.Identity.UserFunds().Where(f => f.HasICPrice == true && f.Auth == true).ToList(), "FundID", "Name");

            if (TempData["LoginResult"] != null)
            {
                IdentityResult LoginResult = TempData["LoginResult"] as IdentityResult;
                AddErrors(LoginResult);
            }

            return View();
        }
        [HttpPost]
        public ActionResult Create(CreateRedemptionViewModel model)
        {

            if (!ModelState.IsValid)
            {
                string[] errors = new string[] { "something went wrong try again" };
                IdentityResult LoginResult = new IdentityResult(errors);
                TempData["LoginResult"] = LoginResult;
                AddErrors(LoginResult);
                return RedirectToAction("Create");
            }
         

            var Fund = dbContext.Funds.SingleOrDefault(f => f.FundID == model.fund_id);
            var Block = dbContext.Block.Where(s => s.fund_id == model.fund_id && s.cust_id ==model.cust_id && s.BlockCmb == 0).Select(c => c.qty_block).ToList().Sum() - dbContext.Block.Where(s => s.fund_id == model.fund_id && s.cust_id == model.cust_id  && s.BlockCmb == 1 && s.blockauth == true).Select(c => c.qty_block).ToList().Sum();

            var AvailableForRedemption = dbContext.Subscriptions.Where(s => s.fund_id == model.fund_id && s.cust_id == model.cust_id && s.auth == 1).Select(c => c.units).ToList().Sum() - dbContext.Redemptions.Where(s => s.fund_id == model.fund_id && s.cust_id == model.cust_id).Select(c => c.units).ToList().Sum();
            var pendeingRedemption = dbContext.Redemptions.Where(s => s.fund_id == model.fund_id && s.cust_id == model.cust_id && s.auth != 1).Select(c => c.units).ToList().Sum();
           
            if (AvailableForRedemption - Block - model.units < 0)
            {
                string[] errors;
                if (Block == 0)
                    errors = new string[] { "Your position is : " + AvailableForRedemption + " but your minimum holding is : " + Fund.min_pos };

                else
                     errors = new string[] { "Your position is : " + AvailableForRedemption + " but you have "+ Block+ " blocked units " +" , your minimum holding is : " + Fund.min_pos };

                IdentityResult LoginResult = new IdentityResult(errors);
                TempData["LoginResult"] = LoginResult;
                AddErrors(LoginResult);
                return RedirectToAction("Create");
            }
            
            if (AvailableForRedemption < (model.units))
            {
                string[] errors = new string[] { "Your position is : " + AvailableForRedemption + " but you have : " + pendeingRedemption + " pending" };
                IdentityResult LoginResult = new IdentityResult(errors);
                TempData["LoginResult"] = LoginResult;
                AddErrors(LoginResult);
                return RedirectToAction("Create");
            }
     

            var Code = (dbContext.LastCodes.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault()) + 1;

           

            decimal? totalAmount = 0;
          
            if (model.pay_method == "By Units")
            {

                totalAmount = model.units * model.NAV;
            }
            else if (model.pay_method == "By Amount")
            {
                var units = model.amount_3 / model.NAV;
                totalAmount = units * model.NAV;

            }
            else
            {
                string[] errors = new string[] { "Some Thing is wrong about calculations " };
                IdentityResult LoginResult = new IdentityResult(errors);
                TempData["LoginResult"] = LoginResult;
                AddErrors(LoginResult);
                return RedirectToAction("Create");
            }
            var UserId = User.Identity.GetUserId();
            var currentUser = dbContext.Users.Where(u => u.Id == UserId).FirstOrDefault();
            var PayMethod = model.pay_method == "By Units" ? 0 : 1;

            var lastcode = dbContext.LastCodes.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault();

            lastcode++;

            var Level = dbContext.UserSecurities.Select(Val => Val.Levels).FirstOrDefault();
            if (Level == Levels.TwoLevels)
            {
                Redemption redemption = new Redemption
                {
                    code = lastcode,

                    cust_id = model.cust_id,
                    amount_3 = decimal.Round(Convert.ToDecimal(model.units * model.NAV), 2),
                    units = model.units,
                    SysDate = DateTime.Now,
                    NAV = model.NAV * 1.00m,
                    fund_id = model.fund_id,
                    inputer = User.Identity.GetUserId(),
                    Checker = User.Identity.GetUserId(),
                    pay_method = (short)PayMethod,
                    Chk = true,
                    auth = 0,
                    nav_date = model.nav_date,
                    Nav_Ddate = model.Nav_Ddate,
                    ProcessingDate = model.ProcessingDate,
                    total = decimal.Round(Convert.ToDecimal(totalAmount), 2),
                    system_date = DateTime.Now,
                    UserID = User.Identity.GetUserId(),
                    delreason = DeleteFlag.NotDeleted,
                    GTF_Flag = (model.cust_id == accountType.ZeroAccount) ? (short)5 : (short)6

                };
                Trans trans = new Trans()
                {
                    transid = lastcode,
                    total_value = decimal.Round(Convert.ToDecimal(model.units * model.NAV), 2),
                    unit_price = model.NAV,
                    quantity = (decimal)model.units,
                    SysDate = DateTime.Now,
                    cust_id = model.cust_id.ToString(),
                    fund_id = model.fund_id,
                    inputer = User.Identity.GetUserId(),
                    auth = 0,
                    UserID = User.Identity.GetUserId(),
                    value_date = model.nav_date,
                    Nav_Ddate = model.Nav_Ddate,
                    ProcessingDate = model.ProcessingDate,
                    Flag = -1,
                    pur_sal = 1,


                    GTF_Flag = (model.cust_id == accountType.ZeroAccount) ? (short)5 : (short)6
                };
                   
                dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                try
                {
                    dbContext.Redemptions.Add(redemption);
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
                    //    throw;
                    ///throw;
                }

                try
                {
                    dbContext.Trans.Add(trans);
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
                    //    throw;
                    ///throw;
                }

                return RedirectToAction("Details", new { Code = redemption.code });
            }
            else
            {
                Redemption redemption = new Redemption
                {
                    code = lastcode,
                    cust_id = model.cust_id,
                    amount_3 = decimal.Round(Convert.ToDecimal(model.units * model.NAV), 2),
                    units = model.units,
                    SysDate = DateTime.Now,
                    NAV = model.NAV * 1.00m,
                    fund_id = model.fund_id,
                    inputer = User.Identity.GetUserId(),
                    Chk = false,
                    auth = 0,

                    pay_method = (short)PayMethod,
                    nav_date = model.nav_date,
                    Nav_Ddate = model.Nav_Ddate,
                    ProcessingDate = model.ProcessingDate,
                    total = decimal.Round(Convert.ToDecimal(totalAmount), 2),
                    system_date = DateTime.Now,
                    UserID = User.Identity.GetUserId(),
                    delreason = DeleteFlag.NotDeleted,
                    GTF_Flag = (model.cust_id == accountType.ZeroAccount) ? (short)5 : (short)6
                    
               };
                dbContext.Redemptions.Add(redemption);
                Trans trans = new Trans()
                {
                    
                    transid = lastcode,
                    total_value = decimal.Round(Convert.ToDecimal(model.units * model.NAV), 2),
                    quantity = model.units.Value,
                    unit_price = model.NAV,
                    SysDate = DateTime.Now,
                    cust_id = model.cust_id,
                    fund_id = model.fund_id,
                    inputer = User.Identity.GetUserId(),
                    auth = 0,
                    GTF_Flag = (model.cust_id == accountType.ZeroAccount) ? (short)5 : (short)6,
                    UserID = User.Identity.GetUserId(),
                    value_date = model.nav_date.Value,
                    Nav_Ddate = model.Nav_Ddate,
                    ProcessingDate = model.ProcessingDate,
                    pur_sal = 1
                   
                  
                };
                dbContext.LastCodes.Add(new LastCode { Code = lastcode });
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
                    //    throw;
                    ///throw;
                }

                try
                {
                    dbContext.Trans.Add(trans);
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
                    //    throw;
                    ///throw;
                }

                return RedirectToAction("Details", new { Code = redemption.code });
            }


           
        }

        public ViewResult Search(string sortOrder,string RadioCHeck, string currentFilter, int? page, SearchRedViewModel model)
        {

            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;


            if (page==null&&sortOrder == null && RadioCHeck == null && currentFilter == null  && model.TotalAmountTo == null
                && model.CodeFrom == null && model.CodeTo == null
                && model.CustomerId == null && model.Funds == null && model.NumberOfUnits == null
                && model.NavDateFrom == null && model.NavDateTo == null && model.TotalAmountFrom == null
                && model.Authorize == null)
            {
                int pageSize = 4;
                int pageNumber = (page ?? 1);

                var Reds = dbContext.Redemptions.Where(c => c.delreason == DeleteFlag.NotDeleted).Take(0);
                var prices = dbContext.ICPrices.ToList();
                var UserFunds = User.Identity.UserFunds().Where(f => prices.Select(p => p.FundId).ToList().Contains(f.FundID) && f.Auth == true).ToList();
                ViewData["Funds"] = new SelectList(UserFunds, "FundID", "Name");
                return View(Reds.ToPagedList(pageNumber, pageSize));

            }
            else
            {
                //ViewData["CustType"] = new SelectList(dbContext.CustTypes.ToList(), "CustTypeID", "Name");
                //ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "BranchID", "Name");
                var prices = dbContext.ICPrices.ToList();
                var UserFunds = User.Identity.UserFunds().Where(f => prices.Select(p => p.FundId).ToList().Contains(f.FundID) && f.Auth == true).ToList();
                ViewData["Funds"] = new SelectList(UserFunds, "FundID", "Name"); //new SelectList(User.Identity.UserFunds().Where(f => f.HasICPrice == true && f.Auth == true).ToList(), "FundID", "Name");
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.DateSortParm = sortOrder == "Code" ? "code_desc" : "Code";

                //if (model.AccountNo != null)
                //{
                //    page = 1;
                //}
                //else
                //{
                //    model.AccountNo = currentFilter;
                //}

                //ViewBag.CurrentFilter = model.AccountNo;

                var Reds = dbContext.Redemptions.Where(c => c.delreason == DeleteFlag.NotDeleted);
                var currentuserId = User.Identity.GetUserId();
                var currentUser = dbContext.Users.Where(u => u.Id == currentuserId).First();

                //if (!currentUser.BranchRight)
                //{
                //    Reds = Reds.Where(u => u.branch_id == currentUser.BranchId);
                //}

                //if (!String.IsNullOrEmpty(model.AccountNo))
                //{
                //    Reds = Reds.Where(s => s.cust_acc_no == model.AccountNo);
                //    ViewData["AccountNo"] = model.AccountNo;
                //}


                if (!String.IsNullOrEmpty(model.CustomerId))
                {
                    //var code = int.Parse(Code);
                    Reds = Reds.Where(s => s.cust_id == model.CustomerId);
                    ViewData["CustomerId"] = model.CustomerId;
                }

                if (!String.IsNullOrEmpty(model.CodeFrom))
                {
                    var code = int.Parse(model.CodeFrom);
                    Reds = Reds.Where(s => s.code >= code);
                    ViewData["CodeFrom"] = model.CodeFrom;
                }
                // var x = Subs.ToList();
                if (!String.IsNullOrEmpty(model.CodeTo))
                {
                    // int IdNumber;
                    int code = int.Parse(model.CodeTo);
                    Reds = Reds
                   .Where(u => u.code <= code)
                   .Select(s => s);
                    ViewData["CodeTo"] = code;
                }
                if (!String.IsNullOrEmpty(model.NavDateFrom))
                {
                    var FromDate = Convert.ToDateTime(model.NavDateFrom);
                    Reds = Reds
                   .Where(u => u.nav_date >= FromDate)
                   .Select(s => s);

                }

                if (!String.IsNullOrEmpty(model.NavDateTo))
                {
                    var ToDate = Convert.ToDateTime(model.NavDateTo);
                    Reds = Reds
                   .Where(u => u.nav_date <= ToDate)
                   .Select(s => s);
                    //ViewData["ArName"] = ArName;

                }

                if (!String.IsNullOrEmpty(model.Funds))
                {
                    int FundID = int.Parse(model.Funds);
                    Reds = Reds
                   .Where(u => u.fund_id == FundID)
                   .Select(s => s);
                    ViewData["Funds"] = new SelectList(UserFunds, "FundID", "Name", FundID);

                }


                //if (!String.IsNullOrEmpty(model.BranchId))
                //{

                //    int BranchID = int.Parse(model.BranchId);
                //    Reds = Reds
                //   .Where(u => u.branch_id == BranchID)
                //   .Select(s => s);
                //    ViewData["BranchId"] = BranchID;

                //}

                if (!String.IsNullOrEmpty(model.NumberOfUnits))
                {

                    var units = decimal.Parse(model.NumberOfUnits);
                    Reds = Reds
                   .Where(u => u.amount_3 == units)
                   .Select(s => s);
                    //ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "BranchID", "Name", BranchID);
                    ViewData["NumberOfUnits"] = units;
                }

                if (!String.IsNullOrEmpty(model.TotalAmountFrom))
                {

                    var Amount = decimal.Parse(model.TotalAmountFrom);
                    Reds = Reds
                   .Where(u => u.total >= Amount)
                   .Select(s => s);
                    //ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "BranchID", "Name", BranchID);

                }
                if (!String.IsNullOrEmpty(model.TotalAmountTo))
                {

                    int Amount = int.Parse(model.TotalAmountTo);
                    Reds = Reds
                   .Where(u => u.total <= Amount)
                   .Select(s => s);

                }
                //checkRadio 
                if (!String.IsNullOrEmpty(RadioCHeck))
                {
                    //Case Auth
                    if (RadioCHeck == "1")
                    {
                        Reds = Reds.Where(s => s.auth == 1);

                    }
                    //Case Checker
                    if (RadioCHeck == "2")
                    {
                        Reds = Reds.Where(s => s.Chk == true && s.auth != 1);

                    }
                    //Case maker
                    if (RadioCHeck == "3")
                    {
                        Reds = Reds.Where(s => s.auth != 1 && s.Chk == false);
                       
                    }
                    //Case All
                    if (RadioCHeck == "4")
                    {
                        Reds = Reds.Where(c => c.delreason == DeleteFlag.NotDeleted);

                    }


                }
                //

                //var ghx = Subs.ToList();
                TempData["RedSForExc"] = Reds.Select(Co => Co.code).ToList();
                switch (sortOrder)
                {
                    case "name_desc":
                        Reds = Reds.OrderByDescending(s => s.Fund.Name);
                        break;
                    case "Code":
                        Reds = Reds.OrderBy(s => s.code);
                        break;
                    case "code_desc":
                        Reds = Reds.OrderByDescending(s => s.code);
                        break;
                    default:  // Name ascending 
                        //Reds = Reds.OrderBy(s => s.appliction_no);
                        Reds = Reds.OrderBy(s => s.code);
                        break;
                }



                //int count = Currencies.ToList().Count();
                //IDs = new string[count];
                //IDs = Currencies.Select(s => s.Code).ToArray();

                int pageSize = 4;
                int pageNumber = (page ?? 1);
                ViewData["radioCHeck"] = RadioCHeck;
                int count = Reds.ToList().Count();
                IDs = new int[count];
                IDs = Reds.Select(s => s.code).ToArray();
                Array.Sort(IDs);

                return View(Reds.ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult Edit(int Code)
        {
            var sub = dbContext.Redemptions.Where(s => s.code == Code).FirstOrDefault();
            CreateRedemptionViewModel model = new CreateRedemptionViewModel()
            {
                code = sub.code,
                amount_3 = sub.amount_3,
                appliction_no = sub.appliction_no,
                units = sub.units,
                BranchName = dbContext.Branches.Where(b => b.BranchID == sub.branch_id).Select(b => b.BName).FirstOrDefault(),
                CustomerName = dbContext.Customers.Where(c => c.CustomerID == sub.cust_id.ToString()).Select(c => c.EnName).FirstOrDefault(),
                cust_id = sub.cust_id.ToString(),
                NAV = sub.NAV,
                branch_id = sub.branch_id,
                nav_date = sub.nav_date,
                ProcessingDate = sub.ProcessingDate,
                Nav_Ddate = sub.Nav_Ddate,
            
                other_fees = sub.other_fees,
                pay_method = sub.pay_method == 0 ? "By Units" : "By Amount",
                pur_date = sub.pur_date,
                sub_fees = sub.sub_fees,
                cust_acc_no = sub.cust_acc_no,
                total = sub.total


            };
            
         
            ViewData["Funds"] = new SelectList(dbContext.Funds.Where(f => f.FundID == sub.fund_id).ToList(), "FundID", "Name");
            
            return View(model);
        }

        [HttpPost]
        [AuthorizedRights(Screen = "Redemption", Right = "Update")]
        public ActionResult Edit(CreateRedemptionViewModel model)
        {
            
            var sub = dbContext.Redemptions.Where(s => s.code == model.code).FirstOrDefault();

            var trans = dbContext.Trans.Where(s => s.transid == sub.code).FirstOrDefault();

            var Customer = dbContext.Customers
               .Where(c => c.CustomerID == sub.cust_id.ToString())
               .FirstOrDefault();

            var mm = model.units;
             try
            { 
            trans.quantity = model.units.Value;
            }
            catch(NullReferenceException e)
            {

            }
            decimal? totalAmount = 0;

            if (model.pay_method == "By Units")
            {
                totalAmount = model.units * model.NAV;


            }
            else if (model.pay_method == "By Amount")
            {
                var units = model.amount_3 / model.NAV;
                totalAmount = units * model.NAV;


            }
            else
            {
                string[] errors = new string[] { "Some Thing is wrong about calculations " };
                IdentityResult LoginResult = new IdentityResult(errors);
                TempData["LoginResult"] = LoginResult;
                AddErrors(LoginResult);
                return RedirectToAction("Edit");
            }


            

            sub.amount_3 = decimal.Round(Convert.ToDecimal(model.units * model.NAV), 2);
            sub.units = model.units;
            sub.NAV = (decimal.Round(Convert.ToDecimal(model.NAV), 2)) * 1.00m;
            sub.inputer = User.Identity.GetUserId();
            var PayMethod = model.pay_method == "By Units" ? 0 : 1;
            sub.pay_method = (short)PayMethod;
            sub.pur_date = DateTime.Now;
            sub.nav_date = model.nav_date;

            sub.ProcessingDate = model.ProcessingDate;
            sub.Nav_Ddate = model.Nav_Ddate;

            
            sub.total = decimal.Round(Convert.ToDecimal(totalAmount), 2);
            sub.flag_tr = 1;
            sub.delreason = DeleteFlag.NotDeleted;


            trans.quantity = (decimal)model.units;
            trans.unit_price = model.NAV;
            // trans.inputer = User.Identity.GetUserId();
            trans.payment_met = (short)PayMethod;
            trans.entry_date = DateTime.Now;

            trans.value_date = model.nav_date;
          
            trans.total_value = (decimal)totalAmount;
            trans.flag_tr = 1;

            trans.ProcessingDate = model.ProcessingDate;
            trans.Nav_Ddate = model.Nav_Ddate;

            trans.curr_id = dbContext.Funds.SingleOrDefault(f => f.FundID == model.fund_id).CurrencyID;
            trans.SysDate = DateTime.Now;
        
            trans.auth = 0;
            

            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = model.code });
        }
        [AuthorizedRights(Screen = "Redemption", Right = "Read")]
        public ActionResult Details(int Code)
        {
            CreateRedemptionViewModel model = new CreateRedemptionViewModel();
            var reds = dbContext.Redemptions.Where(s => s.code == Code).FirstOrDefault();
            var trans = dbContext.Trans.Where(s => s.code == Code).FirstOrDefault();
            var CurrentUser = User.Identity.GetUserId();

        
            var ThisUser = User.Identity.GetUserId();

            if (reds.Chk == false && reds.inputer != CurrentUser)
            {
                model.CheckBtn = true;
            }
            if (reds.Chk == true && reds.Checker != null && reds.auth == 0 && reds.Checker != CurrentUser)
            {
                model.AuthBtn = true;
            }
            if (reds.Chk == true && reds.Checker != null && reds.auth == 1 && reds.auther != null && reds.Checker != CurrentUser)
            {
                model.UnAuthBtn = true;
            }
            if (reds.auth == 0 || reds.auth == null)
            {
                model.DeleteBtn = true;
                model.EditBtn = true;
            }


            model.code = reds.code;
            model.amount_3 = reds.amount_3;
            model.units = reds.units;
            model.CustomerName = dbContext.Customers.Where(c => c.CustomerID == reds.cust_id.ToString()).Select(c => c.EnName).FirstOrDefault();
            model.cust_id = reds.cust_id.ToString();
            model.NAV = reds.NAV;
         
            model.nav_date = reds.nav_date;
            model.ProcessingDate = reds.ProcessingDate;
            model.Nav_Ddate = reds.Nav_Ddate;

            model.pay_method = reds.pay_method == 0 ? "By Units" : "By Amount";

            model.total = reds.total;
             
            
            ViewData["nav_date"] = reds.nav_date.ToString();
            ViewData["Date"] = reds.pur_date.ToString();
            ViewData["PayMethod"] = model.pay_method;
            var fundname = dbContext.Funds.Where(f => f.FundID == reds.fund_id).Select(s => s.Name).FirstOrDefault();
            ViewData["Funds"] = fundname;
            if (IDs != null)
            {
                if (model.code == IDs.Last())
                {
                    TempData["Last"] = "Last";
                }
                if (model.code == IDs.First())
                {
                    TempData["First"] = "First";
                }
            }
            else
            {
                TempData["Last"] = "Last";
                TempData["First"] = "First";
            }
           

            return View(model);
        }



        [AuthorizedRights(Screen = "Redemption", Right = "Read")]
        public ActionResult Next(int id)
        {
            var arrlenth = IDs.Count();
            var LastObj = dbContext.Redemptions.OrderBy(s => s.code).ToList().LastOrDefault(s => s.code == IDs[arrlenth - 1]);
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
                var Code = dbContext.Redemptions.Where(u => u.code == id).Select(s => s.code).FirstOrDefault();
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
        [AuthorizedRights(Screen = "Redemption", Right = "Read")]
        public ActionResult Previous(int id)
        {
            var arrindex = IDs[0];
            var FirstObj = dbContext.Redemptions.OrderBy(s => s.code).FirstOrDefault(s => s.code == arrindex);
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
                var Code = dbContext.Redemptions.Where(u => u.code == id).Select(s => s.code).FirstOrDefault();
                return RedirectToAction("Details", new { Code = Code });
            }

        }
        [AuthorizedRights(Screen = "Redemption", Right = "Read")]
        public ActionResult ExportToExcel()
        {
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var gv = new GridView();
            var List = (List<int>)TempData["RedSForExc"];
            gv.DataSource = dbContext.Redemptions.Where(Del => List.Contains(Del.code) && Del.delreason == DeleteFlag.NotDeleted).Select(x => new { Code = x.code, CustomerName = x.Customer.EnName, CustomerID = x.cust_id, NOofUnits = x.amount_3, AppNo = x.appliction_no, BranchName = x.Branch.BName, BranchId = x.branch_id, FundName = x.Fund.Name, Auth = x.auth, NavDate = x.nav_date, Nav = x.NAV, Total = x.total, CustomerAccNO = x.cust_acc_no }).ToList();

            //gv.DataSource = (from s in dbContext.Redemptions
            //                 select new
            //                 {
            //                     s.code,



            //                 })
            //                     .ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Redemption" + NowTime + ".xls");
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
        [AuthorizedRights(Screen = "Redemption", Right = "Read")]
        public ActionResult ExportToPDF()
        {
            var Redemptions = dbContext.Redemptions.Select(s => s).Where(s => IDs.Contains(s.code)).OrderBy(u => u.code).ToList();
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var report = new PartialViewAsPdf("~/Areas/Redemptions/Views/Redemption/ExportToPDF.cshtml", Redemptions);
            return report;

        }

        public ActionResult Application(int id)
        {
            var sub = dbContext.Redemptions.Select(s => s).Where(s => s.code == id).FirstOrDefault();

            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var report = new PartialViewAsPdf("~/Areas/Redemptions/Views/Redemption/ApplicationR.cshtml", sub);
            return report;

        }

        public ActionResult Recipt(int id)
        {
            var sub = dbContext.Redemptions.Select(s => s).Where(s => s.code == id).FirstOrDefault();

            //string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var report = new PartialViewAsPdf("~/Areas/Redemptions/Views/Redemption/ReciptR.cshtml", sub);
            return report;

        }
        [AuthorizedRights(Screen = "Redemption", Right = "Authorized")]
        public ActionResult AuthorizeRedemption(int Code)
        {
            var redemption = dbContext.Redemptions.Where(f => f.code == Code).FirstOrDefault();
            var Trans = dbContext.Trans.Where(f => f.transid == redemption.code).FirstOrDefault();
            var ICPrice = dbContext.ICPrices
              .SingleOrDefault(i => i.FundId == redemption.fund_id && i.Auth == true && DbFunctions.TruncateTime(i.Date) == redemption.nav_date.Value.Date);
            var availableUnits = dbContext.Subscriptions.Where(s => s.fund_id == redemption.fund_id && s.cust_id == redemption.cust_id && s.auth == 1).Select(c => c.units).ToList().Sum() - dbContext.Redemptions.Where(s => s.fund_id == redemption.fund_id && s.cust_id == redemption.cust_id).Select(c => c.units).ToList().Sum();
            var pendeingRedemption = dbContext.Redemptions.Where(s => s.fund_id == redemption.fund_id && s.cust_id == redemption.cust_id && s.auth != 1).Select(c => c.units).ToList().Sum();
            var Block = dbContext.Block.Where(s => s.fund_id == redemption.fund_id && s.cust_id == redemption.cust_id && s.BlockCmb == 0).Select(c => c.qty_block).ToList().Sum() - dbContext.Block.Where(s => s.fund_id == redemption.fund_id && s.cust_id == redemption.cust_id && s.BlockCmb == 1 && s.blockauth == true).Select(c => c.qty_block).ToList().Sum();

           
            if (ICPrice == null)
            {
                Session["FailedAuthorizeRed"] = "No iCprice has been added for today.";
                return RedirectToAction("Details", new { Code = Code });
            }
            if (availableUnits - Block < redemption.units)
            {
                Session["FailedUnAuthorizeSub"] = (Block == 0)
                    ? "You won't be able to authorize this redemption because you have : " + pendeingRedemption + "pending redemption."
                    : "You won't be able to authorize this redemption because you have : " + pendeingRedemption + "pending redemption." + " and " + Block + " blocked units.";
                return RedirectToAction("Details", new { Code = Code });
            }
            if (redemption.auth == 1)
            {
                Session["FailedAuthorizeRed"] = "It is Already Autherized.";
                return RedirectToAction("Details", Code);
            }
            if (!redemption.Chk.Value)
            {
                Session["FailedAuthorizeRed"] = "It must be checked first.";
                return RedirectToAction("Details", Code);
            }
            
            Session["FailedAuthorizeRed"] = null;
            redemption.auth = 1;
            redemption.unauth = 0;
            redemption.auther = User.Identity.GetUserId();
            redemption.NAV = ICPrice.Price;
            redemption.nav_date = ICPrice.Date;

            Trans.unit_price = ICPrice.Price;
            Trans.value_date = ICPrice.Date;
            Trans.auth = 1;

            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = Code });

        }

        [AuthorizedRights(Screen = "Redemption", Right = "Authorized")]
        public ActionResult UnAuthorizeRedemption(int Code)
        {
            var Redemption = dbContext.Redemptions.Where(f => f.code == Code).FirstOrDefault();
          
            if (Redemption.unauth == 1)
            {
                Session["FailedUnAuthorizeRed"] = "It is Already Not Autherized.";
                return RedirectToAction("Details", new { Code = Code });
            }
            Session["FailedUnAuthorizeRed"] = null;
            Redemption.unauth = 1;
            Redemption.auth = 0;
            Redemption.auther = User.Identity.GetUserId();


            var Trans = dbContext.Trans.Where(f => f.cust_id == Redemption.cust_id.ToString()).FirstOrDefault();

            Trans.auth = 0;

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
            return RedirectToAction("Details", new { Code = Code });
        }
        [AuthorizedRights(Screen = "Redemption", Right = "Check")]
        public ActionResult CheckRedemptions(int Code)
        {
            var Redemptions = dbContext.Redemptions.Where(f => f.code == Code).FirstOrDefault();
            Redemptions.Chk = true;
            Redemptions.Checker = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = Code });

        }

        [AuthorizedRights(Screen = "Redemption", Right = "Delete")]
        public ActionResult Delete(int Code)
        {
            var Redemption = dbContext.Redemptions.Where(f => f.code == Code).FirstOrDefault();
            var Trans = dbContext.Trans.Where(f => f.transid == Code).FirstOrDefault();
           
            Redemption.delreason = DeleteFlag.Deleted;
            dbContext.Trans.Remove(Trans);
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

        //NEw Function
        [HttpPost]
        public JsonResult GetICPriceInfo(int FundId,string customer_id)

        {
            var Fund0 = dbContext.Funds.SingleOrDefault(ic => ic.FundID == FundId);

            //var availableUnits = (Fund0.no_ics - dbContext.Subscriptions.Where(s => s.fund_id == FundId).Select(c => c.units).ToList().Sum()) + dbContext.Redemptions.Where(s => s.fund_id == FundId && s.auth == 1).Select(c => c.units).ToList().Sum();
            var AvailableForRedemption =  dbContext.Subscriptions.Where(s => s.fund_id == FundId && s.cust_id==customer_id && s.auth == 1).Select(c => c.units).ToList().Sum() - dbContext.Redemptions.Where(s => s.fund_id == FundId && s.cust_id ==customer_id).Select(c => c.units).ToList().Sum();
           
           ICPrice LastInsertrdDay = null;
           if (AvailableForRedemption == 0)
            {
                return Json(new { NOPrice = "3" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var today = DateTime.Today.Date;

                LastInsertrdDay = dbContext.ICPrices.Where(ic => ic.FundId == FundId && ic.Auth == true && DbFunctions.TruncateTime(ic.Date) <= today).OrderByDescending(i => i.Date).FirstOrDefault();
                if (LastInsertrdDay == null)
                        {
                            return Json(new { NOPrice = "1" }, JsonRequestBehavior.AllowGet);
                        }
                   var Fund = new
                            {
                                Date = LastInsertrdDay.Date,
                                Nav = LastInsertrdDay.Price,
                                Nav_Ddate = LastInsertrdDay.Date,
                                ProcessingDate = DateTime.Now
                         };
                            var result = Newtonsoft.Json.JsonConvert.SerializeObject(Fund);

                            return Json(result);

                        }
             
        }

        public ActionResult UploadSurrenders()
        {



            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadSurrenders(UploadSurrendersViewModel vm)
        {



          


            List<string> warningMessages = new List<string>();
            HttpPostedFileBase files = vm.file; //Read the Posted Excel File  
            var path = Server.MapPath("~/Temp");
            //Read the Posted Excel File  
            ISheet sheet; //Create the ISheet object to read the sheet cell values  
            string filename = Path.GetFileName(Server.MapPath(files.FileName)); //get the uploaded file name  
            string fullPath = Path.Combine(path, filename);
            files.SaveAs(fullPath);
            var fileExt = Path.GetExtension(filename); //get the extension of uploaded excel file  
            byte[] buffer; if (fileExt == ".xls")
            {
                HSSFWorkbook hssfwb = new HSSFWorkbook(files.InputStream); //HSSWorkBook object will read the Excel 97-2000 formats  
                sheet = hssfwb.GetSheetAt(0); //get first Excel sheet from workbook  
            }
            else
            {
                XSSFWorkbook hssfwb = new XSSFWorkbook(files.InputStream); //XSSFWorkBook will read 2007 Excel format  
                sheet = hssfwb.GetSheetAt(0); //get first Excel sheet from workbook   
            }
            var policyNo = (sheet.GetRow(1).GetCell(1).ToString()).Split(':')[1].Trim();
            var Policy = dbContext.Policy.Include("AllocationRules").SingleOrDefault(p => p.PolicyNo == policyNo && p.Auth);
            if (Policy == null)
            {
                ModelState.AddModelError("file", "The Policy either does not exist or has not yet been authorized");
                return View(vm);
            }
            var VestingRule = dbContext.VestingRules.Include("VestingRuleDetails").SingleOrDefault(v => v.PolicyId == Policy.Id && v.TransactionType == (byte)VestingRuleTransactionType.Surrender && v.Auth);

            if (VestingRule == null)
            {
                ModelState.AddModelError("file", "The Policy have not Vesting Rules for this fund or has not yet been authorized");
                return View(vm);
            }
            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month - 1));


            var employeesIds = new List<string>();
            var lastTransCode = dbContext.Trans.OrderByDescending(t => t.code).FirstOrDefault().code + 1;
            var lastcode = dbContext.LastCodes.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault() + 1;
     
            var Level = dbContext.UserSecurities.Select(Val => Val.Levels).FirstOrDefault();
            DateTime transactionDate;
            int insertedRow = 0;
            using (DbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
               
                buffer = System.IO.File.ReadAllBytes(fullPath);
                System.IO.File.Delete(fullPath);
                var excel = new ExcelDetails
                {
                    Name = vm.file.FileName + DateTime.Now.ToString("dd/MM/yy,hh:mm"),
                    Status = (byte)ExcelStatus.Pending,
                    Screen = (byte)ExcelTransactionType.Surrender,
                    uploadDate = DateTime.Now,
                    FileContent = buffer,
                    ContentType = files.ContentType,
                    FileExtension = fileExt,
                    FileSize = files.ContentLength,
                    Maker = User.Identity.GetUserId(),
                    Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                    Chk = (Level == Levels.TwoLevels) ? true : false,
                    Auther = null,
                    Auth = false
                };
                dbContext.ExcelDetails.Add(excel);
                dbContext.SaveChanges();
                for (int row = 3; row <= sheet.LastRowNum; row++) //Loop the records upto filled row  
                {
                try
                {

                        if (
                                  sheet.GetRow(row).GetCell(1).ToString() == "" &&
                                  sheet.GetRow(row).GetCell(2).ToString() == "" &&
                                  sheet.GetRow(row).GetCell(3).ToString() == "" &&
                                  sheet.GetRow(row).GetCell(4).ToString() == "" &&
                                  sheet.GetRow(row).GetCell(5).ToString() == "" &&
                                  sheet.GetRow(row).GetCell(6).ToString() == ""

                            )
                        {
                            //ignore row
                        }
                        else if
                        (
                             sheet.GetRow(row).GetCell(1).ToString() == "" ||
                             sheet.GetRow(row).GetCell(2).ToString() == "" ||
                             sheet.GetRow(row).GetCell(3).ToString() == "" ||
                             sheet.GetRow(row).GetCell(4).ToString() == "" ||
                             sheet.GetRow(row).GetCell(5).ToString() == "" ||
                             sheet.GetRow(row).GetCell(6).ToString() == ""

                        )
                        {

                            warningMessages.Add("The data of row : " + (row + 1) + " not complete.");

                        }
                        else if
                     (

                              !DateTime.TryParse(sheet.GetRow(row).GetCell(6).ToString(), out transactionDate)
                     )
                        {
                            warningMessages.Add("Surrender date of row : " + (row + 1) + " are incorrect.");
                        }
                        else
                        {
                            var employeeId = sheet.GetRow(row).GetCell(2).ToString();
                            var employeeName = sheet.GetRow(row).GetCell(3).ToString();
                            var employee = dbContext.Customers.SingleOrDefault(c => c.CustomerID == employeeId && c.Auth && c.CompanyId == Policy.CompanyId && c.EnName.Equals(employeeName.Trim()));
                            if (employee != null && dbContext.EmployeePolicies.Any(c => c.CustomerId == employeeId && c.CompanyId == Policy.CompanyId && c.PolicyId == Policy.Id && !c.isSurrendered))
                            {
                                employeesIds.Add(employeeId);
                                if (dbContext.Subscriptions.Any(s => s.cust_id == employeeId && s.PolicyId == Policy.Id))
                                {
                                    if (dbContext.Redemptions.Any(r => r.cust_id == employeeId && r.PolicyId == Policy.Id && r.auth != 1) || dbContext.Subscriptions.Any(r => r.cust_id == employeeId && r.PolicyId == Policy.Id && r.auth != 1))
                                    {
                                        warningMessages.Add("The employee of row : " + (row + 1) + " has pending transaction so it cannot be surrended.");
                                    }
                                    else
                                    {
                                        

                                            var transactionDateWithoutTime = transactionDate.Date;
                                        



                                        if (dbContext.Redemptions.Any(r => r.cust_id == employeeId && r.PolicyId == Policy.Id && DbFunctions.TruncateTime(r.Nav_Ddate) <= transactionDateWithoutTime) || dbContext.Subscriptions.Any(r => r.cust_id == employeeId && r.PolicyId == Policy.Id && DbFunctions.TruncateTime(r.Nav_Ddate) <= transactionDateWithoutTime))
                                        {
                                            warningMessages.Add("The employee of row : " + (row + 1) + " has transaction after "+ transactionDateWithoutTime + " so it cannot be surrended.");
                                        }
                                        else
                                        {
                                            var numberOfYears = (VestingRule.Base == (byte)VestingRuleBase.Joining)
                                            ? (DateTime.Now.Date - employee.DateOfContribute.Date).Days / 365
                                            : (DateTime.Now.Date - employee.DateOfHiring.Date).Days / 365;
                                            VestingRuleDetails vestingRuleDetail = VestingRule.VestingRuleDetails.SingleOrDefault(vr => numberOfYears >= vr.FromYear && numberOfYears <= vr.ToYear);

                                            if (vestingRuleDetail != null)
                                            {

                                                foreach (var allocationRule in Policy.AllocationRules)
                                                {

                                                    var icprice = dbContext.ICPrices.Where(ic => ic.FundId == allocationRule.FundId && ic.Auth == true && DbFunctions.TruncateTime(ic.Date) == transactionDateWithoutTime).OrderByDescending(i => i.Date).FirstOrDefault();

                                                    if (icprice == null)//icprice from api
                                                    {
                                                        warningMessages.Add("The Fund  : " + allocationRule.Fund.Name + " has no price.");
                                                    }
                                                    else
                                                    {
                                                        var allContribution = dbContext.Subscriptions.Where(s => s.auth == 1 && s.PolicyId == Policy.Id && s.cust_id == employeeId && s.fund_id == allocationRule.FundId);
                                                        var allWithdrawal = dbContext.Redemptions.Where(f => f.auth == 1 && f.PolicyId == Policy.Id && f.cust_id == employeeId && f.fund_id == allocationRule.FundId);


                                                        Decimal employeeContributions = (allContribution.Where(ec => ec.GTF_Flag == 1).Select(ecu => ecu.units).ToList().Sum().Value - allWithdrawal.Where(ec => ec.GTF_Flag == 1).Select(ecu => ecu.units).ToList().Sum().Value) * icprice.Price;
                                                        if (employeeContributions != 0)
                                                        {
                                                            var AmountWillBeAddedAsSubForZeroAcc = (employeeContributions * icprice.Price) * Decimal.Divide((100 - (vestingRuleDetail.PercentageOfEmpShare)), 100);
                                                            var AmountWillBeAddedAsRedForSurrendedEmp = employeeContributions * icprice.Price;

                                                            Redemption SuurendedEmployeeContributions;
                                                            Trans transSuurendedEmployeeContributions;
                                                            SuurendedEmployeeContributions = new Redemption
                                                            {

                                                                code = lastcode,
                                                                cust_id = employeeId,
                                                                NAV = icprice.Price,
                                                                amount_3 = AmountWillBeAddedAsRedForSurrendedEmp,
                                                                units = AmountWillBeAddedAsRedForSurrendedEmp / icprice.Price,
                                                                SysDate = DateTime.Now,
                                                                fund_id = allocationRule.FundId,
                                                                inputer = User.Identity.GetUserId(),
                                                                Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                                                                Chk = (Level == Levels.TwoLevels) ? true : false,
                                                                auther = null,
                                                                auth = 0,
                                                                GTF_Flag = 1,
                                                                UserID = User.Identity.GetUserId(),
                                                                PolicyId = Policy.Id,
                                                                delreason = DeleteFlag.NotDeleted,


                                                                nav_date = icprice.Date,
                                                                Nav_Ddate = icprice.Date,
                                                                ProcessingDate = icprice.Date,
                                                                total = AmountWillBeAddedAsRedForSurrendedEmp,
                                                                system_date = DateTime.Now,
                                                                ExcelId = excel.Id
                                                            };
                                                            transSuurendedEmployeeContributions = new Trans()
                                                            {

                                                                transid = lastcode,
                                                                total_value = AmountWillBeAddedAsRedForSurrendedEmp,
                                                                quantity = AmountWillBeAddedAsRedForSurrendedEmp / icprice.Price,
                                                                unit_price = icprice.Price,
                                                                SysDate = DateTime.Now,
                                                                cust_id = employeeId,
                                                                fund_id = allocationRule.FundId,
                                                                inputer = User.Identity.GetUserId(),
                                                                auth = 0,
                                                                GTF_Flag = 5,
                                                                UserID = User.Identity.GetUserId(),
                                                                pur_sal = 1,
                                                                PolicyId = Policy.Id,
                                                                value_date = icprice.Date,
                                                                Nav_Ddate = icprice.Date,
                                                                ProcessingDate = icprice.Date,
                                                                ExcelId = excel.Id
                                                            };
                                                            dbContext.Redemptions.Add(SuurendedEmployeeContributions);
                                                            dbContext.Trans.Add(transSuurendedEmployeeContributions);
                                                            dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                                                            lastcode++;

                                                            //--------------
                                                            Subscription subscriptionZeroAccount;
                                                            Trans transZeroAccount;
                                                            subscriptionZeroAccount = new Subscription
                                                            {

                                                                code = lastcode,
                                                                cust_id = accountType.ZeroAccount,
                                                                NAV = icprice.Price,
                                                                amount_3 = AmountWillBeAddedAsSubForZeroAcc,
                                                                units = AmountWillBeAddedAsSubForZeroAcc / icprice.Price,
                                                                SysDate = DateTime.Now,
                                                                fund_id = allocationRule.FundId,
                                                                inputer = User.Identity.GetUserId(),
                                                                Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                                                                Chk = (Level == Levels.TwoLevels) ? true : false,
                                                                auther = null,
                                                                auth = 0,
                                                                GTF_Flag = 1,
                                                                UserID = User.Identity.GetUserId(),
                                                                delreason = DeleteFlag.NotDeleted,
                                                                PolicyId = Policy.Id,
                                                                nav_date = icprice.Date,
                                                                Nav_Ddate = icprice.Date,
                                                                ProcessingDate = icprice.Date,
                                                                total = AmountWillBeAddedAsSubForZeroAcc,
                                                                system_date = DateTime.Now,
                                                                ExcelId = excel.Id
                                                            };
                                                            transZeroAccount = new Trans()
                                                            {

                                                                transid = lastcode,
                                                                total_value = AmountWillBeAddedAsSubForZeroAcc,
                                                                quantity = AmountWillBeAddedAsSubForZeroAcc / icprice.Price,
                                                                unit_price = icprice.Price,
                                                                SysDate = DateTime.Now,
                                                                cust_id = accountType.ZeroAccount,
                                                                fund_id = allocationRule.FundId,
                                                                inputer = User.Identity.GetUserId(),
                                                                auth = 0,
                                                                resigned_id = lastTransCode,
                                                                GTF_Flag = 5,
                                                                UserID = User.Identity.GetUserId(),
                                                                pur_sal = 0,
                                                                PolicyId = Policy.Id,
                                                                value_date = icprice.Date,
                                                                Nav_Ddate = icprice.Date,
                                                                ProcessingDate = icprice.Date,
                                                                CustomerID = employeeId,

                                                                ExcelId = excel.Id
                                                            };

                                                            dbContext.Subscriptions.Add(subscriptionZeroAccount);
                                                            dbContext.Trans.Add(transZeroAccount);
                                                            dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                                                            lastcode++;
                                                            lastTransCode++;

                                                        }
                                                        Decimal employerContributions = (allContribution.Where(ec => ec.GTF_Flag == 2).Select(ecu => ecu.units).ToList().Sum().Value - allWithdrawal.Where(ec => ec.GTF_Flag == 2).Select(ecu => ecu.units).ToList().Sum().Value) * icprice.Price;

                                                        if (employerContributions != 0)
                                                        {
                                                            var AmountWillBeAddedAsSubForZeroAcc = (employerContributions * icprice.Price) * Decimal.Divide((100 - (vestingRuleDetail.PercentageOfCompanyShare)), 100);
                                                            var AmountWillBeAddedAsRedForSurrendedEmp = employerContributions * icprice.Price;
                                                            Redemption SuurendedEmployeeContributions;
                                                            Trans transSuurendedEmployeeContributions;
                                                            SuurendedEmployeeContributions = new Redemption
                                                            {

                                                                code = lastcode,
                                                                cust_id = employeeId,
                                                                NAV = icprice.Price,
                                                                amount_3 = AmountWillBeAddedAsRedForSurrendedEmp,
                                                                units = AmountWillBeAddedAsRedForSurrendedEmp / icprice.Price,
                                                                SysDate = DateTime.Now,
                                                                fund_id = allocationRule.FundId,
                                                                inputer = User.Identity.GetUserId(),
                                                                Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                                                                Chk = (Level == Levels.TwoLevels) ? true : false,
                                                                auther = null,
                                                                auth = 0,
                                                                GTF_Flag = 2,
                                                                UserID = User.Identity.GetUserId(),
                                                                PolicyId = Policy.Id,
                                                                delreason = DeleteFlag.NotDeleted,


                                                                nav_date = icprice.Date,
                                                                Nav_Ddate = icprice.Date,
                                                                ProcessingDate = icprice.Date,
                                                                total = AmountWillBeAddedAsRedForSurrendedEmp,
                                                                system_date = DateTime.Now,
                                                                ExcelId = excel.Id
                                                            };
                                                            transSuurendedEmployeeContributions = new Trans()
                                                            {

                                                                transid = lastcode,
                                                                total_value = AmountWillBeAddedAsRedForSurrendedEmp,
                                                                quantity = AmountWillBeAddedAsRedForSurrendedEmp / icprice.Price,
                                                                unit_price = icprice.Price,
                                                                SysDate = DateTime.Now,
                                                                cust_id = employeeId,
                                                                fund_id = allocationRule.FundId,
                                                                inputer = User.Identity.GetUserId(),
                                                                auth = 0,
                                                                GTF_Flag = 5,
                                                                PolicyId = Policy.Id,
                                                                UserID = User.Identity.GetUserId(),
                                                                pur_sal = 1,
                                                                value_date = icprice.Date,
                                                                Nav_Ddate = icprice.Date,
                                                                ProcessingDate = icprice.Date,
                                                                ExcelId = excel.Id
                                                            };
                                                            dbContext.Redemptions.Add(SuurendedEmployeeContributions);
                                                            dbContext.Trans.Add(transSuurendedEmployeeContributions);
                                                            dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                                                            lastcode++;

                                                            Subscription subscriptionZeroAccount;
                                                            Trans transZeroAccount;
                                                            subscriptionZeroAccount = new Subscription
                                                            {
                                                                code = lastcode,
                                                                cust_id = employeeId,
                                                                NAV = icprice.Price,
                                                                amount_3 = AmountWillBeAddedAsSubForZeroAcc,
                                                                units = AmountWillBeAddedAsSubForZeroAcc / icprice.Price,
                                                                SysDate = DateTime.Now,
                                                                fund_id = allocationRule.FundId,
                                                                inputer = User.Identity.GetUserId(),
                                                                Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                                                                Chk = (Level == Levels.TwoLevels) ? true : false,
                                                                auther = null,
                                                                auth = 0,
                                                                GTF_Flag = 5,
                                                                UserID = User.Identity.GetUserId(),
                                                                delreason = DeleteFlag.NotDeleted,
                                                                PolicyId = Policy.Id,
                                                                nav_date = icprice.Date,
                                                                Nav_Ddate = icprice.Date,
                                                                ProcessingDate = icprice.Date,
                                                                total = AmountWillBeAddedAsSubForZeroAcc,
                                                                system_date = DateTime.Now,
                                                                ExcelId = excel.Id
                                                            };
                                                            transZeroAccount = new Trans()
                                                            {

                                                                transid = lastcode,
                                                                total_value = AmountWillBeAddedAsSubForZeroAcc,
                                                                quantity = AmountWillBeAddedAsSubForZeroAcc / icprice.Price,
                                                                unit_price = icprice.Price,
                                                                SysDate = DateTime.Now,
                                                                cust_id = accountType.ZeroAccount,
                                                                fund_id = allocationRule.FundId,
                                                                inputer = User.Identity.GetUserId(),
                                                                auth = 0,
                                                                GTF_Flag = 5,
                                                                UserID = User.Identity.GetUserId(),
                                                                pur_sal = 0,
                                                                PolicyId = Policy.Id,
                                                                value_date = icprice.Date,
                                                                Nav_Ddate = icprice.Date,
                                                                ProcessingDate = icprice.Date,
                                                                resigned_id = lastTransCode,
                                                                CustomerID = employeeId,
                                                                ExcelId = excel.Id
                                                            };
                                                            dbContext.Subscriptions.Add(subscriptionZeroAccount);
                                                            dbContext.Trans.Add(transZeroAccount);
                                                            dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                                                            lastcode++;
                                                            lastTransCode++;
                                                        }

                                                        Decimal additionalEmployeeContributions = (allContribution.Where(ec => ec.GTF_Flag == 3).Select(ecu => ecu.units).ToList().Sum().Value - allWithdrawal.Where(ec => ec.GTF_Flag == 3).Select(ecu => ecu.units).ToList().Sum().Value) * icprice.Price;
                                                        if (additionalEmployeeContributions != 0)
                                                        {
                                                            var AmountWillBeAddedAsSubForZeroAcc = (additionalEmployeeContributions * icprice.Price) * Decimal.Divide((100 - (vestingRuleDetail.PercentageOfEmpShareBooster)), 100);
                                                            var AmountWillBeAddedAsRedForSurrendedEmp = additionalEmployeeContributions * icprice.Price;
                                                            Redemption SuurendedEmployeeContributions;
                                                            Trans transSuurendedEmployeeContributions;
                                                            SuurendedEmployeeContributions = new Redemption
                                                            {

                                                                code = lastcode,
                                                                cust_id = employeeId,
                                                                NAV = icprice.Price,
                                                                amount_3 = AmountWillBeAddedAsRedForSurrendedEmp,
                                                                units = AmountWillBeAddedAsRedForSurrendedEmp / icprice.Price,
                                                                SysDate = DateTime.Now,
                                                                fund_id = allocationRule.FundId,
                                                                inputer = User.Identity.GetUserId(),
                                                                Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                                                                Chk = (Level == Levels.TwoLevels) ? true : false,
                                                                auther = null,
                                                                auth = 0,
                                                                GTF_Flag = 3,
                                                                UserID = User.Identity.GetUserId(),
                                                                PolicyId = Policy.Id,
                                                                delreason = DeleteFlag.NotDeleted,


                                                                nav_date = icprice.Date,
                                                                Nav_Ddate = icprice.Date,
                                                                ProcessingDate = icprice.Date,
                                                                total = AmountWillBeAddedAsRedForSurrendedEmp,
                                                                system_date = DateTime.Now,
                                                                ExcelId = excel.Id
                                                            };
                                                            transSuurendedEmployeeContributions = new Trans()
                                                            {

                                                                transid = lastcode,
                                                                total_value = AmountWillBeAddedAsRedForSurrendedEmp,
                                                                quantity = AmountWillBeAddedAsRedForSurrendedEmp / icprice.Price,
                                                                unit_price = icprice.Price,
                                                                SysDate = DateTime.Now,
                                                                cust_id = employeeId,
                                                                fund_id = allocationRule.FundId,
                                                                inputer = User.Identity.GetUserId(),
                                                                auth = 0,
                                                                PolicyId = Policy.Id,

                                                                GTF_Flag = 5,
                                                                UserID = User.Identity.GetUserId(),
                                                                pur_sal = 1,
                                                                value_date = icprice.Date,
                                                                Nav_Ddate = icprice.Date,
                                                                ProcessingDate = icprice.Date,
                                                                ExcelId = excel.Id
                                                            };
                                                            dbContext.Redemptions.Add(SuurendedEmployeeContributions);
                                                            dbContext.Trans.Add(transSuurendedEmployeeContributions);
                                                            dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                                                            lastcode++;

                                                            Subscription subscriptionZeroAccount;
                                                            Trans transZeroAccount;
                                                            subscriptionZeroAccount = new Subscription
                                                            {
                                                                code = lastcode,
                                                                cust_id = employeeId,
                                                                NAV = icprice.Price,
                                                                amount_3 = AmountWillBeAddedAsSubForZeroAcc,
                                                                units = AmountWillBeAddedAsSubForZeroAcc / icprice.Price,
                                                                SysDate = DateTime.Now,
                                                                fund_id = allocationRule.FundId,
                                                                inputer = User.Identity.GetUserId(),
                                                                Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                                                                Chk = (Level == Levels.TwoLevels) ? true : false,
                                                                auther = null,
                                                                auth = 0,
                                                                GTF_Flag = 5,
                                                                UserID = User.Identity.GetUserId(),
                                                                delreason = DeleteFlag.NotDeleted,
                                                                PolicyId = Policy.Id,
                                                                nav_date = DateTime.Now.Date,
                                                                Nav_Ddate = DateTime.Now.Date,
                                                                ProcessingDate = DateTime.Now.Date,
                                                                total = AmountWillBeAddedAsSubForZeroAcc,
                                                                system_date = DateTime.Now,
                                                                ExcelId = excel.Id
                                                            };
                                                            transZeroAccount = new Trans()
                                                            {

                                                                transid = lastcode,
                                                                total_value = AmountWillBeAddedAsSubForZeroAcc,
                                                                quantity = AmountWillBeAddedAsSubForZeroAcc / icprice.Price,
                                                                unit_price = icprice.Price,
                                                                SysDate = DateTime.Now,
                                                                cust_id = accountType.ZeroAccount,
                                                                fund_id = allocationRule.FundId,
                                                                inputer = User.Identity.GetUserId(),
                                                                auth = 0,
                                                                GTF_Flag = 5,
                                                                UserID = User.Identity.GetUserId(),
                                                                pur_sal = 0,
                                                                PolicyId = Policy.Id,
                                                                value_date = icprice.Date,
                                                                Nav_Ddate = icprice.Date,
                                                                ProcessingDate = icprice.Date,
                                                                resigned_id = lastTransCode,
                                                                CustomerID = employeeId,
                                                                ExcelId = excel.Id
                                                            };
                                                            dbContext.Subscriptions.Add(subscriptionZeroAccount);
                                                            dbContext.Trans.Add(transZeroAccount);
                                                            dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                                                            lastcode++;
                                                            lastTransCode++;
                                                        }

                                                        Decimal additionalEmployerContributions = (allContribution.Where(ec => ec.GTF_Flag == 4).Select(ecu => ecu.units).ToList().Sum().Value - allWithdrawal.Where(ec => ec.GTF_Flag == 4).Select(ecu => ecu.units).ToList().Sum().Value) * icprice.Price;
                                                        if (additionalEmployerContributions != 0)
                                                        {
                                                            var AmountWillBeAddedAsSubForZeroAcc = (employerContributions * icprice.Price) * Decimal.Divide((100 - (vestingRuleDetail.PercentageOfCompanyShareBooster)), 100);
                                                            var AmountWillBeAddedAsRedForSurrendedEmp = employerContributions * icprice.Price;
                                                            Redemption SuurendedEmployeeContributions;
                                                            Trans transSuurendedEmployeeContributions;
                                                            SuurendedEmployeeContributions = new Redemption
                                                            {

                                                                code = lastcode,
                                                                cust_id = employeeId,
                                                                NAV = icprice.Price,
                                                                amount_3 = AmountWillBeAddedAsRedForSurrendedEmp,
                                                                units = AmountWillBeAddedAsRedForSurrendedEmp / icprice.Price,
                                                                SysDate = DateTime.Now,
                                                                fund_id = allocationRule.FundId,
                                                                inputer = User.Identity.GetUserId(),
                                                                Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                                                                Chk = (Level == Levels.TwoLevels) ? true : false,
                                                                auther = null,
                                                                auth = 0,
                                                                GTF_Flag = 4,
                                                                UserID = User.Identity.GetUserId(),
                                                                PolicyId = Policy.Id,
                                                                delreason = DeleteFlag.NotDeleted,

                                                                nav_date = icprice.Date,
                                                                Nav_Ddate = icprice.Date,
                                                                ProcessingDate = icprice.Date,
                                                                total = AmountWillBeAddedAsRedForSurrendedEmp,
                                                                system_date = DateTime.Now,
                                                                ExcelId = excel.Id
                                                            };
                                                            transSuurendedEmployeeContributions = new Trans()
                                                            {

                                                                transid = lastcode,
                                                                total_value = AmountWillBeAddedAsRedForSurrendedEmp,
                                                                quantity = AmountWillBeAddedAsRedForSurrendedEmp / icprice.Price,
                                                                unit_price = icprice.Price,
                                                                SysDate = DateTime.Now,
                                                                cust_id = employeeId,
                                                                fund_id = allocationRule.FundId,
                                                                inputer = User.Identity.GetUserId(),
                                                                auth = 0,
                                                                GTF_Flag = 5,
                                                                UserID = User.Identity.GetUserId(),
                                                                pur_sal = 1,
                                                                value_date = icprice.Date,
                                                                Nav_Ddate = icprice.Date,
                                                                PolicyId = Policy.Id,
                                                                ProcessingDate = icprice.Date,
                                                                ExcelId = excel.Id
                                                            };
                                                            dbContext.Redemptions.Add(SuurendedEmployeeContributions);
                                                            dbContext.Trans.Add(transSuurendedEmployeeContributions);
                                                            dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                                                            lastcode++;

                                                            Subscription subscriptionZeroAccount;
                                                            Trans transZeroAccount;
                                                            subscriptionZeroAccount = new Subscription
                                                            {
                                                                code = lastcode,
                                                                cust_id = employeeId,
                                                                NAV = icprice.Price,
                                                                amount_3 = AmountWillBeAddedAsSubForZeroAcc,
                                                                units = AmountWillBeAddedAsSubForZeroAcc / icprice.Price,
                                                                SysDate = DateTime.Now,
                                                                fund_id = allocationRule.FundId,
                                                                inputer = User.Identity.GetUserId(),
                                                                GTF_Flag = 5,
                                                                UserID = User.Identity.GetUserId(),
                                                                delreason = DeleteFlag.NotDeleted,
                                                                Chk = true,
                                                                auth = 0,
                                                                PolicyId = Policy.Id,
                                                                nav_date = icprice.Date,
                                                                Nav_Ddate = icprice.Date,
                                                                ProcessingDate = icprice.Date,
                                                                total = AmountWillBeAddedAsSubForZeroAcc,
                                                                system_date = DateTime.Now,
                                                                ExcelId = excel.Id

                                                            };
                                                            transZeroAccount = new Trans()
                                                            {

                                                                transid = lastcode,
                                                                total_value = AmountWillBeAddedAsSubForZeroAcc,
                                                                quantity = AmountWillBeAddedAsSubForZeroAcc / icprice.Price,
                                                                unit_price = icprice.Price,
                                                                SysDate = DateTime.Now,
                                                                cust_id = accountType.ZeroAccount,
                                                                fund_id = allocationRule.FundId,
                                                                inputer = User.Identity.GetUserId(),
                                                                auth = 0,
                                                                GTF_Flag = 5,
                                                                UserID = User.Identity.GetUserId(),
                                                                pur_sal = 0,
                                                                PolicyId = Policy.Id,
                                                                value_date = icprice.Date,
                                                                Nav_Ddate = icprice.Date,
                                                                ProcessingDate = icprice.Date,
                                                                resigned_id = lastTransCode,
                                                                CustomerID = employeeId,
                                                                ExcelId = excel.Id
                                                            };
                                                            dbContext.Subscriptions.Add(subscriptionZeroAccount);
                                                            dbContext.Trans.Add(transZeroAccount);
                                                            dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                                                            lastcode++;
                                                            lastTransCode++;
                                                        }

                                                        insertedRow++;
                                                    }

                                                }

                                            }
                                            else
                                            {
                                                warningMessages.Add("The employee of row : " + (row + 1) + " His period does not have a rule for calculating his Financial dues.");
                                            }
                                        }

                                    }

                                }
                                else
                                {
                                    warningMessages.Add("The employee of row : " + (row + 1) + " does not have any dues.");
                                }


                            }
                            else
                            {
                                warningMessages.Add("The employee of row : " + (row + 1) + " is not exist.");

                            }

                        }

                }
                catch (Exception ex)
                {
                    warningMessages.Add("The employee data of row : " + (row + 1) + " is invalid.");
                }

            }




                ViewBag.Message = warningMessages;

                if (warningMessages.Count == 0 && insertedRow > 0)
                {
                    try
                    {
                        dbContext.EmployeePolicies.Where(c => employeesIds.Contains(c.CustomerId) && c.CompanyId == Policy.CompanyId && c.PolicyId == Policy.Id).ToList().ForEach(c => c.isSurrendered = true);

                        dbContext.SaveChanges();
                        transaction.Commit();

                        ViewBag.Message = "File Uploaded Successfully.";

                    }
                    catch (DbEntityValidationException ex)
                    {

                        transaction.Rollback();
                        ViewBag.Message = "Check constraints.";
                    }
                }
                else
                {
                    transaction.Rollback();
                }






            }

            return View(vm);
        }

        //------------------
        public ActionResult UploadWithdrawals()
        {


            ViewData["TransactionType"] = VestingRuleTransactionType.Withdrawal;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadWithdrawals(UploadWithdrawalsViewModel vm)
        {






            List<string> warningMessages = new List<string>();
            HttpPostedFileBase files = vm.file; //Read the Posted Excel File  
            var path = Server.MapPath("~/Temp");
            //Read the Posted Excel File  
            ISheet sheet; //Create the ISheet object to read the sheet cell values  
            string filename = Path.GetFileName(Server.MapPath(files.FileName)); //get the uploaded file name  
            string fullPath = Path.Combine(path, filename);
            files.SaveAs(fullPath);
            var fileExt = Path.GetExtension(filename); //get the extension of uploaded excel file  
            byte[] buffer; if (fileExt == ".xls")
            {
                HSSFWorkbook hssfwb = new HSSFWorkbook(files.InputStream); //HSSWorkBook object will read the Excel 97-2000 formats  
                sheet = hssfwb.GetSheetAt(0); //get first Excel sheet from workbook  
            }
            else
            {
                XSSFWorkbook hssfwb = new XSSFWorkbook(files.InputStream); //XSSFWorkBook will read 2007 Excel format  
                sheet = hssfwb.GetSheetAt(0); //get first Excel sheet from workbook   
            }
            var policyNo = (sheet.GetRow(1).GetCell(1).ToString()).Split(':')[1].Trim();
            var Policy = dbContext.Policy.Include("AllocationRules").SingleOrDefault(p => p.PolicyNo == policyNo && p.Auth);
            if (Policy == null)
            {
                ModelState.AddModelError("file", "The Policy either does not exist or has not yet been authorized");
                return View(vm);
            }
            var VestingRule = dbContext.VestingRules.Include("VestingRuleDetails").SingleOrDefault(v => v.PolicyId == Policy.Id && v.TransactionType == (byte)VestingRuleTransactionType.Withdrawal && v.Auth);

            if (VestingRule == null)
            {
                ModelState.AddModelError("file", "The Policy have not Vesting Rules for this fund or has not yet been authorized");
                return View(vm);
            }
            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month - 1));

            
           
            var lastcode = dbContext.LastCodes.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault();
            lastcode++;
            var Level = dbContext.UserSecurities.Select(Val => Val.Levels).FirstOrDefault();
            DateTime transactionDate;
            int insertedRow = 0;
            int checkNumber;
            
            using (DbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {

                buffer = System.IO.File.ReadAllBytes(fullPath);
                System.IO.File.Delete(fullPath);
                var excel = new ExcelDetails
                {
                    Name = vm.file.FileName + DateTime.Now.ToString("dd:MM:yy , hh:mm"),
                    Status = (byte)ExcelStatus.Pending,
                    Screen = (byte)ExcelTransactionType.Withdrawal,
                    uploadDate = DateTime.Now,
                    FileContent = buffer,
                    ContentType = files.ContentType,
                    FileExtension = fileExt,
                    FileSize = files.ContentLength,
                    Maker = User.Identity.GetUserId(),
                    Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                    Chk = (Level == Levels.TwoLevels) ? true : false,
                    Auther = null,
                    Auth = false
                };
                dbContext.ExcelDetails.Add(excel);
                dbContext.SaveChanges();
                for (int row = 4; row <= sheet.LastRowNum; row++) //Loop the records upto filled row  
            {
                try
                {

                    if (
                              sheet.GetRow(row).GetCell(1).ToString() == "" &&
                              sheet.GetRow(row).GetCell(2).ToString() == "" &&
                              sheet.GetRow(row).GetCell(3).ToString() == "" &&
                              sheet.GetRow(row).GetCell(4).ToString() == "" &&
                              sheet.GetRow(row).GetCell(5).ToString() == "" &&
                              sheet.GetRow(row).GetCell(6).ToString() == "" &&
                              sheet.GetRow(row).GetCell(7).ToString() == "" &&
                              sheet.GetRow(row).GetCell(8).ToString() == "" &&
                              sheet.GetRow(row).GetCell(9).ToString() == "" &&
                              sheet.GetRow(row).GetCell(10).ToString() == "" &&
                              sheet.GetRow(row).GetCell(11).ToString() == ""

                        )
                    {
                        //ignore row
                        warningMessages.Add("The data of row : " + (row + 1) + " not complete.");
                    }
                    else if
                    (
                         sheet.GetRow(row).GetCell(1).ToString() == ""  ||
                         sheet.GetRow(row).GetCell(2).ToString() == ""  ||
                         sheet.GetRow(row).GetCell(3).ToString() == ""  ||
                         sheet.GetRow(row).GetCell(4).ToString() == ""  ||
                         sheet.GetRow(row).GetCell(5).ToString() == ""  ||
                         sheet.GetRow(row).GetCell(6).ToString() == ""  ||
                         sheet.GetRow(row).GetCell(7).ToString() == ""  ||
                         sheet.GetRow(row).GetCell(8).ToString() == ""  ||
                         sheet.GetRow(row).GetCell(9).ToString() == ""  ||
                         sheet.GetRow(row).GetCell(10).ToString() == "" ||
                         sheet.GetRow(row).GetCell(11).ToString() == "" 
                         


                    )
                    {

                        warningMessages.Add("The data of row : " + (row + 1) + " not complete.");

                    }
                        else if
                   (
                          
                            !(int.TryParse(sheet.GetRow(row).GetCell(6).ToString(), out checkNumber)) ||
                            !(int.TryParse(sheet.GetRow(row).GetCell(7).ToString(), out checkNumber)) ||
                            !(int.TryParse(sheet.GetRow(row).GetCell(8).ToString(), out checkNumber)) ||
                            !(int.TryParse(sheet.GetRow(row).GetCell(9).ToString(), out checkNumber)) 
                           

                   )
                        {
                            warningMessages.Add("Amounts to be withdrawn of row : " + (row + 1) + " are incorrect.");
                        }
                        else if
                (

                         !(int.TryParse(sheet.GetRow(row).GetCell(10).ToString(), out checkNumber)) 
                        


                )
                        {
                            warningMessages.Add("The Fund of row : " + (row + 1) + " is not exist.");
                        }
                        else if
                      (

                               !DateTime.TryParse(sheet.GetRow(row).GetCell(11).ToString(), out transactionDate)
                      )
                        {
                            warningMessages.Add("withdrawal date of row : " + (row + 1) + " are incorrect.");
                        }
                        else
                    {
                        var employeeId = sheet.GetRow(row).GetCell(2).ToString();
                        var employeeName = sheet.GetRow(row).GetCell(3).ToString();
                        var employee = dbContext.Customers.SingleOrDefault(c => c.CustomerID == employeeId && c.Auth && c.CompanyId == Policy.CompanyId && c.EnName.Equals(employeeName.Trim()));
                        if (employee != null && dbContext.EmployeePolicies.Any(c => c.CustomerId == employeeId && c.CompanyId == Policy.CompanyId && c.PolicyId == Policy.Id && !c.isSurrendered))
                        {
                         
                            if (dbContext.Subscriptions.Where(s => s.cust_id == employeeId && s.PolicyId == Policy.Id).Count() > 0)
                            {
                                var numberOfYears = (VestingRule.Base == (byte)VestingRuleBase.Joining)
                                ? Decimal.Divide((DateTime.Now.Date - employee.DateOfContribute.Date).Days , 365)
                                : Decimal.Divide((DateTime.Now.Date - employee.DateOfHiring.Date).Days  , 365);
            VestingRuleDetails vestingRuleDetail = VestingRule.VestingRuleDetails.SingleOrDefault(vr => numberOfYears >= vr.FromYear && numberOfYears <= vr.ToYear);

                                if (vestingRuleDetail != null)
                                {

                                
                                        var fund_id_excel = int.Parse(sheet.GetRow(row).GetCell(10).ToString());
                                        var Fund = dbContext.Funds.SingleOrDefault(f => f.FundID == fund_id_excel  && f.Auth);
                                        if (Fund != null)
                                    {
                                            var transactionDateWithoutTime = transactionDate.Date;

                                            var icprice = dbContext.ICPrices.Where(ic => ic.FundId == Fund.FundID && ic.Auth == true && DbFunctions.TruncateTime(ic.Date) == transactionDateWithoutTime).OrderByDescending(i => i.Date).FirstOrDefault();

                                            if (icprice == null)//icprice from api
                {
                    warningMessages.Add("The Fund with code : " + sheet.GetRow(row).GetCell(10).ToString() + " has no price.");
                }
                else
                {
                                            var allContribution = dbContext.Subscriptions.Where(s => s.auth == 1 && s.PolicyId == Policy.Id && s.cust_id == employeeId && s.fund_id == Fund.FundID);
                                            var allWithdrawal = dbContext.Redemptions.Where(f => f.PolicyId == Policy.Id && f.cust_id == employeeId && f.fund_id == Fund.FundID);


                                            Decimal employeeWithdrawal;
                    if (Decimal.TryParse(Convert.ToString(sheet.GetRow(row).GetCell(6)), out employeeWithdrawal))
                    {
                        if (employeeWithdrawal != 0)
                        {

                                                    Decimal employeeContributions = (allContribution.Where(ec => ec.GTF_Flag == 1).Select(ecu => ecu.units).ToList().Sum().Value - allWithdrawal.Where(ec => ec.GTF_Flag == 1).Select(ecu => ecu.units).ToList().Sum().Value) * icprice.Price;
                                                    if (Decimal.Divide(employeeWithdrawal , employeeContributions)*100 <= vestingRuleDetail.PercentageOfEmpShare)
                                                    {
                                                        Redemption redemptionEmployeeWithdrawal;
                                                        Trans transEmployeeWithdrawal;
                                                        redemptionEmployeeWithdrawal = new Redemption
                                                        {
                                                            code = lastcode,
                                                            cust_id = employeeId,
                                                            NAV = icprice.Price,//api to get icprice
                                                            amount_3 = employeeWithdrawal ,
                                                            units = employeeWithdrawal  / icprice.Price,//api to get icprice
                                                            SysDate = DateTime.Now,
                                                            fund_id = int.Parse(Fund.Code),
                                                            inputer = User.Identity.GetUserId(),

                                                            Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                                                            Chk = (Level == Levels.TwoLevels) ? true : false,
                                                            auther = null,
                                                            auth = 0,
                                                            GTF_Flag = 1,
                                                            UserID = User.Identity.GetUserId(),
                                                            PolicyId = Policy.Id,
                                                            delreason = DeleteFlag.NotDeleted,

                                                            nav_date = icprice.Date,//api to get icprice
                                                            Nav_Ddate = icprice.Date,//api to get icprice
                                                            ProcessingDate = icprice.Date,//api to get icprice
                                                            total = employeeWithdrawal,
                                                            system_date = DateTime.Now,
                                                            ExcelId = excel.Id
                                                        };
                                                        transEmployeeWithdrawal = new Trans()
                                                        {

                                                            transid = lastcode,
                                                            total_value = employeeWithdrawal,
                                                            quantity = employeeWithdrawal  / icprice.Price,//api to get icprice
                                                            unit_price = icprice.Price,//api to get icprice
                                                            SysDate = DateTime.Now,
                                                            cust_id = employeeId,
                                                            fund_id = int.Parse(Fund.Code),
                                                            inputer = User.Identity.GetUserId(),

                                                            auth = 0,
                                                            value_date = icprice.Date,//api to get icprice
                                                            Nav_Ddate = icprice.Date,//api to get icprice
                                                            ProcessingDate = icprice.Date,//api to get icprice
                                                            GTF_Flag = 1,
                                                            UserID = User.Identity.GetUserId(),
                                                            PolicyId = Policy.Id,
                                                            ExcelId = excel.Id
                                                        };
                                                        dbContext.Redemptions.Add(redemptionEmployeeWithdrawal);
                                                        dbContext.Trans.Add(transEmployeeWithdrawal);
                                                        dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                                                        lastcode++;
                                                    }
                                                    else
                                                    {
                                                        warningMessages.Add("The employee of row " + (row + 1) + " his position of Employee share is : " + employeeContributions + " but withdrawal rule allow " +vestingRuleDetail.PercentageOfEmpShare+"% only to this employee based on the time he spent in the company ");
                                                    }


                                                }
                    }

                    Decimal employerWithdrawal;
                    if (Decimal.TryParse(Convert.ToString(sheet.GetRow(row).GetCell(7)), out employerWithdrawal))
                    {
                        if (employerWithdrawal != 0)
                        { 
                                                    Decimal employerContributions = (allContribution.Where(ec => ec.GTF_Flag == 2).Select(ecu => ecu.units).ToList().Sum().Value - allWithdrawal.Where(ec => ec.GTF_Flag == 2).Select(ecu => ecu.units).ToList().Sum().Value) * icprice.Price;
                                                    if (Decimal.Divide(employerWithdrawal, employerContributions) * 100 <= vestingRuleDetail.PercentageOfCompanyShare)
                                                    {


                                                        Redemption redemptionEmployerWithdrawal;
                            Trans transEmployerWithdrawal;
                            redemptionEmployerWithdrawal = new Redemption
                            {

                                code = lastcode,
                                cust_id = employeeId,
                                NAV = icprice.Price,//api to get icprice
                                amount_3 = employerWithdrawal ,
                                units = employerWithdrawal  / icprice.Price,//api to get icprice
                                SysDate = DateTime.Now,
                                fund_id = int.Parse(Fund.Code),
                                inputer = User.Identity.GetUserId(),
                                Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                                Chk = (Level == Levels.TwoLevels) ? true : false,
                                auther = null,
                                auth = 0,
                                GTF_Flag = 2,
                                UserID = User.Identity.GetUserId(),
                                PolicyId = Policy.Id,
                                delreason = DeleteFlag.NotDeleted,

                                nav_date = icprice.Date,//api to get icprice
                                Nav_Ddate = icprice.Date,//api to get icprice
                                ProcessingDate = icprice.Date,//api to get icprice
                                total = employerWithdrawal ,
                                system_date = DateTime.Now,
                                ExcelId = excel.Id
                            };
                                                    transEmployerWithdrawal = new Trans()
                            {

                                transid = lastcode,
                                total_value = employerWithdrawal ,
                                quantity = employerWithdrawal  / icprice.Price,//api to get icprice
                                unit_price = icprice.Price,//api to get icprice
                                SysDate = DateTime.Now,
                                cust_id = employeeId,
                                fund_id = int.Parse(Fund.Code),
                                inputer = User.Identity.GetUserId(),
                                auth = 0,
                                value_date = icprice.Date,//api to get icprice
                                Nav_Ddate = icprice.Date,//api to get icprice
                                ProcessingDate = icprice.Date,//api to get icprice
                                GTF_Flag = 2,
                                PolicyId = Policy.Id,
                                UserID = User.Identity.GetUserId(),
                                pur_sal = 0,
                                                        ExcelId = excel.Id
                                                    };
                            dbContext.Redemptions.Add(redemptionEmployerWithdrawal);
                            dbContext.Trans.Add(transEmployerWithdrawal);
                            dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                            lastcode++;
                                                    }
                                                    else
                                                    {
                                                        warningMessages.Add("The employee of row " + (row + 1) + " his position of company share is : " + employerContributions + " but withdrawal rule allow " + vestingRuleDetail.PercentageOfCompanyShare + "% only to this employee based on the time he spent in the company ");
                                                    }
                                                }
                    }

                    Decimal additionalEmployeeWithdrawal;
                    if (Decimal.TryParse(Convert.ToString(sheet.GetRow(row).GetCell(8)), out additionalEmployeeWithdrawal))
                    {

                        if (Policy.HasBooster)
                        {
                            if (additionalEmployeeWithdrawal != 0)
                            {
                                                        Decimal additionalEmployeeContributions = (allContribution.Where(ec => ec.GTF_Flag == 3).Select(ecu => ecu.units).ToList().Sum().Value - allWithdrawal.Where(ec => ec.GTF_Flag == 3).Select(ecu => ecu.units).ToList().Sum().Value) * icprice.Price;

                                                        if (Decimal.Divide(additionalEmployeeWithdrawal, additionalEmployeeContributions) * 100 <= vestingRuleDetail.PercentageOfEmpShareBooster)
                                                        {

                                                            Redemption redemptionAdditionalEmployeeWithdrawal;
                                Trans transAdditionalEmployeeWithdrawal;
                                redemptionAdditionalEmployeeWithdrawal = new Redemption
                                {

                                    code = lastcode,
                                    cust_id = employeeId,
                                    NAV = icprice.Price,//api to get icprice
                                    amount_3 = additionalEmployeeWithdrawal ,
                                    units = additionalEmployeeWithdrawal / icprice.Price,//api to get icprice
                                    SysDate = DateTime.Now,
                                    fund_id = int.Parse(Fund.Code),
                                    inputer = User.Identity.GetUserId(),
                                    Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                                    Chk = (Level == Levels.TwoLevels) ? true : false,
                                    auther = null,
                                    auth = 0,
                                    GTF_Flag = 3,
                                    UserID = User.Identity.GetUserId(),
                                    PolicyId = Policy.Id,
                                    delreason = DeleteFlag.NotDeleted,

                                    nav_date = icprice.Date,//api to get icprice
                                    Nav_Ddate = icprice.Date,//api to get icprice
                                    ProcessingDate = icprice.Date,//api to get icprice
                                    total = additionalEmployeeWithdrawal ,
                                    system_date = DateTime.Now,
                                    ExcelId = excel.Id
                                };
                                transAdditionalEmployeeWithdrawal = new Trans()
                                {

                                    transid = lastcode,
                                    total_value = additionalEmployeeWithdrawal ,
                                    quantity = additionalEmployeeWithdrawal / icprice.Price,//api to get icprice
                                    unit_price = icprice.Price,//api to get icprice
                                    SysDate = DateTime.Now,
                                    cust_id = employeeId,
                                    fund_id = int.Parse(Fund.Code),
                                    inputer = User.Identity.GetUserId(),
                                    auth = 0,
                                    value_date = icprice.Date,//api to get icprice
                                    Nav_Ddate = icprice.Date,//api to get icprice
                                    ProcessingDate = icprice.Date,//api to get icprice
                                    GTF_Flag = 3,
                                    PolicyId = Policy.Id,
                                    UserID = User.Identity.GetUserId(),
                                    pur_sal = 0,
                                    ExcelId = excel.Id
                                };
                                dbContext.Redemptions.Add(redemptionAdditionalEmployeeWithdrawal);
                                dbContext.Trans.Add(transAdditionalEmployeeWithdrawal);
                                dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                                lastcode++;
                                                    }
                                                    else
                                                    {
                                                        warningMessages.Add("The employee of row " + (row + 1) + " his position of Employee share(Booster) is : " + additionalEmployeeContributions + " but withdrawal rule allow " + vestingRuleDetail.PercentageOfEmpShareBooster + "% only to this employee based on the time he spent in the company ");
                                                    }
                                                }
                        }
                        else
                        {
                            if (additionalEmployeeWithdrawal != 0)
                            {
                                warningMessages.Add("The employee of row : " + (row + 1) + " has value in employee share(Booster) but this policy doesn't allow booster.");
                            }
                        }


                    }

                    Decimal additionalEmployerWithdrawal;
                    if (Decimal.TryParse(Convert.ToString(sheet.GetRow(row).GetCell(9)), out additionalEmployerWithdrawal))
                    {

                        if (Policy.HasBooster)
                        {
                            if (additionalEmployerWithdrawal != 0)
                            {

                                                        Decimal additionalEmployerContributions = (allContribution.Where(ec => ec.GTF_Flag == 4).Select(ecu => ecu.units).ToList().Sum().Value - allWithdrawal.Where(ec => ec.GTF_Flag == 4).Select(ecu => ecu.units).ToList().Sum().Value) * icprice.Price;
                                                        if (Decimal.Divide(additionalEmployerWithdrawal, additionalEmployerContributions) * 100 <= vestingRuleDetail.PercentageOfCompanyShareBooster)
                                                        {
                                                            Redemption redemptionAdditionalEmployerWithdrawal;
                                Trans transAdditionalEmployerWithdrawal;
                                redemptionAdditionalEmployerWithdrawal = new Redemption
                                {

                                    code = lastcode,
                                    cust_id = employeeId,
                                    NAV = icprice.Price,//api to get icprice
                                    amount_3 = additionalEmployerWithdrawal ,
                                    units = additionalEmployerWithdrawal / icprice.Price,//api to get icprice
                                    SysDate = DateTime.Now,
                                    fund_id = int.Parse(Fund.Code),
                                    inputer = User.Identity.GetUserId(),
                                    Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                                    Chk = (Level == Levels.TwoLevels) ? true : false,
                                    auther = null,
                                    auth = 0,
                                    GTF_Flag = 4,
                                    UserID = User.Identity.GetUserId(),
                                    PolicyId = Policy.Id,
                                    delreason = DeleteFlag.NotDeleted,

                                    nav_date = icprice.Date,//api to get icprice
                                    Nav_Ddate = icprice.Date,//api to get icprice
                                    ProcessingDate = icprice.Date,//api to get icprice
                                    total = additionalEmployerWithdrawal ,
                                    system_date = DateTime.Now,
                                    ExcelId = excel.Id
                                };
                                transAdditionalEmployerWithdrawal = new Trans()
                                {

                                    transid = lastcode,
                                    total_value = additionalEmployerWithdrawal ,
                                    quantity = additionalEmployerWithdrawal  / icprice.Price,//api to get icprice
                                    unit_price = icprice.Price,//api to get icprice
                                    SysDate = DateTime.Now,
                                    cust_id = employeeId,
                                    fund_id = int.Parse(Fund.Code),
                                    inputer = User.Identity.GetUserId(),
                                    auth = 0,
                                    value_date = icprice.Date,//api to get icprice
                                    Nav_Ddate = icprice.Date,//api to get icprice
                                    ProcessingDate = icprice.Date,//api to get icprice
                                    GTF_Flag = 4,
                                    PolicyId = Policy.Id,
                                    UserID = User.Identity.GetUserId(),
                                    pur_sal = 0,
                                    ExcelId = excel.Id
                                };
                                dbContext.Redemptions.Add(redemptionAdditionalEmployerWithdrawal);
                                dbContext.Trans.Add(transAdditionalEmployerWithdrawal);
                                dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                                lastcode++;
                                                }
                                                else
                                                {
                                                    warningMessages.Add("The employee of row " + (row + 1) + " his position of company share(Booster) is : " + additionalEmployerContributions + " but withdrawal rule allow " + vestingRuleDetail.PercentageOfCompanyShareBooster + "% only to this employee based on the time he spent in the company ");
                                                }
                                            }
                        }
                        else
                        {
                            if (additionalEmployerWithdrawal != 0)
                            {
                                warningMessages.Add("The employee of row : " + (row + 1) + " has value in company share(Booster) but this policy doesn't allow booster.");
                            }
                        }

                    }
                    insertedRow++;
                }
            }
            else
            {
                                        warningMessages.Add("The Fund of row : " + (row + 1) + " is not exist.");

                                    }
                                                                     
                                }
                                else
                                {
                                    warningMessages.Add("The employee of row : " + (row + 1) + " His period does not have a rule for calculating his Financial dues.");
                                }
                            }
                            else
                            {
                                warningMessages.Add("The employee of row : " + (row + 1) + " does not have any dues.");
                            }
                        }
                        else
                        {
                            warningMessages.Add("The employee of row : " + (row + 1) + " is not exist.");
                        }
                    }

                }
                catch (Exception ex)
                {
                    warningMessages.Add("The employee data of row : " + (row + 1) + " is invalid.");
                }

            }



            ViewBag.Message = warningMessages;

            if (warningMessages.Count == 0 && insertedRow > 0)
            {
                try
                {
                    dbContext.SaveChanges();
                    transaction.Commit();

                    ViewBag.Message = "File Uploaded Successfully.";

                }
                catch (DbEntityValidationException ex)
                {

                    transaction.Rollback();
                    ViewBag.Message = "Check constraints.";
                }
            }
            else
            {
                transaction.Rollback();
            }






        }




            return View(vm);
        }

        
    }
}