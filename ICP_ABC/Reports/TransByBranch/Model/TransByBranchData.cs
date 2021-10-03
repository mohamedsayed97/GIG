using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICP_ABC.Reports.TransByBranch.Model
{
    public class TransByBranchData
    {
        public string Fname { get; set; }
        public string Price { get; set; }
        public string ename { get; set; }
        public string transid { get; set; }
        public string cust_id { get; set; }
        public double quantity { get; set; }
        public DateTime value_date { get; set; }
        public double navToday { get; set; }
        public string inputer { get; set; }
        public string auther { get; set; }
        public string bname { get; set; }
        public string branch_id { get; set; }
    }
}