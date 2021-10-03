using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICP_ABC.Reports.branchAverage.Model
{
    public class branchAverageData
    {
        public string cbo_type { get; set; }
        public string Fname { get; set; }
        public string Price { get; set; }
        public string branch_id { get; set; }
        public string BName { get; set; }
        public double total_sub { get; set; }
        public double total_red { get; set; }
        public double tot { get; set; }
        public double nav { get; set; }
        public double AVG { get; set; }

    }
}