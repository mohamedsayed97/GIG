using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.FundRights.Models
{
    public class FundRightsViewModel
    {
        //public string name { get; set; }       
        public string GroupID { get; set; }
        public string FundId { get; set; }
        public string Code { get; set; }
    }
    public class RightsListViewModel
    {
        public List<FundRightsViewModel> objectList { get; set; }
    }

    public class CreateViewModel
    {

        [Required]
        [StringLength(4)]
        public string Code { get; set; }


        [Required]
        [Display(Name = "Group Right ID")]
        public int FundRightID { get; set; }


    }

    public class EditViewModel
    {
        [Key]
        [StringLength(4)]
        [Required]
        public string Code { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Group Name")]
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
        [Display(Name = "Group Name")]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        //---------------
        public bool AuthForEditAndDelete { get; set; }


    }
}