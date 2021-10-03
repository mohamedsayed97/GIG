using ICP_ABC.Areas.Branches.Models;
using ICP_ABC.Areas.Customers.Models;
using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Areas.ICPrices.Models;
using ICP_ABC.Areas.Policies.Models;
using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Subscriptions.Models
{
    [Table("Subscription")]
    public class Subscription
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int code { get; set; }
        [ForeignKey("Branch")]
        public int? branch_id{ get; set; }
        [MaxLength(30)]
	    public string cust_acc_no{ get; set; }        
        [ForeignKey("Customer")]
    
        public string cust_id { get; set; }
        [ForeignKey("Fund")]
        public int? fund_id{ get; set; }

      
        public int? appliction_no { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? pur_date{ get; set; }
        public decimal?  units{ get; set; }
   
        [Range(0, 99999999999.99999)]
        public decimal? amount_3{ get; set; }
        //[RegularExpression(@"^\d+\.\d{0,5}$")]
        [Range(0, 9999999999999999.99)]
        public decimal? sub_fees{ get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]

        public DateTime? nav_date{ get; set; }
        public DateTime? Nav_Ddate{ get; set; }
        public DateTime? ProcessingDate{ get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]

        public DateTime? system_date{ get; set; }        
        //[ForeignKey("ApplicationUser")]
        public string inputer { get; set; }
	    public short?  auth{ get; set; }
       // [ForeignKey("ApplicationUser")]
        public string auther{ get; set; }
	    public short?  flag_tr { get; set; }
        [MaxLength(10)]
        public char? time_stamp{ get; set; }
	    public short? pay_method{ get; set; }
        [MaxLength(50)]
        public string pay_no{ get; set; }
        [MaxLength(50)]
        public string pay_bank{ get; set; }
        [Range(0, 9999999999999999999999.99)]
        //[RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal total { get; set; }
        [Range(0, 9999999999999999.99)]
        
        public decimal NAV { get; set; }
        [Range(0, 9999999999999999.99)]
        //[RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal? other_fees { get; set; }
        public DeleteFlag delreason { get; set; } = DeleteFlag.NotDeleted;
        public int? unauth{ get; set; }
        public int? Flag{ get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }
        public bool? Chk { get; set; }
        public string Checker { get; set; }
        public DateTime SysDate { get; set; } = DateTime.Now;
        public virtual Customer Customer { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Branch Branch { get; set; }
        //public virtual ICPrice ICPrice { get; set; }
        public virtual Fund Fund { get; set; }
        [Required]
        public int PolicyId { get; set; }
        public virtual Policy Policy { get; set; }

        public short? GTF_Flag { get; set; }

        public int? ExcelId { get; set; }

    }

    

    static class accountType
    {
        public const string ZeroAccount = "EID202101";
        public const string FundSideAccount = "EID202102";
    }

}