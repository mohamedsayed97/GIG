using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Customers.Models
{
    public class CreateViewModel
    {
        [Required]
        public string CustomerID { get; set; }

      
        public string EnName { get; set; }
       
        public string ArName { get; set; }
        [Phone]
        public string tel1 { get; set; }

       
        public string EnAddress1 { get; set; }

        public string ArAddress1 { get; set; }

        [EmailAddress]
        public string Email1 { get; set; }



        public string EnAddress2 { get; set; }

        public string ArAddress2 { get; set; }


        public int Salary { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime DateOfHiring { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime DateOfContribute { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public int? CityId { get; set; }
        public int? NationalityId { get; set; }

     
        public string IdNumber { get; set; }

        /// <summary>
        /// //////////
        /// </summary>









       
        public bool? Check { get; set; }
     
        public bool? Auth { get; set; }

         public bool? AuthForEditAndDelete { get; set; }

    } //---------------
      
}