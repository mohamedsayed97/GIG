using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Company.Models
{
    public class CompanyViewModel
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
    }
    public class CompanyDetailsViewModel{

        [Display(Name = "Company ID")]
        public int CompanyID { get; set; }

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
        public String Phone1 { get; set; }

        [Display(Name = "Phone 2")]
        public String Phone2 { get; set; }

        public bool EditFlag { get; set; }


        public string Maker { get; set; }


        public bool Auth { get; set; }
        public string Auther { get; set; }

        public bool AuthForEditAndDelete { get; set; }

    }

}