using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Subscriptions.Models
{
    [Table("LastCode")]
    public class LastCode
    {
        [Key]
        [Column("iden")]
        public int ID { get; set; }
        public int Code { get; set; }
    }
}