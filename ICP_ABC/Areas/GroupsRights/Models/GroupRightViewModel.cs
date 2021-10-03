using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.GroupsRights.Models
{
    public class RightsViewModel
    {
        public string name { get; set; }
        public bool hasCreate { get; set; }
        public bool hasUpdate { get; set; }
        public bool hasDelete { get; set; }
        public bool hasRead { get; set; }
        public bool Authorize { get; set; }
        public bool Check { get; set; }
        public bool noneOfAll { get; set; }
        public string GroupID { get; set; }
        public string FormID { get; set; }
        public string Code { get; set; }
    }
    public class RightsListViewModel
    {
        public List<RightsViewModel> objectList { get; set; }
    }

        public class CreateViewModel
    {

        [Required]
        [StringLength(4)]
        public string Code { get; set; }


        [Required]
        [Display(Name = "Group Right ID")]
        public int GroupID { get; set; }


    }
 
    public class EditViewModel
    {
        [Key]
        [StringLength(4)]
        [Required]
        public string Code { get; set; }
        
        public int GId { get; set; }
        public List<GroupRight> GroupRight { get; set; }
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

        //-----------------------------------------------

        public bool AuthForEditAndDelete { get; set; }



    }


}