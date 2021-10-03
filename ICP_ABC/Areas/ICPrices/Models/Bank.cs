using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
namespace ICP_ABC.Areas.ICPrices.Models
{
    public class Bank
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int Code { get; set; }
        [Required]
        public string Name { get; set; }
    }
}