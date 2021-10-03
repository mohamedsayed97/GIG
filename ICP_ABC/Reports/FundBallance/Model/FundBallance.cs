using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Areas.Funds.Models;

namespace ICP_ABC.Reports.FundBallance.Model
{
    public class FundBallanceVM
    {
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
        List<SelectListItem> allItems = new List<SelectListItem>();
    }
    public class FundTypes
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class FundBallanceData
    {
        public string fund_id { get; set; }
        public string fname { get; set; }
        public string units { get; set; }
        public string navDate { get; set; }
        public string totalPos { get; set; }
    }


}