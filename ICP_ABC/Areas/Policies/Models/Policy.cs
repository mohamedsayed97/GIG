using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Policies.Models
{
    public class Policy
    {
        [Key , DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Index(IsUnique = true)]
        public string PolicyNo { get; set; }

        [Required]
        public string PolicyHolderName { get; set; }

        [Required]
        public byte PaymentFrequency { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime EffectiveDate { get; set; }

        [Required]
        public byte Status { get; set; }

        [Required]
        public int CompanyId { get; set; }
        public ICP_ABC.Areas.Company.Models.Company Company { get; set; }

        [Required]
        public byte CalculationBasis { get; set; }

        [Required]
        public byte BusinessChannel { get; set; }

        [Required]
        public string Maker { get; set; }
        [Required]
        public bool HasWithdrawal { get; set; }
        [Required]
        public bool HasBooster { get; set; }

        public bool Chk { get; set; }
        public string Checker { get; set; }

        public bool Auth { get; set; }
        public string Auther { get; set; }
        public DeleteFlag DeletFlag { get; set; } = DeleteFlag.NotDeleted;
        public DateTime SysDate { get; set; } = DateTime.Now;

        public ICollection<AllocationRule> AllocationRules { get; set; }
    }
}

