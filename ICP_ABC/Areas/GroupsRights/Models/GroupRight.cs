using DocumentFormat.OpenXml.Spreadsheet;
using ICP_ABC.Areas.Group.Models;
using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.GroupsRights.Models
{
    
    public class GroupRight
    {
        [Key]
        public int GroupRightID { get; set; }

        [Required]
        [StringLength(4)]
        
        public string Code { get; set; }

        
        public bool Create { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool Read { get; set; }
        [ForeignKey("Screen")]
        public int FormID { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }

        [ForeignKey("UserGroups")]
        public int GroupId { get; set; }
        public bool Authorized { get; set; }
        public bool None { get; set; }


        public bool Check { get; set; }

        public bool EditFlag { get; set; }

        public DeleteFlag DeletFlag { get; set; } = DeleteFlag.NotDeleted;
        [Required]
        public string Maker { get; set; }
        
        public bool Chk { get; set; }
        public string Checker { get; set; }
        
        public bool Auth { get; set; }
        public string Auther { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual Screen Screen { get; set; }

        public virtual UserGroup UserGroups { get; set; }
    }
}