using DocumentFormat.OpenXml.Spreadsheet;
using ICP_ABC.Areas.Branches.Models;
using ICP_ABC.Areas.Cities.Models;
using ICP_ABC.Areas.Company.Models;
using ICP_ABC.Areas.Customers.Models;
using ICP_ABC.Areas.CustTypes.Models;
using ICP_ABC.Areas.Nationalities.Models;
using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Customers.Models
{
    [Table("Customer")]
    public class Customer
    {
        [Key]
        public string CustomerID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
     
        public string tel1 { get; set; }

        public int CompanyId { get; set; }
        public ICP_ABC.Areas.Company.Models.Company Company { get; set; }

        public string EnName { get; set; }
       
        public string ArName { get; set; }
     
        public string EnAddress1 { get; set; }

        public string ArAddress1 { get; set; }

        public string Email1 { get; set; }

        [Required]

        public DateTime DateOfHiring { get; set; }

        [Required]

        public DateTime DateOfContribute { get; set; }

        [Required]

        public DateTime DateOfBirth { get; set; }

        public int Salary { get; set; }
        public int? ExcelId { get; set; }
        [ForeignKey("Nationality")]
        public int? NationalityId { get; set; }
        public virtual Nationality Nationality { get; set; }

        [ForeignKey("City")]
        public int? CityId { get; set; }
        public virtual City City { get; set; }
     
        public string IdNumber { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        public string Maker { get; set; }

        public bool? Chk { get; set; }
        public string Checker { get; set; }

        public bool Auth { get; set; }
        public string Auther { get; set; }

        [Column("entry_date")]
        public DateTime SysDate { get; set; } = DateTime.Now;

        /// <summary>
        /// ////////////
        /// </summary>




        public string EnAddress2 { get; set; }
       
        public string ArAddress2 { get; set; }             
        
        public string Email2 { get; set; }
       
        public string Email3 { get; set; }
       
        public string Email4 { get; set; }
      
        public int? PostalCode { get; set; }
      
        public string CRNumber { get; set; }
       
        public string Sector { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
               
        public DateTime? IssuanceDate { get; set; }

        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone no.")]
        [StringLength(20)]
        [Column(TypeName = "char")]
        public string tel2 { get; set; }
        public int? tel3 { get; set; }
        public int? Fax1 { get; set; }
        public int? Fax2 { get; set; }
        public int? Fax3 { get; set; }

      
        [ForeignKey("CustType")]
        public int? CustTypeId { get; set; }

       
 
        [ForeignKey("Branch")]
        public int? BranchId { get; set; }


       

       
        [ForeignKey("IdentityType")]
        public int? idType { get; set; }

      
     
        public bool? EditFlag { get; set; }
        public DeleteFlag? DeletFlag { get; set; } = DeleteFlag.NotDeleted;
        
    

       
        public virtual CustType CustType { get; set; }
       
        public virtual UserIdentityType IdentityType { get; set; }
        public virtual Branch Branch { get; set; }
     

  
        public string AR_PrimaryAddress { get; set; }
        
        public string EN_PrimaryAddress { get; set; }

    }

   
}