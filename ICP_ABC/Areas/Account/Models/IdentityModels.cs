using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using ICP_ABC.Areas.Branches.Models;
using ICP_ABC.Areas.Cities.Models;
using ICP_ABC.Areas.Group.Models;
using ICP_ABC.Areas.GroupsRights.Models;
using ICP_ABC.Areas.Nationalities.Models;
using ICP_ABC.Models.DBSetup;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ICP_ABC.Areas.Currencies.Models;
using ICP_ABC.Areas.Sponsors.Models;
using ICP_ABC.Areas.CustTypes.Models;
using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Areas.UsersSecurity.Models;
using System.Collections.Generic;
using ICP_ABC.Areas.FundRights.Models;
using ICP_ABC.Areas.FundTimes.Models;
using ICP_ABC.Areas.ICPrices.Models;
using ICP_ABC.Areas.Customers.Models;
using ICP_ABC.Areas.Subscriptions.Models;
using ICP_ABC.Areas.Redemptions.Models;
using ICP_ABC.Areas.Blocks.Models;
using ICP_ABC.Areas.Clients.Models;
using ICP_ABC.Areas.Calendars.Models;
using ICP_ABC.Areas.Account.Models;
using ICP_ABC.Areas.Policies.Models;
using ICP_ABC.Areas.VestingRules.Models;
using ICP_ABC.Areas.Company.Models;
using ICP_ABC.Areas.ExcelAuthorization.Models;

namespace ICP_ABC.Models
{
    public enum DeleteFlag
    {
        NotDeleted = -1,
        Deleted = 0
        
    }
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        //[Key]
        //public string UserId { get; set; }
        [Required]
        [StringLength(4)]
        [Index(IsUnique = true)]
        public string Code { get; set; }
        [StringLength(50)]
        [Required]
        public override string UserName { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        //[Required]
        //[MinLength(8)]
        // public string Password { get; set; }

        [Required]
        //[DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        public DateTime ExpireDate { get; set; }

        [Required]
        [ForeignKey("Branch")]
        public int BranchId { get; set; }

        [Required]
        [ForeignKey("UserGroup")]
        public int GroupId { get; set; }

        //[Required]
        //[MaxLength(100)]
        //public string FullName { get; set; }

        public bool IsAdmin { get; set; }

        [ForeignKey("Title")]
        public int TitleId { get; set; }

        public bool BranchRight { get; set; }
        public bool UnBlockRight { get; set; }

        public bool IsLocked { get; set; }

        public string defaultHashedPassword { get; set; }

        [Required]
        public bool EditFlag { get; set; }

        public DeleteFlag DeleteFlag { get; set; } =DeleteFlag.NotDeleted;

        public string Maker { get; set; }

        [DefaultValue(0)]
        public bool Chk { get; set; }

        public string Checker { get; set; }

        [DefaultValue(0)]
        public bool Auth { get; set; }

        //public bool DarkMode { get; set; }
        public bool IsLogged { get; set; }
        public string Auther { get; set; }

        [ForeignKey("applicationUser")]
        public string UserId { get; set; }

        public DateTime CreationDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime SysDate { get; set; } = DateTime.Now;

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CloseDueDate { get; set; }

        public virtual ApplicationUser  applicationUser { get; set; }
        
        public virtual UserGroup UserGroup { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual Title Title { get; set; }

        public virtual List<GroupRight> Rights { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("Rights", this.Rights.ToString()));
            return userIdentity;
        }



        
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }


        public virtual DbSet<Calendar> Calendars { get; set; }
        public virtual DbSet<Bank> Bank { get; set; }
        public virtual DbSet<FundDay> FundDay { get; set; }
        public virtual DbSet<Day> Day { get; set; }
        public virtual DbSet<FundAuthDay> FundAuthDay { get; set; }
        //public new virtual DbSet<User> Users { set; get; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<Sponsor> Sponsors { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<Branch> Branches { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Nationality> Nationalities { get; set; }
        public virtual DbSet<CustType>  CustTypes { get; set; }
        public virtual DbSet<Title> Titles { get; set; }
        public virtual DbSet<UserSecurity> UserSecurities { get; set; }
        public virtual DbSet<GroupRight> GroupRights { get; set; }
        public virtual DbSet<Screen> Screens { get; set; }
        public virtual DbSet<Fund> Funds { get; set; }
        public virtual DbSet<FundRight> FundRights { get; set; }
        public virtual DbSet<ICPrice> ICPrices { get; set; }
        public virtual DbSet<FundTime> FundTimes { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<UserIdentityType>   UserIdentityTypes { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }
        public virtual DbSet<Redemption> Redemptions { get; set; }
        public virtual DbSet<LastCode> LastCodes { get; set; }
        public virtual DbSet<Trans> Trans { get; set; }
        public virtual DbSet<Block> Block { get; set; }
        public virtual DbSet<blocktable> blocktable { get; set; }
        public virtual DbSet<Policy> Policy { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<AllocationRule> AllocationRules { get; set; }
        public virtual DbSet<VestingRule> VestingRules { get; set; }
        public virtual DbSet<VestingRuleDetails> VestingRuleDetails { get; set; }
        public virtual DbSet<Client_CodeMap> Client_CodeMap { get; set; }
        public virtual DbSet<Client_CodeMap_LOG> Client_CodeMap_Log { get; set; }
        public virtual DbSet<EmployeePolicy> EmployeePolicies { get; set; }
        public virtual DbSet<ExcelDetails> ExcelDetails { get; set; }
        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>().Property(o => o.sub_fees).HasPrecision(18, 5);
            modelBuilder.Entity<Subscription>().Property(o => o.total).HasPrecision(25, 2);
            modelBuilder.Entity<Subscription>().Property(o => o.other_fees).HasPrecision(18, 5);
            modelBuilder.Entity<Subscription>().Property(o => o.NAV).HasPrecision(18, 5);
            modelBuilder.Entity<ICPrice>().Property(o => o.Price).HasPrecision(18, 5);
            modelBuilder.Entity<Redemption>().Property(o => o.units).HasPrecision(18, 5);
            modelBuilder.Entity<Redemption>().Property(o => o.sub_fees).HasPrecision(18, 5);
            modelBuilder.Entity<Redemption>().Property(o => o.amount_3).HasPrecision(18, 2);
            modelBuilder.Entity<Redemption>().Property(o => o.total).HasPrecision(18, 2);
            modelBuilder.Entity<Redemption>().Property(o => o.NAV).HasPrecision(18, 5);
            modelBuilder.Entity<Redemption>().Property(o => o.other_fees).HasPrecision(18, 5);
           


            base.OnModelCreating(modelBuilder);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

       
    }
}