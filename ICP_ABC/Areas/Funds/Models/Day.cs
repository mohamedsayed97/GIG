using ICP_ABC.Areas.Currencies.Models;
using ICP_ABC.Areas.Sponsors.Models;
using ICP_ABC.Areas.CustTypes.Models;
using ICP_ABC.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICP_ABC.Areas.Funds.Models
{
    public class Day
    {
        [Key]
        public int Id { get; set; }
        public int Day_Id { get; set; }
        public string DayName { get; set; }
       
    }
}