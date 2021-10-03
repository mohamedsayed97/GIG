using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Areas.Policies.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.VestingRules.Models
{
    public class VestingRuleViewModel
    {

        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "The Fund field is required.")]
        public int FundId { get; set; }
        public List<Fund> Funds { get; set; }

       
        [Range(1, int.MaxValue, ErrorMessage = "The Policy field is required.")]
        public int PolicyId { get; set; }
        public List<Policy> Policies { get; set; }

   
       
        [Range(1, int.MaxValue, ErrorMessage = "The TransactionType field is required.")]
        public VestingRuleTransactionType TransactionType { get; set; }

        
        [Range(1, int.MaxValue, ErrorMessage = "The Base field is required.")]
        public VestingRuleBase Base { get; set; }

        [Required(ErrorMessage = "Required")]
        public List<VestingRuleDetailsViewModel> VestingRuleDetails { get; set; }
    }
    public class VestingRuleDetailsViewModel
    {

        [Required(ErrorMessage = "Required")]
        [Range(minimum: 0, maximum: Double.MaxValue, ErrorMessage = "0 - 100")]
        public ushort FromYear { get; set; }

        [Required(ErrorMessage = "Required")]
        [Range(minimum: 0, maximum: Double.MaxValue, ErrorMessage = "0 - 100")]
        public ushort ToYear { get; set; }

        [Required(ErrorMessage = "Required")]
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
    public class VestingRuleDetailsPageViewModel
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Required")]
        public int FundId { get; set; }
        public List<Fund> Funds { get; set; }

        [Required(ErrorMessage = "Required")]
        public int PolicyId { get; set; }
        public List<Policy> Policies { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "The TransactionType field is required.")]
        public VestingRuleTransactionType TransactionType { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "The Base field is required.")]
        public VestingRuleBase Base { get; set; }
        [Required(ErrorMessage = "Required")]
        public List<VestingRuleDetailsViewModel> VestingRuleDetails { get; set; }

        public bool CheckBtn { get; set; }
        public bool AuthBtn { get; set; }
        public bool UnAuthBtn { get; set; }
        public bool EditBtn { get; set; }
        public bool DeleteBtn { get; set; }
    }
    public enum VestingRuleTransactionType
    {
        Surrender = 1,
        Withdrawal = 2
    }
    public enum VestingRuleBase
    {
        Joining = 1,
        Hiring = 2
    }
}