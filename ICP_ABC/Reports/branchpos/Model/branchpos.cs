using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Areas.Branches.Models;
using System.ComponentModel.DataAnnotations;
namespace ICP_ABC.Reports.branchpos.Model
{
    public class branchposVM
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
    }
}