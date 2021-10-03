using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.FundTimes.Models
{
    [Table("FundTime")]
    public class FundTime
    {
        [Key]
        public int FundTimeID { get; set; }
        //public string Code { get; set; }
        [DisplayFormat(DataFormatString = "{0: HH:mm:ss}")]
        [Column("fund_time")]
        public TimeSpan Time { get; set; }
        [ForeignKey("Fund")]
        public int FundId { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }

        public bool EditFlag { get; set; }
        public DeleteFlag DeletFlag { get; set; } = DeleteFlag.NotDeleted;

        public string Maker { get; set; }
        public bool Chk { get; set; }
        public string Checker { get; set; }
        public bool Auth { get; set; }
        public string Auther { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        public virtual Fund Fund { get; set; }


    }
}