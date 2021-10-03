using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Branches.Models
{
    public class CreateViewModel
    {
        [Key]
        public int BranchID { get; set; }
        [StringLength(10)]
        [Required]
        [Index(IsUnique = true)]
        public string branchcode { get; set; }


        [StringLength(50)]
        [Required]
        [Display(Name = "Branch Name")]
        public string BName { get; set; }


    }

    public class EditViewModel
    {
        [Key]
        public int BranchID { get; set; }
        [StringLength(10)]
        [Required]
        [Index(IsUnique = true)]
        public string branchcode { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Branch Name")]
        public string BName { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime SysDate { get; set; } = DateTime.Now;
    }

    public class DetailsViewModel
    {

        [Key]
        public int BranchID { get; set; }
        [StringLength(10)]
        [Required]
        [Index(IsUnique = true)]
        public string branchcode { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Branch Name")]
        public string BName { get; set; }


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