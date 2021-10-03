using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Areas.Policies.Models;
using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.VestingRules.Models
{
    public class VestingRule
    {
        [Key , DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

       
        public int? FundId { get; set; }
        public Fund Fund { get; set; }

        [Required]
        public int PolicyId { get; set; }
        public Policy Policy { get; set; }

        [Required]
        public byte TransactionType { get; set; }

        [Required]
        public byte Base { get; set; }

        [Required]
        public string Maker { get; set; }

        public bool Chk { get; set; }
        public string Checker { get; set; }

        public bool Auth { get; set; }
        public string Auther { get; set; }
        public DeleteFlag DeletFlag { get; set; } = DeleteFlag.NotDeleted;
        public DateTime SysDate { get; set; } = DateTime.Now;

        public ICollection<VestingRuleDetails> VestingRuleDetails { get; set; }
    }

    public class VestingRuleDetails
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int VestingRuleId { get; set; }
        public VestingRule VestingRule { get; set; }

        
        [Required]
        [Column(TypeName ="int")]
        public short FromYear { get; set; }
        [Required]
        [Column(TypeName = "int")]
        public short ToYear { get; set; }

        [Required]
        public byte PercentageOfEmpShare { get; set; }
        [Required]
        public byte PercentageOfCompanyShare { get; set; }
        [Required]
        public byte PercentageOfEmpShareBooster { get; set; }
        [Required]
        public byte PercentageOfCompanyShareBooster { get; set; }
       

    }

 

}

