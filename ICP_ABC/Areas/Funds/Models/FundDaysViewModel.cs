using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Funds.Models
{
    public class FundDaysViewModel
    {
        public CreateFundViewModel ThisFund { get; set; }

        public IEnumerable<FundDay> FundDayForSub { get; set; }
        public IEnumerable<FundDay> FundDayForRed { get; set; }
        public IEnumerable<FundAuthDay> FundAuthDaySub { get; set; }
        public IEnumerable<FundAuthDay> FundAuthDayRed { get; set; }
    }
}