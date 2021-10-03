using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICP_ABC.Reports._UsersRpt.Model
{
    public class _UsersRptData
    {
        public string code { get; set; }
        public string name { get; set; }
        public string GroupName { get; set; }
        public string BranchName { get; set; }
        public string name2 { get; set; }
        public string GroupName2 { get; set; }
        public string BranchName2 { get; set; }
        public string Position { get; set; }
        public string Type { get; set; }
        public string stat { get; set; }
        public DateTime exp_date { get; set; }
        public DateTime System_date { get; set; }
    }
}