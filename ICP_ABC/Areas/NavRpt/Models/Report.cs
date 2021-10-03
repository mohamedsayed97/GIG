using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.NavRpt.Models
{
    public class Report
    {
        public int FundId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}