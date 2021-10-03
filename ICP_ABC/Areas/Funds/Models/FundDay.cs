using ICP_ABC.Areas.Currencies.Models;
using ICP_ABC.Areas.Sponsors.Models;
using ICP_ABC.Areas.CustTypes.Models;
using ICP_ABC.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICP_ABC.Areas.Funds.Models
{
    public class FundDay
    {
        [Key]
        public int Iden { get; set; }

        [Required]
        public int Day_Id { get; set; }
        [Required]
        public int Sub_Red { get; set; }
        //-------FundForigenKey
        [Required]
        [ForeignKey("Fund")]
        public int FundId { get; set; }
        public virtual Fund Fund { get; set; }

    }
}