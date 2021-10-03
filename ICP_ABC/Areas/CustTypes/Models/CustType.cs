using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.CustTypes.Models
{
    [Table("custtype")]
    public class CustType
    {
        [Key]
        public int CustTypeID { get; set; }
        [StringLength(4)]
        [Required]
        [Index(IsUnique = true)]
        public string Code { get; set; }
        [StringLength(50)]
        [Required]
        public string Name { get; set; }

        public int LOP { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }
        [Required]
        public bool EditFlag { get; set; }
        public bool DeletFlag { get; set; }
        [Required]
        public string Maker { get; set; }
        public bool Check { get; set; }
        public string Checker { get; set; }
        public bool Auth { get; set; }
        public string Auther { get; set; }

        public DateTime SysDate { get; set; } = DateTime.Now;
        public ApplicationUser ApplicationUser { get; set; }
    }
}