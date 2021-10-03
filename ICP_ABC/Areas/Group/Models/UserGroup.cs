using ICP_ABC.Areas.FundRights.Models;
using ICP_ABC.Areas.GroupsRights.Models;
using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Group.Models
{
    [Table("UserGroup")]
    public class UserGroup
    {
        [Key]
        public int GroupID { get; set; }

        [Required]
        [StringLength(4)]
        [Index(IsUnique = true)]
        public string Code { get; set; }

        [StringLength(50)]
        [Required]
        [Index(IsUnique = true)]
        [Display(Name = "Group Name")]
        public string Name { get; set; }

        [Required]
        [ForeignKey("ApplicationUsers")]
        public string UserID { get; set; }

        [Required]
        public bool EditFlag { get; set; }


        public DeleteFlag DeletFlag { get; set; } = DeleteFlag.NotDeleted;

        [Required]
        public string Maker { get; set; }

        [DefaultValue(0)]
        public bool Chk { get; set; }

        public string Checker { get; set; }

        [DefaultValue(0)]
        public bool Auth { get; set; }

        public string Auther { get; set; }

        public bool LockGroup { get; set; }

        public DateTime SysDate { get; set; } = DateTime.Now;
        
        public bool HasGroupRight { get; set; }

        public bool HasFundRight { get; set; }

        public virtual ICollection<FundRight> FundRights { get; set; }

        public virtual ICollection<GroupRight> groupRights { get; set; }

        public ICollection<ApplicationUser> ApplicationUsers { get; set; }


        
    }
}