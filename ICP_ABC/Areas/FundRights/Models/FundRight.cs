using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Areas.Group.Models;
using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.FundRights.Models
{
    [Table("FundRight")]
    public class FundRight
    {
        [Key]
        public int FundRightID { get; set; }
        [StringLength(4)]
        [Required]
       
        public string Code { get; set; }
        public bool FundRightAuth { get; set; }

        [ForeignKey("Fund")]
   
        
        public int FundID { get; set; }
        [ForeignKey("UserGroup")]
       
        public int GroupID { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }
        public bool EditFlag { get; set; }

        public DeleteFlag DeletFlag { get; set; } = DeleteFlag.NotDeleted;

        [Required]
        public string Maker { get; set; }

        public bool Chk { get; set; }
        public string Checker { get; set; }

        public bool Auth { get; set; }
        public string Auther { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        public UserGroup  UserGroup { get; set; }
        public Fund  Fund { get; set; }
    }
}