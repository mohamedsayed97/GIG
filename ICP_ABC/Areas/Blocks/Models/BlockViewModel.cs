using ICP_ABC.Areas.Branches.Models;
using ICP_ABC.Areas.Customers.Models;
using ICP_ABC.Areas.Funds.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Blocks.Models
{
    public class BlockViewModel
    {
        [Key]
        public int code { get; set; }
        [MaxLength(30)]

        [Required]
        public string cust_id { get; set; }
        [ForeignKey("Fund")]
        public int fund_id { get; set; }
        [ForeignKey("Branch")]
        public int branch_id { get; set; }
        public string CustomerName { get; set; }
        public string block_date { get; set; }
        public decimal qty_block { get; set; }
        [MaxLength(50)]
        public string block_reson { get; set; }
        public int BlockCmb { get; set; }
        public int unit_price { get; set; }
        public bool Check { get; set; }

        public bool Auth { get; set; }
        //-----------------------------------------------

        public bool AuthForEditAndDelete { get; set; }
    }

    public class EditViewModel
    {
        [Key]
        [StringLength(4)]
        [Required]
        public string Code { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Currency Name")]
        public string Name { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Short Name")]
        public string ShortName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime SysDate { get; set; } = DateTime.Now;
    }

    public class DetailsViewModel
    {
        [Key]
        [StringLength(4)]
        [Required]
        public string Code { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Currency Name")]
        public string Name { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Short Name")]
        public string ShortName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public bool Check { get; set; }
        public string Checker { get; set; }
        public bool Auth { get; set; }
        public string Auther { get; set; }
        public string Maker { get; set; }
    }
}