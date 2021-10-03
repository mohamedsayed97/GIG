using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Reports.FundBallance.Model;

namespace ICP_ABC.Reports.Balancerptfrm.Model
{
    public class BalancerptfrmVM
    {
        public string CustomerCode { get; set; }
        [Required]
        public string Code { get; set; }
        public List<Fund> Funds { get; set; }
        public List<FundTypes> Types { get; set; }
        [Display(Name = "From Date")]
        [Required]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required]
        public DateTime? ToDate { get; set; }

        public int? Fund { get; set; }
        public int FundType { get; set; }
    }
}