using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Clients.Models
{
    public class Client_CodeMap_LOG
    {
        [Key]
        public int ClientLogID { get; set; }
        public int Code  { get; set; }
        [MaxLength(20)]
        public string ICproCID { get; set; }
        [MaxLength(20)]
        public string CoreCID { get; set; }
        public int auth { get; set; }
        [MaxLength(50)]
        public string Auther { get; set; }
        public DateTime StartDate{ get; set; }
        public string V8ename { get; set; }
        public string V8eaddress { get; set; }
        public string V8emaddress { get; set; }
        public string V8City { get; set; }
        public string V8idnumber { get; set; }
        public string V8idtype { get; set; }
        public string V8nation { get; set; }
        public string V8cboType { get; set; }
        public string V8branch { get; set; }
        public string V8tel { get; set; }
        public string V8fax { get; set; }

        [Required]
        public string Maker { get; set; }

        public bool Chk { get; set; }
        public string Checker { get; set; }
        [Required]
        public bool EditFlag { get; set; }
        public DeleteFlag DeleteFlag { get; set; } = DeleteFlag.NotDeleted;
    }
}