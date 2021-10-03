using ICP_ABC.Areas.Branches.Models;
using ICP_ABC.Areas.Customers.Models;
using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Blocks.Models
{
    [Table("Block")]
    public class Block
    {
        [Key]       
        public int code { get; set; }
        
        [ForeignKey("Customer")]
        [Required]
        public string cust_id { get; set; }
        [ForeignKey("Fund")]
        public int fund_id  { get; set; }
        [ForeignKey("Branch")]
        public int branch_id  { get; set; }
        public DateTime	block_date  { get; set; }
        public decimal	qty_block    { get; set; }
        [MaxLength(50)]
        public string block_reson { get; set; }
        public short	flag_tr  { get; set; }
        public DeleteFlag DeletFlag { get; set; } = DeleteFlag.NotDeleted;
        public int BlockCmb { get; set; }
        public bool	blockauth  { get; set; }
        public int unit_price  { get; set; }
        public bool Chk { get; set; }
        public string Checker { get; set; }
        public string Auther { get; set; }
        
         public string Maker { get; set; }
        public virtual Branch  Branch { get; set; }
        public virtual Fund  Fund { get; set; }
        public virtual Customer  Customer { get; set; }
    }
}