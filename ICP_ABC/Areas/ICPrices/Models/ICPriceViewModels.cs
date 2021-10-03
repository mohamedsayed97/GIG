using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.ICPrices.Models
{
    public class CreateViewModel
    {

        [Required]
        [StringLength(4)]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Fund Name")]
        public int FundId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ProcessingDate { get; set; }

        [Required]
      
        public decimal Price { get; set; }

        //-----------------------------------
       
    }

    public class EditViewModel
    {
        [Required]
        [StringLength(4)]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Fund Name")]
        public int FundId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ProcessingDate { get; set; }

        [Required]
        public decimal Price { get; set; }
        [Required]
        public bool EditFlag { get; set; }
    }

    public class DetailsViewModel
    {
        [Required]
        [StringLength(4)]
        public string Code { get; set; }

       
        [Required]
        [Display(Name = "Fund Name")]
        public int FundId { get; set; }

        public DateTime Date { get; set; }

        public DateTime ProcessingDate { get; set; }

       
        public decimal Price { get; set; }

        public bool Check { get; set; }
        
        public bool Auth { get; set; }
        //-----------------------------------------------

        public bool AuthForEditAndDelete { get; set; }


    }
}