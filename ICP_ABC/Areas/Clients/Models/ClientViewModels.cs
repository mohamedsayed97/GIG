
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Clients.Models
{
    public class CreateClient
    {
        [Required]     
        public int Code { get; set; }
        [Required]
        public string ClientNo { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        [EmailAddress]
        [Display(Name = "E-Mail Address")]
        public string EMail { get; set; }
        public int? City { get; set; }
        [Display(Name = "ID Number")]
        public int? IdNumber { get; set; }
        [Display(Name ="ID Type")]
        public int? IdType { get; set; }
        [Display(Name = "C/R Number")]
        public int? CRNumber { get; set; }
        [Display(Name = "Nationality")]
        public int? NationalityId { get; set; }
        [Display(Name = "Client Type")]
        public int? ClientType { get; set; }
        [Display(Name = "Branch")]
        public int? BranchId { get; set; }
        public string FAX { get; set; }
        //[Phone]
        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone no.")]
        public string Telephone { get; set; }
        //public int? Telephone { get; set; }

        ////////////
        public int? CodeV8 { get; set; }
        public string ClientNoV8 { get; set; }
        public string NameV8 { get; set; }
        public string AddressV8 { get; set; }
        [EmailAddress]
        [Display(Name = "E-Mail Address")]
        public string EMailV8 { get; set; }
        public int? CityV8 { get; set; }
        [Display(Name = "ID Number")]
        public int? IdNumberV8 { get; set; }
        [Display(Name = "ID Type")]
        public int? IdTypeV8 { get; set; }
        [Display(Name = "C/R Number")]
        public int? CRNumberV8 { get; set; }
        [Display(Name = "Nationality")]
        public int? NationalityIdV8 { get; set; }
        [Display(Name = "Client Type")]
        public int? ClientTypeV8 { get; set; }
        [Display(Name = "Branch")]
        public int? BranchIdV8 { get; set; }
        public string FAXV8 { get; set; }
        //[Phone]
        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone no.")]
      
        public string TelephoneV8 { get; set; }
    }

    public class SearchViewModel
    {
        public int Code { get; set; }
        public string CustomerName { get; set; }
        public string ICProCid { get; set; }
        public string CoreCid { get; set; }
    }

    public class DetailsviewModel
    {
        [Required]
        public int Code { get; set; }

        public string ClientNo { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        [EmailAddress]
        [Display(Name = "E-Mail Address")]
        public string EMail { get; set; }
        public int? City { get; set; }
        [Display(Name = "ID Number")]
        public int? IdNumber { get; set; }
        [Display(Name = "ID Type")]
        public int? IdType { get; set; }
        [Display(Name = "C/R Number")]
        public int? CRNumber { get; set; }
        [Display(Name = "Nationality")]
        public int? NationalityId { get; set; }
        [Display(Name = "Client Type")]
        public int? ClientType { get; set; }
        [Display(Name = "Branch")]
        public int? BranchId { get; set; }
        public string FAX { get; set; }
        //[Phone]
        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone no.")]
        public string Telephone { get; set; }
        //public int? Telephone { get; set; }

        ////////////
        public int? CodeV8 { get; set; }
        public string ClientNoV8 { get; set; }
        public string NameV8 { get; set; }
        public string AddressV8 { get; set; }
        [EmailAddress]
        [Display(Name = "E-Mail Address")]
        public string EMailV8 { get; set; }
        public int? CityV8 { get; set; }
        [Display(Name = "ID Number")]
        public int? IdNumberV8 { get; set; }
        [Display(Name = "ID Type")]
        public int? IdTypeV8 { get; set; }
        [Display(Name = "C/R Number")]
        public int? CRNumberV8 { get; set; }
        [Display(Name = "Nationality")]
        public int? NationalityIdV8 { get; set; }
        [Display(Name = "Client Type")]
        public int? ClientTypeV8 { get; set; }
        [Display(Name = "Branch")]
        public int? BranchIdV8 { get; set; }
        public string FAXV8 { get; set; }
        //[Phone]
        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone no.")]

        public string TelephoneV8 { get; set; }

        //-----------------------------------------------
        public int auth { get; set; }
        [MaxLength(50)]
        public string Auther { get; set; }
        public string Maker { get; set; }

        public bool Chk { get; set; }
        public string Checker { get; set; }

       
        public int AuthForEditAndDelete { get; set; }

    }
}