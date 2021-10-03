using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace ICP_ABC.Reports.Rpt_CLientMapV8.Model
{
    public class Rpt_CLientMapV8VM
    {
        [Display(Name = "From Date")]
        [Required]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required]
        public DateTime? ToDate { get; set; }
    }
}