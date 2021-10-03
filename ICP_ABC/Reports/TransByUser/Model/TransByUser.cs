using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ICP_ABC.Areas.Account.Models;


namespace ICP_ABC.Reports.TransByUser.Model
{
    public class TransByUserVM
    {

        public List<User> users { get; set; }
        public int? user { get; set; }

        [Display(Name = "From Date")]
        [Required]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required]
        public DateTime? ToDate { get; set; }

        public string Auth { get; set; }

        public string TransactionOver { get; set; }
    }
}