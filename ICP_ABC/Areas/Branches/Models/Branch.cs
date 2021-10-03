using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Branches.Models
{
    [Table("Branch")]
    public class Branch
    {
        [Key]
        public int BranchID { get; set; }
        [StringLength(10)]
        [Required]
        [Index(IsUnique = true)]
        public string branchcode { get; set; }
        [StringLength(50)]
        [Required]
        public string BName { get; set; }
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
        public bool Auth { get; set; }
        public string Auther { get; set; }

        public DateTime SysDate { get; set; } = DateTime.Now;
        public ICollection<ApplicationUser> ApplicationUser { get; set; }

    }
}