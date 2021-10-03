using ICP_ABC.Areas.Customers.Models;
using ICP_ABC.Areas.Policies.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Subscriptions.Models
{
    public class Trans
    {
        [Key]
        public int code  { get; set; }
       
        public string cust_id   { get; set; }
        [StringLength(30)]
        public string cust_acc_no  { get; set; }

        public int fund_id { get; set; }
        [Required]
        public int PolicyId { get; set; }
        public virtual Policy Policy { get; set; }

        public int resigned_id { get; set; }

        public DateTime? value_date  { get; set; }

        public DateTime? entry_date { get; set; }

        public decimal quantity   { get; set; }

        public decimal unit_price   { get; set; }

        public short  pur_sal  { get; set; }

        public int branch_id { get; set; }

        public short payment_met  { get; set; }

        public DateTime? time_stamp  { get; set; }

        public int user_id { get; set; }

        public short auth  { get; set; }

        public int auther { get; set; }

        public decimal fees   { get; set; }

        [Range(0, 9999999999999999.99)]
       
        public decimal? other_fees { get; set; }

        public string inputer  { get; set; }

        public short flag_tr  { get; set; }

        public int curr_id { get; set; }

        public int transid { get; set; }

        public decimal mark_fees   { get; set; }

        public decimal admin_fees   { get; set; }

        public decimal early_fees   { get; set; }

        public DateTime? system_date  { get; set; }

        public decimal upfront_fees   { get; set; }

        public decimal total_value   { get; set; }
        public DateTime SysDate { get; set; } = DateTime.Now;
        public int Flag { get; set; }
        
        public string UserID { get; set; }

        public string CustomerID { get; set; }
        public Customer Customer { get; set; }

        public DateTime? Nav_Ddate { get; set; }
        public DateTime? ProcessingDate { get; set; }
        public short? GTF_Flag { get; set; }
        public int? ExcelId { get; set; }

    }
}