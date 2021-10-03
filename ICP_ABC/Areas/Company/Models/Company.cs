using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Company.Models
{
    public class Company
    {
        [Key]
        [Required]
     [Display(Name = "Company ID")]
        public int CompanyID { get; set; }

        [Required]
        [Display(Name = "Company Name")]
        [StringLength(50)]
        public String Companyname { get; set; }
        [Display(Name = "Commerical Record")]
        public String ComericalRecord { get; set; }

        [Display(Name = "Address 1 ")]
        public String Address1 { get; set; }

        [Display(Name = "Address 2 ")]
        public String Address2 { get; set; }

        [Display(Name = "Phone 1 ")]
        [MaxLength(11)]
        public String Phone1 { get; set; }

        [Display(Name = "Phone 2")]
        [MaxLength(11)]
        public String Phone2 { get; set; }

        [Required]
        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }

        [Required]
        public bool EditFlag { get; set; }

        [DefaultValue(-1)]
        public DeleteFlag DeletFlag { get; set; } = DeleteFlag.NotDeleted;
        [Required]
        public string Maker { get; set; }

    
        [DefaultValue(0)]
        public bool Auth { get; set; }
        public string Auther { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
   

    }
}