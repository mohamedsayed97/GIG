using ICP_ABC.Areas.Subscriptions.Models;
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
using ICP_ABC.Areas.ICPrices.Models;
using System.Data.Entity.Validation;
using Rotativa;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using PagedList;
using ICP_ABC.Areas.UsersSecurity.Models;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Globalization;
using ICP_ABC.Areas.Policies.Models;

namespace ICP_ABC.Areas.Subscriptions.Controllers
{

    [Authorize]
    public class SubscriptionController : Controller
    {
        private ApplicationDbContext dbContext;
        static int[] IDs;

        public SubscriptionController()
        {
           
                this.dbContext = new ApplicationDbContext();
            
            
        }
        // GET: Subscriptions/Subscriptions
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Create()
        {
            var lastcode = dbContext.LastCodes.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault();
            ViewData["LastCode"] = lastcode + 1;
            ViewData["Funds"] = new SelectList(dbContext.Funds.ToList(), "FundID", "Name");
            if (TempData["LoginResult"] != null)
            {
                IdentityResult LoginResult = TempData["LoginResult"] as IdentityResult;
                AddErrors(LoginResult);
            }
            return View();
        }
        [HttpPost]
        public ActionResult Create(CreateSubscriptionViewModel model)
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
            var availableUnits = (Fund.no_ics - dbContext.Subscriptions.Where(s => s.fund_id == Fund.FundID).Select(c => c.units).ToList().Sum()) + dbContext.Redemptions.Where(s => s.fund_id == Fund.FundID && s.auth == 1).Select(c => c.units).ToList().Sum();
          
         

            if ( availableUnits < (  model.units))
            {
                string[] errors = (availableUnits > 0) ? new string[] { "Available fund ceiling : "+  availableUnits } : new string[] { "Available fund ceiling : " + 0 };
                IdentityResult LoginResult = new IdentityResult(errors);
                TempData["LoginResult"] = LoginResult;
                AddErrors(LoginResult);
                return RedirectToAction("Create");
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
                Subscription subscription = new Subscription
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
                    quantity = model.units.Value,
                    unit_price = model.NAV,
                    SysDate = DateTime.Now,
                    cust_id = model.cust_id,
                    fund_id = model.fund_id,
                    inputer = User.Identity.GetUserId(),
                    auth = 0,
                    Flag = -1,
                    GTF_Flag = (model.cust_id == accountType.ZeroAccount) ? (short)5 : (short)6,
                    UserID = User.Identity.GetUserId(),
                    value_date = model.nav_date.Value,
                    Nav_Ddate = model.Nav_Ddate,
                    ProcessingDate = model.ProcessingDate,
                    pur_sal = 0
                };
        
                dbContext.Subscriptions.Add(subscription);
                dbContext.Trans.Add(trans);
                dbContext.LastCodes.Add(new LastCode { Code = lastcode });
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

                return RedirectToAction("Details", new { Code = subscription.code });
            }
            else
            {
                Subscription subscription = new Subscription
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
                    
                    pay_method = 1,
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
                    pur_sal = 0
                };


                dbContext.Subscriptions.Add(subscription);
                dbContext.Trans.Add(trans);
                dbContext.LastCodes.Add(new LastCode { Code = lastcode });
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

