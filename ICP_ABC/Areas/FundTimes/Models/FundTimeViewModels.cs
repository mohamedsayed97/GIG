using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.FundTimes.Models
{
    public class CreateViewModel
    {
        public int FundTimeID { get; set; }
        public int FundId { get; set; }
        //public string Code { get; set; }
        //[RegularExpression( @"/^(0[0-9]|1[0-9]|2[0-3]):[0-5] [0-9]$/")]
        [Required(ErrorMessage = "Time id is required")]
        [RegularExpression(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]?$", ErrorMessage = "Please enter a valid time")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{ HH:mm p}")]
        public string Time { get; set; }
        public bool Check { get; set; }

        public bool Auth { get; set; }

        //---------------
        public bool AuthForEditAndDelete { get; set; }
    }
}