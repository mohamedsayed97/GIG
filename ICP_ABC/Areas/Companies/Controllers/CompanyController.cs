using ICP_ABC.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Mvc;

namespace ICP_ABC.Areas.Companies.Controllers
{
    public class CompanyController : Controller
    {
        private ApplicationDbContext dbContext = new ApplicationDbContext();
        UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(new ApplicationDbContext());
        static int[] IDs;
        // GET: Policies/Policy
        public ActionResult Index()
        {
            return View();
        }

//        // [Route("Create")]
//        [AuthorizedRights(Screen = "Company", Right = "Create")]
//        public ActionResult Create()
//        {
//            var model = new PolicyViewModel
//            {
//                Funds = dbContext.Funds.Where(f=> f.Auth).ToList(),
//                EffectiveDate = DateTime.Now
//            };
//            return View(model);
//        }


//        public object checkAllocationRules(List<AllocationRuleViewModel> AllocationRules,bool HasBooster)
//        {
//            string errorMessage = "";
//            bool valid = true;
          
//                if (!HasBooster)
//                {
//                    if (AllocationRules.Select(ar => (int)ar.PercentageOfEmpShare).Sum() != 100)
//                    {
//                        if (errorMessage == "") errorMessage = "The percentage for EmpShare column should be 100";
//                        else errorMessage = errorMessage + " & The percentage for EmpShare column should be 100";
//                        valid = false;
//                    }
//                    if (AllocationRules.Select(ar => (int)ar.PercentageOfCompanyShare).Sum() != 100)
//                    {
//                        if (errorMessage == "") errorMessage = "The percentage for CompanyShare column should be 100";
//                        else errorMessage = errorMessage + " & The percentage for CompanyShare column should be 100";
//                        valid = false;
//                    }
//                }
//                else
//                {
//                    if (AllocationRules.Select(ar => (int)ar.PercentageOfEmpShare).Sum() != 100)
//                    {
//                        if (errorMessage == "") errorMessage = "The percentage for EmpShare column should be 100";
//                        else errorMessage = errorMessage + " & The percentage for EmpShare column should be 100";
//                        valid = false;
//                    }
//                    if (AllocationRules.Select(ar => (int)ar.PercentageOfCompanyShare).Sum() != 100)
//                    {
//                        if (errorMessage == "") errorMessage = "The percentage for CompanyShare column should be 100";
//                        else errorMessage = errorMessage + " & The percentage for CompanyShare column should be 100";
//                        valid = false;
//                    }
//                    if (AllocationRules.Select(ar => (int)ar.PercentageOfEmpShareBooster).Sum() != 100)
//                    {
//                        if (errorMessage == "") errorMessage = "The percentage for EmpShare(Booster) column should be 100";
//                        else errorMessage = errorMessage + " & The percentage for EmpShare(Booster) column should be 100";
//                        valid = false;
//                    }
//                    if (AllocationRules.Select(ar => (int)ar.PercentageOfCompanyShareBooster).Sum() != 100)
//                    {
//                        if (errorMessage == "") errorMessage = "The percentage for CompanyShare(Booster) column should be 100";
//                        else errorMessage = errorMessage + " & The percentage for CompanyShare(Booster) column should be 100";
//                        valid = false;
//                    }
//                }
            
         
//            return new { Message = errorMessage,isValid = valid };
//        }

//        [AuthorizedRights(Screen = "Policy", Right = "Create")]
//        [HttpPost]
//        public ActionResult Create(PolicyViewModel model)
//        {
//            var error = "";
//            if (model.AllocationRules == null)
//            {
//                if (error == "") error = "The policy must have at least one allocation rule";
//                else error = error + " & The policy must have at least one allocation rule";
               
//            }
//            else
//            {
//                dynamic checkRules = checkAllocationRules(model.AllocationRules.ToList(), model.HasBooster);
//                if (!checkRules.isValid)
//                {
//                    ModelState.AddModelError("", checkRules.Message);
//                    if (error == "") error = checkRules.Message;
//                    else error = error + " & "+ checkRules.Message;
//                }
//            }
           
//            if (dbContext.Policy.Any(p => p.PolicyNo == model.PolicyNo))
//            {
//                if (error == "") error = "Policy No is already taken.";
//                else error = error + " & Policy No is already taken.";
//            }
//            if (dbContext.Policy.Any(p => p.PolicyHolderName == model.PolicyHolderName))
//            {
//                if (error == "") error = "PolicyHolder Name No is already taken.";
//                else error = error + " & PolicyHolder Name No is already taken.";
//            }
//            if (!ModelState.IsValid || error != "")
//            {
//                return Json(new { success = false, issue = model, errors = error});
//            }
          

            

//            var Level = dbContext.UserSecurities.Select(Val => Val.Levels).FirstOrDefault();
//            if (Level == Levels.TwoLevels)
//            {
//                Policy policy = new Policy
//                {
//                    PolicyNo = model.PolicyNo,
//                    PolicyHolderName = model.PolicyHolderName,
//                    BusinessChannel = model.BusinessChannel,
//                    CalculationBasis = model.CalculationBasis,
//                    EffectiveDate = model.EffectiveDate,
//                    PaymentFrequency = model.PaymentFrequency,
//                    Status = model.Status,
//                    HasBooster = model.HasBooster,
//                    HasWithdrawal = model.HasWithdrawal,
//                    AllocationRules = model.AllocationRules.Select(ar => new AllocationRule {
//                        FundId = ar.FundId,
//                        PercentageOfCompanyShare = ar.PercentageOfCompanyShare,
//                        PercentageOfCompanyShareBooster = ar.PercentageOfCompanyShareBooster,
//                        PercentageOfEmpShare = ar.PercentageOfEmpShare,
//                        PercentageOfEmpShareBooster = ar.PercentageOfEmpShareBooster
//                    }).ToList(),

//                    Auth = false,
//                    Chk = true,
//                    Maker = User.Identity.GetUserId(),
//                    Checker = User.Identity.GetUserId(),
//                    SysDate = DateTime.Now,
//                    DeletFlag = DeleteFlag.NotDeleted
//                };
//                dbContext.Policy.Add(policy);
//                try
//                {
//                    dbContext.SaveChanges();
//                }
//                catch (DbEntityValidationException e)
//                {
//                    return Json(new { success = false, issue = model, errors = "something went wrong" });
//                }
//                return Json(new { success = true, policyNo = model.PolicyNo });
//            }
//            else
//            {
//                Policy policy = new Policy
//                {
//                    PolicyNo = model.PolicyNo,
//                    PolicyHolderName = model.PolicyHolderName,
//                    BusinessChannel = model.BusinessChannel,
//                    CalculationBasis = model.CalculationBasis,
//                    EffectiveDate = model.EffectiveDate,
//                    PaymentFrequency = model.PaymentFrequency,
//                    Status = model.Status,
//                    HasBooster = model.HasBooster,
//                    HasWithdrawal = model.HasWithdrawal,
//                    AllocationRules = model.AllocationRules.Select(ar => new AllocationRule
//                    {
//                        FundId = ar.FundId,
//                        PercentageOfCompanyShare = ar.PercentageOfCompanyShare,
//                        PercentageOfCompanyShareBooster = ar.PercentageOfCompanyShareBooster,
//                        PercentageOfEmpShare = ar.PercentageOfEmpShare,
//                        PercentageOfEmpShareBooster = ar.PercentageOfEmpShareBooster
//                    }).ToList(),
//                    Auth = false,
//                    Chk = false,
//                    Maker = User.Identity.GetUserId(),
//                    Checker = null,
//                    SysDate = DateTime.Now,
//                    DeletFlag = DeleteFlag.NotDeleted

//                };
//                dbContext.Policy.Add(policy);
//                try
//                {
//                    dbContext.SaveChanges();
//                }
//                catch (DbEntityValidationException e)
//                {
//                    return Json(new { success = false, issue = model, errors = "something went wrong" });
//                }
//                return Json(new { success = true  , policyNo = model.PolicyNo});

//            }
//        }

//        [AuthorizedRights(Screen = "Policy", Right = "Update")]
//        public ActionResult Edit(string PolicyNo)
//        {

//            var model = dbContext.Policy.Where(c => c.PolicyNo == PolicyNo).Include("AllocationRules").FirstOrDefault();
//            PolicyViewModel policy = new PolicyViewModel
//            {
//                Funds = dbContext.Funds.Where(f => f.Auth).ToList(),
           
//                PolicyNo = model.PolicyNo,
//                PolicyHolderName = model.PolicyHolderName,
//                BusinessChannel = model.BusinessChannel,
//                CalculationBasis = model.CalculationBasis,
//                EffectiveDate = model.EffectiveDate,
//                PaymentFrequency = model.PaymentFrequency,
//                Status = model.Status,
//                HasBooster = model.HasBooster,
//                HasWithdrawal = model.HasWithdrawal,
//                AllocationRules = model.AllocationRules.Select(ar => new AllocationRuleViewModel()
//                {
//                    FundId = ar.FundId,
//                    PercentageOfCompanyShare = ar.PercentageOfCompanyShare,
//                    PercentageOfCompanyShareBooster = ar.PercentageOfCompanyShareBooster,
//                    PercentageOfEmpShare = ar.PercentageOfEmpShare,
//                    PercentageOfEmpShareBooster = ar.PercentageOfEmpShareBooster,
//                    Fund = ar.Fund
//                }).ToList(),
//            };
//            return View(policy);
//        }
//        //   [Route("Edit")]
//        //[ValidateAntiForgeryToken]
//        [HttpPost]
//        [AuthorizedRights(Screen = "Policy", Right = "Update")]
//        public ActionResult Edit(PolicyViewModel model)
//        {
//            var error = "";
//            if (model.AllocationRules == null)
//            {
//                if (error == "") error = "The policy must have at least one allocation rule";
//                else error = error + " & The policy must have at least one allocation rule";

//            }
//            else
//            {
//                dynamic checkRules = checkAllocationRules(model.AllocationRules.ToList(), model.HasBooster);
//                if (!checkRules.isValid)
//                {
          
//                    if (error == "") error = checkRules.Message;
//                    else error = error + " & " + checkRules.Message;
//                }
//            }

         
//            if (!ModelState.IsValid || error != "")
//            {
//                return Json(new { success = false, issue = model, errors = error });
//            }


//            var Policy = dbContext.Policy.Where(c => c.PolicyNo == model.PolicyNo).Include("AllocationRules").FirstOrDefault();
//            dbContext.AllocationRules.RemoveRange(Policy.AllocationRules);
//            if (Policy != null)
//            {
//                Policy.PolicyNo = model.PolicyNo;
//                Policy.PolicyHolderName = model.PolicyHolderName;
//                Policy.BusinessChannel = model.BusinessChannel;
//                Policy.CalculationBasis = model.CalculationBasis;
//                Policy.EffectiveDate = model.EffectiveDate;
//                Policy.PaymentFrequency = model.PaymentFrequency;
//                Policy.Status = model.Status;
//                Policy.HasBooster = model.HasBooster;
//                Policy.HasWithdrawal = model.HasWithdrawal;
//                Policy.AllocationRules = model.AllocationRules.Select(ar => new AllocationRule()
//                {
//                    FundId = ar.FundId,
//                    PercentageOfCompanyShare = ar.PercentageOfCompanyShare,
//                    PercentageOfCompanyShareBooster = ar.PercentageOfCompanyShareBooster,
//                    PercentageOfEmpShare = ar.PercentageOfEmpShare,
//                    PercentageOfEmpShareBooster = ar.PercentageOfEmpShareBooster,
//                    Fund = ar.Fund
//                }).ToList();
//                try
//                {
//                    dbContext.SaveChanges();
//                }
//                catch (DbUpdateException ex)
//                {
//                    return Json(new { success = false, issue = model, errors = "something went wrong" });
//                }

//                return Json(new { success = true, policyNo = model.PolicyNo });
//            }

//            return Json(new { success = true });
//        }


//        [AuthorizedRights(Screen = "Policy", Right = "Read")]
//        public ViewResult Search(string sortOrder, string RadioCHeck, string currentFilter, string PolicyNo, int? page, string PolicyHolderName, int? PaymentFrequency, int? Status, int? BusinessChannel, int? CalculationBasis, DateTime? EffectiveDate)
//        {

//            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
//            ViewBag.SecuLevel = seculevel;
//            //var LogUser = User.Identity.GetUserId();
//            //var thisUser = dbContext.Users.Where(x => x.Id == LogUser && x.BranchRight == true).FirstOrDefault();

//            if (page == null && PolicyNo == null && PolicyHolderName == null && PaymentFrequency == null && EffectiveDate == null && Status == null && CalculationBasis == null && BusinessChannel == null)
//            {
//                int pageSize = 10;
//                int pageNumber = (page ?? 1);

//                var Policies = dbContext.Policy.Where(c => c.DeletFlag == DeleteFlag.NotDeleted).Take(0);

//                return View(Policies.ToPagedList(pageNumber, pageSize));

//            }

//            else
//            {



//                ViewBag.CurrentSort = sortOrder;
//                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
//                ViewBag.DateSortParm = sortOrder == "Code" ? "code_desc" : "Code";

//                ViewBag.CurrentFilter = PolicyHolderName;

//                var Policies = dbContext.Policy.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);
//                var currentuserId = User.Identity.GetUserId();
//                var currentUser = dbContext.Users.Where(u => u.Id == currentuserId).First();


//                if (!String.IsNullOrEmpty(PolicyHolderName))
//                {
//                    Policies = Policies.Where(s => s.PolicyHolderName.Contains(PolicyHolderName));
//                }


//                if (!String.IsNullOrEmpty(PolicyNo))
//                {
//                    //var code = int.Parse(Code);
//                    Policies = Policies.Where(s => s.PolicyNo == PolicyNo);
//                    ViewData["PolicyNo"] = PolicyNo;
//                }
//                /*


//EffectiveDate
//7
//CalculationBasis
//BusinessChannel
//      */
//                /*
//                             var ICPrice = dbContext.ICPrices
//                          .SingleOrDefault(i => i.FundId == Subscription.fund_id && i.Auth == true && DbFunctions.TruncateTime(i.Date) == Subscription.nav_date.Value.Date);

//          */
//                if (!String.IsNullOrEmpty(PaymentFrequency.ToString()))
//                {
//                    Policies = Policies
//                   .Where(u => u.PaymentFrequency == PaymentFrequency);

//                    ViewData["PaymentFrequency"] = PaymentFrequency;
//                }

//                if (!String.IsNullOrEmpty(CalculationBasis.ToString()))
//                {
//                    Policies = Policies
//                   .Where(u => u.CalculationBasis == CalculationBasis);

//                    ViewData["CalculationBasis"] = CalculationBasis;
//                }

//                if (!String.IsNullOrEmpty(BusinessChannel.ToString()))
//                {
//                    Policies = Policies
//                   .Where(u => u.BusinessChannel == BusinessChannel);

//                    ViewData["BusinessChannel"] = BusinessChannel;
//                }

//                if (!String.IsNullOrEmpty(EffectiveDate.ToString()))
//                {
//                    Policies = Policies
//                   .Where(u => DbFunctions.TruncateTime(u.EffectiveDate) == EffectiveDate.Value.Date);

//                    ViewData["EffectiveDate"] = EffectiveDate;
//                }

//                //checkRadio 
//                if (!String.IsNullOrEmpty(RadioCHeck))
//                {
//                    //Case Auth
//                    if (RadioCHeck == "1")
//                    {
//                        Policies = Policies.Where(s => s.Auth == true);

//                    }
//                    //Case Checker
//                    if (RadioCHeck == "2")
//                    {
//                        Policies = Policies.Where(s => s.Chk == true && s.Auth == false);

//                    }
//                    //Case maker
//                    if (RadioCHeck == "3")
//                    {
//                        Policies = Policies.Where(s => s.Auth == false && s.Chk == false);

//                    }
//                    //Case All
//                    if (RadioCHeck == "4")
//                    {
//                        Policies = Policies;

//                    }


//                }
//                //

//                switch (sortOrder)
//                {
//                    case "PolicyHolderName_desc":

//                        Policies = Policies.OrderByDescending(s => s.PolicyHolderName);

//                        break;

//                    case "PolicyNo_desc":
//                        Policies = Policies.OrderByDescending(s => s.PolicyNo);
//                        break;
//                    default:  // Name ascending 
//                        //funds = funds.OrderBy(s => s.Name);
//                        Policies = Policies.OrderBy(s => s.Id);
//                        break;
//                }


//                int pageSize = 10;
//                int pageNumber = (page ?? 1);
//                ViewData["radioCHeck"] = RadioCHeck;
//                int count = Policies.ToList().Count();
//                IDs = new int[count];
//                IDs = Policies.Select(s => s.Id).ToArray();

//                int[] myInts = IDs;
//                Array.Sort(myInts);
//                IDs = myInts.Select(x => x).ToArray();

//                return View(Policies.ToPagedList(pageNumber, pageSize));
//            }
//        }

//        [AuthorizedRights(Screen = "Policy", Right = "Read")]
//        public ActionResult Details(string PolicyNo)
//        {
//            PolicyDetailsViewModel model = new PolicyDetailsViewModel();
//            model.DeleteBtn = true;
//            model.EditBtn = true;
//            var policy = dbContext.Policy.SingleOrDefault(p => p.PolicyNo == PolicyNo);
//            var CurrentUser = User.Identity.GetUserId();
//            if (policy.Chk == false && policy.Maker != CurrentUser)
//            {
//                model.CheckBtn = true;
//            }
//            if (policy.Chk == true && policy.Checker != null && policy.Auth == false && policy.Checker != CurrentUser)
//            {
//                model.AuthBtn = true;
//            }
//            if (policy.Chk == true && policy.Checker != null && policy.Auth == true && policy.Auther != null && policy.Checker != CurrentUser)
//            {
//                model.UnAuthBtn = true;
//            }
//            if (policy.Auth == true)
//            {
//                model.DeleteBtn = false;
//                model.EditBtn = false;
//            }


//            model.code = policy.Id;
//            model.BusinessChannel = policy.BusinessChannel;
//            model.CalculationBasis = policy.CalculationBasis;
//            model.EffectiveDate = policy.EffectiveDate;
//            model.PaymentFrequency = policy.PaymentFrequency;
//            model.Status = policy.Status;
//            model.PolicyHolderName = policy.PolicyHolderName;
//            model.PolicyNo = policy.PolicyNo;

//            if (IDs != null)
//            {
//                if (model.code == IDs.Last())
//                {
//                    TempData["Last"] = "Last";
//                }
//                if (model.code == IDs.First())
//                {
//                    TempData["First"] = "First";
//                }
//            }
//            else
//            {
//                TempData["Last"] = "Last";
//                TempData["First"] = "First";
//            }
//            return View(model);
//        }

//        [AuthorizedRights(Screen = "Policy", Right = "Read")]
//        public ActionResult Next(int id)
//        {
//            var arrlenth = IDs.Count();
//            var LastObj = dbContext.Policy.OrderBy(s => s.Id).ToList().LastOrDefault(s => s.Id == IDs[arrlenth - 1]);
//            if (id == LastObj.Id)
//            {
//                TempData["Last"] = "Last";
//                return RedirectToAction("Details", new { PolicyNo = LastObj.PolicyNo });
//            }
//            else
//            {
//                int counter = 0;
//                int next = 0;
//                foreach (var item in IDs)
//                {

//                    if (item == id)
//                    {
//                        next = counter + 1;

//                    }
//                    counter++;
//                }
//                id = IDs[next];
//                var Code = dbContext.Policy.Where(u => u.Id == id).Select(s => s.PolicyNo).FirstOrDefault();
//                if (Code == LastObj.PolicyNo)
//                {
//                    TempData["Last"] = "Last";
//                }
//                else
//                {
//                    TempData["Last"] = "NotLast";
//                }

//                return RedirectToAction("Details", new { PolicyNo = Code });
//            }

//        }

//        [AuthorizedRights(Screen = "Policy", Right = "Read")]
//        public ActionResult Previous(int id)
//        {
//            var arrindex = IDs[0];
//            var FirstObj = dbContext.Policy.OrderBy(s => s.Id).FirstOrDefault(s => s.Id == arrindex);
//            if (id == FirstObj.Id)
//            {
//                TempData["First"] = "First";
//                return RedirectToAction("Details", new { PolicyNo = FirstObj.PolicyNo });
//            }
//            else
//            {
//                int counter = 0;
//                int previous = 0;
//                foreach (var item in IDs)
//                {

//                    if (item == id)
//                    {
//                        previous = counter - 1;

//                    }
//                    counter++;
//                }
//                id = IDs[previous];

//                if (id == FirstObj.Id)
//                {
//                    TempData["First"] = "First";
//                }
//                else
//                {
//                    TempData["First"] = "NotFirst";
//                }
//                var Code = dbContext.Policy.Where(u => u.Id == id).Select(s => s.PolicyNo).FirstOrDefault();
//                return RedirectToAction("Details", new { PolicyNo = Code });
//            }

//        }

        


//        public ActionResult Authorize(string PolicyNo)
//        {
//            var Policy = dbContext.Policy.Where(f => f.PolicyNo == PolicyNo).FirstOrDefault();
//            Policy.Auth = true;
//            Policy.Auther = User.Identity.GetUserId();
//            dbContext.SaveChanges();
//            return RedirectToAction("Details", new { PolicyNo = PolicyNo });
//        }

//        [AuthorizedRights(Screen = "Policy", Right = "Check")]
//        public ActionResult Check(string PolicyNo)
//        {
//            var Policy = dbContext.Policy.Where(f => f.PolicyNo == PolicyNo).FirstOrDefault();
//            Policy.Chk = true;
//            Policy.Checker = User.Identity.GetUserId();
//            dbContext.SaveChanges();
//            return RedirectToAction("Details", new { PolicyNo = PolicyNo });

//        }


//        [AuthorizedRights(Screen = "Policy", Right = "Delete")]
//        public ActionResult Delete(string PolicyNo)
//        {
//            var Policy = dbContext.Policy.Where(f => f.PolicyNo == PolicyNo).FirstOrDefault();
//            Policy.DeletFlag = DeleteFlag.Deleted;
//            dbContext.SaveChanges();
//            return RedirectToAction("Index");
//        }

        //private void AddErrors(IdentityResult result)
        //{
        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError("", error);
        //    }
        //}
    }
}