                return RedirectToAction("Details", new { Code = subscription.code });
            }


            
        }

        public ViewResult Search(string sortOrder,string RadioCHeck, string currentFilter, int? page,  SearchViewModel model)
        {
            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;

            if (page == null&& sortOrder == null && RadioCHeck == null && currentFilter == null &&  model.TotalAmountTo == null
                && model.CodeFrom == null && model.CodeTo == null 
                && model.CustomerId == null && model.Funds == null && model.NumberOfUnits == null
                && model.NavDateFrom == null && model.NavDateTo == null && model.TotalAmountFrom == null)
            {
                int pageSize = 20;
                int pageNumber = (page ?? 1);
                
                var Subs = dbContext.Subscriptions.Where(c => c.delreason == DeleteFlag.NotDeleted).Take(0);
                var prices = dbContext.ICPrices.ToList();
                var UserFunds = User.Identity.UserFunds().Where(f => prices.Select(p => p.FundId).ToList().Contains(f.FundID) && f.Auth == true).ToList();
                ViewData["Funds"] = new SelectList(UserFunds, "FundID", "Name");
                return View(Subs.ToPagedList(pageNumber, pageSize));

            }
            else
            {
                var prices = dbContext.ICPrices.Where(PRIce=> PRIce.DeletFlag== DeleteFlag.NotDeleted).ToList();
                var UserFunds = User.Identity.UserFunds().Where(f => prices.Select(p => p.FundId).ToList().Contains(f.FundID) && f.Auth == true && f.DeletFlag == DeleteFlag.NotDeleted).ToList();
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

               // ViewBag.CurrentFilter = model.AccountNo;

                var Subs = dbContext.Subscriptions.Where(c => c.delreason == DeleteFlag.NotDeleted);
                var currentuserId = User.Identity.GetUserId();
                var currentUser = dbContext.Users.Where(u => u.Id == currentuserId).First();

                //if (!currentUser.BranchRight)
                //{
                //    Subs = Subs.Where(u => u.branch_id == currentUser.BranchId);
                //}

                //if (!String.IsNullOrEmpty(model.AccountNo))
                //{
                //    Subs = Subs.Where(s => s.cust_acc_no == model.AccountNo);
                //    ViewData["AccountNo"] = model.AccountNo;
                //}


                if (!String.IsNullOrEmpty(model.CustomerId))
                {
                    //var code = int.Parse(Code);
                    Subs = Subs.Where(s => s.cust_id == model.CustomerId);
                    ViewData["CustomerId"] = model.CustomerId;
                }

                if (!String.IsNullOrEmpty(model.CodeFrom))
                {
                    var code = int.Parse(model.CodeFrom);
                    Subs = Subs.Where(s => s.code >= code);
                    ViewData["CodeFrom"] = model.CodeFrom;
                }
                var x = Subs.ToList();
                if (!String.IsNullOrEmpty(model.CodeTo))
                {
                    // int IdNumber;
                    int code = int.Parse(model.CodeTo);
                    Subs = Subs
                   .Where(u => u.code <= code)
                   .Select(s => s);
                    ViewData["CodeTo"] = code;
                }
                if (!String.IsNullOrEmpty(model.NavDateFrom))
                {
                    var FromDate = Convert.ToDateTime(model.NavDateFrom);
                    Subs = Subs
                   .Where(u => u.nav_date >= FromDate)
                   .Select(s => s);

                }

                if (!String.IsNullOrEmpty(model.NavDateTo))
                {
                    var ToDate = Convert.ToDateTime(model.NavDateTo);
                    Subs = Subs
                   .Where(u => u.nav_date <= ToDate)
                   .Select(s => s);
                    //ViewData["ArName"] = ArName;

                }

                if (!String.IsNullOrEmpty(model.Funds))
                {
                    int FundID = int.Parse(model.Funds);
                    Subs = Subs
                   .Where(u => u.fund_id == FundID)
                   .Select(s => s);
                    ViewData["Funds"] = new SelectList(UserFunds, "FundID", "Name", FundID);

                }


                //if (!String.IsNullOrEmpty(model.BranchId))
                //{

                //    int BranchID = int.Parse(model.BranchId);
                //    Subs = Subs
                //   .Where(u => u.branch_id == BranchID)
                //   .Select(s => s);
                //    ViewData["BranchId"] = BranchID;

                //}

                if (!String.IsNullOrEmpty(model.NumberOfUnits))
                {

                    var units = decimal.Parse(model.NumberOfUnits);
                    Subs = Subs
                   .Where(u => u.amount_3 == units)
                   .Select(s => s);
                    //ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "BranchID", "Name", BranchID);
                    ViewData["NumberOfUnits"] = units;
                }

                if (!String.IsNullOrEmpty(model.TotalAmountFrom))
                {

                    //var Amount = Convert.ToInt32(model.TotalAmountFrom);
                    var Amount = decimal.Parse(model.TotalAmountFrom);
                    Subs = Subs
                   .Where(u => u.total >= Amount)
                   .Select(s => s);
                    //ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "BranchID", "Name", BranchID);

                }
                if (!String.IsNullOrEmpty(model.TotalAmountTo))
                {

                    //var Amount = Convert.ToInt32(model.TotalAmountTo);
                    var Amount = decimal.Parse(model.TotalAmountTo);
                    Subs = Subs
                   .Where(u => u.total <= Amount)
                   .Select(s => s);

                }

             


                //checkRadio 
                if (!String.IsNullOrEmpty(RadioCHeck))
                {
                    //Case Auth
                    if (RadioCHeck == "1")
                    {
                        Subs = Subs.Where(s => s.auth == 1);

                    }
                    //Case Checker
                    if (RadioCHeck == "2")
                    {
                        Subs = Subs.Where(s => s.Chk == true && s.auth != 1);

                    }
                    //Case maker
                    if (RadioCHeck == "3")
                    {
                        Subs = Subs.Where(s => s.auth != 1 && s.Chk == false);

                    }
                    //Case All
                    if (RadioCHeck == "4")
                    {
                        Subs = Subs.Where(c => c.delreason == DeleteFlag.NotDeleted);

                    }
                    
                }
                //
                TempData["SubSForExc"] = Subs.Select(Co => Co.code).ToList();
                var ghx = Subs.ToList();
                switch (sortOrder)
                {
                    case "name_desc":
                        Subs = Subs.OrderByDescending(s => s.Fund.Name);
                        break;
                    case "Code":
                        Subs = Subs.OrderBy(s => s.code);
                        break;
                    case "code_desc":
                        Subs = Subs.OrderByDescending(s => s.code);
                        break;
                    default:  // Name ascending 
                        //Subs = Subs.OrderBy(s => s.appliction_no);
                        Subs = Subs.OrderBy(s => s.code);
                        break;
                }
                //int count = Currencies.ToList().Count();
                //IDs = new string[count];
                //IDs = Currencies.Select(s => s.Code).ToArray();

                int pageSize = 20;
                int pageNumber = (page ?? 1);
                ViewData["radioCHeck"] = RadioCHeck;
                int count = Subs.ToList().Count();
                IDs = new int[count];
                IDs = Subs.Select(s => s.code).ToArray();
                Array.Sort(IDs);
                return View(Subs.ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult Edit(int Code)
        {
            var sub = dbContext.Subscriptions.Where(s => s.code == Code).FirstOrDefault();
            if(sub != null) { 
            CreateSubscriptionViewModel model = new CreateSubscriptionViewModel()
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
                total = sub.total,
                cust_acc_no= sub.cust_acc_no


            };
            //ViewData["Date"] = sub.pur_date.ToString();
            //ViewData["PayMethod"] = model.pay_method;
            //TempData["Code"] = Code;
            

                ViewData["Funds"] = new SelectList(dbContext.Funds.Where(f => f.FundID == sub.fund_id).ToList(), "FundID", "Name");
                return View(model); 
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Edit(CreateSubscriptionViewModel model)
        {
            //var code = int.Parse(TempData["Code"].ToString());
            var sub = dbContext.Subscriptions.Where(s => s.code == model.code).FirstOrDefault();

            var trans = dbContext.Trans.Where(s => s.transid == sub.code).FirstOrDefault();
            
            var Customer = dbContext.Customers
               .Where(c => c.CustomerID == sub.cust_id.ToString())
               .FirstOrDefault();

        


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
            sub.NAV = (decimal.Round(Convert.ToDecimal(model.NAV), 2))* 1.00m;
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
            trans.ProcessingDate = model.ProcessingDate;
            trans.Nav_Ddate = model.Nav_Ddate;

           
            trans.total_value = (decimal)totalAmount;
            trans.flag_tr = 1;
     
            trans.curr_id = dbContext.Funds.SingleOrDefault(f=>f.FundID == model.fund_id).CurrencyID;
            trans.SysDate = DateTime.Now;
            
            trans.auth = 0;
          

            try
            {
                dbContext.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
            return RedirectToAction("Details", new { Code = model.code });
        }
        [AuthorizedRights(Screen = "Subscription", Right = "Read")]
        public ActionResult Details(int Code)
        {
            CreateSubscriptionViewModel model = new CreateSubscriptionViewModel();
            var sub = dbContext.Subscriptions.Where(s => s.code == Code).FirstOrDefault();
            var CurrentUser = User.Identity.GetUserId();
            if(sub.Chk == false && sub.inputer != CurrentUser)
            {
                model.CheckBtn = true;
            }
            if (sub.Chk == true && sub.Checker != null && sub.auth == 0 && sub.Checker != CurrentUser)
            {
                model.AuthBtn = true;
            }
            if (sub.Chk == true && sub.Checker != null && sub.auth == 1 && sub.auther != null && sub.Checker != CurrentUser)
            {
                model.UnAuthBtn = true;
            }
            if (sub.auth == 0 || sub.auth == null)
            {
                model.DeleteBtn = true;
                model.EditBtn = true;
            }


                model.code = sub.code;
                model.amount_3 = sub.amount_3;
                model.units = sub.units;
                model.CustomerName = dbContext.Customers.Where(c => c.CustomerID == sub.cust_id.ToString()).Select(c => c.EnName).FirstOrDefault();
                model.cust_id = sub.cust_id;
                model.NAV = sub.NAV;
              
                model.nav_date = sub.nav_date;
                model.ProcessingDate = sub.ProcessingDate;
                model.Nav_Ddate = sub.Nav_Ddate;
              
                model.pay_method = sub.pay_method == 0 ? "By Units" : "By Amount";
                
                model.total = sub.total;
                
                model.system_date = DateTime.Now.ToString();
                
           
            ViewData["nav_date"] = sub.nav_date.ToString();
            ViewData["Date"] = sub.pur_date.ToString();
            ViewData["PayMethod"] = model.pay_method;
            var fundname =   dbContext.Funds.Where(f => f.FundID == sub.fund_id).Select(s => s.Name).FirstOrDefault() ;
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

        

        [AuthorizedRights(Screen = "Subscription", Right = "Read")]
        public ActionResult Next(int id)
        {
            var arrlenth = IDs.Count();
            var LastObj = dbContext.Subscriptions.OrderBy(s => s.code).ToList().LastOrDefault(s => s.code == IDs[arrlenth - 1]);
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
                var Code = dbContext.Subscriptions.Where(u => u.code == id).Select(s => s.code).FirstOrDefault();
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
        [AuthorizedRights(Screen = "Subscription", Right = "Read")]
        public ActionResult Previous(int id)
        {
            var arrindex = IDs[0];
            var FirstObj = dbContext.Subscriptions.OrderBy(s => s.code).FirstOrDefault(s => s.code == arrindex);
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
                var Code = dbContext.Subscriptions.Where(u => u.code == id).Select(s => s.code).FirstOrDefault();
                return RedirectToAction("Details", new { Code = Code });
            }

        }
        [AuthorizedRights(Screen = "Subscription", Right = "Read")]
        public ActionResult ExportToExcel()
        {
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var gv = new GridView();
            var List = (List<int>)TempData["SubSForExc"];
            gv.DataSource = dbContext.Subscriptions.Where(Del => List.Contains(Del.code) && Del.delreason == DeleteFlag.NotDeleted).Select(x => new { Code = x.code, CustomerName= x.Customer.EnName, CustomerID= x.cust_id, NOofUnits=x.amount_3, AppNo=x.appliction_no, BranchName=x.Branch.BName,BranchId=x.branch_id, FundName = x.Fund.Name, Auth = x.auth,NavDate = x.nav_date,Nav = x.NAV,Total = x.total,CustomerAccNO=x.cust_acc_no }).ToList();

            //gv.DataSource = (from s in dbContext.Subscriptions
            //                 select new
            //                 {
            //                     s.code,
                                 


            //                 })
            //                     .ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Subscription" + NowTime + ".xls");
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
        //[AuthorizedRights(Screen = "Subscription", Right = "Read")]
        public ActionResult ExportToPDF()
        {
            var Subscription = dbContext.Subscriptions.Select(s => s).Where(s => IDs.Contains(s.code)).OrderBy(u => u.code).ToList();

            //string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var report = new PartialViewAsPdf("~/Areas/Subscriptions/Views/Subscription/ExportToPDF.cshtml", Subscription);
            return report;

        }

        public ActionResult Application(int id)
        {
            var sub = dbContext.Subscriptions.Select(s => s).Where(s => s.code== id).FirstOrDefault();
            
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var report = new PartialViewAsPdf("~/Areas/Subscriptions/Views/Subscription/Application.cshtml", sub);
            return report;

        }

        public ActionResult Recipt(int id)
        {
            var sub = dbContext.Subscriptions.Select(s => s).Where(s => s.code == id).FirstOrDefault();

            //string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var report = new PartialViewAsPdf("~/Areas/Subscriptions/Views/Subscription/Recipt.cshtml", sub);
            return report;

        }
        [AuthorizedRights(Screen = "Subscription", Right = "Authorized")]
        public ActionResult AuthorizeSubscription(int Code)
        {
            var Subscription = dbContext.Subscriptions.Where(f => f.code == Code).FirstOrDefault();
            var Trans = dbContext.Trans.Where(f => f.transid == Subscription.code).FirstOrDefault();
            var ICPrice = dbContext.ICPrices
                .SingleOrDefault(i => i.FundId == Subscription.fund_id && i.Auth == true && DbFunctions.TruncateTime(i.Date) == Subscription.nav_date.Value.Date);
                

          
                
            if (ICPrice == null)
            {
                Session["FailedAuthorizeSub"] = "No iCprice has been added for today.";
                return RedirectToAction("Details", new { Code = Code });
            }
                if (Subscription.auth == 1)
            {
                Session["FailedAuthorizeSub"] = "It is Already Autherized.";
                return RedirectToAction("Details", Code);
            }
            if (!Subscription.Chk.Value)
            {
                Session["FailedAuthorizeSub"] = "It must be checked first.";
                return RedirectToAction("Details", Code);
            }
            Session["FailedAuthorizeSub"] = null;
            //if (Subscription.pur_date <= ICPrice.Date )
            //{
            //    ViewData["Failed"] = "Failed";
            //    return RedirectToAction("Details", Code);

            //}
            Session["FailedAuthorizeRed"] = null;
            Subscription.auth = 1;
            Subscription.unauth = 0;
            Subscription.auther = User.Identity.GetUserId();
            Subscription.NAV = ICPrice.Price;
            Subscription.Nav_Ddate = ICPrice.Date;
        

            Trans.unit_price = ICPrice.Price;
            Trans.value_date =  ICPrice.Date;
            Trans.auth = 1;
           
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

        [AuthorizedRights(Screen = "Subscription", Right = "Authorized")]
        public ActionResult UnAuthorizeSubscription(int Code)
        {
            var Subscription = dbContext.Subscriptions.Where(f => f.code == Code).FirstOrDefault();
            var Fund = dbContext.Funds.SingleOrDefault(f => f.FundID == Subscription.fund_id);
            var availableUnits = dbContext.Subscriptions.Where(s => s.fund_id == Fund.FundID && s.cust_id == Subscription.cust_id && s.auth == 1).Select(c => c.units).ToList().Sum() - dbContext.Redemptions.Where(s => s.fund_id == Fund.FundID && s.cust_id == Subscription.cust_id).Select(c => c.units).ToList().Sum();
            var pendeingRedemption = dbContext.Redemptions.Where(s => s.fund_id == Subscription.fund_id && s.cust_id == Subscription.cust_id && s.auth != 1).Select(c => c.units).ToList().Sum();
            var Block = dbContext.Block.Where(s => s.fund_id == Subscription.fund_id && s.cust_id == Subscription.cust_id && s.BlockCmb == 0).Select(c => c.qty_block).ToList().Sum() - dbContext.Block.Where(s => s.fund_id == Subscription.fund_id && s.cust_id == Subscription.cust_id && s.BlockCmb == 1 && s.blockauth == true).Select(c => c.qty_block).ToList().Sum();

            if (Subscription.unauth == 1)
            {
                Session["FailedUnAuthorizeSub"] = "It is Already Not Autherized.";
                return RedirectToAction("Details", new { Code = Code });
            }
            if (availableUnits - Block < Subscription.units)
            {
                Session["FailedUnAuthorizeSub"] =(Block == 0)
                    ?"You won't be able to unauthorize this subscription because you have : "+ pendeingRedemption + "pending redemption."
                    : "You won't be able to unauthorize this subscription because you have : " + pendeingRedemption + "pending redemption." +  " and " + Block + " blocked units.";
                return RedirectToAction("Details", new { Code = Code });
            }
            Session["FailedUnAuthorizeSub"] = null;
            Subscription.unauth = 1;
            Subscription.auth = 0;
            Subscription.auther = User.Identity.GetUserId();


            var Trans = dbContext.Trans.Where(f => f.transid == Subscription.code).FirstOrDefault();

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
        [AuthorizedRights(Screen = "Subscription", Right = "Check")]
        public ActionResult CheckSubscription(int Code)
        {
            var Subscription = dbContext.Subscriptions.Where(f => f.code == Code).FirstOrDefault();
            var trans = dbContext.Trans.Where(f => f.cust_id == Subscription.cust_id).FirstOrDefault();
            //trans.c = true;
            Subscription.Chk = true;
            Subscription.Checker = User.Identity.GetUserId();
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

        [AuthorizedRights(Screen = "Subscription", Right = "Delete")]
        public ActionResult Delete(int Code)
        {
            var Subscriptin = dbContext.Subscriptions.Where(f => f.code == Code).FirstOrDefault();
            var Trans = dbContext.Trans.Where(f => f.transid == Code).FirstOrDefault();
            
            Subscriptin.delreason = DeleteFlag.Deleted;
            Trans.Flag = 0;
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
        public JsonResult GetICPriceInfo(int FundId)

        {
             var Fund0 = dbContext.Funds.SingleOrDefault(ic => ic.FundID == FundId);
            var availableUnits = (Fund0.no_ics  - dbContext.Subscriptions.Where(s => s.fund_id == FundId).Select(c => c.units).ToList().Sum() )+ dbContext.Redemptions.Where(s => s.fund_id == FundId && s.auth == 1).Select(c => c.units).ToList().Sum();
            ICPrice LastInsertrdDay = null;
            if (availableUnits == 0)
            {
                return Json(new { NOPrice = "3" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var today = DateTime.Today.Date;
                LastInsertrdDay = dbContext.ICPrices.Where(ic => ic.FundId == FundId && ic.Auth == true &&DbFunctions.TruncateTime(ic.Date) <= today).OrderByDescending(i => i.Date).FirstOrDefault();
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
                    return Json(result );
       }
      }

       

      
      

      
        public ActionResult UploadContributions()
        {



            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadContributions(UploadContributionsViewModel vm)
        {


          

            List<string> warningMessages = new List<string>();
            HttpPostedFileBase files = vm.file;
            var path = Server.MapPath("~/Temp");
            //Read the Posted Excel File  
            ISheet sheet; //Create the ISheet object to read the sheet cell values  
            string filename = Path.GetFileName(Server.MapPath(files.FileName)); //get the uploaded file name  
            string fullPath = Path.Combine(path,filename);
            files.SaveAs(fullPath);
            var fileExt = Path.GetExtension(filename); //get the extension of uploaded excel file           
            byte[] buffer;
            if (fileExt == ".xls")
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
            var policy = dbContext.Policy.Include("AllocationRules").SingleOrDefault(p => p.PolicyNo == policyNo && p.Auth);
            if (policy == null)
            {
                ModelState.AddModelError("file", "The Policy either does not exist or has not yet been authorized");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Message = null;
                return View(vm);
            }
            var lastcode = dbContext.LastCodes.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault();
            lastcode++;
            int checkNumber;
            int insertedRow = 0;
            var Level = dbContext.UserSecurities.Select(Val => Val.Levels).FirstOrDefault();
            using (DbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {

                buffer = System.IO.File.ReadAllBytes(fullPath);
                System.IO.File.Delete(fullPath);
                var excel = new ExcelDetails
                {
                    Name = vm.file.FileName + DateTime.Now.ToString("dd/MM/yy,hh:mm"),
                    Status = (byte)ExcelStatus.Pending,
                    Screen = (byte)ExcelTransactionType.Contribution,
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
                              sheet.GetRow(row).GetCell(11).ToString() == "" &&
                              sheet.GetRow(row).GetCell(12).ToString() == ""
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
                         sheet.GetRow(row).GetCell(6).ToString() == "" ||
                         sheet.GetRow(row).GetCell(7).ToString() == "" ||
                         sheet.GetRow(row).GetCell(8).ToString() == ""
                    )
                    {

                        warningMessages.Add("The employee data of row : " + (row + 1) + " not complete.");

                    }
                    else if
                    (
                             sheet.GetRow(row).GetCell(9).ToString() == "" ||
                             sheet.GetRow(row).GetCell(10).ToString() == "" ||
                             sheet.GetRow(row).GetCell(11).ToString() == "" ||
                             sheet.GetRow(row).GetCell(12).ToString() == "" ||
                             !(int.TryParse(sheet.GetRow(row).GetCell(9).ToString(), out checkNumber)) ||
                             !(int.TryParse(sheet.GetRow(row).GetCell(10).ToString(), out checkNumber)) ||
                             !(int.TryParse(sheet.GetRow(row).GetCell(11).ToString(), out checkNumber)) ||
                             !(int.TryParse(sheet.GetRow(row).GetCell(12).ToString(), out checkNumber))
                    )
                    {
                        warningMessages.Add("The employee contributions of row : " + (row + 1) + " are incorrect.");
                    }
                    else if
                    (
                         int.Parse(sheet.GetRow(row).GetCell(9).ToString()) == 0 &&
                         int.Parse(sheet.GetRow(row).GetCell(10).ToString()) == 0 &&
                         int.Parse(sheet.GetRow(row).GetCell(11).ToString()) == 0 &&
                         int.Parse(sheet.GetRow(row).GetCell(12).ToString()) == 0
                    )
                    {
                        warningMessages.Add("The employee contributions of row : " + (row + 1) + " At least must have one value for one of the four contributions.");
                    }
                    else
                    {
                        var employeeId = sheet.GetRow(row).GetCell(2).ToString();
                        var employeeName = sheet.GetRow(row).GetCell(3).ToString();
                        var employee = dbContext.Customers.SingleOrDefault(c => c.CustomerID == employeeId && c.Auth && c.CompanyId == policy.CompanyId && c.EnName.Equals(employeeName.Trim()));

                        var CustId = sheet.GetRow(row).GetCell(2).ToString();
                        if (employee != null && dbContext.EmployeePolicies.Any(c => c.CustomerId == CustId && c.CompanyId == policy.CompanyId && c.PolicyId == policy.Id && !c.isSurrendered))
                        {
                            foreach (var allocationRule in policy.AllocationRules)
                            {
                                    var transactionDateWithoutTime = vm.processingDate.Date;

                                    var icprice = dbContext.ICPrices.Where(ic => ic.FundId == allocationRule.FundId && ic.Auth == true && DbFunctions.TruncateTime(ic.Date) == transactionDateWithoutTime).OrderByDescending(i => i.Date).FirstOrDefault();

                                    if (icprice == null)//icprice from api
                                    {
                                        warningMessages.Add("The Fund  : " + allocationRule.Fund.Name + " has no price.");
                                    }
                                    else { 
                                    Decimal employeeContributions;
                            if (Decimal.TryParse(Convert.ToString(sheet.GetRow(row).GetCell(9)), out employeeContributions))
                            {
                                if (employeeContributions != 0)
                                {
                                    Subscription subscriptionEmployeeContributions;
                                    Trans transEmployeeContributions;
                                    subscriptionEmployeeContributions = new Subscription
                                    {
                                     
                                        code = lastcode,
                                        cust_id = CustId,
                                        NAV = icprice.Price,//api to get icprice
                                        amount_3 = employeeContributions * Decimal.Divide(allocationRule.PercentageOfEmpShare , 100),
                                       PolicyId = policy.Id,
                                        units =( employeeContributions * Decimal.Divide(allocationRule.PercentageOfEmpShare , 100)) / icprice.Price,//api to get icprice
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

                                     
                                        nav_date = icprice.Date,//api to get icprice
                                        Nav_Ddate = icprice.Date,//api to get icprice
                                        ProcessingDate = icprice.Date,//api to get icprice
                                        total = employeeContributions * Decimal.Divide(allocationRule.PercentageOfEmpShare , 100),
                                        system_date = DateTime.Now,
                                        ExcelId = excel.Id
                                    };

                                           
                                    transEmployeeContributions = new Trans()
                                    {
                                    
                                       
                                        transid = lastcode,
                                        total_value = employeeContributions * Decimal.Divide(allocationRule.PercentageOfEmpShare , 100),
                                        quantity = (employeeContributions * Decimal.Divide(allocationRule.PercentageOfEmpShare , 100)) / icprice.Price,//api to get icprice
                                        unit_price = icprice.Price,//api to get icprice
                                        SysDate = DateTime.Now,
                                        cust_id = CustId,
                                        fund_id = allocationRule.FundId,
                                        inputer = User.Identity.GetUserId(),
                                        auth = 0,
                                        GTF_Flag = 1,
                                        UserID = User.Identity.GetUserId(),
                                        PolicyId = policy.Id,
                                        pur_sal = 0,
                                        value_date = icprice.Date,//api to get icprice
                                        Nav_Ddate = icprice.Date,//api to get icprice
                                        ProcessingDate = icprice.Date,//api to get icprice,
                                        ExcelId = excel.Id

                                    };
                                    dbContext.Subscriptions.Add(subscriptionEmployeeContributions);
                                    dbContext.Trans.Add(transEmployeeContributions);
                                    dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                                    lastcode++;
                                }
                            }

                            Decimal employerContributions;
                            if (Decimal.TryParse(Convert.ToString(sheet.GetRow(row).GetCell(10)), out employerContributions))
                            {
                                if (employerContributions != 0)
                                {
                                    Subscription subscriptionEmployerContributions;
                                    Trans transEmployerContributions;
                                    subscriptionEmployerContributions = new Subscription
                                    {
                                        
                                        code = lastcode,
                                        cust_id = CustId,
                                        NAV = icprice.Price,//api to get icprice
                                        amount_3 = employerContributions * Decimal.Divide(allocationRule.PercentageOfCompanyShare , 100),
                                        units = (employerContributions  * Decimal.Divide(allocationRule.PercentageOfCompanyShare , 100)) / icprice.Price,//api to get icprice
                                        SysDate = DateTime.Now,
                                        fund_id = allocationRule.FundId,
                                        inputer = User.Identity.GetUserId(),
                                        Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                                        Chk = (Level == Levels.TwoLevels) ? true : false,
                                        auther = null,
                                        auth = 0,
                                        GTF_Flag = 2,
                                        UserID = User.Identity.GetUserId(),
                                        PolicyId = policy.Id,
                                        delreason = DeleteFlag.NotDeleted,

                                    
                                        nav_date = icprice.Date,//api to get icprice
                                        Nav_Ddate = icprice.Date,//api to get icprice
                                        ProcessingDate = icprice.Date,//api to get icprice
                                        total = employerContributions * Decimal.Divide(allocationRule.PercentageOfCompanyShare , 100),
                                        system_date = DateTime.Now,
                                        ExcelId = excel.Id
                                    };
                                    transEmployerContributions = new Trans()
                                    {
                                       
                                        transid = lastcode,
                                        total_value = employerContributions * Decimal.Divide(allocationRule.PercentageOfCompanyShare , 100),
                                        quantity = (employerContributions * Decimal.Divide(allocationRule.PercentageOfCompanyShare , 100)) / icprice.Price,//api to get icprice
                                        unit_price = icprice.Price,//api to get icprice
                                        SysDate = DateTime.Now,
                                        cust_id = CustId,
                                        fund_id = allocationRule.FundId,
                                        inputer = User.Identity.GetUserId(),
                                        auth = 0,
                                        GTF_Flag = 2,
                                        UserID = User.Identity.GetUserId(),
                                        pur_sal = 0,
                                        value_date = icprice.Date,
                                        Nav_Ddate = icprice.Date,
                                        ProcessingDate = icprice.Date,
                                        PolicyId = policy.Id,
                                        ExcelId = excel.Id
                                    };
                                    dbContext.Subscriptions.Add(subscriptionEmployerContributions);
                                    dbContext.Trans.Add(transEmployerContributions);
                                    dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                                    lastcode++;
                                }
                            }

                            Decimal additionalEmployeeContributions;
                            if (Decimal.TryParse(Convert.ToString(sheet.GetRow(row).GetCell(11)), out additionalEmployeeContributions))
                            {
                                        if (policy.HasBooster)
                                        {
                                            if (additionalEmployeeContributions != 0)
                                            {
                                                Subscription subscriptionAdditionalEmployeeContributions;
                                                Trans transAdditionalEmployeeContributions;
                                                subscriptionAdditionalEmployeeContributions = new Subscription
                                                {

                                                    code = lastcode,
                                                    cust_id = CustId,
                                                    NAV = icprice.Price,//api to get icprice
                                                    amount_3 = additionalEmployeeContributions * Decimal.Divide(allocationRule.PercentageOfEmpShareBooster, 100),
                                                    units = (additionalEmployeeContributions * Decimal.Divide(allocationRule.PercentageOfEmpShareBooster, 100)) / icprice.Price,//api to get icprice
                                                    SysDate = DateTime.Now,
                                                    fund_id = allocationRule.FundId,
                                                    inputer = User.Identity.GetUserId(),
                                                    Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                                                    Chk = (Level == Levels.TwoLevels) ? true : false,
                                                    auther = null,
                                                    auth = 0,
                                                    GTF_Flag = 3,
                                                    UserID = User.Identity.GetUserId(),

                                                    delreason = DeleteFlag.NotDeleted,
                                                    PolicyId = policy.Id,
                                                   
                                                    nav_date = icprice.Date,//api to get icprice
                                                    Nav_Ddate = icprice.Date,//api to get icprice
                                                    ProcessingDate = icprice.Date,//api to get icprice
                                                    total = additionalEmployeeContributions * Decimal.Divide(allocationRule.PercentageOfEmpShareBooster, 100),
                                                    system_date = DateTime.Now,
                                                    ExcelId = excel.Id
                                                };
                                                transAdditionalEmployeeContributions = new Trans()
                                                {
                                                  
                                                    transid = lastcode,
                                                    total_value = additionalEmployeeContributions * Decimal.Divide(allocationRule.PercentageOfEmpShareBooster, 100),
                                                    quantity = (additionalEmployeeContributions * Decimal.Divide(allocationRule.PercentageOfEmpShareBooster, 100)) / icprice.Price,//api to get icprice
                                                    unit_price = icprice.Price,//api to get icprice
                                                    SysDate = DateTime.Now,
                                                    cust_id = CustId,
                                                    fund_id = allocationRule.FundId,
                                                    inputer = User.Identity.GetUserId(),
                                                    auth = 0,
                                                    GTF_Flag = 3,
                                                    UserID = User.Identity.GetUserId(),
                                                    pur_sal = 0,
                                                    value_date = icprice.Date,//api to get icprice
                                                    Nav_Ddate = icprice.Date,//api to get icprice
                                                    ProcessingDate = icprice.Date,//api to get icprice
                                                    PolicyId = policy.Id,
                                                    ExcelId = excel.Id
                                                };
                                                dbContext.Subscriptions.Add(subscriptionAdditionalEmployeeContributions);
                                                dbContext.Trans.Add(transAdditionalEmployeeContributions);
                                                dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                                                lastcode++;
                                            }
                                        }
                                        else
                                        {
                                            if (additionalEmployeeContributions != 0)
                                            {
                                                warningMessages.Add("The employee of row : " + (row + 1) + " has value in employee share(Booster) but this policy doesn't allow booster.");
                                            }
                                        }
                                     
                            }

                            Decimal additionalEmployerContributions;
                            if (Decimal.TryParse(Convert.ToString(sheet.GetRow(row).GetCell(12)), out additionalEmployerContributions))
                            {
                                        if (policy.HasBooster)
                                        {
                                            if (additionalEmployerContributions != 0)
                                            {
                                                Subscription subscriptionAdditionalEmployerContributions;
                                                Trans transAdditionalEmployerContributions;
                                                subscriptionAdditionalEmployerContributions = new Subscription
                                                {

                                                    code = lastcode,
                                                    cust_id = CustId,
                                                    NAV = icprice.Price,//api to get icprice
                                                    amount_3 = additionalEmployerContributions * Decimal.Divide(allocationRule.PercentageOfCompanyShareBooster, 100),
                                                    units = (additionalEmployerContributions * Decimal.Divide(allocationRule.PercentageOfCompanyShareBooster, 100)) / icprice.Price,//api to get icprice
                                                    SysDate = DateTime.Now,
                                                    fund_id = allocationRule.FundId,
                                                    inputer = User.Identity.GetUserId(),
                                                    Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                                                    Chk = (Level == Levels.TwoLevels) ? true : false,
                                                    auther = null,
                                                    auth = 0,
                                                    GTF_Flag = 4,
                                                    UserID = User.Identity.GetUserId(),

                                                    delreason = DeleteFlag.NotDeleted,
                                          
                                                    PolicyId = policy.Id,
                                                    nav_date = icprice.Date,//api to get icprice
                                                    Nav_Ddate = icprice.Date,//api to get icprice
                                                    ProcessingDate = icprice.Date,//api to get icprice
                                                    total = additionalEmployerContributions * Decimal.Divide(allocationRule.PercentageOfCompanyShareBooster, 100),
                                                    system_date = DateTime.Now,
                                                    ExcelId = excel.Id
                                                };
                                                transAdditionalEmployerContributions = new Trans()
                                                {
                                                    
                                                    transid = lastcode,
                                                    total_value = additionalEmployerContributions * Decimal.Divide(allocationRule.PercentageOfCompanyShareBooster, 100),
                                                    quantity = (additionalEmployerContributions * Decimal.Divide(allocationRule.PercentageOfCompanyShareBooster, 100)) / icprice.Price,//api to get icprice
                                                    unit_price = icprice.Price,//api to get icprice
                                                    SysDate = DateTime.Now,
                                                    cust_id = CustId,
                                                    fund_id = allocationRule.FundId,
                                                    inputer = User.Identity.GetUserId(),
                                                    auth = 0,
                                                    GTF_Flag = 4,
                                                    UserID = User.Identity.GetUserId(),
                                                    pur_sal = 0,
                                                    value_date = icprice.Date,//api to get icprice
                                                    Nav_Ddate = icprice.Date,//api to get icprice
                                                    ProcessingDate = icprice.Date,//api to get icprice
                                                    PolicyId = policy.Id,
                                                    ExcelId = excel.Id
                                                };
                                                dbContext.Subscriptions.Add(subscriptionAdditionalEmployerContributions);
                                                dbContext.Trans.Add(transAdditionalEmployerContributions);
                                                dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                                                lastcode++;
                                            }
                                        }
                                        else
                                        {
                                            if (additionalEmployerContributions != 0)
                                            {
                                                warningMessages.Add("The employee of row : " + (row + 1) + " has value in company share(Booster) but this policy doesn't allow booster.");
                                            }
                                        }
                                      
                            }
                            insertedRow++;
                                }
                               

                            }
                        }
                        else
                        {
                            warningMessages.Add("The employee of row : " + (row + 1) + " not exist.");

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

        public ActionResult UploadAdditions()
        {
         
            
            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadAdditions(UploadAdditionsViewModel vm)
        {

          
            List<string> warningMessages = new List<string>() ;
            HttpPostedFileBase files = vm.file; //Read the Posted Excel File  
            var path = Server.MapPath("~/Temp");
            //Read the Posted Excel File  
            ISheet sheet; //Create the ISheet object to read the sheet cell values  
            string filename = Path.GetFileName(Server.MapPath(files.FileName)); //get the uploaded file name  
            string fullPath = Path.Combine(path, filename);
            files.SaveAs(fullPath);
            var fileExt = Path.GetExtension(filename); //get the extension of uploaded excel file  
            byte[] buffer;
            if (fileExt == ".xls")
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
            var policy = dbContext.Policy.Include("AllocationRules").SingleOrDefault(p => p.PolicyNo == policyNo && p.Auth);
            if (policy == null)
            {
                ModelState.AddModelError("file", "The Policy either does not exist or has not yet been authorized");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Message = null;
                return View(vm);
            }

            var Level = dbContext.UserSecurities.Select(Val => Val.Levels).FirstOrDefault();

            var lastcode = dbContext.LastCodes.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault();
            lastcode++;
            int checkNumber;
            int insertedRow = 0;
            using (DbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
               
                    buffer = System.IO.File.ReadAllBytes(fullPath);
                    System.IO.File.Delete(fullPath);
                    var excel = new ExcelDetails
                    {
                        Name = vm.file.FileName + DateTime.Now.ToString("dd/MM/yy,hh:mm"),
                        Status = (byte)ExcelStatus.Pending,
                        Screen = (byte)ExcelTransactionType.Addition,
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
                                  sheet.GetRow(row).GetCell(11).ToString() == "" &&
                                  sheet.GetRow(row).GetCell(12).ToString() == ""
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
                             sheet.GetRow(row).GetCell(6).ToString() == "" ||
                             sheet.GetRow(row).GetCell(7).ToString() == "" ||
                             sheet.GetRow(row).GetCell(8).ToString() == ""
                        )
                        {

                            warningMessages.Add("The employee data of row : " + (row + 1) + " not complete.");

                        }
                        else if
                        (
                                
                                 !(int.TryParse(sheet.GetRow(row).GetCell(9).ToString(), out checkNumber)) ||
                                 !(int.TryParse(sheet.GetRow(row).GetCell(10).ToString(), out checkNumber)) ||
                                 !(int.TryParse(sheet.GetRow(row).GetCell(11).ToString(), out checkNumber)) ||
                                 !(int.TryParse(sheet.GetRow(row).GetCell(12).ToString(), out checkNumber))
                        )
                        {
                            warningMessages.Add("The employee contributions of row : " + (row + 1) + " are incorrect.");
                        }
                        else if
                        (
                             int.Parse(sheet.GetRow(row).GetCell(9).ToString()) == 0 &&
                             int.Parse(sheet.GetRow(row).GetCell(10).ToString()) == 0 &&
                             int.Parse(sheet.GetRow(row).GetCell(11).ToString()) == 0 &&
                             int.Parse(sheet.GetRow(row).GetCell(12).ToString()) == 0
                        )
                        {
                            warningMessages.Add("The employee contributions of row : " + (row + 1) + " At least must have one value for one of the four contributions.");
                        }
                        else
                        {

                            var CustId = sheet.GetRow(row).GetCell(2).ToString();
                            if (!dbContext.EmployeePolicies.Any(c => c.CustomerId == CustId && c.CompanyId == policy.CompanyId && c.PolicyId == policy.Id))
                            {
                                dbContext.EmployeePolicies.Add(
                                      new EmployeePolicy
                                      {
                                          CustomerId = CustId,
                                          CompanyId = policy.CompanyId,
                                          PolicyId = policy.Id,
                                          isSurrendered = false
                                      }
                                 );
                                CultureInfo provider = CultureInfo.InvariantCulture;
                                var customer = new Customers.Models.Customer
                                {
                                    CustomerID = CustId,
                                    EnName = sheet.GetRow(row).GetCell(3).ToString(),
                                    DateOfBirth = DateTime.ParseExact(sheet.GetRow(row).GetCell(4).ToString(), "dd/MM/yyyy", provider),
                                    DateOfHiring = DateTime.ParseExact(sheet.GetRow(row).GetCell(6).ToString(), "dd-MMM-yyyy", provider),
                                    DateOfContribute = DateTime.ParseExact(sheet.GetRow(row).GetCell(5).ToString(), "dd/MM/yyyy", provider),
                                    Salary = Convert.ToInt32(sheet.GetRow(row).GetCell(8).NumericCellValue),
                                    SysDate = DateTime.Now,
                                    Maker = User.Identity.GetUserId(),
                                    Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                                    Chk = (Level == Levels.TwoLevels) ? true : false,
                                    Auther = User.Identity.GetUserId(),
                                    Auth = true,
                                    CompanyId = policy.CompanyId,
                                    DeletFlag = DeleteFlag.NotDeleted,
                                    ExcelId = excel.Id
    };
                                dbContext.Customers.Add(
                                  customer
                                    );
                                foreach (var allocationRule in policy.AllocationRules)
                                {
                                    var transactionDateWithoutTime = vm.processingDate.Date;

                                    var icprice = dbContext.ICPrices.Where(ic => ic.FundId == allocationRule.FundId && ic.Auth == true && DbFunctions.TruncateTime(ic.Date) == transactionDateWithoutTime).OrderByDescending(i => i.Date).FirstOrDefault();

                                    if (icprice == null)//icprice from api
                                    {
                                        warningMessages.Add("The Fund  : " + allocationRule.Fund.Name + " has no price.");
                                    }
                                    else
                                    {
                                        Decimal employeeContributions;
                                        if (Decimal.TryParse(Convert.ToString(sheet.GetRow(row).GetCell(9)), out employeeContributions))
                                        {
                                            if (employeeContributions != 0)
                                            {
                                                Subscription subscriptionEmployeeContributions;
                                                Trans transEmployeeContributions;
                                                subscriptionEmployeeContributions = new Subscription
                                                {
                                                    code = lastcode,
                                                    cust_id = CustId,
                                                    NAV = icprice.Price,//api to get icprice
                                                    amount_3 = employeeContributions * Decimal.Divide(allocationRule.PercentageOfEmpShare, 100),
                                                    units = (employeeContributions * Decimal.Divide(allocationRule.PercentageOfEmpShare, 100)) / icprice.Price,//api to get icprice
                                                    SysDate = DateTime.Now,
                                                    fund_id = allocationRule.FundId,
                                                    inputer = User.Identity.GetUserId(),

                                                    Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                                                    Chk = (Level == Levels.TwoLevels) ? true : false,
                                                    auther = User.Identity.GetUserId(),
                                                    auth = 1,
                                                    GTF_Flag = 1,
                                                    UserID = User.Identity.GetUserId(),
                                                    PolicyId = policy.Id,
                                                    delreason = DeleteFlag.NotDeleted,

                                                    nav_date = icprice.Date,//api to get icprice
                                                    Nav_Ddate = icprice.Date,//api to get icprice
                                                    ProcessingDate = icprice.Date,//api to get icprice
                                                    total = employeeContributions * Decimal.Divide(allocationRule.PercentageOfEmpShare, 100),
                                                    system_date = DateTime.Now,
                                                    ExcelId = excel.Id
                                                };
                                                transEmployeeContributions = new Trans()
                                                {

                                                    transid = lastcode,
                                                    total_value = employeeContributions * Decimal.Divide(allocationRule.PercentageOfEmpShare, 100),
                                                    quantity = (employeeContributions * Decimal.Divide(allocationRule.PercentageOfEmpShare, 100)) / icprice.Price,//api to get icprice
                                                    unit_price = icprice.Price,//api to get icprice
                                                    SysDate = DateTime.Now,
                                                    cust_id = CustId,
                                                    fund_id = allocationRule.FundId,
                                                    inputer = User.Identity.GetUserId(),

                                                    auth = 1,
                                                    value_date = icprice.Date,//api to get icprice
                                                    Nav_Ddate = icprice.Date,//api to get icprice
                                                    ProcessingDate = icprice.Date,//api to get icprice
                                                    GTF_Flag = 1,
                                                    UserID = User.Identity.GetUserId(),
                                                    PolicyId = policy.Id,
                                                    
                                                    ExcelId = excel.Id
                                                };
                                                dbContext.Subscriptions.Add(subscriptionEmployeeContributions);
                                                dbContext.Trans.Add(transEmployeeContributions);
                                                dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                                                lastcode++;
                                            }
                                        }

                                        Decimal employerContributions;
                                        if (Decimal.TryParse(Convert.ToString(sheet.GetRow(row).GetCell(10)), out employerContributions))
                                        {
                                            if (employerContributions != 0)
                                            {
                                                Subscription subscriptionEmployerContributions;
                                                Trans transEmployerContributions;
                                                subscriptionEmployerContributions = new Subscription
                                                {

                                                    code = lastcode,
                                                    cust_id = CustId,
                                                    NAV = icprice.Price,//api to get icprice
                                                    amount_3 = employerContributions * Decimal.Divide(allocationRule.PercentageOfCompanyShare, 100),
                                                    units = (employerContributions * Decimal.Divide(allocationRule.PercentageOfCompanyShare, 100)) / icprice.Price,//api to get icprice
                                                    SysDate = DateTime.Now,
                                                    fund_id = allocationRule.FundId,
                                                    inputer = User.Identity.GetUserId(),
                                                    Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                                                    Chk = (Level == Levels.TwoLevels) ? true : false,
                                                    auther = null,
                                                    auth = 0,
                                                    GTF_Flag = 2,
                                                    UserID = User.Identity.GetUserId(),
                                                    PolicyId = policy.Id,
                                                    delreason = DeleteFlag.NotDeleted,

                                                    nav_date = icprice.Date,//api to get icprice
                                                    Nav_Ddate = icprice.Date,//api to get icprice
                                                    ProcessingDate = icprice.Date,//api to get icprice
                                                    total = employerContributions * Decimal.Divide(allocationRule.PercentageOfCompanyShare, 100),
                                                    system_date = DateTime.Now,
                                                    ExcelId = excel.Id
                                                };
                                                transEmployerContributions = new Trans()
                                                {

                                                    transid = lastcode,
                                                    total_value = employerContributions * Decimal.Divide(allocationRule.PercentageOfCompanyShare, 100),
                                                    quantity = (employerContributions * Decimal.Divide(allocationRule.PercentageOfCompanyShare, 100)) / icprice.Price,//api to get icprice
                                                    unit_price = icprice.Price,//api to get icprice
                                                    SysDate = DateTime.Now,
                                                    cust_id = CustId,
                                                    fund_id = allocationRule.FundId,
                                                    inputer = User.Identity.GetUserId(),
                                                    auth = 0,
                                                    value_date = icprice.Date,//api to get icprice
                                                    Nav_Ddate = icprice.Date,//api to get icprice
                                                    ProcessingDate = icprice.Date,//api to get icprice
                                                    GTF_Flag = 2,
                                                    PolicyId = policy.Id,
                                                    UserID = User.Identity.GetUserId(),
                                                    pur_sal = 0,
                                                    ExcelId = excel.Id
                                                };
                                                dbContext.Subscriptions.Add(subscriptionEmployerContributions);
                                                dbContext.Trans.Add(transEmployerContributions);
                                                dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                                                lastcode++;
                                            }
                                        }

                                        Decimal additionalEmployeeContributions;
                                        if (Decimal.TryParse(Convert.ToString(sheet.GetRow(row).GetCell(11)), out additionalEmployeeContributions))
                                        {

                                            if (policy.HasBooster)
                                            {
                                                if (additionalEmployeeContributions != 0)
                                                {
                                                    Subscription subscriptionAdditionalEmployeeContributions;
                                                    Trans transAdditionalEmployeeContributions;
                                                    subscriptionAdditionalEmployeeContributions = new Subscription
                                                    {

                                                        code = lastcode,
                                                        cust_id = CustId,
                                                        NAV = icprice.Price,//api to get icprice
                                                        amount_3 = additionalEmployeeContributions * Decimal.Divide(allocationRule.PercentageOfEmpShareBooster, 100),
                                                        units = (additionalEmployeeContributions * Decimal.Divide(allocationRule.PercentageOfEmpShareBooster, 100)) / icprice.Price,//api to get icprice
                                                        SysDate = DateTime.Now,
                                                        fund_id = allocationRule.FundId,
                                                        inputer = User.Identity.GetUserId(),
                                                        Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                                                        Chk = (Level == Levels.TwoLevels) ? true : false,
                                                        auther = null,
                                                        auth = 0,
                                                        GTF_Flag = 3,
                                                        UserID = User.Identity.GetUserId(),
                                                        PolicyId = policy.Id,
                                                        delreason = DeleteFlag.NotDeleted,

                                                        nav_date = icprice.Date,//api to get icprice
                                                        Nav_Ddate = icprice.Date,//api to get icprice
                                                        ProcessingDate = icprice.Date,//api to get icprice
                                                        total = additionalEmployeeContributions * Decimal.Divide(allocationRule.PercentageOfEmpShareBooster, 100),
                                                        system_date = DateTime.Now,
                                                        ExcelId = excel.Id
                                                    };
                                                    transAdditionalEmployeeContributions = new Trans()
                                                    {

                                                        transid = lastcode,
                                                        total_value = additionalEmployeeContributions * Decimal.Divide(allocationRule.PercentageOfEmpShareBooster, 100),
                                                        quantity = (additionalEmployeeContributions * Decimal.Divide(allocationRule.PercentageOfEmpShareBooster, 100)) / icprice.Price,//api to get icprice
                                                        unit_price = icprice.Price,//api to get icprice
                                                        SysDate = DateTime.Now,
                                                        cust_id = CustId,
                                                        fund_id = allocationRule.FundId,
                                                        inputer = User.Identity.GetUserId(),
                                                        auth = 0,
                                                        value_date = icprice.Date,//api to get icprice
                                                        Nav_Ddate = icprice.Date,//api to get icprice
                                                        ProcessingDate = icprice.Date,//api to get icprice
                                                        GTF_Flag = 3,
                                                        PolicyId = policy.Id,
                                                        UserID = User.Identity.GetUserId(),
                                                        pur_sal = 0,
                                                        ExcelId = excel.Id
                                                    };
                                                    dbContext.Subscriptions.Add(subscriptionAdditionalEmployeeContributions);
                                                    dbContext.Trans.Add(transAdditionalEmployeeContributions);
                                                    dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                                                    lastcode++;
                                                }
                                            }
                                            else
                                            {
                                                if (additionalEmployeeContributions != 0)
                                                {
                                                    warningMessages.Add("The employee of row : " + (row + 1) + " has value in employee share(Booster) but this policy doesn't allow booster.");
                                                }
                                            }


                                        }

                                        Decimal additionalEmployerContributions;
                                        if (Decimal.TryParse(Convert.ToString(sheet.GetRow(row).GetCell(12)), out additionalEmployerContributions))
                                        {

                                            if (policy.HasBooster)
                                            {
                                                if (additionalEmployerContributions != 0)
                                                {
                                                    Subscription subscriptionAdditionalEmployerContributions;
                                                    Trans transAdditionalEmployerContributions;
                                                    subscriptionAdditionalEmployerContributions = new Subscription
                                                    {

                                                        code = lastcode,
                                                        cust_id = CustId,
                                                        NAV = icprice.Price,//api to get icprice
                                                        amount_3 = additionalEmployerContributions * Decimal.Divide(allocationRule.PercentageOfCompanyShareBooster, 100),
                                                        units = (additionalEmployerContributions * Decimal.Divide(allocationRule.PercentageOfCompanyShareBooster, 100)) / icprice.Price,//api to get icprice
                                                        SysDate = DateTime.Now,
                                                        fund_id = allocationRule.FundId,
                                                        inputer = User.Identity.GetUserId(),
                                                        Checker = (Level == Levels.TwoLevels) ? User.Identity.GetUserId() : null,
                                                        Chk = (Level == Levels.TwoLevels) ? true : false,
                                                        auther = null,
                                                        auth = 0,
                                                        GTF_Flag = 4,
                                                        UserID = User.Identity.GetUserId(),
                                                        PolicyId = policy.Id,
                                                        delreason = DeleteFlag.NotDeleted,

                                                        nav_date = icprice.Date,//api to get icprice
                                                        Nav_Ddate = icprice.Date,//api to get icprice
                                                        ProcessingDate = icprice.Date,//api to get icprice
                                                        total = additionalEmployerContributions * Decimal.Divide(allocationRule.PercentageOfCompanyShareBooster, 100),
                                                        system_date = DateTime.Now,
                                                        ExcelId = excel.Id
                                                    };
                                                    transAdditionalEmployerContributions = new Trans()
                                                    {

                                                        transid = lastcode,
                                                        total_value = additionalEmployerContributions * Decimal.Divide(allocationRule.PercentageOfCompanyShareBooster, 100),
                                                        quantity = (additionalEmployerContributions * Decimal.Divide(allocationRule.PercentageOfCompanyShareBooster, 100)) / icprice.Price,//api to get icprice
                                                        unit_price = icprice.Price,//api to get icprice
                                                        SysDate = DateTime.Now,
                                                        cust_id = CustId,
                                                        fund_id = allocationRule.FundId,
                                                        inputer = User.Identity.GetUserId(),
                                                        auth = 0,
                                                        value_date = icprice.Date,//api to get icprice
                                                        Nav_Ddate = icprice.Date,//api to get icprice
                                                        ProcessingDate = icprice.Date,//api to get icprice
                                                        GTF_Flag = 4,
                                                        PolicyId = policy.Id,
                                                        UserID = User.Identity.GetUserId(),
                                                        pur_sal = 0,
                                                        ExcelId = excel.Id
                                                    };
                                                    dbContext.Subscriptions.Add(subscriptionAdditionalEmployerContributions);
                                                    dbContext.Trans.Add(transAdditionalEmployerContributions);
                                                    dbContext.LastCodes.Add(new LastCode { Code = lastcode });
                                                    lastcode++;
                                                }
                                            }
                                            else
                                            {
                                                if (additionalEmployerContributions != 0)
                                                {
                                                    warningMessages.Add("The employee of row : " + (row + 1) + " has value in company share(Booster) but this policy doesn't allow booster.");
                                                }
                                            }

                                        }
                                        insertedRow++;
                                    }
                                    

                                }
                            }
                            else
                            {
                                warningMessages.Add("The employee of row : " + (row + 1) + " is already exist.");

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


     
        public ActionResult UploadModifications()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadModifications(UploadModificationsViewModel vm)
        {



            if (!ModelState.IsValid)
            {
                ViewBag.Message = null;
                return View(vm);
            }

            List<string> warningMessages = new List<string>();
            HttpPostedFileBase files = vm.file; //Read the Posted Excel File  
            var path = Server.MapPath("~/Temp");
            //Read the Posted Excel File  
            ISheet sheet; //Create the ISheet object to read the sheet cell values  
            string filename = Path.GetFileName(Server.MapPath(files.FileName)); //get the uploaded file name  
            string fullPath = Path.Combine(path, filename);
            files.SaveAs(fullPath); var 
            fileExt = Path.GetExtension(filename); //get the extension of uploaded excel file  
            byte[] buffer;
            if (fileExt == ".xls")
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
            var policy = dbContext.Policy.Include("AllocationRules").SingleOrDefault(p => p.PolicyNo == policyNo && p.Auth);
            if (policy == null)
            {
                ModelState.AddModelError("file", "The Policy either does not exist or has not yet been authorized");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Message = null;
                return View(vm);
            }
            var lastcode = dbContext.LastCodes.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault();
            lastcode++;
            
            int insertedRow = 0;
            var customers = dbContext.Customers.ToList();
            List<Customers.Models.Customer> customersWillBeUpdated = new List<Customers.Models.Customer>();
            var Level = dbContext.UserSecurities.Select(Val => Val.Levels).FirstOrDefault();

            using (DbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {

                buffer = System.IO.File.ReadAllBytes(fullPath);
                System.IO.File.Delete(fullPath);
                var excel = new ExcelDetails
                {
                    Name = vm.file.FileName + DateTime.Now.ToString("dd:MM:yy , hh:mm"),
                    Status = (byte)ExcelStatus.Pending,
                    Screen = (byte)ExcelTransactionType.Modification,
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
                              sheet.GetRow(row).GetCell(8).ToString() == "" 
                             
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
                         sheet.GetRow(row).GetCell(6).ToString() == "" ||
                         sheet.GetRow(row).GetCell(7).ToString() == "" ||
                         sheet.GetRow(row).GetCell(8).ToString() == ""
                    )
                    {

                        warningMessages.Add("The employee data of row : " + (row + 1) + " not complete.");

                    }
                   
                    else
                    {
                        var CustId = sheet.GetRow(row).GetCell(2).ToString();

                        if (dbContext.Customers.Any(c => c.CustomerID == CustId && c.CompanyId == policy.CompanyId))
                        {
                            CultureInfo provider = CultureInfo.InvariantCulture;
                            var customer = new Customers.Models.Customer
                            {
                                CustomerID = CustId,
                                EnName = sheet.GetRow(row).GetCell(3).ToString(),
                                DateOfBirth = DateTime.ParseExact(sheet.GetRow(row).GetCell(4).ToString(), "dd/MM/yyyy", provider),
                                DateOfHiring = DateTime.ParseExact(sheet.GetRow(row).GetCell(6).ToString(), "dd-MMM-yyyy", provider),
                                DateOfContribute = DateTime.ParseExact(sheet.GetRow(row).GetCell(5).ToString(), "dd/MM/yyyy", provider),
                                Salary = Convert.ToInt32(sheet.GetRow(row).GetCell(8).NumericCellValue),
                                SysDate = DateTime.Now,
                                Maker = User.Identity.GetUserId(),
                                Auth = false,
                                DeletFlag = DeleteFlag.NotDeleted
                            };
                            customersWillBeUpdated.Add(customer);

                           
                            
                           insertedRow++;
                        }
                        else
                        {
                            warningMessages.Add("The employee of row : " + (row + 1) + " not exist.");

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
                        var ids = customersWillBeUpdated.Select(cu => cu.CustomerID).ToList();
                        dbContext.Customers
        .Where(x => ids.Contains(x.CustomerID) && x.CompanyId == policy.CompanyId)
        .ToList()
        .ForEach(c =>
        {
            c.EnName = customersWillBeUpdated.SingleOrDefault(cu => cu.CustomerID == c.CustomerID).EnName;
            c.DateOfBirth = customersWillBeUpdated.SingleOrDefault(cu => cu.CustomerID == c.CustomerID).DateOfBirth;
            c.DateOfHiring = customersWillBeUpdated.SingleOrDefault(cu => cu.CustomerID == c.CustomerID).DateOfHiring;
            c.DateOfContribute = customersWillBeUpdated.SingleOrDefault(cu => cu.CustomerID == c.CustomerID).DateOfContribute;
            c.Salary = customersWillBeUpdated.SingleOrDefault(cu => cu.CustomerID == c.CustomerID).Salary;
            c.SysDate = customersWillBeUpdated.SingleOrDefault(cu => cu.CustomerID == c.CustomerID).SysDate;
            c.Maker = customersWillBeUpdated.SingleOrDefault(cu => cu.CustomerID == c.CustomerID).Maker;
            c.Auth = true;
            c.DeletFlag = DeleteFlag.NotDeleted;
        });

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