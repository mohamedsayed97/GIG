using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICP_ABC.Reports.fundposfrm.Model
{
    public class fundposfrmData
    {
        public string fname { get; set; }
        public string price { get; set; }
        public string bname { get; set; }
        public string branch_id { get; set; }
        public string cust_id { get; set; }
        public string ename { get; set; }
        public double total_sub { get; set; }
        public double total_red { get; set; }
        public double noshares { get; set; }
        public double natoday { get; set; }
    }
}