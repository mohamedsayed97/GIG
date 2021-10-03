using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Areas.Funds.Models;


namespace ICP_ABC.Reports.FundType.Model
{
    public class FundTypeVM
    {
        public List<Fund> Funds { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        [Display(Name = "From Date")]
        [Required]
        public DateTime? FromDate { get; set; }
        public int Fund { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        [Display(Name = "To Date")]
        [Required]
        public DateTime? ToDate { get; set; }
    

    }
   




}