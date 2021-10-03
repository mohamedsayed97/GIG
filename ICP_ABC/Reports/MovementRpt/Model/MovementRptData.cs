using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICP_ABC.Reports.MovementRpt.Model
{
    public class MovementRptData
    {
        public string aname { get; set; }
        public string aaddress1 { get; set; }
        public string Bname { get; set; }
        public string cust_id { get; set; }
        public string short_name { get; set; }
        public string nav_today { get; set; }
        public string fname { get; set; }
        public string price { get; set; }
        public string unit_price { get; set; }
        public string qty { get; set; }
        public string tot { get; set; }
        public string navToday { get; set; }
        public DateTime value_date { get; set; }
    }
}