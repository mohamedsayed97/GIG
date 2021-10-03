using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.ICPrices.Models
{
    [Table("ICPrice")]
    public class ICPrice
    {
        [Key]
      
        public int ICPriceID { get; set; }

        [Required]
        [StringLength(4)]
 
        public string Code { get; set; }
      
        [Required]
        [Display(Name = "Fund Name")]
        [ForeignKey("Fund")]
       
        
        public int FundId { get; set; }
        [Column("ICDate")]
        public DateTime Date { get; set; }

        //------------------------

        [Column("ProcessingDate")]
        public DateTime ProcessingDate { get; set; }


        [Range(0, 9999999999999999.99999)]
        [RegularExpression(@"^\d+\.\d{0,5}$")]
        public decimal Price { get; set; }

        [Required]
        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }
        [Required]
        public bool EditFlag { get; set; }

        public DeleteFlag DeletFlag { get; set; } = DeleteFlag.NotDeleted;
        [Required]
        public string Maker { get; set; }

        public bool Chk { get; set; }
        public string Checker { get; set; }

        
         [Column("Navauth")]
        public bool Auth { get; set; }
        public string Auther { get; set; }

        public DateTime SysDate { get; set; } = DateTime.Now;
        public ApplicationUser ApplicationUser { get; set; }
        public virtual Fund  Fund { get; set; }
    }
}