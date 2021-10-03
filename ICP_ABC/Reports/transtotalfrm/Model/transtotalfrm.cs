using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Areas.Branches.Models;

namespace ICP_ABC.Reports.transtotalfrm.Model
{
    public class transtotalfrmVM
    {
        public List<Fund> Funds { get; set; }
        public int? Fund { get; set; }
        public List<Branch> Branches { get; set; }
        public int? Branche { get; set; }
        [Display(Name = "From Date")]
        [Required]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required]
        public DateTime? ToDate { get; set; }
        public string Auth { get; set; }
        public string TransactionOver { get; set; }
    }
}