using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ICP_ABC.Areas.Funds.Models;

namespace ICP_ABC.Reports.blockreport.Model
{
    public class blockreportVM
    {
        public string CustomerCode { get; set; }
        [Required]
        public string Code { get; set; }
        public List<Fund> Funds { get; set; }
        public int? Fund { get; set; }
    }
}