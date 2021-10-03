using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Redemptions.Models
{
    public class withdrawalExelHelper
    {
        public string EmployeeId { get; set; }
              
        public string FundId { get; set; }
 
        public decimal PercentageOfEmpShare { get; set; }
        
        public decimal PercentageOfCompanyShare { get; set; }
        
        public decimal PercentageOfEmpShareBooster { get; set; }
        
        public decimal PercentageOfCompanyShareBooster { get; set; }
    }
}