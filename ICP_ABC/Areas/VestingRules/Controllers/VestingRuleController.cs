using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Areas.Policies.Models;
using ICP_ABC.Areas.Redemptions.Controllers;
using ICP_ABC.Areas.UsersSecurity.Models;
using ICP_ABC.Areas.VestingRules.Models;
using ICP_ABC.Extentions;
using ICP_ABC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ICP_ABC.Areas.VestingRules.Controllers
{
 

    public class VestingRuleController : Controller
    {
       
        private ApplicationDbContext dbContext = new ApplicationDbContext();
        UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(new ApplicationDbContext());
        static int[] IDs;
        // GET: Policies/Policy
        public ActionResult Index()
        {
          
            var model = new VestingRuleViewModel
            {
                Funds = dbContext.Funds.Where(f => f.DeletFlag == DeleteFlag.NotDeleted).ToList(),
                Policies = dbContext.Policy.Where(p => p.DeletFlag == DeleteFlag.NotDeleted).ToList(),
                VestingRuleDetails = new List<VestingRuleDetailsViewModel>()
                {
                    new Models.VestingRuleDetailsViewModel {  }
                }
            };
        
            return View(model);
        }

      
        [AuthorizedRights(Screen = "VestingRule", Right = "Create")]
        public ActionResult Create()
        {
            var model = new VestingRuleViewModel
            {
                Funds = new List<Fund>(),
                Policies =  dbContext.Policy.Where(p => p.Auth).ToList() ,
                VestingRuleDetails = new List<VestingRuleDetailsViewModel>()
                {
                    new Models.VestingRuleDetailsViewModel {  }
                }
            };
   
            return View(model);
            
        }


        [AuthorizedRights(Screen = "VestingRule", Right = "Create")]
        [HttpPost]
        public ActionResult Create(VestingRuleViewModel model)
        {
            var Model = new VestingRuleViewModel
            {
                Funds = dbContext.Funds.Where(f => f.Auth).ToList(),
                Policies = dbContext.Policy.Where(p => p.Auth).ToList(),
                VestingRuleDetails = model.VestingRuleDetails
            };
            var error = "";
            if(model.TransactionType == VestingRuleTransactionType.Surrender)
            {
               
                    ModelState["model.FundId"].Errors.Clear();
            }
            if (dbContext.VestingRules.Any(vr => vr.PolicyId == model.PolicyId && vr.FundId == model.FundId &&  vr.TransactionType == (byte)model.TransactionType))
            {
               if (error == "") error = "There is a Vesting Rule for that policy and Fund already.";
                else error = error + " & There is a Vesting Rule for this policy and Fund already.";
            }
         
            if (model.TransactionType == VestingRuleTransactionType.Withdrawal && !dbContext.Policy.SingleOrDefault(p=>p.Id == model.PolicyId).HasWithdrawal)
            {
                 if (error == "") error = "This policy not allow withdrawal option.";
                else error = error + " & This policy not allow withdrawal option.";
            }

            dynamic CheckIntervals = checkIntervals(model.VestingRuleDetails.Select(vs => new { vs.FromYear, vs.ToYear }));
            if (!CheckIntervals.isValid)
            {
                ModelState.AddModelError("PolicyId", "invalid intervals");

                if (error == "") error = "invalid intervals.";
                else error = error + " & invalid intervals.";
            }

            if (!ModelState.IsValid || error != "")
            {
                return Json(new { success = false, issue = model, errors = error });
            }

            VestingRule vestingRule = new VestingRule
            {
                PolicyId = model.PolicyId,
                FundId = model.FundId,
                TransactionType = (byte)model.TransactionType,
                Base = (byte)model.Base,
                VestingRuleDetails = model.VestingRuleDetails.Select(vr => new VestingRuleDetails()
                {
                    FromYear = (short)vr.FromYear,
                    ToYear = (short)vr.ToYear,
                    PercentageOfCompanyShare = vr.PercentageOfCompanyShare,
                    PercentageOfCompanyShareBooster = vr.PercentageOfCompanyShareBooster,
                    PercentageOfEmpShareBooster = vr.PercentageOfEmpShareBooster,
                    PercentageOfEmpShare = vr.PercentageOfEmpShare
                }).ToList()
            };
            var Level = dbContext.UserSecurities.Select(Val => Val.Levels).FirstOrDefault();
            if (Level == Levels.TwoLevels)
            {

                vestingRule.Auth = false;
                vestingRule.Chk = true;
                vestingRule.Maker = User.Identity.GetUserId();
                vestingRule.Checker = User.Identity.GetUserId();
                vestingRule.SysDate = DateTime.Now;
                vestingRule.DeletFlag = DeleteFlag.NotDeleted;
               
                dbContext.VestingRules.Add(vestingRule);
                try
                {
                    dbContext.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    return Json(new { success = false, issue = model, errors = "something went wrong" });
                    throw;
                }
                return Json(new { success = true, id = vestingRule.Id });
            }
            else
            {
                vestingRule.Auth = false;
                vestingRule.Chk = false;
                vestingRule.Maker = User.Identity.GetUserId();
                vestingRule.Checker = null;
                vestingRule.SysDate = DateTime.Now;
                vestingRule.DeletFlag = DeleteFlag.NotDeleted;

              
                dbContext.VestingRules.Add(vestingRule);
                try
                {
                    dbContext.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    return Json(new { success = false, issue = model, errors = "something went wrong" });
                    throw;
                }
                return Json(new { success = true, id = vestingRule.Id });

            }
        }

        [AuthorizedRights(Screen = "VestingRule", Right = "Update")]
        public ActionResult Edit(int id)
        {

            var model = dbContext.VestingRules.Where(c => c.Id == id).Include("VestingRuleDetails").FirstOrDefault();
            VestingRuleViewModel vestingRuleVM = new VestingRuleViewModel
            {
                Id = model.Id,
                Funds = dbContext.Funds.Where(f => f.DeletFlag == DeleteFlag.NotDeleted).ToList(),
                Policies = dbContext.Policy.Where(p => p.DeletFlag == DeleteFlag.NotDeleted).ToList(),
                Base = (VestingRuleBase)model.Base,
                FundId = model.FundId.Value,
                PolicyId = model.PolicyId,
                TransactionType = (VestingRuleTransactionType)model.TransactionType,
                VestingRuleDetails = model.VestingRuleDetails.Select(vr => new VestingRuleDetailsViewModel()
                {
                    
                    FromYear = (ushort)vr.FromYear,
                    ToYear = (ushort)vr.ToYear,
                    PercentageOfCompanyShare = vr.PercentageOfCompanyShare,
                    PercentageOfCompanyShareBooster = vr.PercentageOfCompanyShareBooster,
                    PercentageOfEmpShareBooster = vr.PercentageOfEmpShareBooster,
                    PercentageOfEmpShare = vr.PercentageOfEmpShare
                }).ToList()
            };
            return View(vestingRuleVM);
        }

        [HttpPost]
        [AuthorizedRights(Screen = "VestingRule", Right = "Update")]
        public ActionResult Edit(VestingRuleViewModel model)
        {
            var Model = new VestingRuleViewModel
            {
                Funds = dbContext.Funds.Where(f => f.Auth).ToList(),
                Policies = dbContext.Policy.Where(p => p.Auth).ToList(),
                VestingRuleDetails = model.VestingRuleDetails
            };
            var error = "";
            

            dynamic CheckIntervals = checkIntervals(model.VestingRuleDetails.Select(vs => new { vs.FromYear, vs.ToYear }));
            if (!CheckIntervals.isValid)
            {
                ModelState.AddModelError("PolicyId", "invalid intervals");

                if (error == "") error = "invalid intervals.";
                else error = error + " & invalid intervals.";
            }

            if (!ModelState.IsValid || error != "")
            {
                return Json(new { success = false, issue = model, errors = error });
            }
            var VestingRuleInDb = dbContext.VestingRules.Where(c => c.Id == model.Id).Include("VestingRuleDetails").FirstOrDefault();
            dbContext.VestingRuleDetails.RemoveRange(VestingRuleInDb.VestingRuleDetails);
            if (model != null)
            {
                try
                {
            

                    VestingRuleInDb.Base = (byte)model.Base;
                    VestingRuleInDb.FundId = model.FundId;
                VestingRuleInDb.PolicyId = model.PolicyId;
                VestingRuleInDb.TransactionType = (byte)model.TransactionType;
                VestingRuleInDb.VestingRuleDetails = model.VestingRuleDetails.Select(vr => new VestingRuleDetails()
                {
                    FromYear = (short)vr.FromYear,
                    ToYear = (short)vr.ToYear,
                    PercentageOfCompanyShare = vr.PercentageOfCompanyShare,
                    PercentageOfCompanyShareBooster = vr.PercentageOfCompanyShareBooster,
                    PercentageOfEmpShareBooster = vr.PercentageOfEmpShareBooster,
                    PercentageOfEmpShare = vr.PercentageOfEmpShare
                }).ToList();
                return Json(new { success = true, id = VestingRuleInDb.Id });
                }
                catch
                {
                    return Json(new { success = false, issue = model, errors = "something went wrong" });
                }
            }
            return Json(new { success = false, issue = model, errors = "No vesting rule with this id" });

        }


        [AuthorizedRights(Screen = "VestingRule", Right = "Read")]
        public ViewResult Search(string sortOrder, string RadioCHeck, string currentFilter, int? Id, int? Base, int? page, int? FundId, int? PolicyId, int? TransactionType)
        {

            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;
          
            if (page == null && Id == null && RadioCHeck == null && Base == null && FundId == null && PolicyId == null && TransactionType == null)
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                ViewData["Id"] = Id;
                ViewData["Funds"] = new SelectList(dbContext.Funds.Where(f=>f.Auth).ToList(), "FundID", "Name");
                ViewData["Policies"] = new SelectList(dbContext.Policy.Where(f => f.Auth).ToList(), "Id", "PolicyHolderName");
                var transactionTypes = from VestingRuleTransactionType d in Enum.GetValues(typeof(VestingRuleTransactionType))
                                       select new { ID = (int)d, Name = d.ToString() };
                ViewData["TransactionTypes"] = new SelectList(transactionTypes, "ID", "Name", TransactionType);

                var bases = from VestingRuleBase d in Enum.GetValues(typeof(VestingRuleBase))
                                       select new { ID = (int)d, Name = d.ToString() };
                ViewData["Bases"] = new SelectList(transactionTypes, "ID", "Name", Base);
                var VestingRules = dbContext.VestingRules.Where(c => c.Auth).Take(0).Include("Policy");

                return View(VestingRules.ToPagedList(pageNumber, pageSize));

            }

            else
            {
                ViewData["Id"] = Id;
                ViewData["Funds"] = new SelectList(dbContext.Funds.Where(f => f.Auth).ToList(), "FundID", "Name");
                ViewData["Policies"] = new SelectList(dbContext.Policy.Where(f => f.Auth).ToList(), "Id", "PolicyHolderName");
                var transactionTypes = from VestingRuleTransactionType d in Enum.GetValues(typeof(VestingRuleTransactionType))
                                       select new { ID = (int)d, Name = d.ToString() };
                ViewData["TransactionTypes"] = new SelectList(transactionTypes, "ID", "Name", TransactionType);

                var bases = from VestingRuleBase d in Enum.GetValues(typeof(VestingRuleBase))
                            select new { ID = (int)d, Name = d.ToString() };
                ViewData["Bases"] = new SelectList(transactionTypes, "ID", "Name", Base);

                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.DateSortParm = sortOrder == "Code" ? "code_desc" : "Code";

                ViewBag.CurrentFilter = Id;

                var VestingRules = dbContext.VestingRules.Where(c => c.DeletFlag == DeleteFlag.NotDeleted).Include("Policy"); 
                var currentuserId = User.Identity.GetUserId();
                var currentUser = dbContext.Users.Where(u => u.Id == currentuserId).First();


               

                if (!String.IsNullOrEmpty(Id.ToString()))
                {

                    VestingRules = VestingRules.Where(s => s.Id == Id);
                    ViewData["Id"] = Id;
                }
  
              

                if (!String.IsNullOrEmpty(PolicyId.ToString()))
                {
                    VestingRules = VestingRules
                   .Where(u => u.Id == PolicyId);

                    ViewData["PolicyId"] = PolicyId;
                }
                if (!String.IsNullOrEmpty(FundId.ToString()))
                {
                    VestingRules = VestingRules
                   .Where(u => u.FundId == FundId);

                    ViewData["FundId"] = FundId;
                }

                if (!String.IsNullOrEmpty(Base.ToString()))
                {
                    VestingRules = VestingRules
                   .Where(u => u.Base == Base);

                    ViewData["Base"] = Base;
                }

                if (!String.IsNullOrEmpty(TransactionType.ToString()))
                {
                    VestingRules = VestingRules
                   .Where(u => u.TransactionType == TransactionType);

                    ViewData["TransactionType"] = TransactionType;
                }

                //checkRadio 
                if (!String.IsNullOrEmpty(RadioCHeck))
                {
                    //Case Auth
                    if (RadioCHeck == "1")
                    {
                        VestingRules = VestingRules.Where(s => s.Auth == true);

                    }
                    //Case Checker
                    if (RadioCHeck == "2")
                    {
                        VestingRules = VestingRules.Where(s => s.Chk == true && s.Auth == false);

                    }
                    //Case maker
                    if (RadioCHeck == "3")
                    {
                        VestingRules = VestingRules.Where(s => s.Auth == false && s.Chk == false);

                    }
                    //Case All
                    if (RadioCHeck == "4")
                    {
                        VestingRules = VestingRules;

                    }


                }
                //

                switch (sortOrder)
                {
                    case "Id_desc":

                        VestingRules = VestingRules.OrderByDescending(s => s.Id);

                        break;

                    
                    default:  // Name ascending 
                        //funds = funds.OrderBy(s => s.Name);
                        VestingRules = VestingRules.OrderBy(s => s.Id);
                        break;
                }


                int pageSize = 10;
                int pageNumber = (page ?? 1);
                ViewData["radioCHeck"] = RadioCHeck;
                int count = VestingRules.ToList().Count();
                IDs = new int[count];
                IDs = VestingRules.Select(s => s.Id).ToArray();

                int[] myInts = IDs;
                Array.Sort(myInts);
                IDs = myInts.Select(x => x).ToArray();

                return View(VestingRules.ToPagedList(pageNumber, pageSize));
            }
        }

        [AuthorizedRights(Screen = "VestingRule", Right = "Read")]
        public ActionResult Details(int id)
        {
            VestingRuleDetailsPageViewModel model = new VestingRuleDetailsPageViewModel();
            model.DeleteBtn = true;
            model.EditBtn = true;
            var VestingRule = dbContext.VestingRules.Include("VestingRuleDetails").SingleOrDefault(p => p.Id == id);
            var CurrentUser = User.Identity.GetUserId();
            if (VestingRule.Chk == false && VestingRule.Maker != CurrentUser)
            {
                model.CheckBtn = true;
            }
            if (VestingRule.Chk == true && VestingRule.Checker != null && VestingRule.Auth == false && VestingRule.Checker != CurrentUser)
            {
                model.AuthBtn = true;
            }
            if (VestingRule.Chk == true && VestingRule.Checker != null && VestingRule.Auth == true && VestingRule.Auther != null && VestingRule.Checker != CurrentUser)
            {
                model.UnAuthBtn = true;
            }
            if (VestingRule.Auth == true)
            {
                model.DeleteBtn = false;
                model.EditBtn = false;
            }


            model.Id = VestingRule.Id;
            model.Funds = dbContext.Funds.Where(f => f.Auth).ToList();
            model.Policies = dbContext.Policy.Where(p => p.Auth).ToList();
            model.Base = (VestingRuleBase)VestingRule.Base;
            model.FundId = VestingRule.FundId.Value;
            model.PolicyId = VestingRule.PolicyId;
             model.TransactionType = (VestingRuleTransactionType)VestingRule.TransactionType;
             model.VestingRuleDetails = VestingRule.VestingRuleDetails.Select(vr => new VestingRuleDetailsViewModel()
                {

                    FromYear = (ushort)vr.FromYear,
                    ToYear = (ushort)vr.ToYear,
                    PercentageOfCompanyShare = vr.PercentageOfCompanyShare,
                    PercentageOfCompanyShareBooster = vr.PercentageOfCompanyShareBooster,
                    PercentageOfEmpShareBooster = vr.PercentageOfEmpShareBooster,
                    PercentageOfEmpShare = vr.PercentageOfEmpShare
                }).ToList();
            

            if (IDs != null)
            {
                if (model.Id == IDs.Last())
                {
                    TempData["Last"] = "Last";
                }
                if (model.Id == IDs.First())
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

        [AuthorizedRights(Screen = "VestingRule", Right = "Read")]
        public ActionResult Next(int id)
        {
            var arrlenth = IDs.Count();
            var LastObj = dbContext.VestingRules.OrderBy(s => s.Id).ToList().LastOrDefault(s => s.Id == IDs[arrlenth - 1]);
            if (id == LastObj.Id)
            {
                TempData["Last"] = "Last";
                return RedirectToAction("Details", new { id = LastObj.Id });
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
                var Code = dbContext.VestingRules.Where(u => u.Id == id).Select(s => s.Id).FirstOrDefault();
                if (Code == LastObj.Id)
                {
                    TempData["Last"] = "Last";
                }
                else
                {
                    TempData["Last"] = "NotLast";
                }

                return RedirectToAction("Details", new { id = Code });
            }

        }

        [AuthorizedRights(Screen = "VestingRule", Right = "Read")]
        public ActionResult Previous(int id)
        {
            var arrindex = IDs[0];
            var FirstObj = dbContext.VestingRules.OrderBy(s => s.Id).FirstOrDefault(s => s.Id == arrindex);
            if (id == FirstObj.Id)
            {
                TempData["First"] = "First";
                return RedirectToAction("Details", new { id = FirstObj.Id });
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

                if (id == FirstObj.Id)
                {
                    TempData["First"] = "First";
                }
                else
                {
                    TempData["First"] = "NotFirst";
                }
                var Code = dbContext.Policy.Where(u => u.Id == id).Select(s => s.PolicyNo).FirstOrDefault();
                return RedirectToAction("Details", new { id = Code });
            }

        }



        
             [AuthorizedRights(Screen = "VestingRule", Right = "Authorized")]
        public ActionResult Authorize(int id)
        {
            var VestingRule = dbContext.VestingRules.Where(f => f.Id == id).FirstOrDefault();
            VestingRule.Auth = true;
            VestingRule.Auther = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }

        [AuthorizedRights(Screen = "VestingRule", Right = "Check")]
        public ActionResult Check(int id)
        {
            var VestingRule = dbContext.VestingRules.Where(f => f.Id == id).FirstOrDefault();
            VestingRule.Chk = true;
            VestingRule.Checker = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { id = id });

        }


        [AuthorizedRights(Screen = "VestingRule", Right = "Delete")]
        public ActionResult Delete(int id)
        {
            var VestingRule = dbContext.VestingRules.Where(f => f.Id == id).FirstOrDefault();
            VestingRule.DeletFlag = DeleteFlag.Deleted;
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        private object checkIntervals(IEnumerable<dynamic> intervals)
        {
            int i = 0;
            List<ushort> Intervals = new List<ushort>();
            foreach(var year in intervals)
            {
                if(i == 0)
                {
                    if(year.ToYear<= year.FromYear)
                      return new { isValid = false, index = i };
                    else
                    {
                        Intervals.Add(year.FromYear);
                        Intervals.Add(year.ToYear);
                    }
                }
                else
                {
                    if (year.ToYear <= year.FromYear)
                     return new { isValid = false, index = i };
                    else
                    {
                        if(year.FromYear == Intervals.Max())
                        {
                            Intervals.Add(year.FromYear);
                            Intervals.Add(year.ToYear);
                        }
                        else
                         return new { isValid = false, index = i };
                    }
                }
                i++;
            }
            return new { isValid = true };

        }

        public JsonResult GetFundsByPolicy(int id)
        {
            return Json(new { Funds = dbContext.Policy.Include(p => p.AllocationRules).Include(c => c.AllocationRules.Select(f => f.Fund)).SingleOrDefault(p => p.Id == id).AllocationRules.Select(ar => new { FundID = ar.Fund.FundID, Name = ar.Fund.Name }).ToList() }, JsonRequestBehavior.AllowGet);
        }
    }
}