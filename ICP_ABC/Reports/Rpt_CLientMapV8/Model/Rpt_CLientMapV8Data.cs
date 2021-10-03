using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICP_ABC.Reports.Rpt_CLientMapV8.Model
{
    public class Rpt_CLientMapV8Data
    {
        public string ICproCID { get; set; }
        public string CoreCID { get; set; }
        public string ename { get; set; }
        public DateTime StartDate { get; set; }
    }
}