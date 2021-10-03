using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Areas.Policies.Models;
using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Policies.Models
{
    public class AllocationRule
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PolicyId { get; set; }
        public Policy Policy { get; set; }

        [Required]
        public int FundId { get; set; }
        public Fund Fund { get; set; }

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

