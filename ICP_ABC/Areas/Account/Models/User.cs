using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Account.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Code")]
        
        public string Code { get; set; }

        [StringLength(50)]
        [Display(Name = "Name")]
        [Column("name")]
        public string UserName { get; set; }
        [Column("branch_id")]
        public int BranchID { get; set; }
        [Column("group_id")]
        public int GroupID { get; set; }
        [Column("branch_right")]
        public bool BranchRight { get; set; }
        //---- equal Titeld
        public int Position { get; set; }

        [StringLength(10, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [Column("pass")]
        public string Password { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Column("exp_date")]
        public DateTime ExpireDate { get; set; }
        public bool EditFlag { get; set; }
        public DeleteFlag DeleteFlag { get; set; } = DeleteFlag.NotDeleted;
        //---- equal IsAdmin 
        public bool Admin { get; set; }
        public bool LockUser { get; set; }
        //---- equal Maker 
        [Column("Userid")]
        public string UserIDForMaker { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Column("sys_date")]
        public DateTime SysDate { get; set; } = DateTime.Now;
    }
}