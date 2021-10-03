using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Currencies.Models
{
    [Table("Currency")]
    public class Currency
    {
        [Key]
        public int CurrencyID { get; set; }
       
        [Required]
        [StringLength(4)]
        [Index(IsUnique = true)]
        public string Code { get; set; }
        [StringLength(50)]
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        
        public string ShortName { get; set; }
        [Required]
      
        public decimal Rate { get; set; }
        [Required]
        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }
        [Required]
        public bool EditFlag { get; set; }
        [DefaultValue(-1)]
        public DeleteFlag DeletFlag { get; set; } = DeleteFlag.NotDeleted;
        [Required]
        public string Maker { get; set; }
        [DefaultValue(0)]
        public bool Chk { get; set; }
        public string Checker { get; set; }
        [DefaultValue(0)]
        public bool Auth { get; set; }
        public string Auther { get; set; }

        public DateTime SysDate { get; set; } = DateTime.Now;
        public ApplicationUser ApplicationUser { get; set; }
    }
}