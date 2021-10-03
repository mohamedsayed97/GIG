using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Clients.Models
{
    public class Client_CodeMap
    {
        [Key]
        public int ClientID { get; set; }
        public int Code { get; set; }
        [StringLength(20)]
        public string  ICproCID { get; set; }
        [StringLength(20)]
        public string CoreCID { get; set; }
        public int auth { get; set; }
	    public string  Auther { get; set; }
	    public DateTime StartDate { get; set; }

        [Required]
        public string Maker { get; set; }

        public bool Chk { get; set; }
        public string Checker { get; set; }
        [Required]
        public bool EditFlag { get; set; }
        public DeleteFlag DeleteFlag { get; set; } = DeleteFlag.NotDeleted;

    }
}