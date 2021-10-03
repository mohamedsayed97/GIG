using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Models.DBSetup
{
    public class Title
    {
        [Key]
        public int TitleID { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }
        [Required]
        public bool EditFlag { get; set; }
        [DefaultValue(-1)]
        public bool DeletFlag { get; set; }
        [Required]
        public string Maker { get; set; }
        [DefaultValue(0)]
        public bool Check { get; set; }
        public string Checker { get; set; }
        [DefaultValue(0)]
        public bool Auth { get; set; }
        public string Auther { get; set; }

        public DateTime SysDate { get; set; } = DateTime.Now;
        public ICollection<ApplicationUser> ApplicationUser { get; set; }

        //public ICollection<ApplicationUser> applicationUsers { get; set; }
    }
}