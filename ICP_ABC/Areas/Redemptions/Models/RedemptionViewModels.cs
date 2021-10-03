using ICP_ABC.Areas.Branches.Models;
using ICP_ABC.Areas.Customers.Models;
using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Areas.ICPrices.Models;
using ICP_ABC.Areas.Subscriptions.Models;
using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Redemptions.Models
{
    public class CreateRedemptionViewModel
    {
        [Key]
        public int code { get; set; }

        public string BranchName { get; set; }
        public int? branch_id { get; set; }

        public DateTime? ProcessingDate { get; set; }
        public string cust_acc_no { get; set; }
      
        [Required]
        
        [Display(Name ="Customer ID")]
        public string cust_id { get; set; }
        [Required]
        public int fund_id { get; set; }
        public string system_date { get; set; }
      
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Application Number")]
        public int? appliction_no { get; set; }
        public DateTime? pur_date { get; set; }
        //[Range(0, 99999999999.99999)]
        //[Required(ErrorMessage ="NO. Of units is required")]
       //[MinLength(int.MaxValue, ErrorMessage ="minnnnnnnn")]
       //[MaxLength(int.MaxValue,ErrorMessage ="maxxxxxx")]
       // [RegularExpression(@"^\d+\.\d{0,5}$",ErrorMessage ="Units Is bigger than or smaller than the range")]
        public decimal? units { get; set; }
        //[RegularExpression(@"^\d+\.\d{0,2}$")]
        //[Range(0, 99999999999.99999)]
        [Display(Name = "Total Unit Price")]
        public decimal? amount_3 { get; set; }
      
        [Display(Name = "Red Fees")]
        public decimal? sub_fees { get; set; }
        public DateTime? nav_date { get; set; }  
        public DateTime? Nav_Ddate { get; set; }  
        [StringLength(20)]
        public string pay_method { get; set; }
        [MaxLength(50)]
        public string pay_no { get; set; }
        [MaxLength(50)]
        public string pay_bank { get; set; }
        [Range(0, 99999999999999.99)]
        //[RegularExpression(@"^\d+\d{0,2}$")]
        public decimal total { get; set; }
        [Range(0, 9999999999999.99)]
        //[RegularExpression(@"^\d+\d{0,2}$")]
        [Required]
        public decimal NAV { get; set; }
        [Range(0, 9999999999.99)]
        [RegularExpression(@"^\d{0,17}(\.\d{1,2})?$")]
        public decimal? other_fees { get; set; }

        public bool CheckBtn { get; set; }
        public bool AuthBtn { get; set; }
        public bool UnAuthBtn { get; set; }
        public bool EditBtn { get; set; }
        public bool DeleteBtn { get; set; }

        public AccountType AccountType { get; set; }
    }

    public class SearchRedViewModel
    {
        
        //public string sortOrder { get; set; }
        //public string currentFilter { get; set; }
        public string AccountNo { get; set; }
        //public int? page { get; set; }
        public string CustomerId { get; set; }
        public string NavDateFrom { get; set; }
        public string NavDateTo { get; set; }
        public string Funds { get; set; }
        public string CodeFrom { get; set; }
        public string CodeTo { get; set; }
        public string BranchId { get; set; }
        public string TotalAmountFrom { get; set; }
        public string TotalAmountTo { get; set; }
        public string NumberOfUnits { get; set; }
        public string Authorize { get; set; }



    }
}