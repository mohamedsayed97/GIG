using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICP_ABC.Reports.TransactionByFund.Model
{
    public class TransactionByFundData
    {
        public string Fname { get; set; }
        public string Price { get; set; }
        public string type { get; set; }
        public string BankName { get; set; }
        public string transid { get; set; }
        public string ename { get; set; }
        public string cust_id { get; set; }
        public int quantity { get; set; }
        public string total_value { get; set; }
        public double navToday { get; set; }
        public string inputer { get; set; }
        public string auther { get; set; }
        public string value_date { get; set; }
        public string bname { get; set; }


    }
}