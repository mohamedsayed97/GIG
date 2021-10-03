using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Sponsors.Models
{
    public class CreateViewModel
    {

        [Required]
        [StringLength(4)]
        public string Code { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Group Name")]
        public string Name { get; set; }
    }

    public class EditViewModel
    {
        [Key]
        [StringLength(4)]
        [Required]
        public string Code { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Sponsor Name")]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime SysDate { get; set; } = DateTime.Now;
    }

    public class DetailsViewModel
    {
        [Key]
        [StringLength(4)]
        [Required]
        public string Code { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Sponsor Name")]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public bool Check { get; set; }
        public string Checker { get; set; }
        public bool Auth { get; set; }
        public string Auther { get; set; }
        //---------------
        public bool AuthForEditAndDelete { get; set; }

    }
}