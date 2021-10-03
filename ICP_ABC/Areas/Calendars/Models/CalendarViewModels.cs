using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Calendars.Models
{
    public class CreateViewModel
    {

        [Required]
        [StringLength(4)]
        public string Code { get; set; }

        [StringLength(50)]
        
        [Required(ErrorMessage = "Name is required")]
        public string Vacation_Name { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date is required, Must Be Today Or day in future Not before .")]
        public DateTime Vacation_date { get; set; }

        //[required]
        //[foreignkey("applicationuser")]
        //public string userid { get; set; }

        //[required]
        //public string maker { get; set; }
    }


    public class EditViewModel
    {
        [Key]
        [StringLength(4)]
        [Required]
        public string Code { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Name is required")]
        public string Vacation_Name { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date is required, Must Be Today Or day in future Not before .")]
        public DateTime Vacation_date { get; set; }
    }

    public class DetailsViewModel
    {
        [Key]
        [StringLength(4)]
        [Required]
        public string Code { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Name is required")]
        public string Vacation_Name { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date is required, Must Be Today Or day in future Not before .")]
        public DateTime Vacation_date { get; set; }

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