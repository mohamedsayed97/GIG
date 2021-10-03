using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.UsersSecurity.Models
{
    public enum Levels
    {
        TwoLevels = 2,
        ThreeLevels = 3
    }

    [Table("UserSecurity")]
    public class UserSecurity
    {
        [Column("code")]
        [Key]
        public int UserSecurityId { get; set; }

        [Range(1, 5)]
        public int NumberOfTrials { get; set; }//number of logs

        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int ExpireInterval { get; set; }//number of days 

        [Column("SecLevel")]
        public Levels Levels { get; set; }
        [Required]
        [ForeignKey("ApplicationUser")]
        [Column("User_log")]
        public string UserID { get; set; }
        [Required]
        public bool EditFlag { get; set; }
        [DefaultValue(0)]
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
        //------------
        public bool ViewTransaction { get; set; }
        public bool CreateTransaction { get; set; }
    }
}