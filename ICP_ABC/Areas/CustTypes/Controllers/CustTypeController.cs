using ICP_ABC.Models;
using ICP_ABC.Areas.CustTypes.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;

namespace ICP_ABC.Areas.CustTypes.Controllers
{
    public class CustTypeController : Controller
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext dbContext = new ApplicationDbContext();
        //static string[] IDs;

        public CustTypeController() { }


        public CustTypeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

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

        // GET: Group
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var lastcode = dbContext.Nationalities.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault();

            var trieng = int.TryParse(lastcode, out int Code);
            if (trieng)
            {
                ViewData["LastCode"] = Code + 1;
            }

            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {

                CustType custType  = new CustType
                {
                    Code = model.Code,
                    Name = model.Name,
                    Maker = User.Identity.GetUserId(), //dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                    UserID = User.Identity.GetUserId()// dbContext.Users.Where(s => s.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                };
                dbContext.CustTypes.Add(custType);
                dbContext.SaveChanges();
            }
            else
            {
                return View(model);
            }
            return View();
        }

        public ActionResult Edit(int id)
        {
            return View(dbContext.CustTypes
                  .Where(g => g.CustTypeID == id)
                  .FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Edit(EditViewModel model)
        {
            var currentCustType = dbContext.Nationalities.Where(g => g.Code == model.Code).FirstOrDefault();
            currentCustType.Name = model.Name;
            currentCustType.SysDate = DateTime.Now;
            currentCustType.UserID = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return View();
        }

        public ViewResult Search(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Code" ? "code_desc" : "Code";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var CustTypes = from s in dbContext.CustTypes
                                select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                CustTypes = CustTypes.Where(s => s.Name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    CustTypes = CustTypes.OrderByDescending(s => s.Name);
                    break;
                case "Code":
                    CustTypes = CustTypes.OrderBy(s => s.Code);
                    break;
                case "code_desc":
                    CustTypes = CustTypes.OrderByDescending(s => s.Code);
                    break;
                default:  // Name ascending 
                    CustTypes = CustTypes.OrderBy(s => s.CustTypeID);
                    break;
            }
            //int count = cities.ToList().Count();
            //IDs = new string[count];
            //IDs = cities.Select(s => s.Code).ToArray();

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(CustTypes.ToPagedList(pageNumber, pageSize));
        }

        //public ViewResult Details(string Code)
        //{
        //    City city = new City();
        //    city = dbContext.Cities.Find(Code);
        //    var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
        //    var manager = new UserManager<ApplicationUser>(store);
        //    DetailsViewModel model = new DetailsViewModel
        //    {
        //        UserName = appUser.UserName,
        //        Code = appUser.Code,
        //        BranchRight = appUser.BranchRight,
        //        SysDate = appUser.SysDate,
        //        ExpireDate = appUser.ExpireDate,
        //        Maker = dbContext.Users.Where(u => u.Id == appUser.Maker).Select(s => s.UserName).FirstOrDefault(),
        //        LastEditor = dbContext.Users.Where(u => u.UserId == appUser.UserId).Select(s => s.UserName).FirstOrDefault()

        //    };
        //    ViewData["Groups"] = new SelectList(dbContext.UserGroups.ToList(), "GroupID", "Name", appUser.GroupId);
        //    ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "BranchID", "Name", appUser.BranchId);
        //    ViewData["Positions"] = new SelectList(dbContext.Positions.ToList(), "PositionID", "Name", appUser.PositionId);

        //    return View(model);
        //}

        //public ActionResult Next(string id)
        //{
        //    var arrlenth = IDs.Count();
        //    var LastObj = dbContext.Users.OrderBy(s => s.Code).ToList().LastOrDefault(s => s.Code == IDs[arrlenth - 1]);
        //    if (id == LastObj.Code)
        //    {
        //        TempData["Last"] = "Last";
        //        return RedirectToAction("Details", new { UserName = LastObj.UserName });
        //    }
        //    else
        //    {
        //        int counter = 0;
        //        int next = 0;
        //        foreach (var item in IDs)
        //        {

        //            if (item == id)
        //            {
        //                next = counter + 1;

        //            }
        //            counter++;
        //        }
        //        id = IDs[next];
        //        var UserName = dbContext.Users.Where(u => u.Code == id).Select(s => s.UserName).FirstOrDefault();

        //        TempData["Last"] = "NotLast";
        //        TempData["First"] = "NotFirst";
        //        return RedirectToAction("Details", new { UserName = UserName });
        //    }

        //}

        //public ActionResult Previous(string id)
        //{
        //    var arrindex = IDs[0];
        //    var FirstObj = dbContext.Cities.OrderBy(s => s.Code).FirstOrDefault(s => s.Code == arrindex);
        //    if (id == FirstObj.Code)
        //    {
        //        TempData["First"] = "First";
        //        return RedirectToAction("Details", new { UserName = FirstObj.Name });
        //    }
        //    else
        //    {
        //        int counter = 0;
        //        int previous = 0;
        //        foreach (var item in IDs)
        //        {

        //            if (item == id)
        //            {
        //                previous = counter - 1;

        //            }
        //            counter++;
        //        }
        //        id = IDs[previous];
        //        TempData["First"] = "NotFirst";
        //        var UserName = dbContext.Users.Where(u => u.Code == id).Select(s => s.UserName).FirstOrDefault();
        //        return RedirectToAction("Details", new { UserName = UserName });
        //    }

        //}
    }
}