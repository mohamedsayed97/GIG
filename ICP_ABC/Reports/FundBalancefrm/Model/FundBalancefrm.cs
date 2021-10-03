using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ICP_ABC.Areas.Funds.Models;
namespace ICP_ABC.Reports.FundBalancefrm.Model
{
    public class FundBalancefrmVM
    {
        public List<Fund> Funds { get; set; }

        [Display(Name = "From Date")]
        [Required]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required]
        public DateTime? ToDate { get; set; }

        public int? Fund { get; set; }
    }
}