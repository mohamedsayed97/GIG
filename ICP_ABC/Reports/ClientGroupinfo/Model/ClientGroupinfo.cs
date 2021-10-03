using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ICP_ABC.Reports.ClientGroupinfo.Model
{
    public class ClientGroupinfoVM
    {
        public string CustomerCode { get; set; }
        //[Required]
        public string Code { get; set; }

        [Display(Name = "From Date")]
        [Required]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required]
        public DateTime? ToDate { get; set; }


    }
}