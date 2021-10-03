using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICP_ABC.Reports.frmDeleteSub_Red.Model
{
    public class frmDeleteSub_RedVM
    {
        public string Code { get; set; }

        public string ClientType { get; set; }

        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }

        public List<Sub_RedType> Types { get; set; }
        public string Type { get; set; }

        [Display(Name = "From Date")]
        [Required]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required]
        public DateTime? ToDate { get; set; }
    }
    public class Sub_RedType
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}