using ICP_ABC.Areas.Funds.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Policies.Models
{
    public class PolicyViewModel
    {
        [Required]
        public string PolicyNo { get; set; }

        public List<Fund> Funds { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public List<ICP_ABC.Areas.Company.Models.Company> Companies { get; set; }

        [Required]
        public string PolicyHolderName { get; set; }

        [Required]
        public byte PaymentFrequency { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime EffectiveDate { get; set; }

        [Required]
        public byte Status { get; set; }

        [Required]
        public byte CalculationBasis { get; set; }

        [Required]
        public byte BusinessChannel { get; set; }

        [Required]
        public bool HasWithdrawal { get; set; }
        [Required]
        public bool HasBooster { get; set; }
        public List<AllocationRuleViewModel> AllocationRules { get; set; }
    }


    public class AllocationRuleViewModel
    {
        [Required]
        public int FundId { get; set; }
        public Fund Fund { get; set; }

        [Required(ErrorMessage ="Required")]
        [Range(minimum:0,maximum:100,ErrorMessage ="0 - 100")]
        public byte PercentageOfEmpShare { get; set; }
        [Required(ErrorMessage = "Required")]
        [Range(minimum: 0, maximum: 100, ErrorMessage = "0 - 100")]
        public byte PercentageOfCompanyShare { get; set; }
        [Required(ErrorMessage = "Required")]
        [Range(minimum: 0, maximum: 100, ErrorMessage = "0 - 100")]
        public byte PercentageOfEmpShareBooster { get; set; }
        [Required(ErrorMessage = "Required")]
        [Range(minimum: 0, maximum: 100, ErrorMessage = "0 - 100")]
        public byte PercentageOfCompanyShareBooster { get; set; }
    }

    public class PolicyDetailsViewModel
    {
       
        public int CompanyId { get; set; }
        

        public int code { get; set; }

        public string PolicyNo { get; set; }

       
        public string PolicyHolderName { get; set; }

       
        public byte PaymentFrequency { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
     
        public DateTime EffectiveDate { get; set; }

      
        public byte Status { get; set; }

    
        public byte CalculationBasis { get; set; }

        
        public byte BusinessChannel { get; set; }
      
        public List<ICP_ABC.Areas.Company.Models.Company> Companies { get; set; }
        public List<Fund> Funds { get; set; }

        public bool HasWithdrawal { get; set; }
        
        public bool HasBooster { get; set; }
        public List<AllocationRuleViewModel> AllocationRules { get; set; }

        public bool CheckBtn { get; set; }
        public bool AuthBtn { get; set; }
        public bool UnAuthBtn { get; set; }
        public bool EditBtn { get; set; }
        public bool DeleteBtn { get; set; }
    }
}